using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(VehicleController))]
public class TrafficAgent : MonoBehaviour
{
    //: Editor Properties

    [Header("References")]

    public RoadNetwork network;

    [Header("Thresholds")]

    public float nodeReachedThreshold;
    public int maxResetAttempts = 100;

    //: Unity Callbacks

	private void Start()
	{
        vehicleController = GetComponent<VehicleController>();
        Reset();
	}

	private void Update()
	{
        if (targetNode == null)
        {
            Reset();
            return;
        }

        UpdateNavigation();
    }

    //: Private Methods

    private void UpdateNavigation()
    {
        if ((targetNode.transform.position - transform.position).sqrMagnitude >= nodeReachedThreshold)
        {
            return;
        }

        if (nextTargetNode == null)
        {
            var newTarget = RandomNode(targetNode.destinations);
            if (newTarget == null)
            {
                Reset();
                return;
            }

            startNode = targetNode;
            SetTarget(newTarget, null);
        }        
        else
        {
            var newNextTarget = RandomNode(nextTargetNode.destinations);
            startNode = targetNode;
            targetNode = nextTargetNode;
            nextTargetNode = newNextTarget;
            SetTarget(targetNode, nextTargetNode);
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
        SetTarget(newTarget, RandomNode(newTarget.destinations));
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

    private void SetTarget(NetworkNode newTarget, NetworkNode newNextTarget)
    {
        targetNode = newTarget;
        nextTargetNode = newNextTarget;
        ((ITrafficAgent)vehicleController).Target = targetNode.transform.position;
        ((ITrafficAgent)vehicleController).NextTarget = nextTargetNode.transform.position;
    }

    //: Private variables

    private NetworkNode startNode;
    private NetworkNode targetNode;
    private NetworkNode nextTargetNode;

    private VehicleController vehicleController;
}
