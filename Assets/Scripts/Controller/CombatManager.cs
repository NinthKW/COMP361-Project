using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Model;
using System.Data;
using Mono.Data.Sqlite;
using System;
using System.Linq;

namespace Assets.Scripts.Controller 
{
    public class CombatManager : MonoBehaviour
    {
        public static CombatManager Instance;
        
        [Header("Combat Settings")]
        // [SerializeField] private int maxSoldiersAllowed = 5;
        [SerializeField] private float enemyTurnDelay = 1f;
        
        [SerializeField] private List<Soldier> _availableSoldiers = new();
        [SerializeField] private List<Enemy> _availableEnemies = new();
        [SerializeField] private List<Enemy> _waitingEnemies = new();
        [SerializeField] private List<Character> _selectedCharacters = new();
        [SerializeField] private List<Character> _enemyCharacters = new();
        [SerializeField] private string dbPath = "URI=file:" + Application.streamingAssetsPath + "/database.db";

        public Mission currentMission;

        public bool IsCombatActive { get; private set; }
        public bool IsPlayerTurn { get; private set; }
        public System.Action<bool> OnCombatEnd;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
            InitializeAvailableUnits();
        }

        private void InitializeAvailableUnits()
        {
            _availableSoldiers.Clear();

            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();
                
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT 
                            name, 
                            role, 
                            level,
                            exp,
                            health,
                            attack,
                            defense,
                            max_hp
                        FROM Soldier";

                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        try
                        {
                            var role = new Role(reader.GetString(1));
                            var soldier = new Soldier(
                                name: reader.GetString(0),
                                role: role,
                                level: reader.GetInt32(2),
                                health: reader.GetInt32(4),
                                attack: reader.GetInt32(5),
                                defense: reader.GetInt32(6),
                                maxHealth: reader.GetInt32(7)
                            );
                            soldier.GainExp(reader.GetInt32(3)); // 单独设置经验值

                            _availableSoldiers.Add(soldier);
                            Debug.Log($"Loaded soldier: {soldier.Name} ({role.GetRoleName()})");
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Failed to load soldier: {ex.Message}");
                        }
                    }
                }
            }
            // Enemy initialization
            // TODO: Load enemies from database
            //_availableEnemies.Clear();
            //_availableEnemies.Add(new Enemy("Slime", 20, 3, 1, 5));
            //_availableEnemies.Add(new Enemy("Goblin", 30, 5, 1, 10));
            //_availableEnemies.Add(new Enemy("Orc", 60, 10, 2, 20));
            //_availableEnemies.Add(new Enemy("Dragon", 150, 20, 5, 50));
        }
        public Soldier CreateNewSoldier(string soldierName, string roleType)
        {
            try
            {
                var role = new Role(roleType);
                var newSoldier = new Soldier(
                    name: soldierName,
                    role: role,
                    level: 1,
                    health: role.MaxHealth,
                    attack: role.BaseAtk,
                    defense: role.BaseDef,
                    maxHealth: role.MaxHealth
                );
                
                // 将新士兵存入数据库
                using (var connection = new SqliteConnection(dbPath))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            INSERT INTO Soldier 
                                (name, role, level, exp, health, attack, defense)
                            VALUES
                                (@name, @role, @level, @exp, @health, @attack, @defense)";
                        
                        command.Parameters.AddWithValue("@name", soldierName);
                        command.Parameters.AddWithValue("@role", roleType);
                        command.Parameters.AddWithValue("@level", 1);
                        command.Parameters.AddWithValue("@exp", 0);
                        command.Parameters.AddWithValue("@health", role.MaxHealth);
                        command.Parameters.AddWithValue("@attack", role.BaseAtk);
                        command.Parameters.AddWithValue("@defense", role.BaseDef);

                        
                        command.ExecuteNonQuery();
                    }
                }
                
                _availableSoldiers.Add(newSoldier);
                return newSoldier;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to create soldier: {ex.Message}");
                return null;
            }
        }

        public void UpdateInitialEnemies(Mission mission) 
        {
            _availableEnemies.Clear();
            _waitingEnemies.Clear(); // 清空等待敌人列表

            if (mission == null || mission.AssignedEnemies == null || mission.AssignedEnemies.Count == 0)
            {
                Debug.LogError("No enemies assigned to the mission.");
                return;
            }

            foreach (var enemy in mission.AssignedEnemies)
            {
                _availableEnemies.Add(enemy);
                Debug.Log($"Added enemy: {enemy.Name}");
            }
            _availableEnemies = _availableEnemies.GetRange(index: 0, 3); // For testing purposes, limit to 3 enemies
        }

        // 修改后的 StartCombat 方法，传入 Mission 对象和玩家选定的士兵列表
        public void StartCombat(Mission mission, List<Soldier> selectedSoldiers)
        {
            _selectedCharacters.Clear();
            _enemyCharacters.Clear();
            _availableEnemies.Clear();
            _waitingEnemies.Clear(); // 清空等待敌人列表

            if (selectedSoldiers == null || selectedSoldiers.Count == 0)
            {
                Debug.LogError("Cannot start combat: no soldiers selected.");
                return;
            }
            _selectedCharacters.AddRange(selectedSoldiers);

            if (mission == null)
            {
                Debug.LogError("Cannot start combat: mission is null.");
                return;
            }
            if (mission.AssignedEnemies == null || mission.AssignedEnemies.Count == 0)
            {
                Debug.LogError($"Mission '{mission.name}' has no assigned enemies.");
                return;
            }

            //_enemyCharacters.AddRange(mission.AssignedEnemies);

            // 将当前任务的敌人加载到 _availableEnemies 和 _waitingEnemies 中
            for (int i = 0; i < mission.AssignedEnemies.Count; i++)
            {
                if (i < 6)
                {
                    _availableEnemies.Add(mission.AssignedEnemies[i]);  // 用于 UI 显示
                    _enemyCharacters.Add(mission.AssignedEnemies[i]);   // 用于战斗逻辑处理
                }
                else
                {
                    _waitingEnemies.Add(mission.AssignedEnemies[i]); // 剩下的敌人存入等待列表
                }
                Debug.Log($"Added enemy to _availableEnemies: {mission.AssignedEnemies[i].Name}");
            }

            if (_selectedCharacters.Count == 0 || _enemyCharacters.Count == 0)
            {
                Debug.LogError("Cannot start combat with empty units");
                return;
            }

            IsCombatActive = true;
            IsPlayerTurn = true;
            _availableEnemies = _availableEnemies.GetRange(index: 0, 3); // For testing purposes, limit to 3 enemies
            _enemyCharacters = _enemyCharacters.GetRange(index: 0, 3); // For testing purposes, limit to 3 enemies
            _waitingEnemies.Clear(); // 清空等待敌人列表 for testing purposes
            Debug.Log($"Combat started: {_selectedCharacters.Count} vs {_enemyCharacters.Count}");
        }

        public void ProcessAttack(Character attacker, Character target)
        {
            if (!ValidateAttack(attacker, target)) return;
            if (attacker.GameObject == null || target.GameObject == null) return;

            // Execute attack
            attacker.Attack(target);
            
            // Handle experience
            if (target.IsDead() && attacker is Soldier soldier)
            {
                soldier.GainExp((target as Enemy)?.ExperienceReward ?? 0);
            }

            // Cleanup dead units
            CheckAndReplaceDeadEnemies();

            // Check combat status
            if (CheckCombatEnd()) return;
        }

        private bool ValidateAttack(Character attacker, Character target)
        {
            if (!IsCombatActive)
            {
                Debug.LogWarning("Combat is not active");
                return false;
            }

            if (attacker.IsDead() || target.IsDead())
            {
                Debug.LogWarning("Cannot attack with/on dead character");
                return false;
            }

            if (IsPlayerTurn && !_selectedCharacters.Contains(attacker))
            {
                Debug.LogWarning("Not a valid player character");
                return false;
            }

            if (!IsPlayerTurn && !_enemyCharacters.Contains(attacker))
            {
                Debug.LogWarning("Not a valid enemy character");
                return false;
            }

            return true;
        }

        // 在回合结束时调用，检查并补充敌人
        public void CheckAndReplaceDeadEnemies()
        {
            var deadEnemies = _enemyCharacters.Where(e => e.IsDead()).ToList();
            _selectedCharacters.RemoveAll(c => c != null && c.IsDead());
            _enemyCharacters.RemoveAll(c => c != null && c.IsDead());

            foreach (var deadEnemy in deadEnemies)
            {
                if (_waitingEnemies.Count > 0)
                {
                    var newEnemy = _waitingEnemies[0];
                    _waitingEnemies.RemoveAt(0);
                    _availableEnemies.Add(newEnemy);
                    _enemyCharacters.Add(newEnemy);
                    Debug.Log($"Replaced dead enemy with: {newEnemy.Name}");
                }
            }
        }

        private void CleanupDeadUnits()
        {
            _selectedCharacters.RemoveAll(c => c != null && c.IsDead());
            _enemyCharacters.RemoveAll(c => c != null && c.IsDead());
        }

        public bool CheckCombatEnd()
        {
            if (_selectedCharacters.Count == 0)
            {
                EndCombat(false);
                return true;
            }

            if (_enemyCharacters.Count == 0)
            {
                EndCombat(true);
                return true;
            }

            return false;
        }

        private IEnumerator<WaitForSeconds> SwitchTurnRoutine()
        {
            IsPlayerTurn = !IsPlayerTurn;
            Debug.Log($"Turn switched to: {(IsPlayerTurn ? "Player" : "Enemy")}");
            yield return new WaitForSeconds(enemyTurnDelay);
        }

        public Soldier GetRandomSoldier()
        {
            if (_selectedCharacters.Count == 0) return null;
            return (Soldier)_selectedCharacters[UnityEngine.Random.Range(0, _selectedCharacters.Count)];
        }

        public void EndCombat(bool victory)
        {
            IsCombatActive = false;
            Debug.Log($"Combat ended with {(victory ? "victory" : "defeat")}");
            
            // Reward experience
            if (victory)
            {
                _selectedCharacters.ForEach(c => {
                    if (c is Soldier soldier) soldier.GainExp(50);
                });
            }

            OnCombatEnd?.Invoke(victory);
            CleanupCombat();
        }

        private void CleanupCombat()
        {
            _selectedCharacters.Clear();
            _enemyCharacters.Clear();
        }

        public void EndCurrentTurn()
        {
            // IsPlayerTurn = !IsPlayerTurn;
            StartCoroutine(SwitchTurnRoutine());
        }

        public void SetcurrentMission(Mission mission)
        {
            currentMission = mission;
        }

        #region Helper Methods
        public List<Soldier> GetAvailableSoldiers() => new(_availableSoldiers);
        public List<Enemy> GetAvailableEnemies() => new(_availableEnemies);
        public List<Character> GetSelectedCharacters() => new(_selectedCharacters);
        public List<Character> GetEnemyCharacters() => new(_enemyCharacters);
        public List<Enemy> GetWaitingEnemies() => new(_waitingEnemies);
        public bool IsAlly(Character character) => 
            character is Soldier;

        public bool IsEnemy(Character character) => 
            character is Enemy;
        #endregion
    }
}