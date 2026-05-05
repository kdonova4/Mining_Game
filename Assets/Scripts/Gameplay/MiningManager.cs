using UnityEngine;

public class MiningManager : MonoBehaviour
{
    [Header("Base Stats")]
    [Tooltip("Successful Mine Chance")]
    [SerializeField]
    private float BaseSuccessChance = .1f;

    [Tooltip("Minimum Yield")]
    [SerializeField]
    private int MinimumYield = 1;

    [Tooltip("Maximum Yield")]
    [SerializeField]
    private int MaximumYield = 1;

    [Header("Modifiers")]
    private float ChanceModifier = 0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int MineResult()
    {
        float chance = BaseSuccessChance + ChanceModifier;

        if(Random.value <= chance)
        {
            return Random.Range(MinimumYield, MaximumYield + 1);
        }

        return 0;
    }

    void UpgradeMineChance(float amount)
    {
        ChanceModifier += .05f;
    }
}
