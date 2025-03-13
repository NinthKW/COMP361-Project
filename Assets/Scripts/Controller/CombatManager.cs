using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Model;
namespace Assets.Scripts.Controller 
{
    public class CombatManager : MonoBehaviour
    {
        public static CombatManager Instance;
        
        [SerializeField] private int maxSoldiersAllowed = 5;
        
        private List<Soldier> availableSoldiers = new();
        private List<Enemy> availableEnemies = new();
        private List<Soldier> selectedSoldiers = new();
        private List<Enemy> missionEnemies = new();
        
        public bool isCombatActive = false;
        private bool isPlayerTurn = true;

        [Header("Combat Visuals")]
        [SerializeField] private Canvas combatCanvas;
        [SerializeField] private GameObject soldierPrefab = default;   // Circle prefab
        [SerializeField] private GameObject enemyPrefab = default;   // Triangle prefab

        // Dictionaries linking visual objects to their data models
        private Dictionary<GameObject, Soldier> soldierVisuals = new();
        private Dictionary<GameObject, Enemy> enemyVisuals = new();

        // Currently selected game objects
        private GameObject selectedSoldierVisual;
        private GameObject selectedEnemyVisual;
        
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
            
            InitializeAvailableSoldiers();
            InitializeAvailableEnemies();

        }
        
        private void InitializeAvailableSoldiers()
        {
            availableSoldiers.Add(new Soldier(new Role(RoleType.Snipper)));
            availableSoldiers.Add(new Soldier(new Role(RoleType.Medic)));
            availableSoldiers.Add(new Soldier(new Role(RoleType.Army)));
            availableSoldiers.Add(new Soldier(new Role(RoleType.Engineer)));
            availableSoldiers.Add(new Soldier(new Role(RoleType.Scott)));
        }
        
        private void InitializeAvailableEnemies()
        {
            availableEnemies.Add(new Enemy("Goblin", 10, 2, 1));
            availableEnemies.Add(new Enemy("Orc", 20, 5, 2));
            availableEnemies.Add(new Enemy("Dragon", 50, 10, 3));
        }

        
        public List<Soldier> GetAvailableSoldiers()
        {
            return availableSoldiers;
        }
        
        public bool SelectSoldier(Soldier soldier)
        {
            if (selectedSoldiers.Count >= maxSoldiersAllowed)
                return false;
                
            selectedSoldiers.Add(soldier);
            return true;
        }
        
        public void SetMissionEnemies(List<Enemy> enemies)
        {
            missionEnemies.Clear();
            missionEnemies.AddRange(enemies);
            Debug.Log($"Mission enemies set: {missionEnemies.Count} enemies");
        }
        
        public void PrepareForCombat()
        {
            if (selectedSoldiers.Count == 0)
            {
                // Use all available soldiers if none selected (for testing)
                selectedSoldiers.AddRange(availableSoldiers);
            }
            
            if (missionEnemies.Count == 0)
            {
                // Use default enemies if none selected
                missionEnemies.AddRange(availableEnemies);
            }
            
            Debug.Log($"Combat prepared with {selectedSoldiers.Count} soldiers and {missionEnemies.Count} enemies");
        }

        public bool StartCombat()
        {
            if (selectedSoldiers.Count == 0 || missionEnemies.Count == 0)
            {
                PrepareForCombat();
            }
            
            isCombatActive = true;
            isPlayerTurn = true; // Player starts first
            Debug.Log("Starting Combat");
            
            return isCombatActive;
        }

        public bool IsPlayerTurn()
        {
            if (!isCombatActive)
            {
                Debug.LogWarning("No active combat");
                return false;
            }
            return isPlayerTurn;
        }

        public void Attack(object attacker, object target, bool endTurn = true)
        {
            // Validate combat state
            if (!isCombatActive) {
                Debug.LogWarning("No active combat");
                return;
            }
            
            // Handle Soldier attacking Enemy
            if (attacker is Soldier soldier && target is Enemy enemy)
            {
                if (!isPlayerTurn) {
                    Debug.LogWarning("Not player's turn");
                    return;
                }
                
                // Perform attack
                enemy.TakeDamage(soldier.GetAttack());
                Debug.Log($"Soldier attacks enemy for {soldier.GetAttack()} damage. Enemy health: {enemy.health}");
                
                // Handle defeated enemy
                if (enemy.health <= 0) {
                    Debug.Log($"Enemy {enemy.name} defeated!");
                    missionEnemies.Remove(enemy);
                }
                
                // Switch turn if requested
                if (endTurn) {
                    isPlayerTurn = false;
                }
            }
            // Handle Enemy attacking Soldier
            else if (attacker is Enemy enemyAttacker && target is Soldier soldierTarget)
            {
                if (isPlayerTurn) {
                    Debug.LogWarning("Not enemy's turn");
                    return;
                }
                
                // Perform attack
                enemyAttacker.Attack(soldierTarget);
                Debug.Log($"{enemyAttacker.name} attacks soldier for {enemyAttacker.damage} damage. Soldier health: {soldierTarget.GetHealth()}");
                
                // Handle defeated soldier
                if (soldierTarget.GetHealth() <= 0) {
                    Debug.Log($"Soldier defeated!");
                    selectedSoldiers.Remove(soldierTarget);
                }
                
                // Switch turn if requested
                if (endTurn) {
                    isPlayerTurn = true;
                }
            }
            else {
                Debug.LogWarning("Invalid attacker/target combination");
                return;
            }
            
            // Check if combat should end
            CheckCombatStatus();
        }

        // Convenience methods that use the unified Attack function
        public void PlayerAttack(int attackingSoldierIndex = 0, int targetEnemyIndex = 0)
        {
            if (attackingSoldierIndex >= 0 && attackingSoldierIndex < selectedSoldiers.Count &&
                targetEnemyIndex >= 0 && targetEnemyIndex < missionEnemies.Count)
            {
                Attack(selectedSoldiers[attackingSoldierIndex], missionEnemies[targetEnemyIndex]);
            }
        }

        public void EnemyAttack()
        {
            if (!isCombatActive || isPlayerTurn) {
                Debug.LogWarning("Not enemy's turn or no active combat");
                return;
            }
            
            foreach (Enemy enemy in new List<Enemy>(missionEnemies))
            {
                if (selectedSoldiers.Count == 0) break;
                
                int targetIndex = Random.Range(0, selectedSoldiers.Count);
                bool isLastEnemy = enemy == missionEnemies[missionEnemies.Count - 1];
                
                Attack(enemy, selectedSoldiers[targetIndex], isLastEnemy);
            }
        }
        
        private void CheckCombatStatus()
        {
            if (IsVictory())
            {
                EndCombat(true);
            }
            else if (IsDefeat())
            {
                EndCombat(false);
            }
        }
        
        public bool IsVictory()
        {
            return isCombatActive && missionEnemies.Count == 0;
        }
        
        public bool IsDefeat()
        {
            return isCombatActive && selectedSoldiers.Count == 0;
        }
        
        private void EndCombat(bool victory)
        {
            isCombatActive = false;
            Debug.Log(victory ? "Combat ended with victory!" : "Combat ended with defeat!");
            
            // Reset combat state
            selectedSoldiers.Clear();
            missionEnemies.Clear();
        }
    }
}
