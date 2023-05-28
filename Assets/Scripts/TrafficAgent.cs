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
    public float maxSpeed;

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
        if (targetNode == null)
        {
            Reset();
            return;
        }
        
        var moveVector = Vector3.MoveTowards(transform.position, targetNode.transform.position, maxSpeed * Time.deltaTime);
        transform.position = moveVector;

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

    //: Private Methods

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
        transform.LookAt(targetNode.transform.position, Vector3.up);
    }

    //: Private variables

    private NetworkNode startNode;
    private NetworkNode targetNode;
}
