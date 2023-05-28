using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrafficAgent : MonoBehaviour
{
    //: Editor Properties

    [Header("References")]

    public RoadNetwork network;

    [Header("Agent settings")]

    public float acceleration;
    public float deceleration;
    public float maxSpeed;
    public float angularAcceleration;
    public float maxAngularVelocity;

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
        UpdatePhysics();

        if (targetNode == null)
        {
            Reset();
            return;
        }

        UpdateNavigation();
        UpdateSteering();
    }

    //: Private Methods

    private void UpdatePhysics()
    {
        transform.Rotate(Vector3.up, angularVelocity);
        transform.position += velocity * Time.deltaTime * transform.forward;
    }

    private void UpdateNavigation()
    {
        if ((targetNode.transform.position - transform.position).sqrMagnitude >= nodeReachedThreshold)
        {
            return;
        }

        var newTarget = RandomNode(targetNode.destinations);
        if (newTarget == null)
        {
            Reset();
            return;
        }

        startNode = targetNode;
        SetTarget(newTarget);
    }

    private void UpdateSteering()
    {
        UpdateRotation();
        UpdateAcceleration();
    }

    private void UpdateRotation()
    {
        var toTarget = targetNode.transform.position - transform.position;

        var angleToTarget = Vector3.SignedAngle(transform.forward, toTarget, Vector3.up);
        Debug.Log(angleToTarget);
        if (angleToTarget > 0.0f)
        {
            angularVelocity += angularAcceleration * Time.deltaTime;
        }
        else if (angleToTarget < 0.0f)
        {
            angularVelocity -= angularAcceleration * Time.deltaTime;
        }
    }

    private void UpdateAcceleration()
    {
        var remainingDistance = (targetNode.transform.position - transform.position).magnitude;
        var decelerationDistance = (velocity * velocity) / (2.0f * deceleration);

        if (remainingDistance <= decelerationDistance)
        {
            velocity = Mathf.MoveTowards(velocity, 0.0f, deceleration * Time.deltaTime);
        }
        else
        {
            velocity = Mathf.MoveTowards(velocity, maxSpeed, acceleration * Time.deltaTime);
        }
    }

    private void Reset(int attempts = 0)
    {
        for (int i = 0; i < 1000; i++)
        {
            startNode = RandomNode(network.nodes);

            if (startNode.origins.Count == 0 && !startNode.isShapingCurve)
            {
                break;
            }
        }
        transform.position = startNode.transform.position;

        var newTarget = RandomNode(startNode.destinations);
        if (newTarget == null)
        {
            if (attempts >= maxResetAttempts)
            {
                Debug.LogError("No valid target node found in " + attempts + " attempts");
                return;
            }
            Reset(attempts + 1);
            return;
        }

        Debug.Log("Resetting target to '" + newTarget.gameObject.name + "'");
        SetTarget(newTarget);
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

    private void SetTarget(NetworkNode newTarget)
    {
        targetNode = newTarget;
        //transform.LookAt(targetNode.transform.position, Vector3.up);
    }

    //: Private variables

    private NetworkNode startNode;
    private NetworkNode targetNode;

    private float velocity;
    private float angularVelocity;
}
