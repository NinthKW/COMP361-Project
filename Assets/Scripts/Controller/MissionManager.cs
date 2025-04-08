using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Model;
using System.Data;
using Mono.Data.Sqlite;

namespace Assets.Scripts.Controller
{
    public class MissionManager : MonoBehaviour
    {
        public static MissionManager Instance;
        public List<Mission> missions = new List<Mission>();

        private string dbPath;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                dbPath = "URI=file:" + Application.streamingAssetsPath + "/database.db";
                Debug.Log("Database path: " + dbPath);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {

        }


        public void LoadMissionsFromGame()
        {
            missions.Clear();
            missions.AddRange(Game.Instance.MissionsData);
            Debug.Log($"Loaded {missions.Count} missions from Game instance.");
        }


        public void StartMission(Mission selectedMission)
        {
            CombatManager.Instance.SetcurrentMission(selectedMission);
            if (selectedMission != null)
            {
                Debug.Log("Starting Mission: " + selectedMission.name);
                Invoke(nameof(StartCombat), 0.5f);
            }
            else
            {
                Debug.LogWarning("Mission not found: ");
            }
        }

        void StartCombat()
        {
            if (!CombatManager.Instance.gameObject.activeSelf)
            {
                CombatManager.Instance.gameObject.SetActive(true);
            }

            GameManager.Instance.LoadGameState(GameState.MissionPreparationPage);
        }
    }
}
