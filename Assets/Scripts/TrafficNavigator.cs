using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficNavigator : MonoBehaviour
{
    //: Editor Properties

    [Header("References")]
    public INetworkAgent agent;

    [Header("Thresholds")]

    public float nodeReachedThreshold = 1.0f;
    public int maxResetAttempts = 100;

    [Header("Distance Holder")]

    public float targetDistanceToTraffic = 10f;

    public LayerMask trafficLayer;

    public float scanningAngle = 90f;
    public int scanningSteps = 5;

    //: Unity Callbacks

    private void Start()
    {
        agent = GetComponent<INetworkAgent>();
    }

    private void Update()
    {
        scanTrafficForward();
    }

    //: Private Methods
    private void scanTrafficForward()
    {
        bool hasHit = false;
        Vector3 location = Vector3.zero;
        for(int i = 0; i<scanningSteps; i++){
            RaycastHit hit;
            float angle = (scanningAngle/scanningSteps)*i-(scanningAngle/2);
            Debug.DrawRay(transform.position, Quaternion.AngleAxis(angle, transform.up) * transform.forward * targetDistanceToTraffic, Color.red);
            Vector3 direction = Quaternion.AngleAxis(angle, transform.up) * transform.forward;
            if (Physics.Raycast(transform.position, direction, out hit, targetDistanceToTraffic,trafficLayer))
            {
                    print("TrafficAgent ahead");
                    location = hit.collider.gameObject.transform.position;
                    hasHit = true;
            }
        }

        if (hasHit == true)
            {
                agent.SpeedLimit = 0;
            } else {
                agent.SpeedLimit = agent.MaxSpeed;
            }
            
    }
}
