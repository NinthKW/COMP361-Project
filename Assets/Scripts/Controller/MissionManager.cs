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
        Debug.Log("MissionManager Awake: " + gameObject.scene.name);

        if (Instance == null)
        {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Debug.Log("MissionManager Instance set");
        }
        else {
                Debug.LogWarning("MissionManager duplicate detected, destroying this one");
                Destroy(gameObject);
        }
    }

        void Start()
        {
            Debug.Log("MissionManager Start in scene: " + gameObject.scene.name);
            Init();
        }

        private void Init()
        {
            Debug.Log("Loading Missions...");
            if (missions == null || missions.Count == 0)
            {
                LoadMissions();
            }
        }

        void LoadMissions()
        {
            missions.Clear();

            // 示例任务 1
            missions.Add(new Mission(
                1,
                "Rescue the Scientists",
                "A group of scientists is being held hostage in a secret alien facility. Rescue them before it's too late!",
                difficulty: 2,
                rewardMoney: 5000,
                rewardResourceId: 1,
                rewardTechId: 2,
                terrainId: 1,    // e.g., forest
                weatherId: 3     // e.g., foggy
            ));

            // 示例任务 2
            missions.Add(new Mission(
                2,
                "Destroy the Alien Base",
                "Intel has located an alien base in the desert. Destroy it to stop the invasion!",
                difficulty: 4,
                rewardMoney: 10000,
                rewardResourceId: 2,
                rewardTechId: 3,
                terrainId: 2,    // e.g., desert
                weatherId: 1     // e.g., clear
            ));

            // 示例任务 3
            missions.Add(new Mission(
                3,
                "Escort the Convoy",
                "Protect the convoy carrying vital supplies through dangerous alien territory.",
                difficulty: 3,
                rewardMoney: 7000,
                rewardResourceId: 3,
                rewardTechId: 4,
                terrainId: 3,    // e.g., mountains
                weatherId: 2     // e.g., rain
            ));

            // 示例任务 4
            missions.Add(new Mission(
                4,
                "Investigate the Crash Site",
                "An unidentified object has crashed nearby. Investigate the site and recover any useful technology.",
                difficulty: 1,
                rewardMoney: 3000,
                rewardResourceId: 1,
                rewardTechId: 1,
                terrainId: 4,    // e.g., tundra
                weatherId: 4     // e.g., snowstorm
            ));

            Debug.Log("Missions Loaded: " + missions.Count);
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