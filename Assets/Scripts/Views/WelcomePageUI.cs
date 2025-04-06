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

    void Start()
    {
        newGameButton.onClick.AddListener(ClickedNewGame);
        loadGameButton.onClick.AddListener(ClickedLoadGame);
        quitButton.onClick.AddListener(ClickedQuit);
    }

    void ClickedNewGame()
    {
        GameManager.Instance.NewGame();
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
