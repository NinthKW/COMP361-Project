using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Model;
using System.Data;
using Mono.Data.Sqlite;


namespace Assets.Scripts.Model
{
    [System.Serializable]
    public class Game
    {
         
        public Resources resourcesData;
        public List <Mission> MissionsData;
        public List <Character> soldiersData;
        public List <Base> basesData;
        public Tech techData;

        public Game()
        {
            this.resourcesData = new Resources();
            this.MissionsData = new List <Mission> ();
            this.soldiersData = new List <Character> ();
            this.basesData = new List <Base> ();
            this.basesData.Add(new Base());
            this.techData = new Tech();
        }

        public Game(string dbName)
        {
            // Load Resources
            int food = 0;
            int wood = 0;
            int stone = 0;
            int metal = 0;
            int fuel = 0;
            int ammo = 0;
            int medicine = 0;

            using (var connection = new SqliteConnection(dbName))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT id, amount FROM Resources;";

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = int.Parse(reader["id"].ToString());
                            int amount = int.Parse(reader["amount"].ToString());

                            switch (id)
                            {
                                case 0:
                                    food = amount;
                                    break;
                                case 1:
                                    wood = amount;
                                    break;
                                case 2:
                                    stone = amount;
                                    break;
                                case 3:
                                    metal = amount;
                                    break;
                                case 4:
                                    fuel = amount;
                                    break;
                                case 5:
                                    ammo = amount;
                                    break;
                                case 6:
                                    medicine = amount;
                                    break;
                                default:
                                    Debug.LogWarning("Unexpected resource id: " + id);
                                    break;
                            }
                        }
                        reader.Close();
                    }
                }
                connection.Close();
            }
            this.resourcesData = new Resources(food, wood, stone, metal, fuel, ammo, medicine);

            // Load Bases
            List<Base> loadedBases = new List<Base>();
            string[] baseTables = new string[] { "base1", "base2", "base3", "base4"};

            using (var connection = new SqliteConnection(dbName))
            {
                connection.Open();
                foreach (string table in baseTables)
                {
                    List<int> buildingIds = new List<int>();
                    List<int> buildingLvls = new List<int>();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT id, level FROM " + table + " ORDER BY id ASC;";
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = int.Parse(reader["id"].ToString());
                                int level = int.Parse(reader["level"].ToString());
                                buildingIds.Add(id);
                                buildingLvls.Add(level);
                            }
                            reader.Close();
                        }
                    }
                    this.basesData.Add(new Base(buildingIds, buildingLvls));
                }
                connection.Close();
            }


            // Load Missions
            this.MissionsData = new List<Mission>();

            using (var connection = new SqliteConnection(dbName))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT id, name, description, difficulty, rewardMoney, rewardAmount, rewardResourceId, terrain, weather FROM Missions ORDER BY id ASC;";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = int.Parse(reader["id"].ToString());
                            string name = reader["name"].ToString();
                            string description = reader["description"].ToString();
                            int difficulty = int.Parse(reader["difficulty"].ToString());
                            int rewardMoney = int.Parse(reader["rewardMoney"].ToString());
                            int rewardAmount = int.Parse(reader["rewardAmount"].ToString());
                            int rewardResourceId = int.Parse(reader["rewardResourceId"].ToString());
                            string terrain = reader["terrain"].ToString();
                            string weather = reader["weather"].ToString();
                            bool isUnlocked = bool.Parse(reader["isUnlocked"].ToString());

                            Mission mission = new Mission(id, name, description, difficulty, rewardMoney, rewardAmount, rewardResourceId, terrain, weather, isUnlocked);
                            this.MissionsData.Add(mission);
                        }
                        reader.Close();
                    }
                }
                connection.Close();
            }

            // Load Characters
            this.soldiersData = new List<Character>();

            using (var connection = new SqliteConnection(dbName))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Characters ORDER BY id ASC;";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Determine the character type (e.g., "Soldier" or "Enemy")
                            string characterType = reader["characterType"].ToString();

                            if (characterType == "Soldier")
                            {
                                string name = reader["name"].ToString();
                                int level = int.Parse(reader["level"].ToString());
                                int health = int.Parse(reader["health"].ToString());
                                int attack = int.Parse(reader["attack"].ToString());
                                int defense = int.Parse(reader["defense"].ToString());
                                int maxHealth = int.Parse(reader["maxHealth"].ToString());

                                // Assuming role information is stored in the table:
                                string roleName = reader["roleName"].ToString();
                                int baseAttackChance = int.Parse(reader["baseAttackChance"].ToString());
                                // Instantiate a Role (you should have a Role class defined accordingly)
                                Role role = new Role(roleName);

                                Soldier soldier = new Soldier(name, role, level, health, attack, defense, maxHealth);
                                this.soldiersData.Add(soldier);
                            }
                            else if (characterType == "Enemy")
                            {
                                string name = reader["name"].ToString();
                                int level = int.Parse(reader["level"].ToString());
                                int health = int.Parse(reader["health"].ToString());
                                int damage = int.Parse(reader["damage"].ToString());
                                int expReward = int.Parse(reader["expReward"].ToString());

                                Enemy enemy = new Enemy(name, health, damage, level, expReward);
                                this.soldiersData.Add(enemy);
                            }
                            else
                            {
                                Debug.LogWarning("Unknown character type: " + characterType);
                            }
                        }
                        reader.Close();
                    }
                }
                connection.Close();
            }

            // Load Tech
            using (var connection = new SqliteConnection(dbName))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT techId, techName, description, costMoney, costResourceId, costResourceAmount, isUnlocked FROM Tech LIMIT 1;";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int techId = int.Parse(reader["techId"].ToString());
                            string techName = reader["techName"].ToString();
                            string description = reader["description"].ToString();
                            float costMoney = float.Parse(reader["costMoney"].ToString());
                            int costResourceId = int.Parse(reader["costResourceId"].ToString());
                            int costResourceAmount = int.Parse(reader["costResourceAmount"].ToString());
                            bool isUnlocked = bool.Parse(reader["isUnlocked"].ToString());

                            Tech tech = new Tech(techId, techName, description, costMoney, costResourceId, costResourceAmount);
                            tech.isUnlocked = isUnlocked;
                            this.techData = tech;
                        }
                        reader.Close();
                    }
                }
                connection.Close();
            }
        }

        public void SaveGameData() 
        {
            // Save Resources
            string dbName = "URI=file:" + Application.persistentDataPath + "/game.db";
            using (var connection = new SqliteConnection(dbName))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    // Loop through resource ids 0-6
                    for (int id = 0; id <= 6; id++)
                    {
                        int amount = this.resourcesData.GetAmount(id);
                        command.CommandText = $"UPDATE Resources SET amount = {amount} WHERE id = {id};";
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }

            // Save Base
            string[] baseTables = new string[] { "base1", "base2", "base3", "base4" };
            using (var connection = new SqliteConnection(dbName))
            {
                connection.Open();
                for (int i = 1; i < this.basesData.Count && (i - 1) < baseTables.Length; i++)
                {
                    string table = baseTables[i - 1];
                    Base currentBase = this.basesData[i];
                    Dictionary<int, int> buildings = currentBase.GetBuildings();
                    foreach (var kvp in buildings)
                    {
                        int buildingId = kvp.Key;
                        int level = kvp.Value;
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = $"UPDATE {table} SET level = {level} WHERE id = {buildingId};";
                            command.ExecuteNonQuery();
                        }
                    }
                }
                connection.Close();
            }

            // Save Mission
            using (var connection = new SqliteConnection(dbName))
            {
                connection.Open();
                foreach (var mission in this.MissionsData)
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"UPDATE Missions SET 
                            name = @name, 
                            description = @description, 
                            difficulty = @difficulty, 
                            rewardMoney = @rewardMoney, 
                            rewardAmount = @rewardAmount, 
                            rewardResourceId = @rewardResourceId, 
                            terrain = @terrain, 
                            weather = @weather, 
                            isCompleted = @isCompleted
                            WHERE id = @id;";
                        command.Parameters.Add(new SqliteParameter("@name", mission.name));
                        command.Parameters.Add(new SqliteParameter("@description", mission.description));
                        command.Parameters.Add(new SqliteParameter("@difficulty", mission.difficulty));
                        command.Parameters.Add(new SqliteParameter("@rewardMoney", mission.rewardMoney));
                        command.Parameters.Add(new SqliteParameter("@rewardAmount", mission.rewardAmount));
                        command.Parameters.Add(new SqliteParameter("@rewardResourceId", mission.rewardResourceId));
                        command.Parameters.Add(new SqliteParameter("@terrain", mission.terrain));
                        command.Parameters.Add(new SqliteParameter("@weather", mission.weather));
                        command.Parameters.Add(new SqliteParameter("@isCompleted", mission.isCompleted ? 1 : 0));
                        command.Parameters.Add(new SqliteParameter("@id", mission.id));
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }

            // Soldiers
            using (var connection = new SqliteConnection(dbName))
            {
                connection.Open();
                foreach (var character in this.soldiersData)
                {
                    using (var command = connection.CreateCommand())
                    {
                        if (character is Soldier soldier)
                        {
                            command.CommandText = @"UPDATE Characters SET 
                                characterType = 'Soldier',
                                name = @name, 
                                level = @level, 
                                health = @health, 
                                attack = @attack, 
                                defense = @defense,
                                roleName = @roleName,
                                baseAttackChance = @baseAttackChance
                                WHERE id = @id;";
                            command.Parameters.Add(new SqliteParameter("@name", soldier.Name));
                            command.Parameters.Add(new SqliteParameter("@level", soldier.Level));
                            command.Parameters.Add(new SqliteParameter("@health", soldier.Health));
                            command.Parameters.Add(new SqliteParameter("@attack", soldier.Atk));
                            command.Parameters.Add(new SqliteParameter("@defense", soldier.Def));
                            // Assume soldier.GetRoleName() returns the role's name and AttackChances represents the base attack chance.
                            command.Parameters.Add(new SqliteParameter("@roleName", soldier.GetRoleName()));
                            command.Parameters.Add(new SqliteParameter("@baseAttackChance", soldier.AttackChances));
                            command.Parameters.Add(new SqliteParameter("@id", soldier.Name));
                            command.ExecuteNonQuery();
                        }
                        else if (character is Enemy enemy)
                        {
                            command.CommandText = @"UPDATE Characters SET 
                                characterType = 'Enemy',
                                name = @name, 
                                level = @level, 
                                health = @health, 
                                damage = @damage, 
                                expReward = @expReward
                                WHERE id = @id;";
                            command.Parameters.Add(new SqliteParameter("@name", enemy.Name));
                            command.Parameters.Add(new SqliteParameter("@level", enemy.Level));
                            command.Parameters.Add(new SqliteParameter("@health", enemy.Health));
                            command.Parameters.Add(new SqliteParameter("@damage", enemy.BaseDamage));
                            command.Parameters.Add(new SqliteParameter("@expReward", enemy.ExperienceReward));
                            command.Parameters.Add(new SqliteParameter("@id", enemy.Name));
                            command.ExecuteNonQuery();
                        }
                    }
                }
                connection.Close();
            }

            // Save Tech
            using (var connection = new SqliteConnection(dbName))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"UPDATE Tech SET 
                        techName = @techName,
                        description = @description,
                        costMoney = @costMoney,
                        costResourceId = @costResourceId,
                        costResourceAmount = @costResourceAmount,
                        isUnlocked = @isUnlocked
                        WHERE techId = @techId;";
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
        }
    }
}