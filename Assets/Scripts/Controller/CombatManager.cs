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
        [SerializeField] private List<Character> _selectedCharacters = new();
        [SerializeField] private List<Character> _enemyCharacters = new();
        [SerializeField] private string dbName = "URI=file:database.db"; 
        
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

            using (var connection = new SqliteConnection(dbName))
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
                            defense 
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
                                defense: reader.GetInt32(6)
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
            _availableEnemies.Clear();
            _availableEnemies.Add(new Enemy("Slime", 20, 3, 1, 5));
            _availableEnemies.Add(new Enemy("Goblin", 30, 5, 1, 10));
            _availableEnemies.Add(new Enemy("Orc", 60, 10, 2, 20));
            _availableEnemies.Add(new Enemy("Dragon", 150, 20, 5, 50));
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
                    defense: role.BaseDef
                );
                
                // 将新士兵存入数据库
                using (var connection = new SqliteConnection(dbName))
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

        public void StartCombat(List<Soldier> selectedSoldiers, List<Enemy> missionEnemies)
        {
            _selectedCharacters.Clear();
            _enemyCharacters.Clear();

            _selectedCharacters.AddRange(selectedSoldiers);
            _enemyCharacters.AddRange(missionEnemies);

            if (_selectedCharacters.Count == 0 || _enemyCharacters.Count == 0)
            {
                Debug.LogError("Cannot start combat with empty units");
                return;
            }

            IsCombatActive = true;
            IsPlayerTurn = true;
            Debug.Log($"Combat started: {_selectedCharacters.Count(c => c != null)} vs {_enemyCharacters.Count(c => c != null)}");
        }

        public void ProcessAttack(Character attacker, Character target)
        {
            if (!ValidateAttack(attacker, target)) return;

            // Execute attack
            attacker.Attack(target);
            
            // Handle experience
            if (target.IsDead() && attacker is Soldier soldier)
            {
                soldier.GainExp((target as Enemy)?.ExperienceReward ?? 0);
            }

            // Cleanup dead units
            CleanupDeadUnits();

            // Check combat status
            if (CheckCombatEnd()) return;

            // Auto switch turns
            if (ShouldSwitchTurn(attacker))
            {
                StartCoroutine(SwitchTurnRoutine());
            }
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

        private bool ShouldSwitchTurn(Character attacker)
        {
            if (!IsPlayerTurn)
            {
                // Enemies act sequentially
                return attacker == _enemyCharacters[^1];
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

        #region Helper Methods
        public List<Soldier> GetAvailableSoldiers() => new(_availableSoldiers);
        public List<Enemy> GetAvailableEnemies() => new(_availableEnemies);
        public List<Character> GetSelectedCharacters() => new(_selectedCharacters);
        public List<Character> GetEnemyCharacters() => new(_enemyCharacters);
        public bool IsAlly(Character character) => 
            _selectedCharacters.Contains(character);

        public bool IsEnemy(Character character) => 
            _enemyCharacters.Contains(character);
        #endregion
    }
}