using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    MainMenu,
    MissionSelect,
    InMission,
    Combat,
    GameOver
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
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            LoadMainMenu();
        }
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;
        Debug.Log("Game State changed to: " + currentState);
    }

    private void LoadSceneIfNotLoaded(string sceneName)
    {
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    public void LoadMainMenu()
    {
        ChangeState(GameState.MainMenu);
        LoadSceneIfNotLoaded("MainMenu");
    }
    public void LoadMissionSelect()
    {
        ChangeState(GameState.MissionSelect);
        LoadSceneIfNotLoaded("MissionSelect");
    }
    public void StartMission(int missionID)
    {
        ChangeState(GameState.InMission);
        LoadSceneIfNotLoaded("MissionScene");
    }

    public void StartCombat()
    {
        ChangeState(GameState.Combat);
        LoadSceneIfNotLoaded("CombatScene");
    }

    public void EndCombat(bool playerWon)
    {
        if (playerWon)
        {
            Debug.Log("Mission Success!");
            ChangeState(GameState.MissionSelect);
            LoadSceneIfNotLoaded("MissionSelect");
        }
        else
        {
            Debug.Log("Game Over!");
            ChangeState(GameState.MainMenu);
            LoadSceneIfNotLoaded("MainMenu");
        }
    }
}
