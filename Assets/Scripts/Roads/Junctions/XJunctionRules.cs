using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IXJunctionRules
{
    public void Update(XJunction junction);
}

[System.Serializable]
public class XJunctionRule_FirstComeFirstServe : IXJunctionRules
{
    //: IXJunctionRules

    void IXJunctionRules.Update(XJunction junction)
    {
        _junction = junction;
        // _agentsInEntry = null;
        _agentsInJunction = null;

        // Debug
        Debug.Log("#allAgentsInJunction = " + allAgentsInJunction.Count);

        foreach (var agent in allAgentsInJunction)
        {
            agent.IsStopped = isJunctionOccupied;
        }

        if (allAgentsInJunction.Count > 0)
        {
            allAgentsInJunction[0].IsStopped = false;
        }
    }

    //: Private Methods

    private bool IsAgentInJunction(INetworkAgent agent)
    {
        XJunction.Direction[] directions = {
            _junction.north, _junction.east, _junction.south, _junction.west
        };

        foreach (var dir in directions)
        {
            foreach (var segments in new XJunction.Segment[][] {
                dir.left, dir.forward, dir.right
            })
            {
                if (IsAgentInSegment(dir.entry, agent))
                {
                    return true;
                }

                foreach (var seg in segments)
                {
                    if (IsAgentInSegment(seg, agent))
                    {
                        return true;
                    }
                }
            }

        }

        return false;
    }

    private bool IsAgentInEntry(INetworkAgent agent)
    {
        XJunction.Direction[] directions = {
            _junction.north, _junction.east, _junction.south, _junction.west
        };

        foreach (var dir in directions)
        {
            if (IsAgentInSegment(dir.entry, agent))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsAgentInSegment(XJunction.Segment segment, INetworkAgent agent)
    {
        return agent.Previous == segment.start && agent.Next == segment.end;
    }

    //: Private Attributes

    private List<INetworkAgent> allAgentsInJunction
    {
        get
        {
            if (_agentsInJunction == null)
            {
                _agentsInJunction = new List<INetworkAgent>();
                foreach (var agent in _junction.Agents)
                {
                    if (IsAgentInJunction(agent))
                    {
                        _agentsInJunction.Add(agent);
                    }
                }
            }
            return _agentsInJunction;
        }
    }

    private List<INetworkAgent> allAgentsInEntry
    {
        get
        {
            if (_agentsInEntry == null)
            {
                _agentsInEntry = new List<INetworkAgent>();
                foreach (var agent in _junction.Agents)
                {
                    if (IsAgentInEntry(agent))
                    {
                        _agentsInEntry.Add(agent);
                        Debug.Log("Agent " + agent + " is in entry");
                    }
                    else
                        Debug.Log("Agent " + agent + " is NOT in entry");
                }
            }
            return _agentsInEntry;
        }
    }

    private bool isJunctionOccupied => allAgentsInJunction.Count > 0;

    //: Private Variables

    private XJunction _junction;
    private List<INetworkAgent> _agentsInJunction;
    private List<INetworkAgent> _agentsInEntry;
}
