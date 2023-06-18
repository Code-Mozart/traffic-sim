using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficNavigator : MonoBehaviour
{
    //: Editor Properties

    [Header("References")]

    public RoadNetwork network;

    public ITrafficAgent agent;

    [Header("Thresholds")]

    public float nodeReachedThreshold;
    public int maxResetAttempts = 100;

    [Header("Distance Holder")]

    public float targetDistanceToTraffic = 10f;

    public LayerMask trafficLayer;

    public float scanningAngle = 90f;
    public int scanningSteps = 5;

    //: Unity Callbacks

    private void Start()
    {
        agent = GetComponent<ITrafficAgent>();
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
        scanTrafficForward();
    }

    //: Private Methods
    private void scanTrafficForward()
    {
        bool hasHit = false;
        Vector3 location = Vector3.zero;
        for(int i = 0; i<scanningSteps; i++){
            RaycastHit hit;
            float angle = (scanningAngle/scanningSteps)*i-(scanningAngle/2);
            Debug.DrawRay(transform.position, Quaternion.AngleAxis(angle, transform.up) * transform.forward * targetDistanceToTraffic, Color.red);
            Vector3 direction = Quaternion.AngleAxis(angle, transform.up) * transform.forward;
            if (Physics.Raycast(transform.position, direction, out hit, targetDistanceToTraffic,trafficLayer))
            {
                    print("TrafficAgent ahead");
                    location = hit.collider.gameObject.transform.position;
                    hasHit = true;
            }
        }

        if (hasHit == true)
            {
                agent.SpeedLimit = 0;
            } else {
                agent.SpeedLimit = agent.MaxSpeed;
            }
            
    }

    private void UpdateNavigation()
    {
        if ((targetNode.transform.position - transform.position).sqrMagnitude >= nodeReachedThreshold)
        {
            return;
        }

        var newTarget = nextTargetNode;
        var newNextTarget = RandomNode(newTarget.destinations);
        if (newTarget == null)
        {
            Reset();
            return;
        }

        startNode = targetNode;
        SetTarget(newTarget, newNextTarget);
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
        var newNextTarget = RandomNode(newTarget.destinations);
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
        SetTarget(newTarget, newNextTarget);
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

    private void SetTarget(NetworkNode newTarget, NetworkNode nextTarget)
    {
        targetNode = newTarget;
        nextTargetNode = nextTarget;

        agent.Target = (targetNode == null) ? null : targetNode.transform.position;
        agent.NextTarget = (nextTargetNode == null) ? null : nextTargetNode.transform.position;
    }

    //: Private variables

    private NetworkNode startNode;
    private NetworkNode targetNode;

    private NetworkNode nextTargetNode;
}
