using UnityEngine;

public class CreditsManager : MonoBehaviour
{
    public long TotalCredits { get; private set; }
    public long TotalCreditsAccrued { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TotalCredits = 0;
        TotalCreditsAccrued = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddCredits(int credts)
    {
        TotalCredits += credts;
        TotalCreditsAccrued += credts;
    }

    public void Withdraw(int credts)
    {
        if(TotalCredits >= credts)
        {
            TotalCredits -= credts;
        }
        else
        {
            TotalCredits = 0;
        }
    }
}
