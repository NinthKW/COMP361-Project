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

        // 数据库文件名
        private string dbPath;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                // 这里是重点：StreamingAssets 路径
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
            LoadMissionsFromDatabase();
        }

        void LoadMissionsFromDatabase()
        {
            missions.Clear();

            Debug.Log("Trying to open DB at: " + dbPath);

            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();
                Debug.Log("Database Opened Successfully!");

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Mission;";

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string missionName = reader.GetString(1);
                            string description = reader.GetString(2);
                            int difficulty = reader.GetInt32(3);
                            int rewardMoney = reader.GetInt32(4);
                            int rewardAmount = reader.GetInt32(5); 
                            int rewardResourceId = reader.GetInt32(6);
                            string terrain = reader.GetString(7);   
                            string weather = reader.GetString(8);   
                            bool unlocked = reader.GetBoolean(9);

                            // 创建 Mission 对象
                            Mission mission = new Mission(
                                id,
                                missionName,
                                description,
                                difficulty,
                                rewardMoney,
                                rewardAmount,
                                rewardResourceId,
                                terrain,
                                weather,
                                unlocked
                            );

                            LoadMissionEnemiesFromDatabase(mission);

                            missions.Add(mission);

                            Debug.Log($"Loaded Mission: {missionName} (Difficulty: {difficulty}, Terrain: {terrain}, Weather: {weather})");
                            
                        }

                        reader.Close();
                    }
                }

                connection.Close();
                Debug.Log($"Total Missions Loaded: {missions.Count}");
            }
        }



        void LoadMissionEnemiesFromDatabase(Mission mission)
        {
            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                SELECT 
                    MISSION_ENEMY.et_id,
                    MISSION_ENEMY.count,
                    ENEMY_TYPES.et_name,
                    ENEMY_TYPES.HP,
                    ENEMY_TYPES.base_ATK,
                    ENEMY_TYPES.base_DPS
                FROM MISSION_ENEMY
                INNER JOIN ENEMY_TYPES ON MISSION_ENEMY.et_id = ENEMY_TYPES.et_ID
                WHERE MISSION_ENEMY.mission_id = @missionId;
            ";

                    command.Parameters.AddWithValue("@missionId", mission.id);

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int count = reader.GetInt32(1);
                            string name = reader.GetString(2);
                            int hp = reader.GetInt32(3);
                            int attack = reader.GetInt32(4);
                            int dps = reader.GetInt32(5);
                            int maxHealth = hp; // 假设最大生命值等于当前生命值

                            for (int i = 0; i < count; i++)
                            {
                                var enemy = new Enemy(name, hp, attack, dps, maxHealth, 1, 10); // TODO: add level and exp to database
                                mission.AssignedEnemies.Add(enemy);
                            }

                            Debug.Log($"Loaded {count} {name}(s) for Mission {mission.name}");
                        }
                    }
                }

                connection.Close();
            }
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
