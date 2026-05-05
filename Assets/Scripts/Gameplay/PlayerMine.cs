using Dono.MiningGame.Game;
using Dono.MiningGame.Gameplay;
using UnityEngine;

public class PlayerMine : MonoBehaviour
{

    [Tooltip("Tool Camera")]
    public Camera ToolCamera;

    [Tooltip("Tool Socket")]
    public Transform ToolSocket;

    [Tooltip("Mining Manager")]
    public MiningManager MiningManager;

    [Tooltip("Mining Range")]
    [SerializeField]
    private float miningRange = 5.0f;


    PlayerInputHandler m_InputHandler;
    MiningManager m_MiningManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_InputHandler = GetComponent<PlayerInputHandler>();
        DebugUtility.HandleErrorIfNullGetComponent<PlayerInputHandler, PlayerMine>(m_InputHandler, this, gameObject);

    }

    // Update is called once per frame
    void Update()
    {
        if(m_InputHandler.GetFireInputDown())
        {
            Mine();
        }
    }

    void Mine()
    {
        Vector3 origin = ToolCamera.transform.position;
        Vector3 direction = ToolCamera.transform.forward;


        if (Physics.Raycast(origin, direction, out RaycastHit hit, miningRange))
        {
            Debug.DrawLine(origin, hit.point, Color.red, 4.0f);
            //Debug.Log("Hit: " + hit.collider.name);


            ResourceSpawner resourceSpawner = hit.collider.GetComponent<ResourceSpawner>();
            if (resourceSpawner != null)
            {
                //Debug.Log("HIT RESOURCE");
                int amountMined = MiningManager.MineResult();
                resourceSpawner.SpawnResource(amountMined);
            }
            else
            {
                //Debug.Log("DID NOT HIT");
            }

        }
    }
}
