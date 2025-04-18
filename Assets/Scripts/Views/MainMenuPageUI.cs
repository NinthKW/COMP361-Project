using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Model;
using Assets.Scripts.Controller;


public class MainMenuPageUI : MonoBehaviour // Fardin, Ziyuan Wang
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
        // researchButton.onClick.AddListener(ClickedResearch);
        inventoryButton.onClick.AddListener(ClickedInventory);
    }

    void ClickedBase()
    {
        BaseManager.Instance.LoadBase();
        InventoryManager.Instance.loadInventory();
        GameManager.Instance.LoadGameState(GameState.BasePage);
    }

    void ClickedStaff()
    {   
        StaffManager.Instance.Load();
        GameManager.Instance.LoadGameState(GameState.StaffPage);
 
    }

    void ClickedMission()
    {   
        MissionManager.Instance.LoadMissionsFromGame();
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
        InventoryManager.Instance.loadInventory();
        GameManager.Instance.LoadGameState(GameState.InventoryPage);
    }

    void ClickedExit()
    {
        GameManager.Instance.SaveGame();
        GameManager.Instance.LoadGameState(GameState.WelcomePage);
    }

} // Fardin,Ziyuan Wang