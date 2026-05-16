using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class Ore : MonoBehaviour
{
    public enum OreType
    {
        Iron,
        Copper,
        Gold,
    }
    public OreType resourceType;
    public bool IsDynamic = true;
    [Tooltip("Max Speed")]
    public float MaxSpeed = 10000f;

    [Tooltip("Will ore spawn with generated direction and velocity")]
    public bool StartingDirection = false;
    public float speedThreshold = 0.05f;
    public float requiredStableTime = 0.5f;


    Rigidbody rb;
    float stableTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.solverIterations = 20;
        rb.maxLinearVelocity = MaxSpeed;
        rb.solverVelocityIterations = 10;

        if (StartingDirection)
        {
            Vector3 direction = new Vector3(
                        UnityEngine.Random.Range(-1f, 1f),
                        UnityEngine.Random.Range(0f, 1f),
                        UnityEngine.Random.Range(-1f, 1f)
                    ).normalized;

            rb.linearVelocity = direction * 8f;
            rb.angularVelocity = UnityEngine.Random.insideUnitSphere * 2f;
        }

    }

    void FixedUpdate()
    {
        if (IsDynamic) return;
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
                Destroy(rb);
            }
        }
        else
        {
            stableTime = 0f;
        }
    }


}
