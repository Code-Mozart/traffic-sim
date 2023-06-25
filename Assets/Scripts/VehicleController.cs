using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VehicleController : MonoBehaviour, ITrafficAgent
{
    //: Editor Properties

    [Header("Vehicle Movement Settings")]

    [Tooltip("The vehicles acceleration in (units/second)/second.")]
    public float acceleration;
    [Tooltip("The vehicles deceleration/break in (units/second)/second.")]
    public float deceleration;
    [Tooltip("The total maximum speed of the vehicle (not the current speed limit) in units/second.")]
    public float maxSpeed;

    [Header("Vehicle Steering Settings")]

    [Tooltip("\"How fast the steering wheel can be turned\" in in degrees/second (actually degrees/second^2).")]
    public float steerSensitivity;
    [Tooltip("\"How fast the steering wheel can be unturned\" in degrees/second (actually degrees/second^2).")]
    public float unsteerSensitivity;
    [Tooltip("The maximum steering angle in degrees (actually degrees/second) i.e. the maximum turn speed.")]
    public float maxSteeringAngle;
    [Tooltip("The minimum velocity the vehicle has to have to move and turn in units/second.")]
    public float minVelocityForSteer;
    [Tooltip("The minimum angle in degrees the vehicle has to have to the target before it starts steering towards it.")]
    public float minAngleBeforeSteer;

    //: Unity Callbacks

    private void Start()
    {
        Reset();
    }

    private void Update()
    {
        if (_target != null) Debug.DrawLine(transform.position, ((Vector3)_target), Color.red);

        UpdatePhysics();
        UpdateControl();
    }

    //: ITrafficAgent

    float ITrafficAgent.MaxSpeed
    {
        get => maxSpeed;
        set => maxSpeed = value;
    }

    float ITrafficAgent.DistanceToTarget
    {
        get
        {
            if (_target == null)
            {
                return -1;
            }
            
            return (((Vector3)_target) - transform.position).magnitude;
        }
    }

    Vector3? ITrafficAgent.Target
    {
        get => _target;
        set => _target = value;
    }

    Vector3? ITrafficAgent.NextTarget
    {
        get => _nextTarget;
        set => _nextTarget = value;
    }

    float ITrafficAgent.SpeedLimit
    {
        get => _speedLimit;
        set => _speedLimit = value;
    }

    float ITrafficAgent.Speed => _velocity;

    //: Public Methods

    public void Reset()
    {
        _velocity = 0.0f;
        _steering = 0.0f;
        _speedLimit = maxSpeed;

        _target = null;
        _nextTarget = null;
    }

    //: Private Methods

    private void UpdatePhysics()
    {
        transform.Rotate(Vector3.up, _steering * Time.deltaTime);
        transform.position += _velocity * Time.deltaTime * transform.forward;
    }

    private void UpdateControl()
    {
        var change = CalculateAcceleration();
        //Debug.Log("change = " + change);
        if (change > 0.0f)
        {
            _velocity = Mathf.MoveTowards(_velocity, maxSpeed, Mathf.Abs(acceleration * change * Time.deltaTime));
        }
        else if (change < 0.0f)
        {
            _velocity = Mathf.MoveTowards(_velocity, 0.0f, Mathf.Abs(deceleration * change * Time.deltaTime));
        }

        UpdateSteering();
    }

    // Returns one of these values:
    // -1.0f ... 0.0f  Decelerate
    // 0.0f            Keep velocity
    // 0.0f ... 1.0f   Accelerate
    private float CalculateAcceleration()
    {
        if (_target == null)
        {
            return -1.0f;
        }

        var toTarget = ((Vector3)_target) - transform.position;
        toTarget = new Vector3(toTarget.x, 0.0f, toTarget.z);

        float desiredVelocity;
        var headingMultiplier = Vector3.Dot(transform.forward, toTarget.normalized);
        if (headingMultiplier >= 0.0f)
        {
            desiredVelocity = Mathf.Min(_speedLimit, maxSpeed) * headingMultiplier;
        }
        else
        {
            desiredVelocity = minVelocityForSteer;

            if (_velocity > desiredVelocity)
            {
                Debug.Log("(a) Returning " + (_velocity - desiredVelocity) / deceleration);
                return (_velocity - desiredVelocity) / deceleration;
            }
            else
            {
                Debug.Log("(b) Returning " + (desiredVelocity - _velocity) / acceleration);
                return (desiredVelocity - _velocity) / acceleration;
            }
        }

        float breakDistance;
        if (_nextTarget != null)
        {
            var nextTargetHeadingMultiplier = Vector3.Dot(toTarget.normalized, (((Vector3)_nextTarget) - ((Vector3)_target)).normalized);
            var velocityAtTarget = Mathf.Max(0.0f, desiredVelocity * nextTargetHeadingMultiplier);
            var velocityDifference = _velocity - velocityAtTarget;
            Debug.Log("nextTargetHeadingMultiplier = "+nextTargetHeadingMultiplier+", _velocity = "+_velocity+", velocityAtTarget = " + velocityAtTarget+", velocityDifference = "+velocityDifference);
            breakDistance = velocityDifference < 0.0f ? 0.0f : Kinematics.s(v: velocityDifference, a: deceleration);
        }
        else
        {
            breakDistance = Kinematics.s(v: _velocity, a: deceleration);
        }

        float remainingDistance = toTarget.magnitude;
        Debug.Log("breakDistance = "+breakDistance+", remainingDistance = " + remainingDistance);
        Debug.DrawRay(transform.position + 0.1f * Vector3.up, transform.forward * breakDistance, Color.magenta);
        if (remainingDistance <= breakDistance)
        {
            Debug.Log("(c) Returning " + -1.0f);
            return -1.0f;
        }
        else if (_velocity < desiredVelocity)
        {
            Debug.Log("(d) Returning (desiredVelocity - _velocity) / acceleration = ("+desiredVelocity+" - "+_velocity+") / "+acceleration+" =" + (desiredVelocity - _velocity) / acceleration);
            return Mathf.Min((desiredVelocity - _velocity) / acceleration, 1.0f);
        }
        else
        {
            Debug.Log("(e) Returning " + 0.0f);
            return 0.0f;
        }
    }

    private void UpdateSteering()
    {
        if (_target == null)
        {
            return;
        }

        var toTarget = ((Vector3)_target) - transform.position;
        toTarget = new Vector3(toTarget.x, 0.0f, toTarget.z);

        var angleToStopSteering = Kinematics.s(v: _steering, a: unsteerSensitivity);
        var angleToTarget = Vector3.SignedAngle(transform.forward, toTarget, Vector3.up);
        
        // DEBUG
        Debug.DrawRay(transform.position, transform.forward, Color.white);
        Debug.DrawRay(transform.position, toTarget, Color.yellow);
        //Debug.Log("angleToTarget = " + angleToTarget + ", steering = " + _steering + ", toTarget = " + toTarget);
        
        if (Mathf.Abs(angleToTarget) <= angleToStopSteering)
        {
            _steering = Mathf.MoveTowards(_steering, 0.0f, unsteerSensitivity * Time.deltaTime);
        }
        else if (Mathf.Abs(angleToTarget) >= minAngleBeforeSteer && Mathf.Abs(_steering) < maxSteeringAngle)
        {
            _steering = Mathf.MoveTowards(_steering, Mathf.Sign(angleToTarget) * maxSteeringAngle, steerSensitivity * Time.deltaTime);
        }
    }

    //: Private Variables

    private float _velocity;
    private float _steering;
    private float _speedLimit;

    private Vector3? _target;
    private Vector3? _nextTarget;
}
