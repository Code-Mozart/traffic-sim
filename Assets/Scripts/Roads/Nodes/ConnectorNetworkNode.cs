using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectorNetworkNode : NetworkNode
{
    public float searchRadius = 1.0f;
    public LayerMask hookNodeLayer;

    private void Awake()
    {
        FindNearbyHookNodes();
    }

    private void OnDrawGizmos()
    {
        FindNearbyHookNodes();
        NodeOutOfRange();

        Gizmos.DrawWireSphere(transform.position, 0.25f);
        foreach (HookNetworkNode node in destinations)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, node.transform.position);
        }
    }

    private void FindNearbyHookNodes()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, searchRadius, hookNodeLayer);

        foreach (Collider collider in colliders)
        {
            HookNetworkNode hookNode = collider.GetComponent<HookNetworkNode>();
            if (hookNode != null)
            {
                // Verbinde den aktuellen ConnectorNode mit dem gefundenen HookNode
                ConnectToHookNode(hookNode);
            }
        }
    }

    private void ConnectToHookNode(HookNetworkNode hookNode)
    {
        if (!destinations.Contains(hookNode))
            destinations.Add(hookNode);
    }

    private void NodeOutOfRange()
    {
        foreach (HookNetworkNode node in destinations.ToArray())
        {
            if (node == null)
            {
                destinations.Remove(node);
            }
            else if (Vector3.Distance(node.transform.position, transform.position) > searchRadius)
            {
                destinations.Remove(node);
            }
        }
    }
}
