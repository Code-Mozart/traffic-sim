using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public float MaxSpeed = 1.0f;
    public float MinSpeed = 0.75f;

    // Start is called before the first frame update
    void Start()
    {
        var renderer = GetComponent<Renderer>();
        var agent = GetComponent<INetworkAgent>();
        renderer.material.color = Random.ColorHSV();

        agent.MaxSpeed = Random.Range(MinSpeed, MaxSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
