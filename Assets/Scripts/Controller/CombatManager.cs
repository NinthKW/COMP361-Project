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
        #region Singleton and Initialization
        public static CombatManager Instance;
        
        [Header("Combat Settings")]
        // [SerializeField] private int maxSoldiersAllowed = 5;
        [SerializeField] private float enemyTurnDelay = 1f;
        
        [SerializeField] private List<Soldier> _availableSoldiers = new();
        [SerializeField] private List<Enemy> _availableEnemies = new();
        [SerializeField] private List<Enemy> _waitingEnemies = new();
        [SerializeField] private List<Soldier> _inBattleSoldiers = new();
        [SerializeField] private List<Character> _inBattleEnemies = new();
        private readonly string dbPath = "URI=file:" + Application.streamingAssetsPath + "/database.db";

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
        #endregion

        #region Database Loading
        private void InitializeAvailableUnits()
        {
            _availableSoldiers.Clear();
            _availableSoldiers = Game.Instance.GetSoldiers();
            Debug.Log($"Loaded {_availableSoldiers.Count} soldiers from Game instance.");

            // using (var connection = new SqliteConnection(dbPath))
            // {
            //     connection.Open();
                
            //     using (var command = connection.CreateCommand())
            //     {
            //         command.CommandText = @"
            //             SELECT 
            //                 name, 
            //                 role, 
            //                 level,
            //                 hp,
            //                 atk,
            //                 def,
            //                 max_hp
            //             FROM Soldier";

            //         using var reader = command.ExecuteReader();
            //         while (reader.Read())
            //         {
            //             try
            //             {
            //                 var role = new Role(reader.GetString(1));
            //                 var soldier = new Soldier(
            //                     name: reader.GetString(0),
            //                     role: role,
            //                     level: reader.GetInt32(2),
            //                     health: reader.GetInt32(3),
            //                     attack: reader.GetInt32(4),
            //                     defense: reader.GetInt32(5),
            //                     maxHealth: reader.GetInt32(6)
            //                 );
            //                 soldier.GainExp(reader.GetInt32(3)); // 单独设置经验值

            //                 _availableSoldiers.Add(soldier);
            //                 Debug.Log($"Loaded soldier: {soldier.Name} ({role.GetRoleName()})");
            //             }
            //             catch (Exception ex)
            //             {
            //                 Debug.LogError($"Failed to load soldier: {ex.Message}");
            //             }
            //         }
            //     }
            // }
        }
        #endregion

        #region Soldier Management
        public void AddSoldier(Soldier soldier)
        {
            if (soldier == null) return;
            if (_availableSoldiers.Contains(soldier)) return;
            
            _availableSoldiers.Add(soldier);
            Debug.Log($"Added soldier: {soldier.Name}");
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
        #endregion


        #region Combat Setup
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
        }

        // 修改后的 StartCombat 方法，传入 Mission 对象和玩家选定的士兵列表
        public void StartCombat(Mission mission, List<Soldier> selectedSoldiers)
        {
            _inBattleEnemies.Clear();
            _inBattleSoldiers.Clear();
            _availableEnemies.Clear();
            _waitingEnemies.Clear();

            if (selectedSoldiers == null || selectedSoldiers.Count == 0)
            {
                Debug.LogError("Cannot start combat: no soldiers selected.");
                return;
            }
            _inBattleSoldiers.AddRange(selectedSoldiers);

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

            currentMission = mission; // 设置当前任务

            // 将当前任务的敌人加载到 _availableEnemies 和 _waitingEnemies 中
            for (int i = 0; i < mission.AssignedEnemies.Count; i++)
            {
                if (i < 6)
                {
                    _availableEnemies.Add(mission.AssignedEnemies[i]);  // 用于 UI 显示
                    _inBattleEnemies.Add(mission.AssignedEnemies[i]);   // 用于战斗逻辑处理
                }
                else
                {
                    _waitingEnemies.Add(mission.AssignedEnemies[i]); // 剩下的敌人存入等待列表
                }
                Debug.Log($"Added enemy to _availableEnemies: {mission.AssignedEnemies[i].Name}");
            }

            if (_inBattleSoldiers.Count == 0 || _inBattleEnemies.Count == 0)
            {
                Debug.LogError("Cannot start combat with empty units");
                return;
            }

            IsCombatActive = true;
            IsPlayerTurn = true;
            Debug.Log($"Combat started: {_inBattleSoldiers.Count} vs {_inBattleEnemies.Count}");
            AudioManager.Instance.PlaySound("Combat Start");
            CheckAndAssignAbilities(); // 检查并分配技能
            ResetAttackChances(); // 重置攻击次数
            ApplyTerrainAndWeatherEffects(); // 应用地形和天气效果
        }
        #endregion

        #region Combat Logic
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

            CheckCombatEnd();
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

            if (IsPlayerTurn && !_inBattleSoldiers.Contains(attacker))
            {
                Debug.LogWarning("Not a valid player character");
                return false;
            }

            if (!IsPlayerTurn && !_inBattleEnemies.Contains(attacker))
            {
                Debug.LogWarning("Not a valid enemy character");
                return false;
            }

            return true;
        }

        // 在回合结束时调用，检查并补充敌人
        public void CheckAndReplaceDeadEnemies()
        {
            var deadEnemies = _inBattleEnemies.Where(e => e.IsDead()).ToList();
            _inBattleSoldiers.RemoveAll(c => c != null && c.IsDead());
            _inBattleEnemies.RemoveAll(c => c != null && c.IsDead());

            foreach (var deadEnemy in deadEnemies)
            {
                if (_waitingEnemies.Count > 0)
                {
                    var newEnemy = _waitingEnemies[0];
                    _waitingEnemies.RemoveAt(0);
                    _availableEnemies.Add(newEnemy);
                    _inBattleEnemies.Add(newEnemy);
                    Debug.Log($"Replaced dead enemy with: {newEnemy.Name}");
                }
            }
        }

        public bool CheckCombatEnd()
        {
            Debug.Log($"Checking combat end: {CountAliveSoldiers()} vs {CountAliveEnemies()}");
            if (CountAliveSoldiers() == 0)
            {
                EndCombat(false);
                return true;
            }

            if (CountAliveEnemies() == 0)
            {
                EndCombat(true);
                return true;
            }

            return false;
        }
        #endregion

        #region Ability Management
        private void CheckAndAssignAbilities()
        {
            foreach (var soldier in _inBattleSoldiers)
            {
            if (soldier == null || soldier.IsDead()) continue;

            string roleName = soldier.GetRoleName();
            // Check for Medic role
            if (roleName.Equals("Medic", StringComparison.OrdinalIgnoreCase))
            {
                if (soldier.Level > 2 && !soldier.Abilities.Any(a => a is HealAbility))
                {
                    var healAbility = gameObject.AddComponent<HealAbility>();
                    healAbility.Initialize("Nano Heal", cost: 1, cooldown: 3, 
                        description: "Base healing ability, scales with caster's attack.", caster: soldier);
                    soldier.Abilities.Add(healAbility);
                }
                if (soldier.Level > 5 && !soldier.Abilities.Any(a => a is HealBuffAbility))
                {
                    var healBuffAbility = gameObject.AddComponent<HealBuffAbility>();
                    healBuffAbility.Initialize("Nano Revival", cost: 1, cooldown: 3, duration: 1,
                        description: "Heal over turn ability, scales with caster's stats.", caster: soldier);
                    soldier.Abilities.Add(healBuffAbility);
                }
                if (soldier.Level > 7 && !soldier.Abilities.Any(a => a is ShieldAbility))
                {
                    var shieldAbility = gameObject.AddComponent<ShieldAbility>();
                    shieldAbility.Initialize("Aegis Surge", cost: 2, cooldown: 3, duration: 1,
                        description: "Shield ability, scales with caster's attack.", caster: soldier);
                    soldier.Abilities.Add(shieldAbility);
                }
            }
            // Check for Tank role
            if (roleName.Equals("Tank", StringComparison.OrdinalIgnoreCase) && soldier.Level > 5)
            {
                if (!soldier.Abilities.Any(a => a is TauntAbility))
                {
                    var tauntAbility = gameObject.AddComponent<TauntAbility>();
                    tauntAbility.Initialize("Defiant Roar", cost: soldier.MaxAttacksPerTurn, cooldown: 1, duration: 2,
                        description: "Taunt ability with defense buff scaling with caster's defense.\n Taunt rounds increase with caster's level.", caster: soldier);
                    soldier.Abilities.Add(tauntAbility);
                }
            }
            // Check for Engineer role
            if (roleName.Equals("Engineer", StringComparison.OrdinalIgnoreCase) && soldier.Level > 7)
            {
                if (!soldier.Abilities.Any(a => a is BuffAtkAbility))
                {
                    var buffAbility = gameObject.AddComponent<BuffAtkAbility>();
                    buffAbility.Initialize("Adrenaline Rush", cost: soldier.MaxAttacksPerTurn, cooldown: 2, duration: soldier.Level,
                        description: "Buff ability with attack and speed buffs scaling with caster's stats.", caster: soldier);
                    soldier.Abilities.Add(buffAbility);
                }
            }
            // Check for Infantry role
            if (roleName.Equals("Infantry", StringComparison.OrdinalIgnoreCase))
            {
                if (soldier.Level > 4 && !soldier.Abilities.Any(a => a is InfantryLifestealAbility))
                {
                    var lifestealAbility = gameObject.AddComponent<InfantryLifestealAbility>();
                    lifestealAbility.Initialize("Tactical Strike", cost: 1, cooldown: 2,
                        description: "Deal damage and lifesteal based on caster's percentage stats and scaling with caster's stats.", caster: soldier);
                    soldier.Abilities.Add(lifestealAbility);
                }
            }
            
            // Check for Sniper role
            if (roleName.Equals("Sniper", StringComparison.OrdinalIgnoreCase))
            {
                if (soldier.Level > 3 && !soldier.Abilities.Any(a => a is SniperDamageAbility))
                {
                    var sniperAbility = gameObject.AddComponent<SniperDamageAbility>();
                    sniperAbility.Initialize("Precision Shot", cost: 2, cooldown: 2,
                        description: "High-damage percing attack dealing percentage damage scaling with caster's attack.", caster: soldier);
                    soldier.Abilities.Add(sniperAbility);
                }
            }
            }
        }
        #endregion

        #region Weather and Terrain Effects
        private int terrainAtkEffect;
        private int terrainDefEffect;
        private int terrainHpEffect;

        private int weatherAtkEffect;
        private int weatherDefEffect;
        private int weatherHpEffect;

        private void ApplyTerrainAndWeatherEffects()
        {
            if (currentMission == null) {
                Debug.LogError("CombatManager: currentMission is null. Cannot apply terrain and weather effects.");
                return;
            }

            // 从当前任务中加载 Terrain 和 Weather 的效果
            terrainAtkEffect = currentMission.terrainAtkEffect;
            Debug.Log($"Terrain Atk Effect: {terrainAtkEffect}");
            terrainDefEffect = currentMission.terrainDefEffect;
            Debug.Log($"Terrain Def Effect: {terrainDefEffect}");
            terrainHpEffect = currentMission.terrainHpEffect;
            Debug.Log($"Terrain HP Effect: {terrainHpEffect}");

            weatherAtkEffect = currentMission.weatherAtkEffect;
            Debug.Log($"Weather Atk Effect: {weatherAtkEffect}");
            weatherDefEffect = currentMission.weatherDefEffect;
            Debug.Log($"Weather Def Effect: {weatherDefEffect}");
            weatherHpEffect = currentMission.weatherHpEffect;
            Debug.Log($"Weather HP Effect: {weatherHpEffect}");

            // 对士兵应用效果
            foreach (var soldier in _inBattleSoldiers)
            {   
                if (soldier == null || soldier.IsDead()) continue;
                soldier.ModifyAttack(terrainAtkEffect + weatherAtkEffect);
                soldier.ModifyDefense(terrainDefEffect + weatherDefEffect);
                soldier.ModifyHP(terrainHpEffect + weatherHpEffect);
            }

        }
        

        #endregion

        #region Turn Management
        private IEnumerator<WaitForSeconds> SwitchTurnRoutine()
        {
            if (!IsPlayerTurn) CheckAndReplaceDeadEnemies();
            IsPlayerTurn = !IsPlayerTurn;
            Debug.Log($"Turn switched to: {(IsPlayerTurn ? "Player" : "Enemy")}");
            AudioManager.Instance.PlaySound("TurnSwitch");
            yield return new WaitForSeconds(enemyTurnDelay);

            if (IsPlayerTurn)
            {
                BuffsCountDown(); // buffs倒计时
                ResetAttackChances(); // 重置攻击次数
                CheckAndAssignAbilities(); // 检查并分配技能
            }
        }

        public Soldier GetRandomSoldier()
        {
            var validSoldiers = _inBattleSoldiers
                .Where(s => s != null)
                .Cast<Soldier>()
                .ToList();

            if (validSoldiers.Count == 0)
            {
                Debug.LogError("No valid soldiers available. Cannot select any soldier.");
                throw new InvalidOperationException("No valid soldier available.");
            }

            var tauntSoldiers = validSoldiers
                .Where(s => s.Buffs.Any(buff => buff.Value.Name.Equals("Taunt", StringComparison.OrdinalIgnoreCase)))
                .ToList();

            if (tauntSoldiers.Count > 0)
            {
                return tauntSoldiers[UnityEngine.Random.Range(0, tauntSoldiers.Count)];
            }

            return validSoldiers[UnityEngine.Random.Range(0, validSoldiers.Count)];
        }
        #endregion

        #region Combat Termination
        public void EndCombat(bool victory)
        {
            IsCombatActive = false;
            Debug.Log($"Combat ended with {(victory ? "victory" : "defeat")}");
            
            // Reward experience
            if (victory)
            {
                ApplyMissionRewards(currentMission);  // 调用奖励分发函数

                string soldierExpDetails = "";

                // Grant each in-battle soldier experience equal to the sum of all in-battle enemies' experience rewards.
                int totalEnemyExp = _inBattleEnemies.Sum(enemy => enemy.Experience);
                _inBattleSoldiers.ForEach(c => {
                    if (c is Soldier soldier) 
                    {
                        soldier.GainExp(totalEnemyExp);  // 士兵获得经验
                        soldierExpDetails += $"- {soldier.Name}: Gained {totalEnemyExp} EXP\n";
                    }
                });

                SaveCombatResults(true, soldierExpDetails);
            }
            else
            {
                SaveCombatResults(false, "");
            }

            OnCombatEnd?.Invoke(victory);
            CleanupCombat();
        }

        public void ApplyMissionRewards(Mission mission)
        {
            if (mission == null)
            {
                Debug.LogError("Mission is null. Cannot apply rewards.");
                return;
            }

            Debug.Log($"Applying rewards for mission: {mission.name}");

            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();

                // 更新 Money
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        UPDATE Resource 
                        SET current_amount = current_amount + @rewardMoney 
                        WHERE name = 'Money';
                    ";
                    command.Parameters.AddWithValue("@rewardMoney", mission.rewardMoney);
                    command.ExecuteNonQuery();
                }

                // 更新资源奖励
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        UPDATE Resource 
                        SET current_amount = current_amount + @rewardAmount 
                        WHERE resource_id = @rewardResourceId;
                    ";
                    command.Parameters.AddWithValue("@rewardAmount", mission.rewardAmount);
                    command.Parameters.AddWithValue("@rewardResourceId", mission.rewardResourceId);
                    command.ExecuteNonQuery();
                }

                Game.Instance.SaveGameData();

                connection.Close();
            }

            Debug.Log($"Rewards applied successfully: Money +{mission.rewardMoney}, Resource ID {mission.rewardResourceId} +{mission.rewardAmount}");
        }


        public void SaveCombatResults(bool victory, string soldierExpDetails)
        {
            PlayerPrefs.SetInt("CombatResult", victory ? 1 : 0);

            if (victory)
            {
                string resourceName = GetResourceNameById(currentMission.rewardResourceId);
                string rewardDetails = $"Rewards:\n" +
                    $"- Money: {currentMission.rewardMoney}\n" +
                    $"- Resource: {resourceName} +{currentMission.rewardAmount}\n";
                
                PlayerPrefs.SetString("RewardDetails", rewardDetails);
                PlayerPrefs.SetString("SoldierExpDetails", soldierExpDetails);
            }
        }

        public string GetResourceNameById(int resourceId)
        {
            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT name FROM Resource WHERE resource_id = @id";
                    command.Parameters.AddWithValue("@id", resourceId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader.GetString(0);
                        }
                    }
                }
            }

            return "Unknown Resource";
        }

        private void CleanupCombat()
        {
            _inBattleSoldiers.Clear();
            _inBattleEnemies.Clear();
        }

        public void EndCurrentTurn()
        {
            // IsPlayerTurn = !IsPlayerTurn;
            StartCoroutine(SwitchTurnRoutine());
        }
        #endregion

        public void SetcurrentMission(Mission mission)
        {
            currentMission = mission;
        }

        #region Helper Methods
        private int CountAliveSoldiers() => _inBattleSoldiers.Count(s => s != null && !s.IsDead());
        public int CountAliveEnemies() => _availableEnemies.Count(e => e != null && !e.IsDead());
        private void BuffsCountDown()
        {
            foreach (var soldier in _inBattleSoldiers)
            {
                if (soldier == null || soldier.IsDead()) continue;
                Dictionary<Ability, Buff> temp = new(soldier.Buffs);
                foreach (var buffPair in temp)
                {
                    buffPair.Value.UpdateDuration(soldier);
                    Debug.Log($"{soldier.Name}'s {buffPair.Key} buff duration: {buffPair.Value.Duration}");
                    if (buffPair.Value.IsExpired())
                    {
                        soldier.Buffs.Remove(buffPair.Key);
                        Debug.Log($"{soldier.Name}'s {buffPair.Key} buff has expired.");
                    }
                }
            }
        }
        public List<Soldier> GetAvailableSoldiers() => new(_availableSoldiers);
        public List<Enemy> GetAvailableEnemies() => new(_availableEnemies);
        public List<Character> GetInBattleSoldiers() => new(_inBattleSoldiers);
        public List<Character> GetInBattleEnemies() => new(_inBattleEnemies);
        public List<Enemy> GetWaitingEnemies() => new(_waitingEnemies);
        public bool IsAlly(Character character) => 
            character is Soldier;

        public bool IsEnemy(Character character) => 
            character is Enemy;

        public void ResetAttackChances()
        {
            foreach (var soldier in _inBattleSoldiers)
            {
                if (soldier == null || soldier.IsDead()) continue;
                soldier.ResetAttackChances();
            }
            foreach (var enemy in _inBattleEnemies)
            {
                if (enemy == null || enemy.IsDead()) continue;
                enemy.ResetAttackChances();
            }
            return;
        }
        #endregion
    }
}