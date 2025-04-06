using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Model;
using Assets.Scripts.Controller;
using TMPro;

public class TechUI : MonoBehaviour
{
    public Button exitButton;
    public Button unlockButton;
    public TechButton[] techButtons; // Array of all tech buttons
    private UnlockButtonController unlockButtonController;
    
    void Awake()
    {

        // Remove any existing UnlockButtonController components from the entire hierarchy
        var controllers = GetComponentsInChildren<UnlockButtonController>(true);
        foreach (var controller in controllers)
        {
            DestroyImmediate(controller);
        }
        
    }

    void Start()
    {
        exitButton.onClick.AddListener(OnBackButtonClicked1);
        
        // Make sure TechManager is initialized
        if (TechManager.Instance == null)
        {
            Debug.LogError("TechManager not found in scene!");
        }
    }

    void OnBackButtonClicked1()
    {
        GameManager.Instance.ChangeState(GameState.MainMenuPage);
        GameManager.Instance.LoadGameState(GameState.MainMenuPage);
    }
}