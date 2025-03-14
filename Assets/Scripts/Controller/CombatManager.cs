using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Model;

namespace Assets.Scripts.Controller 
{
    public class CombatManager : MonoBehaviour
    {
        public static CombatManager Instance;
        
        [Header("Combat Settings")]
        [SerializeField] private int maxSoldiersAllowed = 5;
        [SerializeField] private float enemyTurnDelay = 1f;
        
        private List<Soldier> _availableSoldiers = new();
        private List<Enemy> _availableEnemies = new();
        private List<Character> _selectedCharacters = new();
        private List<Character> _enemyCharacters = new();
        
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
            // Soldier initialization
            _availableSoldiers.Add(new Soldier(new Role(RoleType.Sniper)));
            _availableSoldiers.Add(new Soldier(new Role(RoleType.Medic)));
            _availableSoldiers.Add(new Soldier(new Role(RoleType.Army)));
            _availableSoldiers.Add(new Soldier(new Role(RoleType.Engineer)));
            _availableSoldiers.Add(new Soldier(new Role(RoleType.Scott)));

            // Enemy initialization
            _availableEnemies.Add(new Enemy("Goblin", 30, 5, 1, 10));
            _availableEnemies.Add(new Enemy("Orc", 60, 10, 2, 20));
            _availableEnemies.Add(new Enemy("Dragon", 150, 20, 5, 50));
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
            Debug.Log($"Combat started: {_selectedCharacters.Count} vs {_enemyCharacters.Count}");
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
            _selectedCharacters.RemoveAll(c => c.IsDead());
            _enemyCharacters.RemoveAll(c => c.IsDead());
        }

        private bool CheckCombatEnd()
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
            if (IsPlayerTurn)
            {
                // Player completes all actions before switching
                return attacker == _selectedCharacters[^1];
            }
            else
            {
                // Enemies act sequentially
                return attacker == _enemyCharacters[^1];
            }
        }

        private IEnumerator<WaitForSeconds> SwitchTurnRoutine()
        {
            yield return new WaitForSeconds(enemyTurnDelay);
            
            IsPlayerTurn = !IsPlayerTurn;
            Debug.Log($"Turn switched to: {(IsPlayerTurn ? "Player" : "Enemy")}");

            if (!IsPlayerTurn)
            {
                StartEnemyTurn();
            }
        }

        private void StartEnemyTurn()
        {
            foreach (Enemy enemy in _enemyCharacters)
            {
                if (enemy.IsDead()) continue;
                
                Soldier target = GetRandomSoldier();
                if (target != null)
                {
                    ProcessAttack(enemy, target);
                }
            }
        }

        private Soldier GetRandomSoldier()
        {
            if (_selectedCharacters.Count == 0) return null;
            return (Soldier)_selectedCharacters[Random.Range(0, _selectedCharacters.Count)];
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
            if (IsPlayerTurn)
            {
                IsPlayerTurn = false;
                StartCoroutine(SwitchTurnRoutine());
            }
        }

        #region Helper Methods
        public List<Soldier> GetAvailableSoldiers() => new(_availableSoldiers);
        public List<Enemy> GetAvailableEnemies() => new(_availableEnemies);
        
        public bool IsAlly(Character character) => 
            _selectedCharacters.Contains(character);

        public bool IsEnemy(Character character) => 
            _enemyCharacters.Contains(character);
        #endregion
    }
}