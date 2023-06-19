using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A node in a network. Assign this to an otherwise empty game object.
/// </summary>
public class NetworkNode : MonoBehaviour
{
    public bool isShapingCurve;

    public List<NetworkNode> origins;
    public List<NetworkNode> destinations;
}
