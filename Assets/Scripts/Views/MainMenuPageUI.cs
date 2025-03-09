using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class MainMenuPageUI : MonoBehaviour
{
    public Button baseButton;
    public Button staffButton;
    public Button missionButton;
    public Button techButton;
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
        GameManager.Instance.LoadBasePage();
    }

    void ClickedStaff()
    {
        GameManager.Instance.LoadStaffPage();
    }

    void ClickedMission()
    {
        GameManager.Instance.LoadMissionPage();
    }

    void ClickedTech()
    {
        GameManager.Instance.LoadTechPage();
    }

    void ClickedExit()
    {
        GameManager.Instance.LoadWelcomePage();
    }
}