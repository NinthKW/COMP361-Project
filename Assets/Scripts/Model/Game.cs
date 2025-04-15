using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Model;
using System.Data;
using Mono.Data.Sqlite;
using Assets.Scripts.Controller;

namespace Assets.Scripts.Model
{
    [System.Serializable]
    public class Game
    {
        public static Game Instance;
        public Resources resourcesData;
        public List<Mission> MissionsData;
        public List<Character> soldiersData;
        public List<Base> basesData;
        public List<SoldierEquipment> soldierEquipmentData; //empty on new game
        public Tech techData;
        public Inventory inventory;

        public int maxSoldier;

        public Game()
        {
            Instance = this;
            // Resources
            this.resourcesData = new Resources();
            this.MissionsData = new List<Mission>();
            this.soldiersData = new List<Character>();
            this.basesData = new List<Base>();
            this.techData = new Tech();
            this.inventory = new Inventory();
            this.soldierEquipmentData = new List<SoldierEquipment>();

            maxSoldier = 5;
            
            string dbPath = "URI=file:" + Application.streamingAssetsPath + "/database.db";
            // Bases
            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT building_id, name, description, level, cost, resource_amount, resource_type, x, y FROM Infrastructure ORDER BY building_id ASC;";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int building_id = int.Parse(reader["building_id"].ToString());
                            string name = reader["name"].ToString();
                            string description = reader["description"].ToString();
                            int level = int.Parse(reader["level"].ToString());
                            int cost = int.Parse(reader["cost"].ToString());
                            int resource_amount = int.Parse(reader["resource_amount"].ToString());
                            int resource_type = int.Parse(reader["resource_type"].ToString());
                            int x = int.Parse(reader["x"].ToString());
                            int y = int.Parse(reader["y"].ToString());
                            
                            // For a new game, all bases start locked and not placed.
                            this.basesData.Add(new Base(building_id, name, description, level, cost, resource_amount, resource_type, false, false, x, y));
                        }
                    }
                }
                connection.Close();
            }
            
            foreach (Base building in this.basesData) {
                if (building.name.ToLower().Equals("barracks"))
                {
                    if (building.placed)
                    {
                        maxSoldier += 1;
                    }
                }
            }


            // Missions
            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT mission_id, name, description, difficulty, reward_money, reward_amount, reward_resource, terrain, weather, unlocked, cleared FROM Mission ORDER BY mission_id ASC;";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        bool isFirstMission = true;
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string name = reader.GetString(1);
                            string description = reader.GetString(2);
                            int difficulty = reader.GetInt32(3);
                            int rewardMoney = reader.GetInt32(4);
                            int rewardAmount = reader.GetInt32(5);
                            int rewardResourceId = reader.GetInt32(6);
                            string terrain = reader.GetString(7);
                            string weather = reader.GetString(8);
                            bool unlocked = isFirstMission;
                            bool isclear = false;

                            // 加载 Terrain 和 Weather 效果
                            int terrainAtkEffect = 0;
                            int terrainDefEffect = 0;
                            int terrainHpEffect = 0;

                            int weatherAtkEffect = 0;
                            int weatherDefEffect = 0;
                            int weatherHpEffect = 0;

                            // 读取 Terrain 效果
                            using (var terrainCommand = connection.CreateCommand())
                            {
                                terrainCommand.CommandText = $"SELECT atk_effect, def_effect, hp_effect FROM Terrain WHERE name = '{terrain}';";
                                using (IDataReader terrainReader = terrainCommand.ExecuteReader())
                                {
                                    if (terrainReader.Read())
                                    {
                                        terrainAtkEffect = terrainReader.GetInt32(0);
                                        terrainDefEffect = terrainReader.GetInt32(1);
                                        terrainHpEffect = terrainReader.GetInt32(2);
                                    }
                                }
                            }

                            // 读取 Weather 效果
                            using (var weatherCommand = connection.CreateCommand())
                            {
                                weatherCommand.CommandText = $"SELECT atk_effect, def_effect, hp_effect FROM Weather WHERE name = '{weather}';";
                                using (IDataReader weatherReader = weatherCommand.ExecuteReader())
                                {
                                    if (weatherReader.Read())
                                    {
                                        weatherAtkEffect = weatherReader.GetInt32(0);
                                        weatherDefEffect = weatherReader.GetInt32(1);
                                        weatherHpEffect = weatherReader.GetInt32(2);
                                    }
                                }
                            }

                            // 创建 Mission 对象
                            Mission mission = new Mission(
                                id,
                                name,
                                description,
                                difficulty,
                                rewardMoney,
                                rewardAmount,
                                rewardResourceId,
                                terrain,
                                weather,
                                unlocked,
                                isclear
                            );

                            // 设置 Terrain 和 Weather 效果
                            mission.SetTerrainEffects(terrainAtkEffect, terrainDefEffect, terrainHpEffect);
                            mission.SetWeatherEffects(weatherAtkEffect, weatherDefEffect, weatherHpEffect);

                            // 加载 Enemies
                            using (var enemyCommand = connection.CreateCommand())
                            {
                                enemyCommand.CommandText = @"
                                    SELECT 
                                        MISSION_ENEMY.et_id,
                                        MISSION_ENEMY.count,
                                        ENEMY_TYPES.et_name,
                                        ENEMY_TYPES.HP,
                                        ENEMY_TYPES.base_ATK,
                                        ENEMY_TYPES.base_DPS,
                                        ENEMY_TYPES.exp_reward
                                    FROM MISSION_ENEMY
                                    INNER JOIN ENEMY_TYPES ON MISSION_ENEMY.et_id = ENEMY_TYPES.et_ID
                                    WHERE MISSION_ENEMY.mission_id = @missionId;
                                ";

                                enemyCommand.Parameters.AddWithValue("@missionId", id);

                                using (IDataReader enemyReader = enemyCommand.ExecuteReader())
                                {
                                    while (enemyReader.Read())
                                    {
                                        int count = enemyReader.GetInt32(1);
                                        string enemyName = enemyReader.GetString(2);
                                        int hp = enemyReader.GetInt32(3);
                                        int atk = enemyReader.GetInt32(4);
                                        int dps = enemyReader.GetInt32(5);
                                        int expReward = enemyReader.GetInt32(6);

                                        for (int i = 0; i < count; i++)
                                        {
                                            var enemy = new Enemy(enemyName, hp, atk, dps, hp, 1, expReward, new EquipmentBonus(0, 0)); 
                                            mission.AssignedEnemies.Add(enemy);
                                        }
                                    }
                                }
                            }

                            this.MissionsData.Add(mission);
                            isFirstMission = false;
                        }
                    }
                }
                connection.Close();
            }

            //Soldiers
            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Soldier ORDER BY soldier_id ASC;";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int soldierId = int.Parse(reader["soldier_id"].ToString());
                            string name = reader["name"].ToString();
                            // Override soldier level to 1 for a new game.
                            // int level = 1;
                            // Let level = 10 for testing
                            int level = 10;
                            int health = int.Parse(reader["hp"].ToString());
                            int maxHealth = int.Parse(reader["max_hp"].ToString());
                            int attack = int.Parse(reader["atk"].ToString());
                            int defense = int.Parse(reader["def"].ToString());
                            string roleName = reader["role"].ToString();
                            
                            Role role = new Role(roleName);
                            Soldier soldier = new Soldier(name, role, level, health, attack, defense, maxHealth, soldierId, new EquipmentBonus(0,0));
                            this.soldiersData.Add(soldier);
                        }
                    }
                }
                connection.Close();
            }

            //Tech
            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT tech_id, tech_name, description, cost_money, cost_resources_id, cost_resources_amount FROM TECHNOLOGY LIMIT 1;";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int techId = int.Parse(reader["tech_id"].ToString());
                            string techName = reader["tech_name"].ToString();
                            string description = reader["description"].ToString();
                            float costMoney = float.Parse(reader["cost_money"].ToString());
                            int costResourceId = int.Parse(reader["cost_resources_id"].ToString());
                            int costResourceAmount = int.Parse(reader["cost_resources_amount"].ToString());
                            
                            Tech tech = new Tech(techId, techName, description, costMoney, costResourceId, costResourceAmount);
                            tech.isUnlocked = false;
                            this.techData = tech;
                        }
                    }
                }
                connection.Close();
            }

            // weapons
            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT weapon_id, name, description, damage, cost, resource_amount, resource_type, unlocked FROM Weapon;";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = int.Parse(reader["weapon_id"].ToString());
                            string name = reader["name"].ToString();
                            string description = reader["description"].ToString();
                            int damage = int.Parse(reader["damage"].ToString());
                            int cost = int.Parse(reader["cost"].ToString());
                            int resourceAmount = int.Parse(reader["resource_amount"].ToString());
                            int resourceType = int.Parse(reader["resource_type"].ToString());
                            bool unlocked = bool.Parse(reader["unlocked"].ToString());

                            // Create a new weapon and add it to the inventory.
                            Weapon weapon = new Weapon(id, name, description, damage, cost, resourceAmount, resourceType, unlocked);
                            inventory.AddWeapon(weapon);
                        }
                    }
                }
                connection.Close();
            }

            // equipments
            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT equipment_id, name, hp, def, atk, cost, resource_amount, resource_type, unlocked FROM Equipment;";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = int.Parse(reader["equipment_id"].ToString());
                            string name = reader["name"].ToString();
                            int hp = int.Parse(reader["hp"].ToString());
                            int def = int.Parse(reader["def"].ToString());
                            int atk = int.Parse(reader["atk"].ToString());
                            int cost = int.Parse(reader["cost"].ToString());
                            int resourceAmount = int.Parse(reader["resource_amount"].ToString());
                            int resourceType = int.Parse(reader["resource_type"].ToString());
                            bool unlocked = bool.Parse(reader["unlocked"].ToString());

                            // Creates a new equipment item and add it to the inventory
                            Equipment equipment = new Equipment(id, name, hp, def, atk, cost, resourceAmount, resourceType, unlocked);
                            inventory.AddEquipment(equipment);
                        }
                    }
                }
                connection.Close();
            }
        }


        public Game(string dbPath)
        {
            Instance = this;
            //Set default maxSoldier
            maxSoldier = 5;

            int food = 0;
            int money = 0;
            int iron = 0;
            int wood = 0;
            int titanium = 0;
            int medecine = 0;

            // resources
            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT resource_id, current_amount FROM Resource;";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int resourceId = int.Parse(reader["resource_id"].ToString());
                            int currentAmount = int.Parse(reader["current_amount"].ToString());
                            switch (resourceId)
                            {
                                case 0:
                                    food = currentAmount;
                                    break;
                                case 1:
                                    money = currentAmount;
                                    break;
                                case 2:
                                    iron = currentAmount;
                                    break;
                                case 3:
                                    wood = currentAmount;
                                    break;
                                case 4:
                                    titanium = currentAmount;
                                    break;
                                case 5:
                                    medecine = currentAmount;
                                    break;
                                default:
                                    Debug.LogWarning("Unexpected resource id: " + resourceId);
                                    break;
                            }
                        }
                        reader.Close();
                    }
                }
                connection.Close();
            }
            this.resourcesData = new Resources(food, money, iron, wood, titanium, medecine);

            // Bases
            this.basesData = new List<Base>();
            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT building_id, name, description, level, cost, resource_amount, resource_type, unlocked, placed, x, y FROM Infrastructure ORDER BY building_id ASC;";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int building_id = int.Parse(reader["building_id"].ToString());
                            string name = reader["name"].ToString();
                            string description = reader["description"].ToString();
                            int level = int.Parse(reader["level"].ToString());
                            int cost = int.Parse(reader["cost"].ToString());
                            int resource_amount = int.Parse(reader["resource_amount"].ToString());
                            int resource_type = int.Parse(reader["resource_type"].ToString());
                            bool unlocked = bool.Parse(reader["unlocked"].ToString());
                            bool placed = bool.Parse(reader["placed"].ToString());
                            int x = int.Parse(reader["x"].ToString());
                            int y = int.Parse(reader["y"].ToString());

                            this.basesData.Add(new Base(building_id, name, description, level, cost, resource_amount, resource_type, unlocked, placed, x, y));
                        }
                        reader.Close();
                    }
                }
                connection.Close();
            }

            //Initialize maxSoldier
            foreach (Base building in this.basesData) {
                if (Equals(building.name.ToLower(), "barracks")) 
                {
                    if (building.placed)
                    {
                        maxSoldier += 1;
                    }
                }
            }


            // Missions
            this.MissionsData = new List<Mission>();

            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT mission_id, name, description, difficulty, reward_money, reward_amount, reward_resource, terrain, weather, unlocked, cleared FROM Mission ORDER BY mission_id ASC;";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string name = reader.GetString(1);
                            string description = reader.GetString(2);
                            int difficulty = reader.GetInt32(3);
                            int rewardMoney = reader.GetInt32(4);
                            int rewardAmount = reader.GetInt32(5);
                            int rewardResourceId = reader.GetInt32(6);
                            string terrain = reader.GetString(7);
                            string weather = reader.GetString(8);
                            bool unlocked = reader.GetBoolean(9);
                            bool isclear = reader.GetBoolean(10);

                            // 加载 Terrain 和 Weather 效果
                            int terrainAtkEffect = 0;
                            int terrainDefEffect = 0;
                            int terrainHpEffect = 0;

                            int weatherAtkEffect = 0;
                            int weatherDefEffect = 0;
                            int weatherHpEffect = 0;

                            // 读取 Terrain 效果
                            using (var terrainCommand = connection.CreateCommand())
                            {
                                terrainCommand.CommandText = $"SELECT atk_effect, def_effect, hp_effect FROM Terrain WHERE name = '{terrain}';";
                                using (IDataReader terrainReader = terrainCommand.ExecuteReader())
                                {
                                    if (terrainReader.Read())
                                    {
                                        terrainAtkEffect = terrainReader.GetInt32(0);
                                        terrainDefEffect = terrainReader.GetInt32(1);
                                        terrainHpEffect = terrainReader.GetInt32(2);
                                    }
                                }
                            }

                            // 读取 Weather 效果
                            using (var weatherCommand = connection.CreateCommand())
                            {
                                weatherCommand.CommandText = $"SELECT atk_effect, def_effect, hp_effect FROM Weather WHERE name = '{weather}';";
                                using (IDataReader weatherReader = weatherCommand.ExecuteReader())
                                {
                                    if (weatherReader.Read())
                                    {
                                        weatherAtkEffect = weatherReader.GetInt32(0);
                                        weatherDefEffect = weatherReader.GetInt32(1);
                                        weatherHpEffect = weatherReader.GetInt32(2);
                                    }
                                }
                            }

                            // 创建 Mission 对象
                            Mission mission = new Mission(
                                id,
                                name,
                                description,
                                difficulty,
                                rewardMoney,
                                rewardAmount,
                                rewardResourceId,
                                terrain,
                                weather,
                                unlocked,
                                isclear
                            );

                            // 设置 Terrain 和 Weather 效果
                            mission.SetTerrainEffects(terrainAtkEffect, terrainDefEffect, terrainHpEffect);
                            mission.SetWeatherEffects(weatherAtkEffect, weatherDefEffect, weatherHpEffect);

                            // 加载 Enemies
                            using (var enemyCommand = connection.CreateCommand())
                            {
                                enemyCommand.CommandText = @"
                                    SELECT 
                                        MISSION_ENEMY.et_id,
                                        MISSION_ENEMY.count,
                                        ENEMY_TYPES.et_name,
                                        ENEMY_TYPES.HP,
                                        ENEMY_TYPES.base_ATK,
                                        ENEMY_TYPES.base_DPS,
                                        ENEMY_TYPES.exp_reward
                                    FROM MISSION_ENEMY
                                    INNER JOIN ENEMY_TYPES ON MISSION_ENEMY.et_id = ENEMY_TYPES.et_ID
                                    WHERE MISSION_ENEMY.mission_id = @missionId;
                                ";

                                enemyCommand.Parameters.AddWithValue("@missionId", id);

                                using (IDataReader enemyReader = enemyCommand.ExecuteReader())
                                {
                                    while (enemyReader.Read())
                                    {
                                        int count = enemyReader.GetInt32(1);
                                        string enemyName = enemyReader.GetString(2);
                                        int hp = enemyReader.GetInt32(3);
                                        int atk = enemyReader.GetInt32(4);
                                        int dps = enemyReader.GetInt32(5);
                                        int expReward = enemyReader.GetInt32(6);

                                        for (int i = 0; i < count; i++)
                                        {
                                            var enemy = new Enemy(enemyName, hp, atk, dps, hp, 1, expReward, new EquipmentBonus(0,0)); 
                                            mission.AssignedEnemies.Add(enemy);
                                        }
                                    }
                                }
                            }

                            this.MissionsData.Add(mission);
                        }
                    }
                }
                connection.Close();
            }
            //MissionManager.Instance.missions = this.MissionsData;

            // Soldiers
            this.soldiersData = new List<Character>();
            // Needed for SoldierEquipment
            Dictionary<int, Character> soldierMap = new Dictionary<int, Character>();
            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Soldier ORDER BY soldier_id ASC;";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int soldierId = int.Parse(reader["soldier_id"].ToString());
                            string name = reader["name"].ToString();
                            int level = int.Parse(reader["level"].ToString());
                            int health = int.Parse(reader["hp"].ToString());
                            int maxHealth = int.Parse(reader["max_hp"].ToString());
                            int attack = int.Parse(reader["atk"].ToString());
                            int defense = int.Parse(reader["def"].ToString());
                            string roleName = reader["role"].ToString();

                            Role role = new Role(roleName);
                            Soldier soldier = new(name, role, level, health, attack, defense, maxHealth, soldierId, new EquipmentBonus(0, 0));
                            this.soldiersData.Add(soldier);
                            soldierMap.Add(soldierId, soldier);
                        }
                        reader.Close();
                    }
                }
                connection.Close();
            }

            // tech
            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT tech_id, tech_name, description, cost_money, cost_resources_id, cost_resources_amount, unlocked FROM TECHNOLOGY LIMIT 1;";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int techId = int.Parse(reader["tech_id"].ToString());
                            string techName = reader["tech_name"].ToString();
                            string description = reader["description"].ToString();
                            float costMoney = float.Parse(reader["cost_money"].ToString());
                            int costResourceId = int.Parse(reader["cost_resources_id"].ToString());
                            int costResourceAmount = int.Parse(reader["cost_resources_amount"].ToString());
                            bool isUnlocked = bool.Parse(reader["unlocked"].ToString());

                            Tech tech = new Tech(techId, techName, description, costMoney, costResourceId, costResourceAmount);
                            tech.isUnlocked = isUnlocked;
                            this.techData = tech;
                        }
                        reader.Close();
                    }
                }
                connection.Close();
            }

            this.inventory = new Inventory();
            // weapons
            Dictionary<int, Weapon> weaponMap = new Dictionary<int, Weapon>();
            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT weapon_id, name, description, damage, cost, resource_amount, resource_type, unlocked FROM Weapon;";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = int.Parse(reader["weapon_id"].ToString());
                            string name = reader["name"].ToString();
                            string description = reader["description"].ToString();
                            int damage = int.Parse(reader["damage"].ToString());
                            int cost = int.Parse(reader["cost"].ToString());
                            int resourceAmount = int.Parse(reader["resource_amount"].ToString());
                            int resourceType = int.Parse(reader["resource_type"].ToString());
                            bool unlocked = bool.Parse(reader["unlocked"].ToString());

                            // Create a new weapon and add it to the inventory.
                            Weapon weapon = new Weapon(id, name, description, damage, cost, resourceAmount, resourceType, unlocked);
                            inventory.AddWeapon(weapon);
                            weaponMap.Add(id, weapon);
                        }
                    }
                }
                connection.Close();
            }

            // equipments
            Dictionary<int, Equipment> equipmentMap = new Dictionary<int, Equipment>();
            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT equipment_id, name, hp, def, atk, cost, resource_amount, resource_type, unlocked FROM Equipment;";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = int.Parse(reader["equipment_id"].ToString());
                            string name = reader["name"].ToString();
                            int hp = int.Parse(reader["hp"].ToString());
                            int def = int.Parse(reader["def"].ToString());
                            int atk = int.Parse(reader["atk"].ToString());
                            int cost = int.Parse(reader["cost"].ToString());
                            int resourceAmount = int.Parse(reader["resource_amount"].ToString());
                            int resourceType = int.Parse(reader["resource_type"].ToString());
                            bool unlocked = bool.Parse(reader["unlocked"].ToString());

                            // Creates a new equipment item and add it to the inventory
                            Equipment equipment = new Equipment(id, name, hp, def, atk, cost, resourceAmount, resourceType, unlocked);
                            inventory.AddEquipment(equipment);
                            equipmentMap.Add(id, equipment);
                        }
                    }
                }
                connection.Close();
            }

            //soldier equipments
            this.soldierEquipmentData = new List<SoldierEquipment>();
            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT soldier_ID, weapon_ID, equipment_ID FROM SOLDIER_EQUIPMENT;";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int soldier = int.Parse(reader["soldier_ID"].ToString());
                            int weapon = int.Parse(reader["weapon_ID"].ToString());
                            int equipment = int.Parse(reader["equipment_ID"].ToString());
                            
                            Character soldierObj;
                            Weapon weaponObj;
                            Equipment equipmentObj;

                            // Check if objs are added properly
                            if (!soldierMap.TryGetValue(soldier, out soldierObj))
                            {
                                Debug.LogError("Soldier not found for soldier equipment");
                            }
                            if (!weaponMap.TryGetValue(weapon, out weaponObj) && weapon != -1) //negative id means unequipped
                            {
                                Debug.LogError("Weapon not found for soldier equipment");
                            }
                            if (!equipmentMap.TryGetValue(weapon, out equipmentObj) && equipment != -1) //negative id means unequipped
                            {
                                Debug.LogError("Equipment not found for soldier equipment");
                            }

                            // Adds it to tracker
                            soldierObj.bonusStat.atk += weaponObj.damage;
                            soldierObj.bonusStat.atk += equipmentObj.atk;
                            soldierObj.bonusStat.def += equipmentObj.def;

                            SoldierEquipment newSoldierEquipment = new SoldierEquipment(soldierObj, weaponObj, equipmentObj);
                            this.soldierEquipmentData.Add(newSoldierEquipment);
                        }
                    }
                }
                connection.Close();
            }
        }

        public void SaveGameData()
        {
            string dbPath = "URI=file:" + Application.streamingAssetsPath + "/database.db";

            // resources
            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    for (int id = 0; id <= 5; id++)
                    {
                        int amount = this.resourcesData.GetAmount(id);
                        command.CommandText = $"UPDATE Resource SET current_amount = {amount} WHERE resource_id = {id};";
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }


            // bases
            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();
                List<int> buildingIds = new List<int>();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT building_id FROM Infrastructure ORDER BY building_id ASC;";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            buildingIds.Add(int.Parse(reader["building_id"].ToString()));
                        }
                        reader.Close();
                    }
                }
                foreach (int bid in buildingIds)
                {
                    Base b = this.basesData.Find(x => x.building_id == bid);
                    if (b != null)
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = "UPDATE Infrastructure SET " +
                                "name = @name, " +
                                "description = @description, " +
                                "level = @level, " +
                                "cost = @cost, " +
                                "resource_amount = @resource_amount, " +
                                "resource_type = @resource_type, " +
                                "unlocked = @unlocked, " +
                                "placed = @placed, " + 
                                "x = @x, " +
                                "y = @y " + 
                                "WHERE building_id = @building_id;";
                            command.Parameters.Add(new SqliteParameter("@name", b.name));
                            command.Parameters.Add(new SqliteParameter("@description", b.description));
                            command.Parameters.Add(new SqliteParameter("@level", b.level));
                            command.Parameters.Add(new SqliteParameter("@cost", b.cost));
                            command.Parameters.Add(new SqliteParameter("@resource_amount", b.resource_amount));
                            command.Parameters.Add(new SqliteParameter("@resource_type", b.resource_type));
                            command.Parameters.Add(new SqliteParameter("@unlocked", b.unlocked ? 1 : 0));
                            command.Parameters.Add(new SqliteParameter("@placed", b.placed ? 1 : 0));
                            command.Parameters.Add(new SqliteParameter("@x", b.x));
                            command.Parameters.Add(new SqliteParameter("@y", b.y));
                            command.Parameters.Add(new SqliteParameter("@building_id", b.building_id));
                            command.ExecuteNonQuery();
                        }
                    }
                }
                connection.Close();
            }

            // missions
            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();
                foreach (var mission in this.MissionsData)
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"UPDATE Mission SET 
                            name = @name, 
                            description = @description, 
                            difficulty = @difficulty, 
                            reward_money = @rewardMoney, 
                            reward_amount = @rewardAmount, 
                            reward_resource = @rewardResourceId, 
                            terrain = @terrain, 
                            weather = @weather, 
                            unlocked = @isUnlocked,
                            cleared = @isCleared
                            WHERE mission_id = @id;";
                        command.Parameters.Add(new SqliteParameter("@name", mission.name));
                        command.Parameters.Add(new SqliteParameter("@description", mission.description));
                        command.Parameters.Add(new SqliteParameter("@difficulty", mission.difficulty));
                        command.Parameters.Add(new SqliteParameter("@rewardMoney", mission.rewardMoney));
                        command.Parameters.Add(new SqliteParameter("@rewardAmount", mission.rewardAmount));
                        command.Parameters.Add(new SqliteParameter("@rewardResourceId", mission.rewardResourceId));
                        command.Parameters.Add(new SqliteParameter("@terrain", mission.terrain));
                        command.Parameters.Add(new SqliteParameter("@weather", mission.weather));
                        command.Parameters.Add(new SqliteParameter("@isUnlocked", mission.unlocked ? 1 : 0));
                        command.Parameters.Add(new SqliteParameter("@isCleared", mission.isCompleted ? 1 : 0));
                        command.Parameters.Add(new SqliteParameter("@id", mission.id));
                        command.ExecuteNonQuery();
                    }
                }

                Dictionary<string, int> enemyTypeLookup = new Dictionary<string, int>();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT et_ID, et_name FROM ENEMY_TYPES;";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int etId = reader.GetInt32(0);
                            string etName = reader.GetString(1);
                            enemyTypeLookup[etName] = etId;
                        }
                        reader.Close();
                    }
                }

                foreach (var mission2 in this.MissionsData)
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "DELETE FROM MISSION_ENEMY WHERE mission_id = @missionId;";
                        command.Parameters.Add(new SqliteParameter("@missionId", mission2.id));
                        command.ExecuteNonQuery();
                    }

                    Dictionary<string, int> enemyCounts = new Dictionary<string, int>();
                    foreach (var enemy in mission2.AssignedEnemies)
                    {
                        if (!enemy.IsDead())
                        {
                            if (enemyCounts.ContainsKey(enemy.Name))
                            {
                                enemyCounts[enemy.Name]++;
                            }
                            else
                            {
                                enemyCounts[enemy.Name] = 1;
                            }
                        }
                        
                    }

                    foreach (var pair in enemyCounts)
                    {
                        string enemyTypeName = pair.Key;
                        int count = pair.Value;
                        int enemyTypeId;
                        if (enemyTypeLookup.TryGetValue(enemyTypeName, out enemyTypeId))
                        {
                            using (var command = connection.CreateCommand())
                            {
                                command.CommandText = "INSERT INTO MISSION_ENEMY (mission_id, et_id, count) VALUES (@missionId, @etId, @count);";
                                command.Parameters.Add(new SqliteParameter("@missionId", mission2.id));
                                command.Parameters.Add(new SqliteParameter("@etId", enemyTypeId));
                                command.Parameters.Add(new SqliteParameter("@count", count));
                                command.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            Debug.LogWarning("Enemy type not found in lookup: " + enemyTypeName);
                        }
                    }
                }
                
                connection.Close();
            }        

            // soldiers
            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();
                foreach (var character in this.soldiersData)
                {
                    using (var command = connection.CreateCommand())
                    {
                        if (character is Soldier soldier)
                        {
                            command.CommandText = @"UPDATE Soldier SET 
                                name = @name, 
                                level = @level, 
                                hp = @health, 
                                max_hp = @maxHealth, 
                                atk = @attack, 
                                def = @defense,
                                role = @roleName
                                WHERE soldier_id = @id;";
                            command.Parameters.Add(new SqliteParameter("@name", soldier.Name));
                            command.Parameters.Add(new SqliteParameter("@level", soldier.Level));
                            command.Parameters.Add(new SqliteParameter("@health", soldier.Health));
                            command.Parameters.Add(new SqliteParameter("@maxHealth", soldier.MaxHealth));
                            command.Parameters.Add(new SqliteParameter("@attack", soldier.Atk));
                            command.Parameters.Add(new SqliteParameter("@defense", soldier.Def));
                            command.Parameters.Add(new SqliteParameter("@roleName", soldier.GetRoleName()));
                            command.Parameters.Add(new SqliteParameter("@id", soldier.id));
                            command.ExecuteNonQuery();
                        }
                    }
                }
                connection.Close();
            }

            // for soldier equipment
            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();
                foreach (var soldierEquipment in this.soldierEquipmentData)
                {
                    // Questionable cast from Character to Soldier, but I need to do it like this unless I want to refactor half my code

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"UPDATE Soldier_Equipment SET 
                            soldier_ID = @soldier_ID, 
                            weapon_ID = @weapon_ID, 
                            equipment_ID = @equipment_ID 
                            WHERE soldier_ID = @soldier_ID;";

                        Soldier soldier;
                        if (soldierEquipment.soldier is Soldier)
                        {
                            soldier = (Soldier)soldierEquipment.soldier;
                        } 
                        else
                        {
                            Debug.Log("Soldier not found, cast failed. Creating dummy.");
                            soldier = new Soldier("error", new Role("tank"), 1, 1, 1, 1, 1, 1, new EquipmentBonus(1, 1));
                        }

                        command.Parameters.Add(new SqliteParameter("@soldier_ID", soldier.id));
                        command.Parameters.Add(new SqliteParameter("@weapon_ID", soldierEquipment.weapon.weapon_id));
                        command.Parameters.Add(new SqliteParameter("@equipment_ID", soldierEquipment.equipment.equipment_id));
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }

            // Tech
            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"UPDATE TECHNOLOGY SET 
                        tech_name = @techName,
                        description = @description,
                        cost_money = @costMoney,
                        cost_resources_id = @costResourceId,
                        cost_resources_amount = @costResourceAmount,
                        unlocked = @isUnlocked
                        WHERE tech_id = @techId;";
                    command.Parameters.Add(new SqliteParameter("@techName", this.techData.techName));
                    command.Parameters.Add(new SqliteParameter("@description", this.techData.description));
                    command.Parameters.Add(new SqliteParameter("@costMoney", this.techData.costMoney));
                    command.Parameters.Add(new SqliteParameter("@costResourceId", this.techData.costResourceId));
                    command.Parameters.Add(new SqliteParameter("@costResourceAmount", this.techData.costResourceAmount));
                    command.Parameters.Add(new SqliteParameter("@isUnlocked", this.techData.isUnlocked ? 1 : 0));
                    command.Parameters.Add(new SqliteParameter("@techId", this.techData.techId));
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }

            // Weapons
            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();
                foreach (var weapon in inventory.weapons)
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"UPDATE Weapon SET 
                            name = @name, 
                            description = @description, 
                            damage = @damage, 
                            cost = @cost, 
                            resource_amount = @resourceAmount, 
                            resource_type = @resourceType,
                            unlocked = @unlocked
                            WHERE weapon_id = @weaponId;";
                        command.Parameters.Add(new SqliteParameter("@name", weapon.name));
                        command.Parameters.Add(new SqliteParameter("@description", weapon.description));
                        command.Parameters.Add(new SqliteParameter("@damage", weapon.damage));
                        command.Parameters.Add(new SqliteParameter("@cost", weapon.cost));
                        command.Parameters.Add(new SqliteParameter("@resourceAmount", weapon.resource_amount));
                        command.Parameters.Add(new SqliteParameter("@resourceType", weapon.resource_type));
                        command.Parameters.Add(new SqliteParameter("@weaponId", weapon.weapon_id));
                        command.Parameters.Add(new SqliteParameter("@unlocked", weapon.isUnlocked));
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }

            // Equipment
            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();
                foreach (var equipment in inventory.equipments)
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"UPDATE Equipment SET 
                            name = @name, 
                            hp = @hp, 
                            def = @def, 
                            atk = @atk, 
                            cost = @cost, 
                            resource_amount = @resourceAmount, 
                            resource_type = @resourceType,
                            unlocked = @unlocked
                            WHERE equipment_id = @equipmentId;";
                        command.Parameters.Add(new SqliteParameter("@name", equipment.name));
                        command.Parameters.Add(new SqliteParameter("@hp", equipment.hp));
                        command.Parameters.Add(new SqliteParameter("@def", equipment.def));
                        command.Parameters.Add(new SqliteParameter("@atk", equipment.atk));
                        command.Parameters.Add(new SqliteParameter("@cost", equipment.cost));
                        command.Parameters.Add(new SqliteParameter("@resourceAmount", equipment.resource_amount));
                        command.Parameters.Add(new SqliteParameter("@resourceType", equipment.resource_type));
                        command.Parameters.Add(new SqliteParameter("@equipmentId", equipment.equipment_id));
                        command.Parameters.Add(new SqliteParameter("@unlocked", equipment.isUnlocked));
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }
        }

        public List<Soldier> GetSoldiers()
        {
            List<Soldier> soldiers = new ();
            foreach (var character in this.soldiersData)
            {
                if (character is Soldier soldier)
                {
                    soldiers.Add(soldier);
                }
            }
            return soldiers;
        }
    }
}