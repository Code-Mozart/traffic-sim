using UnityEngine;

public interface ITrafficAgent
{
    // target position in world space
    public Vector3? Target
    {
        get;

        // pass null to stop the car
        set;
    }

    // position of the next target (the target after the current target) in world space
    public Vector3? NextTarget
    {
        get;
    
        // pass null to let the current target be the last position
        set;
    }

    public float SpeedLimit
    {
        get;
        set;
    }

    public float Speed
    {
        get;
    }

    public float MaxSpeed 
    {
        get;
        set;
    }

    public float DistanceToTarget
    {
        get;
    }
}
