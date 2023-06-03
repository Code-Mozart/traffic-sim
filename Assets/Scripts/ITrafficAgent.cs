using UnityEngine;

public interface ITrafficAgent
{
    // target position in world space
    public Vector3 target
    {
        get;

        // pass null to stop the car
        set;
    }

    // position of the next target (the target after the current target) in world space
    public Vector3 nextTarget
    {
        get;
    
        // pass null to let the current target be the last position
        set;
    }

    public float speedLimit
    {
        get;
        set;
    }

    public float speed
    {
        get;
    }

    public float maxSpeed 
    {
        get;
        set;
    }
}
