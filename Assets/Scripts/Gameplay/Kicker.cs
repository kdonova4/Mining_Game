using Dono.MiningGame.Game;
using UnityEngine;

public class Kicker : MonoBehaviour
{
    [Tooltip("Kick force against resources")]
    [Range(0f, 8f)]
    public float KickForce = 2.0f;

    [Tooltip("Interaction Radius")]
    public float InteractionRadius = 2.0f;

    [Tooltip("Layer")]
    public LayerMask Layer;

    CharacterController m_CharacterController;
    void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
        DebugUtility.HandleErrorIfNullGetComponent<CharacterController, Kicker>(m_CharacterController, this, gameObject);
    }

    void FixedUpdate()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, InteractionRadius, Layer);

        foreach (var hitCollider in hitColliders)
        {
            Rigidbody rb = hitCollider.attachedRigidbody;

            if(rb != null)
            {
                Vector3 pushDir = hitCollider.transform.position - transform.position;
                pushDir.y = 0;

                rb.AddForce(pushDir.normalized * KickForce, ForceMode.VelocityChange);
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
}
