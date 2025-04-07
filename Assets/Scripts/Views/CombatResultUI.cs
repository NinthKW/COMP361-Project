using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts.Model;
using Assets.Scripts.Controller;
using UnityEngine.EventSystems;


public class CombatResultUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private TextMeshProUGUI soldierExpText;
    [SerializeField] private Button backToMissionButton;

    private void Start()
    {
        // 读取战斗结果
        bool victory = PlayerPrefs.GetInt("CombatResult", 0) == 1;

        // 显示战斗结果信息
        if (victory)
        {
            resultText.text = "Mission Success!";
            resultText.color = Color.green;

            // 显示奖励信息
            string rewardDetails = PlayerPrefs.GetString("RewardDetails", "No Rewards Found.");
            rewardText.text = rewardDetails;

            // 显示士兵经验信息
            soldierExpText.text = "Soldiers' Experience Gained:\n";
            string soldierExpDetails = PlayerPrefs.GetString("SoldierExpDetails", "No Soldiers Found.");
            soldierExpText.text += soldierExpDetails;
        }
        else
        {
            resultText.text = "Mission Failed!";
            resultText.color = Color.red;
            rewardText.text = "Take care of your soldiers!";
            soldierExpText.text = "";
        }

        backToMissionButton.onClick.AddListener(BackToMissionButtonClicked);
    }


    void BackToMissionButtonClicked()
    {
        // Load the mission scene
        GameManager.Instance.ChangeState(GameState.MissionPage);
        GameManager.Instance.LoadGameState(GameState.MissionPage);
        AudioManager.Instance.PlayMusic("Menu");
    }
}
