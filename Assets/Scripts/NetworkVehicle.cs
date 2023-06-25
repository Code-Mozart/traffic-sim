using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkVehicle : MonoBehaviour, INetworkAgent
{
    //: Editor Properties

    [Header("References")]

    public RoadNetwork network;

    [Header("Vehicle Settings")]

    [Tooltip("The vehicles acceleration in (units/second)/second.")]
    public float acceleration;
    [Tooltip("The vehicles deceleration/break in (units/second)/second.")]
    public float deceleration;
    [Tooltip("The total maximum speed of the vehicle (not the current speed limit) in units/second.")]
    public float maxSpeed;
    [Tooltip("The time it takes the vehicle to turn to any angle in seconds.")]
    public float turnTime;

    [Header("Thresholds")]

    public float nodeReachedThreshold;
    public int maxResetAttempts = 100;

    //: Unity Callbacks

	private void Start()
	{
        Reset();
	}

	private void Update()
	{
        UpdateRoute();
        UpdateSteering();
        UpdatePhysics();
        UpdateVelocity();
    }

    //: INetworkAgent

    List<NetworkNode> INetworkAgent.Route
    {
        get => _route;
        set => _route = value;
    }

    NetworkNode INetworkAgent.Previous => _route.Count > 0 ? _route[0] : null;

    NetworkNode INetworkAgent.Next => _route.Count > 1 ? _route[1] : null;

    NetworkNode INetworkAgent.Destination => _route.Count > 0 ? _route[_route.Count - 1] : null;

    bool INetworkAgent.HasReachedDestination => _route.Count == 1;

    float INetworkAgent.RemainingDistance {
        get
        {
            var sum = 0.0f;
            if (_route.Count > 1)
            {
                sum += ToNext().magnitude;
            }
            for (int i = 2; i < _route.Count; i++)
            {
                var segmentStart = _route[i - 1].transform.position;
                var segmentEnd = _route[i].transform.position;
                sum += Vector3.Distance(segmentStart, segmentEnd);
            }

            return sum;
        }
    }

    float INetworkAgent.SpeedLimit
    {
        get => _speedLimit;
        set => _speedLimit = value;
    }

    float INetworkAgent.Speed => _velocity;

    float INetworkAgent.MaxSpeed
    {
        get => maxSpeed;
        set => maxSpeed = value;
    }

    bool INetworkAgent.IsStopped
    {
        get => _isStopped;
        set => _isStopped = value;
    }

    System.Action<NetworkNode> INetworkAgent.OnNodeReached
    {
        get => throw new System.NotImplementedException();
        set => throw new System.NotImplementedException();
    }

    System.Action<NetworkNode> INetworkAgent.OnDestinationReached
    {
        get => throw new System.NotImplementedException();
        set => throw new System.NotImplementedException();
    }

    //: Private Methods

    private void UpdateSteering()
    {
        if (_turnTimer > 0.0f)
        {
            _turnTimer -= Time.deltaTime;

            if (_turnTimer <= 0.0f)
            {
                _angularVelocity = 0.0f;
                transform.forward = ToNext().normalized;
            }
        }
    }

    private void UpdatePhysics()
    {
        transform.position = Vector3.MoveTowards(transform.position, ((INetworkAgent)this).Next.transform.position, _velocity * Time.deltaTime);
        transform.Rotate(Vector3.up, _angularVelocity * Time.deltaTime);
    }

    private void UpdateRoute()
    {
        // Debug: Draw route
        for (int i = 1; i < _route.Count; i++)
        {
            var segmentStart = _route[i - 1].transform.position;
            var segmentEnd = _route[i].transform.position;
            Debug.DrawLine(segmentStart, segmentEnd, Color.cyan);
        }
        
        if (((INetworkAgent)this).HasReachedDestination)
        {
            Reset();
            return;
        }

        if (ToNext().magnitude <= nodeReachedThreshold)
        {
            _route.RemoveAt(0);

            var angleDifference = Vector3.SignedAngle(transform.forward, ToNext(), Vector3.up);
            _angularVelocity = angleDifference / turnTime;
            _turnTimer = turnTime;
        }
    }

    private void UpdateVelocity()
    {
        // Debug: Draw velocity
        Debug.DrawRay(transform.position + 0.01f * Vector3.up, transform.forward * _velocity, Color.green);

        if (_isStopped)
        {
            _velocity = Mathf.MoveTowards(_velocity, 0.0f, deceleration * Time.deltaTime);
            return;
        }

        float desiredSpeed;
        var distanceToBreak = Kinematics.s(v: _velocity, a: deceleration);
        Debug.Log("distanceToBreak = " + distanceToBreak + ", remainingDistance = " + ((INetworkAgent)this).RemainingDistance);

        // Debug: Draw velocity
        Debug.DrawRay(transform.position + 0.01f * Vector3.down, transform.forward * distanceToBreak, Color.red);

        if (((INetworkAgent)this).RemainingDistance <= distanceToBreak)
        {
            desiredSpeed = 0.0f;
        }
        else
        {
            desiredSpeed = Mathf.Min(maxSpeed, _speedLimit);
        }

        if (_velocity > desiredSpeed)
        {
            _velocity = Mathf.MoveTowards(_velocity, desiredSpeed, deceleration * Time.deltaTime);
        }
        else
        {
            _velocity = Mathf.MoveTowards(_velocity, desiredSpeed, acceleration * Time.deltaTime);
        }
    }

    private void Reset()
    {
        _velocity = 0.0f;
        _angularVelocity = 0.0f;
        _turnTimer = 0.0f;

        _speedLimit = maxSpeed;
        _isStopped = false;

        NetworkNode startNode = null;
        for (int i = 0; i < maxResetAttempts; i++)
        {
            startNode = RandomNode(network.nodes);

            if (startNode.origins.Count == 0 && !startNode.isShapingCurve && startNode.destinations.Count > 0)
            {
                break;
            }
        }
        if (startNode == null)
        {
            Debug.LogError("No valid start node found in " + maxResetAttempts + " attempts");
            return;
        }

        ((INetworkAgent)this).Route = CreateRandomRoute(startNode);

        transform.position = startNode.transform.position;
        transform.forward = ToNext();
    }

    private NetworkNode RandomNode(List<NetworkNode> nodes)
    {
        if (nodes.Count <= 0)
        {
            return null;
        }
        
        var randomIndex = Random.Range(0, nodes.Count);
        return nodes[randomIndex];
    }

    private List<NetworkNode> CreateRandomRoute(NetworkNode start)
    {
        var route = new List<NetworkNode>();
        route.Add(start);

        var currentLast = start;
        while (currentLast.destinations.Count > 0)
        {
            var nextNode = RandomNode(currentLast.destinations);
            // nextNode cant be null with the current implementation of RandomNode()
            route.Add(nextNode);
            currentLast = nextNode;
        }

        return route;
    }

    private Vector3 ToNext()
    {
        var toNext = ((INetworkAgent)this).Next.transform.position - transform.position;
        toNext = new Vector3(toNext.x, 0.0f, toNext.z);
        return toNext;
    }

    //: Private variables

    [Header("Private Variables")]
    
    private float _velocity;
    private float _angularVelocity;
    private float _turnTimer;

    private List<NetworkNode> _route;
    private float _speedLimit;
    private bool _isStopped;
}
