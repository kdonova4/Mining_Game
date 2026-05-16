using Dono.MiningGame.Game;
using Dono.MiningGame.Gameplay;
using UnityEngine;

public class InteractHandler : MonoBehaviour
{

    public float InteractRange;
    public Camera MainCamera;

    IInteractable m_Interactable;
    PlayerInputHandler m_InputHandler;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_InputHandler = GetComponent<PlayerInputHandler>();
        DebugUtility.HandleErrorIfNullGetComponent<PlayerInputHandler, InteractHandler>(m_InputHandler, this, gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        HandleChecker();

        if (m_InputHandler.GetReloadButtonDown() && m_Interactable != null)
        {
            m_Interactable.Interact();
        }

    }

    void HandleChecker()
    {
        m_Interactable = null;

        if (Physics.Raycast(MainCamera.transform.position, MainCamera.transform.forward, out RaycastHit hit, InteractRange))
        {

            Debug.DrawRay(MainCamera.transform.position, MainCamera.transform.forward, Color.red);

            if (hit.collider.gameObject.GetComponent<IInteractable>() != null)
            {
                m_Interactable = hit.collider.GetComponent<IInteractable>();
            }
        }
    }
}
