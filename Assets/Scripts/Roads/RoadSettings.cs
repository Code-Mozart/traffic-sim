using System;
using UnityEngine;

[System.Serializable]
public struct RoadSettings
{
    public Lane[] lanes;
}

[System.Serializable]
public struct Lane
{
    [Header("Road Settings")]

    public LaneDirection direction;
    public LaneType type;
    public float width;

    [Header("Mesh Settings")]

    [Tooltip("The positive height offset of this lane. Mostly relevant for the height difference between road and sidewalk.")]
    public float height;
    [Tooltip("The resolution/number of vertecies for curves. Determines the smootheness of the curve.")]
    public int curveResolution;

    [Header("Road Rules")]

    public float maxSpeed;
}

[System.Serializable]
public enum LaneDirection
{
    Forward,
    Backward
}

[System.Serializable]
public enum LaneType
{
    Pedestrian,
    Vehicles
}
