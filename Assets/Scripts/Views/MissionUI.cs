using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Model;
using Assets.Scripts.Controller;
using TMPro;



public class MissionSelectUI : MonoBehaviour
{
    public Transform missionButtonContainer; 
    public GameObject missionButtonPrefab;   

    public TextMeshProUGUI missionNameText;
    public TextMeshProUGUI missionDescriptionText;

    public Button startButton;              
    public Button backButton;

    private Mission selectedMission;         

    void Start()
    {
        PopulateMissionList();
        startButton.onClick.AddListener(OnStartButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
    }

    void PopulateMissionList()
    {
        Debug.Log("PopulateMissionList called");
        if (MissionManager.Instance == null)
        {
            Debug.LogError("MissionManager.Instance is NULL");
            return;
        }

        if (MissionManager.Instance.missions == null)
        {
            Debug.LogError("missions list is NULL");
            return;
        }

        Debug.Log("Mission count: " + MissionManager.Instance.missions.Count);

        foreach (var mission in MissionManager.Instance.missions)
        {
            Debug.Log("Adding mission: " + mission.name);

            GameObject buttonObj = Instantiate(missionButtonPrefab, missionButtonContainer);

            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            buttonText.text = mission.name;

            Button btn = buttonObj.GetComponent<Button>();
            btn.onClick.AddListener(() => OnMissionSelected(mission));
        }
    }

    void OnMissionSelected(Mission mission)
    {
        selectedMission = mission;
        missionNameText.text = mission.name;

        
        string details = $"Description: {mission.description}\n" +
                         $"Difficulty: {mission.difficulty}\n\n" +
                         $"Rewards:\n" +
                         $"- Money: {mission.rewardMoney}\n" +
                         $"- Resources (ID): {mission.rewardResourceId} (Amount: {mission.rewardAmount})\n" +
                         $"Terrain: {mission.terrain}\n" +
                         $"Weather: {mission.weather}";


        missionDescriptionText.text = details;
    }

    void OnStartButtonClicked()
    {
        if (selectedMission != null)
        {
            MissionManager.Instance.StartMission(selectedMission.id);
        }
        else
        {
            Debug.LogWarning("No mission selected");
        }
    }

    void OnBackButtonClicked()
    {
        GameManager.Instance.ChangeState(GameState.MainMenuPage);
        GameManager.Instance.LoadGameState(GameState.MainMenuPage);
    }

}