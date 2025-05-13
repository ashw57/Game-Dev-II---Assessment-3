using UnityEngine;

public enum GameState
{
    MainMenu,
    Playing,
    Death,
}

public class GameManager : Singleton<GameManager>  
{
    public GameState gameState;

    // ------------------ //
    //       Health       //
    // ------------------ //

    private int health = 3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }



    public void OnHit(int _damage)
    {
        health -= _damage;
        if (health <= 0)
        {
            gameState = GameState.Death;
        }
    }
}
