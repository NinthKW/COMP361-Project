using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WelcomePageUI : MonoBehaviour // Fardin
{
    public Button playButton;
    public Button quitButton;

    void Start()
    {
        playButton.onClick.AddListener(ClickedPlay);
        quitButton.onClick.AddListener(ClickedQuit);
    }

    void ClickedPlay()
    {
        GameManager.Instance.LoadGameState(GameState.MainMenuPage)();
    }
    
    void ClickedQuit()
    {
        GameManager.Instance.QuitGame();
    }
} // Fardin
