using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XJunction : MonoBehaviour
{
    //: Editor Properties

    [Header("Junction Settings")]

    [SerializeReference]
    public IXJunctionRules rules;

    [Header("Junction Nodes")]

    public Direction north;
    public Direction east;
    public Direction south;
    public Direction west;

    //: Public Attributes

    public List<INetworkAgent> Agents => _agents;

    //: Unity Callbacks

	private void Start()
	{
	}

	private void Update()
	{
        if (rules == null)
        {
            Debug.LogWarning("No traffic rules defined for junction " + name);
            return;
        }

        rules.Update(this);
    }

    private void OnDrawGizmosSelected()
    {
        Color Darker(Color c, float brightness = 0.6f)
        {
            return c * brightness;
        }

        void DrawSegment(Segment seg)
        {
            if (seg.start == null || seg.end == null) return;

            var offset = 0.5f * Vector3.up;
            Gizmos.DrawLine(seg.start.transform.position + offset, seg.end.transform.position + offset);
        }

        void DrawSegments(Segment[] segments)
        {
            foreach (var seg in segments) DrawSegment(seg);
        }

        void DrawDirection(Direction dir, Color col)
        {
            Gizmos.color = Darker(col);
            DrawSegment(dir.entry);

            Gizmos.color = col;
            DrawSegments(dir.left);
            DrawSegments(dir.forward);
            DrawSegments(dir.right);
        }

        DrawDirection(north, Color.red);
        DrawDirection(east, Color.yellow);
        DrawDirection(south, Color.cyan);
        DrawDirection(west, Color.blue);
    }

    //: Public Methods

    public void AddAgent(INetworkAgent agent)
    {
        _agents.Add(agent);
    }

    public bool RemoveAgent(INetworkAgent agent)
    {
        if (_agents.Remove(agent))
        {
            agent.IsStopped = false;
            return true;
        }
        return false;
    }
    
    //: Private Methods

    //: Private Variables

    private List<INetworkAgent> _agents = new List<INetworkAgent>();

    //: Structs

    [System.Serializable]
    public struct Segment
    {
        public NetworkNode start;
        public NetworkNode end;
    }

    [System.Serializable]
    public struct Direction
    {
        public Segment entry;
        public Segment[] left;
        public Segment[] forward;
        public Segment[] right;

        //public Queue<INetworkAgent> watingAgents;
    }
}
