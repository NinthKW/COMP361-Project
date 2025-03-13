using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Model;
using Assets.Scripts.Controller;
using TMPro;



public class MissionSelectUI : MonoBehaviour
{
    public Transform missionButtonContainer; // 容器节点
    public GameObject missionButtonPrefab;   // 任务按钮 Prefab

    public TextMeshProUGUI missionNameText;
    public TextMeshProUGUI missionDescriptionText;

    public Button startButton;               // 启动任务按钮
    public Button backButton;

    private Mission selectedMission;         // 当前选择的任务

    void Start()
    {
        PopulateMissionList();
        startButton.onClick.AddListener(OnStartButtonClicked);
        backButton.onClick.AddListener(backButtonClicked);
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

            if (buttonObj == null)
            {
                Debug.LogError("MissionButtonPrefab failed to instantiate!");
                return;
            }

            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            if (buttonText == null)
            {
                Debug.LogError("No TextMeshProUGUI found in MissionButtonPrefab!");
                return;
            }

            buttonText.text = mission.name;

            Button btn = buttonObj.GetComponent<Button>();
            btn.onClick.AddListener(() => OnMissionSelected(mission));
        }
    }

    void OnMissionSelected(Mission mission)
    {
        selectedMission = mission;
        missionNameText.text = mission.name;
        missionDescriptionText.text = mission.description;
    }

    void OnStartButtonClicked()
    {
        if (selectedMission != null)
        {
            MissionManager.Instance.StartMission(selectedMission.id);
        }
    }

    void backButtonClicked()
    {
        GameManager.Instance.ChangeState(GameState.MainMenuPage);
        GameManager.Instance.LoadGameState(GameState.MainMenuPage);
    }

}