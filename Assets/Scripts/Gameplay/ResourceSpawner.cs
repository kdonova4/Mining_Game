using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{

    [Tooltip("Spawn Point")]
    public SphereCollider SpawnPoint;

    [Tooltip("Spawn Resource")]
    public GameObject SpawnObject;

    [Tooltip("Self Spawn Mode")]
    public bool SelfSpawnMode = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(SelfSpawnMode)
            SpawnResource();
    }

    public void SpawnResource()
    {
        Vector3 randomPoint = Random.insideUnitSphere;

        randomPoint *= SpawnPoint.radius;

        Vector3 spawnLocation = SpawnPoint.transform.position + randomPoint;

        Instantiate(SpawnObject, spawnLocation, Quaternion.identity);
    }


    public void SpawnResource(int amount)
    {
        Vector3 randomPoint = Random.insideUnitSphere;

        randomPoint *= SpawnPoint.radius;

        Vector3 spawnLocation = SpawnPoint.transform.position + randomPoint;

        for (int i = 0; i < amount; i++)
        {
            Instantiate(SpawnObject, spawnLocation, Quaternion.identity);
        }

        
    }


}
