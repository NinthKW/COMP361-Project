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
        int missionId = PlayerPrefs.GetInt("MissionID", 0);
        bool victory = PlayerPrefs.GetInt("CombatResult", 0) == 1;

        if (victory)
        {
            resultText.text = "Mission Success!";
            rewardText.text = "You have earned rewards!";

            soldierExpText.text = "Soldiers' Experience Gained:\n";
            foreach (var soldier in CombatManager.Instance.GetAvailableSoldiers())
            {
                if (soldier != null)
                {
                    soldierExpText.text += $"- {soldier.Name}: Gained 50 EXP\n";
                }
            }
        }
        else
        {
            resultText.text = "Mission Failed!";
            rewardText.text = "Take care of your soldiers!";
        }


        backToMissionButton.onClick.AddListener(backToMissionButtonClicked);
    }

    void backToMissionButtonClicked()
    {
        // Load the mission scene
        GameManager.Instance.ChangeState(GameState.MissionPage);
        GameManager.Instance.LoadGameState(GameState.MissionPage);
    }
}
