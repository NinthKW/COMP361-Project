using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuPageUI : MonoBehaviour // Fardin
{
    public Button baseButton;
    public Button staffButton;
    public Button missionButton;
    public Button techButton;
    public Button researchButton;
    public Button inventoryButton;
    public Button exitButton;


    void Start()
    {
        baseButton.onClick.AddListener(ClickedBase);
        staffButton.onClick.AddListener(ClickedStaff);
        missionButton.onClick.AddListener(ClickedMission);
        techButton.onClick.AddListener(ClickedTech);
        exitButton.onClick.AddListener(ClickedExit);
    }

    void ClickedBase()
    {
        GameManager.Instance.LoadGameState(GameState.BasePage);
    }

    void ClickedStaff()
    {
        GameManager.Instance.LoadGameState(GameState.StaffPage);
    }

    void ClickedMission()
    {
        GameManager.Instance.LoadGameState(GameState.MissionPage);
    }

    void ClickedTech()
    {
        GameManager.Instance.LoadGameState(GameState.TechPage);
    }

    void ClickedResearch()
    {
        GameManager.Instance.LoadGameState(GameState.ResearchPage);
    }

    void ClickedInventory()
        {
            GameManager.Instance.LoadGameState(GameState.InventoryPage);
        }

    void ClickedExit()
        {
            SaveGame.Instance.SaveGame();
            GameManager.Instance.LoadGameState(GameState.WelcomePage);
        }
} // Fardin