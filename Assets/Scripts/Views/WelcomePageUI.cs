using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Model;
using Assets.Scripts.Controller;


public class WelcomePageUI : MonoBehaviour // Fardin
{
    public Button newGameButton;
    public Button loadGameButton;
    public Button quitButton;

    public Button temp;

    void Start()
    {
        newGameButton.onClick.AddListener(ClickedNewGame);
        loadGameButton.onClick.AddListener(ClickedLoadGame);
        quitButton.onClick.AddListener(ClickedQuit);
        temp.onClick.AddListener(tempFunction);
    }

    void tempFunction()
    {
        GameManager.Instance.LoadGame();
        GameManager.Instance.LoadGameState(GameState.HospitalPage);
    }

    void ClickedNewGame()
    {
        GameManager.Instance.LoadGame();
        GameManager.Instance.LoadGameState(GameState.MainMenuPage);
    }
    
    void ClickedLoadGame()
    {
        GameManager.Instance.LoadGame();
    }
    void ClickedQuit()
    {
        GameManager.Instance.QuitGame();
    }
} // Fardin
