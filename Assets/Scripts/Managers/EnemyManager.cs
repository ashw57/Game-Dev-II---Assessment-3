using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum EnemyType
{
    Bonnie,
    Chica,
    Foxy,
    Freddy,
    GoldenFreddy
}
public class EnemyManager : Singleton<EnemyManager>
{

    [SerializeField]
    private List<Transform> spawnPoints;

    [Header("EnemyLists")] //This is the folders ill be storing the enemies in
    [SerializeField]
    private GameObject freddyPrefab;
    [SerializeField]
    private GameObject bonniePrefab;
    [SerializeField]
    private GameObject chicaPrefab;
    [SerializeField]
    private GameObject foxyPrefab;
    [SerializeField]
    private GameObject goldenFreddyPrefab;

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

        if (freddyPrefab == null)
        {
            Debug.LogError("Freddy was not found");
            return;
        }

        if (bonniePrefab == null)
        {
            Debug.LogError("Bonnie was not found");
            return;
        }

        if (chicaPrefab == null)
        {
            Debug.LogError("Chica was not found");
            return;
        }

        if (foxyPrefab == null)
        {
            Debug.LogError("Foxy was not found");
            return;
        }

        if (goldenFreddyPrefab == null)
        {
            Debug.LogError("GoldenFreddy was not found");
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

        if (rndRoll <= 0.23f)
        {
            //Spawn Freddy
            Instantiate(freddyPrefab, spawnPoint);
            return;
        }
        if (rndRoll <= 0.46f)
        {
            //Spawn Bonnie
            Instantiate(bonniePrefab, spawnPoint);
            return;
        }
        if (rndRoll <= 0.69f)
        {
            //Spawn Chica
            Instantiate(chicaPrefab, spawnPoint);
            return;
        }
        if (rndRoll <= 0.92f)
        {
            //Spawn Foxy
            Instantiate(foxyPrefab, spawnPoint);
            return;
        }
        if (rndRoll <= 1f)
        {
            //Spawn GoldenFreddy
            Instantiate(goldenFreddyPrefab, spawnPoint);
            return;
        }
    }

}
