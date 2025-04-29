using UnityEngine;

public class Enemy : GameBehaviour
{
    [SerializeField]
    private EnemyType enemyType;

    [SerializeField]
    private int myHealth;
    [SerializeField]
    private float mySpeed;
    [SerializeField] 
    private int myDamage;
    [SerializeField]
    private int myMaxHealth;
    public void Initiate()
    {
        switch (enemyType) 
        {
            case EnemyType.Basic:
                {
                    myHealth = 100;
                    mySpeed = 100;
                    myDamage = 10;
                    return;
                }
            case EnemyType.Tank:
                {
                    myHealth = 300;
                    mySpeed = 50;
                    myDamage = 30;
                    return;
                }
            case EnemyType.Ranger: 
                {
                    myHealth = 50;
                    mySpeed = 150;
                    myDamage = 15;
                    return;
                }
        }
    }

    public void Hit(int _inDamage)
    {
        myHealth -= _inDamage;
        if (myHealth <= 0) 
        {
            myHealth = 0;
            Die();
        }
    }

    public void Die() 
    {
        Destroy(gameObject);
    }
}
