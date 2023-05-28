using System;
using UnityEngine;

public abstract class AbstractRoadFactory : IRoadFactory
{
	public RoadSettings settings;

	public AbstractRoadFactory(RoadSettings settings)
	{
		this.settings = settings;
	}

    public abstract void ProduceNetwork();
    public abstract Mesh ProduceMesh();
}

