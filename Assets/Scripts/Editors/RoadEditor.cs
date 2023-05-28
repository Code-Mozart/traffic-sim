using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoadMeshBuilder))]
public class RoadEditor : Editor
{
    private RoadShape shape;
    private RoadMeshBuilder builder;


    public void OnEnable()
    {
        builder = (RoadMeshBuilder)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var shapeBefore = shape;
        shape = (RoadShape)EditorGUILayout.EnumPopup(shape);

        if (shape != shapeBefore)
        {
            builder.Build();
        }
    }


    private IRoadFactory FindFactory(RoadShape shape)
    {
        switch(shape)
        {
            case RoadShape.Straight:
                return new StraightRoadFactory(builder.settings);
            default:
                throw new UnityException("No factory for road shape " + shape);
        }
    }
}

public enum RoadShape
{
    Straight,
    Curve,
    TJunction,
    XJunction
}
