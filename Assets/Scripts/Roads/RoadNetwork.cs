using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoadNetwork : MonoBehaviour
{
	public bool drawInEditorOnlyIfSelected;
	public Color editorNodeColor;
	public float editorNodeRadius;
	public float editorNodeOffset;

	public List<TrafficAgent> agents;

	public List<NetworkNode> nodes => new List<NetworkNode>(GetComponentsInChildren<NetworkNode>());

	void Start()
	{
	}

	void Update()
	{
    }

    private void OnDrawGizmos()
    {
		if (!drawInEditorOnlyIfSelected)
		{
	        DrawNetwork();
		}
    }

    private void OnDrawGizmosSelected()
    {
        if (drawInEditorOnlyIfSelected)
        {
            DrawNetwork();
        }
    }

	private void DrawNetwork()
    {
        Gizmos.color = editorNodeColor;
        foreach (var node in nodes)
        {
            if (node == null)
            {
                return;
            }

            DrawNode(node);
        }
    }

	private void DrawNode(NetworkNode node)
	{
        var start = node.transform.position + Vector3.up * editorNodeOffset;

		if (!node.isShapingCurve)
		{
	        Gizmos.DrawWireSphere(start, editorNodeRadius);
		}

		foreach (var destination in node.destinations)
		{
			var end = destination.transform.position + Vector3.up * editorNodeOffset;
			Gizmos.DrawLine(start, end);
        }
    }
}
