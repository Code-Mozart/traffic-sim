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
    [Tooltip("The multiplier on the speed limit for the minimum speed while turning")]
    public float minTurnSpeedMultiplier;

    [Header("Thresholds")]

    public float nodeReachedThreshold;
    public int maxResetAttempts = 100;
    public int maxRouteLength = 1000;

    //: Unity Callbacks

	private void Start()
	{
        ResetToNearest();
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

    float INetworkAgent.RemainingDistance
    {
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
        // set
        // {
        //     _isStopped = value;
        //     this.GetComponent<MeshRenderer>().materials[0].color = _isStopped ? Color.red : Color.white;
        // }
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
        if (!((INetworkAgent)this).Next)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, ((INetworkAgent)this).Next.transform.position, _velocity * Time.deltaTime);
        transform.Rotate(Vector3.up, _angularVelocity * Time.deltaTime);
    }

    private void UpdateRoute()
    {
        // Debug: Draw route
        print(_route.Count);
        for (int i = 1; i < _route.Count; i++)
        {
            var segmentStart = _route[i - 1].transform.position;
            var segmentEnd = _route[i].transform.position;
            Debug.DrawLine(segmentStart, segmentEnd, Color.cyan);
        }

        if (((INetworkAgent)this).HasReachedDestination)
        {
            ResetToRandom();
            return;
        }

        if (ToNext().magnitude <= nodeReachedThreshold)
        {
            UpdateJunction();

            _route.RemoveAt(0);

            _angularVelocity = SignedAngleToNext() / turnTime;
            _turnTimer = turnTime;
        }
    }

    private void UpdateJunction()
    {
        var previousJunction = ((INetworkAgent)this).Previous?.transform?.GetComponentInParent<XJunction>();
        var nextJunction = ((INetworkAgent)this).Next?.transform?.GetComponentInParent<XJunction>();

        if (nextJunction != previousJunction)
        {
            previousJunction?.RemoveAgent(this);
            nextJunction?.AddAgent(this);
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

        float desiredSpeed = CalculateDesiredSpeed();
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
    }

    private void ResetToRandom()
    {
        Reset();

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

    private void ResetToNearest()
    {
        Reset();

        var startNode = FindNearestNodeInNetwork();
        ((INetworkAgent)this).Route = CreateRandomRoute(startNode);

        transform.position = startNode.transform.position;
        transform.forward = ToNext();
    }

    private NetworkNode FindNearestNodeInNetwork()
    {
        var nearestSoFar = network.nodes[0];
        var minDistance = Vector3.Distance(transform.position, nearestSoFar.transform.position);
        foreach (var node in network.nodes)
        {
            var distance = Vector3.Distance(transform.position, node.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestSoFar = node;
            }
        }
        return nearestSoFar;
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
        for (int i = 0; i < maxRouteLength && currentLast.destinations.Count > 0; i++)
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
        var next = ((INetworkAgent)this).Next;
        if (!next)
        {
            return transform.forward;
        }

        var toNext = next.transform.position - transform.position;
        toNext = new Vector3(toNext.x, 0.0f, toNext.z);
        return toNext;
    }

    private float SignedAngleToNext()
    {
        return Vector3.SignedAngle(transform.forward, ToNext(), Vector3.up);
    }

    private float CalculateDesiredSpeed()
    {
        var distanceToBreak = Kinematics.s(v: _velocity, a: deceleration);
        //Debug.Log("distanceToBreak = " + distanceToBreak + ", remainingDistance = " + ((INetworkAgent)this).RemainingDistance);

        // Debug: Draw velocity
        Debug.DrawRay(transform.position + 0.01f * Vector3.down, transform.forward * distanceToBreak, Color.red);

        if (((INetworkAgent)this).RemainingDistance <= distanceToBreak)
        {
            return 0.0f;
        }

        var desiredSpeed = Mathf.Min(maxSpeed, _speedLimit);
        var distanceToNext = ToNext().magnitude;

        if (_route.Count > 2 && distanceToNext <= distanceToBreak)
        {
            var toOneAfter = _route[2].transform.position - _route[1].transform.position;
            var dot = Vector3.Dot(transform.forward, toOneAfter.normalized);
            var desiredTurnSpeed = Mathf.Max(dot * desiredSpeed, minTurnSpeedMultiplier * _speedLimit);
            var distanceToTurnSpeed = Kinematics.s(v: _velocity - desiredTurnSpeed, a: deceleration);
            if (distanceToTurnSpeed <= distanceToNext)
            {
                return desiredTurnSpeed;
            }
        }
        
        return desiredSpeed;
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
