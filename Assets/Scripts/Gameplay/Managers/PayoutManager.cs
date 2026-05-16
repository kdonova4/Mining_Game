using System.Collections.Generic;
using UnityEngine;

public class PayoutManager : MonoBehaviour
{

    public int CopperOreValue = 1;
    public int IronOreValue = 2;
    public int GoldOreValue = 3;

    public int CopperIngotValue = 4;
    public int IronIngotValue = 8;
    public int GoldIngotValue = 12;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int CalculatePayout(HashSet<GameObject> resources)
    {
        int payout = 0;
        foreach(GameObject resource in resources)
        {
            Ore ore = resource.GetComponent<Ore>();
            Resource refined = resource.GetComponent<Resource>();
            if (ore)
            {
                switch (ore.resourceType)
                {
                    case Ore.OreType.Copper:
                        payout += CopperOreValue;
                        break;

                    case Ore.OreType.Iron:
                        payout += IronOreValue;
                        break;

                    case Ore.OreType.Gold:
                        payout += GoldOreValue;
                        break;

                    default:
                        break;
                }
            } else if (refined)
            {
                switch(refined.resourceType)
                {
                    case Resource.ResourceType.Copper:
                        payout += CopperIngotValue;
                        break;

                    case Resource.ResourceType.Iron:
                        payout += IronIngotValue;
                        break;

                    case Resource.ResourceType.Gold:
                        payout += GoldIngotValue;
                        break;
                }
            }
        }

        return payout;
    }
}
