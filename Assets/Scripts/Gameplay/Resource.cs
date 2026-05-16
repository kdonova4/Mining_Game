using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Resource : MonoBehaviour
{

    
    public enum ResourceType
    {
        Iron,
        Copper,
        Gold,
    }
    public ResourceType resourceType;
    public bool isDynamic;
    public float speedThreshold = 0.05f;
    public float requiredStableTime = 0.5f;
    [Tooltip("Max Speed")]
    public float MaxSpeed = 10000f;

    [Tooltip("Will resource spawn with generated direction and velocity")]
    public bool StartingDirection = false;
   



    Rigidbody rb;
    float stableTime;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.solverIterations = 20;
        rb.maxLinearVelocity = MaxSpeed;
        rb.solverVelocityIterations = 10;
    }

    void FixedUpdate()
    {
        if (isDynamic) return;
        if (rb == null) return;
        if (rb.isKinematic) return;

        if (rb.linearVelocity.sqrMagnitude < speedThreshold * speedThreshold)
        {
            stableTime += Time.fixedDeltaTime;

            if (stableTime >= requiredStableTime)
            {

                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
                
            }
        }
        else
        {
            stableTime = 0f;
        }
    }


}