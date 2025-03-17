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

       

    void Start()
    {
        
        exitButton.onClick.AddListener(OnBackButtonClicked1);
    }

   
    void OnBackButtonClicked1()
    {
        GameManager.Instance.ChangeState(GameState.MainMenuPage);
        GameManager.Instance.LoadGameState(GameState.MainMenuPage);
    }

}