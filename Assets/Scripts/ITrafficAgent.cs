using UnityEngine;

public interface ITrafficAgent
{
    // target position in world space
    // pass null to stop the car
    public void SetTarget(Vector3 target);

    // position of the next target (the target after the current target) in world space
    // pass null to let the current target be the last position
    public void SetNextTarget(Vector3 nextTarget);

    public void SetSpeedLimit(float limit);

    public float GetAgentSpeed();
    public float GetMaxAgentSpeed();
}
