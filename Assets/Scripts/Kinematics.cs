using UnityEngine;

// Contains kniematic functions
public static class Kinematics
{
    // Calculates the distance to reach a given velocity with a given acceleration as s=(v^2)/(2a)
    public static float s(float v, float a)
    {
        return (v * v) / (2 * a);
    }
}
