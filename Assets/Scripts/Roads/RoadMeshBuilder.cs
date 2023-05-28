using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadMeshBuilder : MonoBehaviour
{
    public RoadSettings settings;

    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;


    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
    }

    void Start()
    {
    }

    void Update()
    {
    }


    public void Build()
    {
        // TODO: dynamically create a mesh based on the settings in the builder.
        Debug.Log("Building road...");

        var mesh = new Mesh();
        mesh.vertices = new Vector3[]
        {
            // 0
            new Vector3(1, 0, 1),

            // 1
            new Vector3(1, 0, -1),

            // 2
            new Vector3(-1, 0, 1),

            // 3
            new Vector3(-1, 0, -1)
        };
        mesh.triangles = new int[]
        {
            0, 1, 3,
            0, 3, 2
        };

        meshFilter.sharedMesh = mesh;
    }
}
