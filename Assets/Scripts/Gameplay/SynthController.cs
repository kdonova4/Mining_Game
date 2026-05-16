
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SynthController : MonoBehaviour
{

    public struct StoredObject
    {
        GameObject prefab;
    }

    [Serializable]
    public struct GoldRatio
    {
        public int ore;
        public int ingot;
    }
    [Serializable]
    public struct IronRatio
    {
        public int ore;
        public int ingot;
    }
    [Serializable]
    public struct CopperRatio
    {
        public int ore;
        public int ingot;
    }



    public Transform ResourceSpawnPos;


    [Tooltip("Gold Ore -> Gold Ingot Ratio")]
    public GoldRatio GoldOreToIngotRatio;

    [Tooltip("Gold Ore -> Gold Ingot Ratio")]
    public IronRatio IronOreToIngotRatio;

    [Tooltip("Gold Ore -> Gold Ingot Ratio")]
    public CopperRatio CopperOreToIngotRatio;


    public GameObject CopperIngot;
    public GameObject IronIngot;
    public GameObject GoldIngot;

    [Tooltip("How many ore does it eat per cycle")]
    public int EatRate = 5;

    [Tooltip("They time it takes to eat ore once detected")]
    public float EatTimer = 3.0f;

    [Range(0f, 1f)]
    public float SpawnTime = 0f;

    float m_SpawnTimer;
    float m_EatTimer;
    List<GameObject> m_ItemsToSpawn = new List<GameObject>();
    List<Collider> m_ItemsToEat = new List<Collider>();
    int m_EatCount;

    Dictionary<Ore.OreType, int> m_ResourceCounts = new Dictionary<Ore.OreType, int>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_SpawnTimer = SpawnTime;
        m_EatTimer = EatTimer;
        m_EatCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(m_ItemsToEat.Count <= 0 || m_EatCount >= EatRate)
        {
            m_EatTimer = EatTimer;
            m_EatCount = 0;
        }
        if (m_SpawnTimer >= 0)
        {
            m_SpawnTimer -= Time.deltaTime;
        }
        if(m_EatTimer >= 0 && m_ItemsToEat.Count > 0)
        {
            m_EatTimer -= Time.deltaTime;
        }
        if (m_ItemsToSpawn.Count() > 0 && m_SpawnTimer <= 0)
        {
            SpawnItems();
        }

        if(m_EatTimer <= 0)
        {
            HandleEatItem();
        }

    }

    void SpawnItems()
    {
        
        GameObject item = m_ItemsToSpawn[0];
        
        if (item.GetComponent<Resource>() == null)
        {
            item.transform.position = ResourceSpawnPos.transform.position;
            item.SetActive(true);
            m_ItemsToSpawn.RemoveAt(0);
            m_SpawnTimer = SpawnTime;
        }
        else
        {
            GameObject newObject = Instantiate(item, ResourceSpawnPos.position, Quaternion.identity);
            Rigidbody rb = newObject.GetComponent<Rigidbody>();
            rb.linearVelocity = (ResourceSpawnPos.transform.forward + ResourceSpawnPos.transform.up) * 6f;
            rb.angularVelocity = UnityEngine.Random.insideUnitSphere * 2f;
            m_ItemsToSpawn.RemoveAt(0);
            m_SpawnTimer = SpawnTime;
        }

    }

    void HandleEatItem()
    {
        for(int i = m_ItemsToEat.Count - 1; i >= 0; i--)
        {
            if(m_EatCount < EatRate)
            {
                Collider item = m_ItemsToEat[i];
                if (item.attachedRigidbody != null)
                {
                    Rigidbody rb = item.attachedRigidbody;
                    if (item.gameObject.GetComponent<Ore>() != null)
                    {
                        // get resource type
                        // add it to dictionary
                        Ore ore = item.gameObject.GetComponent<Ore>();

                        m_ResourceCounts[ore.resourceType] = m_ResourceCounts.GetValueOrDefault(ore.resourceType) + 1;
                        Destroy(item.gameObject);
                        HandlePrepareItems();
                    }
                    else // Do something with non consumable items with rigidbodies (push them back)
                    {

                        if (item.gameObject.GetComponent<Resource>() != null)
                        {
                            Resource resource = item.gameObject.GetComponent<Resource>();

                            if (resource.resourceType == Resource.ResourceType.Copper)
                            {
                                m_ItemsToSpawn.Add(CopperIngot);
                            }
                            else if (resource.resourceType == Resource.ResourceType.Iron)
                            {
                                m_ItemsToSpawn.Add(IronIngot);
                            }
                            else if (resource.resourceType == Resource.ResourceType.Gold)
                            {
                                m_ItemsToSpawn.Add(GoldIngot);
                            }
                            Destroy(item.gameObject);
                            m_ItemsToEat.RemoveAt(i);
                            m_EatCount++;
                            continue;
                        }
                        rb.linearVelocity = Vector3.zero;
                        rb.angularVelocity = Vector3.zero;


                        rb.linearVelocity = (ResourceSpawnPos.transform.forward + ResourceSpawnPos.transform.up) * 6f;
                        rb.angularVelocity = UnityEngine.Random.insideUnitSphere * 2f;
                        item.gameObject.SetActive(false);
                        m_ItemsToSpawn.Add(item.gameObject);
                    }
                }
                m_ItemsToEat.RemoveAt(i);
                m_EatCount++;
            }
            
        }
        foreach(Collider item in m_ItemsToEat)
        {
            
        }
        
    }

    void HandlePrepareItems()
    {
        foreach (var key in m_ResourceCounts.Keys.ToList())
        {
            Ore.OreType type = key;
            int resourceAmount = m_ResourceCounts.GetValueOrDefault(key);
            int ingots;
            int remainder;
            switch (type)
            {
                case Ore.OreType.Copper:
                    

                    ingots = (resourceAmount / CopperOreToIngotRatio.ore) * CopperOreToIngotRatio.ingot;
                    remainder = resourceAmount % CopperOreToIngotRatio.ore;
                    HandlePrepareItems(ingots, Resource.ResourceType.Copper);
                   
                    m_ResourceCounts[type] = remainder;
                    break;

                case Ore.OreType.Iron:
                    ingots = resourceAmount / IronOreToIngotRatio.ore;
                    remainder = resourceAmount % IronOreToIngotRatio.ore;
                    HandlePrepareItems(ingots, Resource.ResourceType.Iron);

                    m_ResourceCounts[type] = remainder;
                    break;

                case Ore.OreType.Gold:
                    ingots = resourceAmount / GoldOreToIngotRatio.ore;
                    remainder = resourceAmount % GoldOreToIngotRatio.ore;
                    HandlePrepareItems(ingots, Resource.ResourceType.Gold);

                    m_ResourceCounts[type] = remainder;
                    break;

                default: break;
            }
        }
    }

    void HandlePrepareItems(int amount, Resource.ResourceType type)
    {
        if (amount == 0) return;
        switch (type)
        {
            case Resource.ResourceType.Copper:
                for (int i = 0; i < amount; i++)
                {
                    m_ItemsToSpawn.Add(CopperIngot);
                }
                break;

            case Resource.ResourceType.Iron:
                for (int i = 0; i < amount; i++)
                {
                    m_ItemsToSpawn.Add(IronIngot);
                }
                break;

            case Resource.ResourceType.Gold:
                for (int i = 0; i < amount; i++)
                {
                    m_ItemsToSpawn.Add(GoldIngot);
                }
                break;

            default: break;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        m_ItemsToEat.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {

        m_ItemsToEat.Remove(other);
    }
}
