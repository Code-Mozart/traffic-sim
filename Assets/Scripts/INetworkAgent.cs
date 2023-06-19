using System.Collections.Generic;
using UnityEngine;

public interface INetworkAgent
{
    //: Route

    // Sets the route the network agent will use.
    public List<NetworkNode> Route
    {
        get;
        set;
    }

    public NetworkNode Previous
    {
        get;
    }

    public NetworkNode Next
    {
        get;
    }

    public NetworkNode Destination
    {
        get;
    }

    public bool HasReachedDestination
    {
        get;
    }

    public float RemainingDistance
    {
        get;
    }

    //: Vehicle Control

    public float SpeedLimit
    {
        get;
        set;
    }

    public bool IsStopped
    {
        get;
        set;
    }

    //: Vehicle Properties

    public float Speed
    {
        get;
    }

    public float MaxSpeed 
    {
        get;
        set;
    }

    //: Callbacks

    // This callback is invoked every time the agent reaches the next node on its route
    // including the destination node.
    public System.Action<NetworkNode> OnNodeReached
    {
        get;
        set;
    }

    // This callback is invoked when the agent reaches its destination.
    public System.Action<NetworkNode> OnDestinationReached
    {
        get;
        set;
    }
}
