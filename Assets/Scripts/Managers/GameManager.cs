using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public enum GameState
{
    MainMenu,
    Playing,
    Death,
}

public class GameManager : Singleton<GameManager> 
{
    public GameState gameState;

    [SerializeField]
    private int health = 3;

    [SerializeField]
    private int score;

    [SerializeField]
    private GameObject heart1;
    [SerializeField]
    private GameObject heart2;
    [SerializeField]
    private GameObject heart3;

    [SerializeField]
    private TMP_Text scoreText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameState = GameState.MainMenu;
        health = 3;
        score = 0;
        StartCoroutine(scoretime());
    }

    private void Update()
    {
        scoreText.text = "Score: " + score;
    }

    // Update is called once per frame
    public void ResetGame()
    {
        health = 3;
        score = 0;
        gameState = GameState.Playing;
        heart1.SetActive(true);
        heart2.SetActive(true);
        heart3.SetActive(true);

    }

    public void GivePoints(int _score)
    {
        score += _score;
    }
    private IEnumerator scoretime()
    {
        if (gameState != GameState.Playing) { yield return null; }
        score++;
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(scoretime());
    }

    public void OnHit(int _damage)
    {
        health -= _damage;
       
        switch (health)
        {
            case 2:
                heart3.gameObject.SetActive(false); break;
            case 1:
                heart3.gameObject.SetActive(false); 
                heart2.gameObject.SetActive(false); 
                break;
            case 0:
                heart2.gameObject.SetActive(false);
                heart1.gameObject.SetActive(false); 
                break;
        }

        if (health <= 0)
        {
            Cursor.lockState = CursorLockMode.None;
            gameState = GameState.Death;
            SceneManager.LoadScene("GameOver");
        }
    }
}
