using System;
using System.Collections.Generic;
using UnityEngine;

public class HookNetworkNode : NetworkNode
{
    public float searchRadius = 1.0f;
    public LayerMask ConnectorNodeLayer;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 0.25f);
        FindNearbyConnectorNodes();
        NodeOutOfRange();
    }

    private void FindNearbyConnectorNodes()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, searchRadius, ConnectorNodeLayer);

        foreach (Collider collider in colliders)
        {
            ConnectorNetworkNode connectorNode = collider.GetComponent<ConnectorNetworkNode>();
            if (connectorNode != null)
            {
                // Verbinde den aktuellen ConnectorNode mit dem gefundenen HookNode
                ConnectToConnectorNode(connectorNode);
            }
        }
    }

    private void ConnectToConnectorNode(ConnectorNetworkNode connectorNode)
    {
        if (!origins.Contains(connectorNode))
            origins.Add(connectorNode);
    }

    private void NodeOutOfRange()
    {
        foreach (ConnectorNetworkNode node in origins.ToArray())
        {
            if (Vector3.Distance(node.transform.position, transform.position) > searchRadius)
            {
                origins.Remove(node);
            }
        }
    }
}
