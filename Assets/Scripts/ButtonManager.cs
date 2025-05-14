using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : Singleton<ButtonManager>
{

    public void QuitGame()
    {
        Application.Quit();
    }
    public void loadLevel()
    {
        SceneManager.LoadScene("Pizzaria");
    }

    public void loadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
