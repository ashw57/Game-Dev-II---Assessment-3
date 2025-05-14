using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : GameBehaviour
{
    [SerializeField]
    private EnemyType enemyType;

    [SerializeField]
    private int myHealth;
    [SerializeField]
    private float mySpeed;
    [SerializeField]
    private int myMaxHealth;

    public Transform playerTransform;       // Reference to the player
    private NavMeshAgent agent;             // NavMeshAgent component

    [Header("Chica Variables")]
    public float chicaWanderRadius = 5f;
    public float chicaWanderTimer = 3f;
    public float chicaTimer;
    public bool chicaIsHealing = false;

    [Header("Freddy Variables")]
    public float freddyWaitingRange = 20f;
    public float freddyWaitDuration = 5f;
    private float freddyWaitTimer;
    private bool freddyWaitingAtRange = false;
    private bool freddyClosingIn = false;

    [SerializeField]
    private ParticleSystem _PS;
    


    public void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent on this GameObject
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else { print("Couldn't find player"); }

        

        Initiate();
    }

    public void Initiate()
    {
        switch (enemyType) 
        {
            case EnemyType.Freddy:
                {
                    myHealth = 100;
                    mySpeed = 100;

                    freddyWaitTimer = freddyWaitDuration;

                    return;
                }
            case EnemyType.Bonnie:
                {
                    myHealth = 300;
                    mySpeed = 150;
                    return;
                }
            case EnemyType.Chica: 
                {
                    myHealth = 50;
                    mySpeed = 100;
                    chicaTimer = chicaWanderTimer;
                    return;
                }
            case EnemyType.Foxy:
                {
                    myHealth = 50;
                    mySpeed = 200;
                    return;
                }
            case EnemyType.GoldenFreddy:
                myHealth = 200;
                mySpeed = 50;
                return;
        }
        agent.speed = mySpeed;
    }

    public void Update()
    {
        float distance = Vector3.Distance(agent.transform.position, playerTransform.position);

        switch (enemyType)
        {
            case EnemyType.Bonnie:
            {
                if (playerTransform != null)
                {
                    agent.SetDestination(playerTransform.position); // Move toward the player
                }
                break;
            }
            case EnemyType.Chica:

                if (myHealth <= myMaxHealth && !chicaIsHealing)
                {
                    StartCoroutine(ChicaHealing());
                }

                int chicaDetectionRange = 20;
                if (distance > chicaDetectionRange) 
                {
                    if (chicaTimer >= chicaWanderTimer)
                    {
                        Vector3 newPos = RandomNavmeshLocation(chicaWanderRadius);
                        agent.SetDestination(newPos);
                        chicaTimer = 0;
                    }
                }
                if (distance < chicaDetectionRange) 
                {
                    if (playerTransform != null)
                    {
                        agent.SetDestination(playerTransform.position); // Move toward the player
                    }
                }
                break;

            case EnemyType.Foxy:
                int foxyDashRange = 10;
                float dashSpeed = mySpeed * 1.1f;
                if (playerTransform != null)
                {
                    agent.SetDestination(playerTransform.position); // Move toward the player
                }

                if (distance < foxyDashRange)
                {
                    agent.speed = dashSpeed;
                }
                else { agent.speed = mySpeed; }
                break;
            
            case EnemyType.Freddy:
                if (playerTransform == null || agent == null)
                    return;

                float distanceToPlayer = Vector3.Distance(agent.transform.position, playerTransform.position);

                if (!freddyClosingIn)
                {
                    if (distanceToPlayer > freddyWaitingRange)
                    {
                        // Approach to waiting range
                        agent.SetDestination(playerTransform.position);
                        freddyWaitingAtRange = false;
                        freddyWaitTimer = freddyWaitDuration; // reset timer if too far
                    }
                    else
                    {
                        if (!freddyWaitingAtRange)
                        {
                            agent.ResetPath(); // Stop when within range
                            freddyWaitingAtRange = true;
                        }

                        // Countdown
                        freddyWaitTimer -= Time.deltaTime;
                        if (freddyWaitTimer <= 0f)
                        {
                            freddyClosingIn = true;
                        }
                    }
                }
                else
                {
                    // Now Freddy closes in
                    agent.SetDestination(playerTransform.position);
                }
                break;
            case EnemyType.GoldenFreddy:
                if (playerTransform != null)
                {
                    agent.SetDestination(playerTransform.position); // Move toward the player
                }
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("collision " + other.name);

        if (other.CompareTag("Player"))
        {
            if (enemyType == EnemyType.GoldenFreddy)
            {
                _GM.OnHit(2);
                DestroySelf();
            }
            else
            {
                _GM.OnHit(1);
                DestroySelf();
            } 
        }



        if (other.CompareTag("Weapon"))
        {
            if (enemyType == EnemyType.GoldenFreddy)
            {
                _GM.GivePoints(200);
                DestroySelf();
            }
            else
            {
                _GM.GivePoints(100);
                DestroySelf();
            }
        }
        
    }

    Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return transform.position; // fallback if no valid point found
    }

    private IEnumerator ChicaHealing()
    {
        chicaIsHealing = true;
        yield return new WaitForSeconds(0.25f);
        myHealth++;
        chicaIsHealing = false;
    }

    private void DestroySelf()
    {
        Instantiate(_PS,transform); 
        Destroy(gameObject);
    }

}
