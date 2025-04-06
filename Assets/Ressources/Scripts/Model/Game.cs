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
        public Resources resourcesData;
        public List<Mission> MissionsData;
        public List<Character> soldiersData;
        public List<Base> basesData;
        public Tech techData;

        public int maxSoldier;

        public Game()
        {
            this.resourcesData = new Resources();
            this.MissionsData = new List<Mission>();
            this.soldiersData = new List<Character>();
            this.basesData = new List<Base>(); //list of buildings
            //this.basesData.Add(new Base(0, "Main Base", "Default main base", 1, 0, 0, 0, true, false, 0, 0));  //i comment this out for now cuz the base class is each building
            this.techData = new Tech();
        }

        public Game(string dbName)
        {
            //Set default maxSoldier
            maxSoldier = 5;

            int food = 0;
            int wood = 0;
            int stone = 0;
            int metal = 0;
            int fuel = 0;
            int ammo = 0;
            int medicine = 0;

            // resources
            using (var connection = new SqliteConnection(dbName))
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
                                    wood = currentAmount;
                                    break;
                                case 2:
                                    stone = currentAmount;
                                    break;
                                case 3:
                                    metal = currentAmount;
                                    break;
                                case 4:
                                    fuel = currentAmount;
                                    break;
                                case 5:
                                    ammo = currentAmount;
                                    break;
                                case 6:
                                    medicine = currentAmount;
                                    break;
                                default:
                                    // Debug.LogWarning("Unexpected resource id: " + resourceId);
                                    break;
                            }
                        }
                        reader.Close();
                    }
                }
                connection.Close();
            }
            this.resourcesData = new Resources(food, wood, stone, metal, fuel, ammo, medicine);

            // Bases
            this.basesData = new List<Base>();
            using (var connection = new SqliteConnection(dbName))
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
            BaseManager.Instance.buildingList = basesData;

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
            using (var connection = new SqliteConnection(dbName))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT mission_id, name, description, difficulty, reward_money, reward_amount, reward_resource, terrain, weather, unlocked FROM Mission ORDER BY mission_id ASC;";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = int.Parse(reader["mission_id"].ToString());
                            string name = reader["name"].ToString();
                            string description = reader["description"].ToString();
                            int difficulty = int.Parse(reader["difficulty"].ToString());
                            int rewardMoney = int.Parse(reader["reward_money"].ToString());
                            int rewardAmount = int.Parse(reader["reward_amount"].ToString());
                            int rewardResourceId = int.Parse(reader["reward_resource"].ToString());
                            string terrain = reader["terrain"].ToString();
                            string weather = reader["weather"].ToString();
                            bool isUnlocked = bool.Parse(reader["unlocked"].ToString());

                            Mission mission = new Mission(id, name, description, difficulty, rewardMoney, rewardAmount, rewardResourceId, terrain, weather, isUnlocked);
                            this.MissionsData.Add(mission);
                        }
                        reader.Close();
                    }
                }
                connection.Close();
            }

            // Soldiers
            this.soldiersData = new List<Character>();
            using (var connection = new SqliteConnection(dbName))
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
                            Soldier soldier = new Soldier(name, role, level, health, attack, defense, maxHealth);
                            this.soldiersData.Add(soldier);
                        }
                        reader.Close();
                    }
                }
                connection.Close();
            }

            // tech
            using (var connection = new SqliteConnection(dbName))
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
        }

        public void SaveGameData()
        {
            string dbName = "URI=file:" + Application.persistentDataPath + "/game.db";

            // resources
            using (var connection = new SqliteConnection(dbName))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    for (int id = 0; id <= 6; id++)
                    {
                        int amount = this.resourcesData.GetAmount(id);
                        command.CommandText = $"UPDATE Resource SET current_amount = {amount} WHERE resource_id = {id};";
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }

            // bases
            using (var connection = new SqliteConnection(dbName))
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
                            command.Parameters.Add(new SqliteParameter("@unlocked", b.placed ? 1 : 0));
                            command.Parameters.Add(new SqliteParameter("@cost", b.x));
                            command.Parameters.Add(new SqliteParameter("@cost", b.y));
                            command.Parameters.Add(new SqliteParameter("@building_id", b.building_id));
                            command.ExecuteNonQuery();
                        }
                    }
                }
                connection.Close();
            }

            // missions
            using (var connection = new SqliteConnection(dbName))
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
                            unlocked = @isUnlocked
                            WHERE mission_id = @id;";
                        command.Parameters.Add(new SqliteParameter("@name", mission.name));
                        command.Parameters.Add(new SqliteParameter("@description", mission.description));
                        command.Parameters.Add(new SqliteParameter("@difficulty", mission.difficulty));
                        command.Parameters.Add(new SqliteParameter("@rewardMoney", mission.rewardMoney));
                        command.Parameters.Add(new SqliteParameter("@rewardAmount", mission.rewardAmount));
                        command.Parameters.Add(new SqliteParameter("@rewardResourceId", mission.rewardResourceId));
                        command.Parameters.Add(new SqliteParameter("@terrain", mission.terrain));
                        command.Parameters.Add(new SqliteParameter("@weather", mission.weather));
                        command.Parameters.Add(new SqliteParameter("@isUnlocked", mission.isCompleted ? 1 : 0));
                        command.Parameters.Add(new SqliteParameter("@id", mission.id));
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }

            // soldiers
            using (var connection = new SqliteConnection(dbName))
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
                            command.Parameters.Add(new SqliteParameter("@attack", soldier.Atk));
                            command.Parameters.Add(new SqliteParameter("@defense", soldier.Def));
                            command.Parameters.Add(new SqliteParameter("@roleName", soldier.GetRoleName()));
                            command.Parameters.Add(new SqliteParameter("@id", soldier.Name));
                            command.ExecuteNonQuery();
                        }
                    }
                }
                connection.Close();
            }

            // Tech
            using (var connection = new SqliteConnection(dbName))
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
        }
    }
}