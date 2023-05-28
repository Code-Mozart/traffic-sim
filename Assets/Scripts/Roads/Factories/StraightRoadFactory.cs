using System;
using UnityEngine;

/// <summary>
/// Produces straight roads.
/// </summary>
public class StraightRoadFactory : AbstractRoadFactory
{
    public StraightRoadFactory(RoadSettings settings) : base(settings)
    {
    }

    public override void ProduceNetwork()
    {

    }

    public override Mesh ProduceMesh()
    {
        return null;
    }
}
