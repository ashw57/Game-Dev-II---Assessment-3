using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum EnemyType
{
    Tank,
    Ranger,
    Basic
}
public class EnemyManager : Singleton<EnemyManager>
{

    [SerializeField]
    private List<Transform> spawnPoints;

    [Header("EnemyLists")] //This is the folders ill be storing the enemies in
    [SerializeField]
    private List<GameObject> tankPrefabs;
    [SerializeField]
    private List<GameObject> rangerPrefabs;
    [SerializeField]
    private List<GameObject> basicPrefabs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FillSpawnPoints();
    }

    private void FillSpawnPoints()
    {
        if (spawnPoints == null) 
        {
            Debug.LogError("No Spawnpoints found in list"); 
            return;
        }

        if (tankPrefabs == null)
        {
            Debug.LogError("No Tanks found in list");
            return;
        }

        if (rangerPrefabs == null)
        {
            Debug.LogError("No Rangers found in list");
            return;
        }

        if (basicPrefabs == null)
        {
            Debug.LogError("No Basics found in list");
            return;
        }

        foreach (Transform t in spawnPoints) 
        {
            SpawnRandomEnemyType(t);
        }
    }

    private void SpawnRandomEnemyType(Transform spawnPoint)
    {
        float rndRoll = Random.Range(0.0f, 1.0f);

        if (rndRoll <= 0.33f)
        {
            //Spawn Tank
            int rndPrefab = Random.Range(0, tankPrefabs.Count);
            GameObject tankToSpawn = tankPrefabs[rndPrefab];
            Instantiate(tankToSpawn, spawnPoint);
            return;
        }
        else if (rndRoll <= 0.66f)
        {
            //Spawn Ranger
            int rndPrefab = Random.Range(0, rangerPrefabs.Count);
            GameObject rangerToSpawn = rangerPrefabs[rndPrefab];
            Instantiate(rangerToSpawn, spawnPoint);
            return;
        }
        else if (rndRoll >= 0.66f)
        {
            //Spawn Basic
            int rndPrefab = Random.Range(0, basicPrefabs.Count);
            GameObject basicToSpawn = basicPrefabs[rndPrefab];
            Instantiate(basicToSpawn, spawnPoint);
            return;
        }
    }

}
