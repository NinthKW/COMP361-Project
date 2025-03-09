using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    WelcomePage, //Fardin
    MainMenuPage,
    TechPage,
    MissionPage,
    BasePage,
    StaffPage,
    ResearchPage, //Fardin
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState currentState;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name != "WelcomePage")
        {
            LoadGameState(WelcomePage);
        }
    }

    public void ChangeState(GameState newState)
    {
        Debug.Log("Game State changed to: " + currentState);
    }

    public void LoadGameState(GameState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case WelcomePage:
                SceneManager.LoadScene("WelcomePage");
                break;
            case MainMenuPage:
                 SceneManager.LoadScene("MainMenuPage");
                 break;
            case TechPage:
                 SceneManager.LoadScene("TechPage");
                 break;
            case MissionPage:
                 SceneManager.LoadScene("MissionPage");
                 break;
            case BasePage:
                 SceneManager.LoadScene("BasePage");
                 break;
            case StaffPage:
                 SceneManager.LoadScene("StaffPage");
                 break;
            case ResearchPage:
                 SceneManager.LoadScene("ResearchPage");
                 break;
        }

        ChangeState(newState);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
