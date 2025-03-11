using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Model;

namespace Assets.Scripts.Controller 
{
    public class MissionManager : MonoBehaviour
    {
        public static MissionManager Instance;
        public List<Mission> missions = new List<Mission>();

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        void Start()
        {
            LoadMissions();
        }

        void LoadMissions()
        {
            missions.Add(new Mission(1, "Rescue Scientists", "Save the captured scientists."));
            missions.Add(new Mission(2, "Destroy Alien Base", "Eliminate all alien forces in the area."));
        }

        public void StartMission(int missionID)
        {
            Mission selectedMission = missions.Find(m => m.id == missionID);
            if (selectedMission != null)
            {
                Debug.Log("Starting Mission: " + selectedMission.name);
                GameManager.Instance.ChangeState(GameState.MissionPage);
                Invoke(nameof(StartCombat), 2f);
            }
        }

        void StartCombat()
        {
            // Make sure the CombatManager is active
            if (!CombatManager.Instance.gameObject.activeSelf)
            {
                CombatManager.Instance.gameObject.SetActive(true);
            }
            CombatManager.Instance.StartCombat();
            GameManager.Instance.ChangeState(GameState.CombatPage);
        }
    }
}