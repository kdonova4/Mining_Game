using UnityEngine;

public class Resource : MonoBehaviour
{
    private Rigidbody rb;
    private bool grounded;
    private float stableTime;

    [SerializeField] private bool isDynamic;
    [SerializeField] private float speedThreshold = 0.05f;
    [SerializeField] private float requiredStableTime = 0.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.solverIterations = 20;
        rb.solverVelocityIterations = 10;
        Vector3 direction = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(0.5f, 1f),
            Random.Range(-1f, 1f)
        ).normalized;

        rb.linearVelocity = direction * 3f;
        rb.angularVelocity = Random.insideUnitSphere * 2f;
    }

    void FixedUpdate()
    {
        if (isDynamic) return;
        if(rb == null) return;
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