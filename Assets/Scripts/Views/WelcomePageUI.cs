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
        BaseManager.Instance.LoadBase();
        GameManager.Instance.LoadGameState(GameState.LoadoutPage);
    }

    void ClickedNewGame()
    {
        GameManager.Instance.NewGame();
        GameManager.Instance.LoadGameState(GameState.MainMenuPage);
    }
    
    void ClickedLoadGame()
    {
        GameManager.Instance.LoadGame();
        GameManager.Instance.LoadGameState(GameState.MainMenuPage);
    }
    void ClickedQuit()
    {
        GameManager.Instance.QuitGame();
    }
} // Fardin
