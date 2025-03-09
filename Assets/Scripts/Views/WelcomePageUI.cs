using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WelcomePageUI : MonoBehaviour // Fardin
{
    public Button newGameButton;
    public Button loadGameButton;
    public Button quitButton;

    void Start()
    {
        newGameButton.onClick.AddListener(ClickedPlay);
        quitButton.onClick.AddListener(ClickedQuit);
    }

    void ClickedNewGame()
    {
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
