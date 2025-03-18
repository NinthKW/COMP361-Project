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
        public List <Soldier> soldiersData;
        public List <Base> basesData;
        public Tech techData;

        public Game()
        {
            this.resourcesData = new Resources();
            this.MissionsData = new List <Mission> ();
            this.soldiersData = new List <Soldier> ();
            this.basesData = new List <Base> ();
            this.basesData.Add(new Base());
            this.techData = new Tech();
        }

        public Game(string dbName)
        {
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
        }    
    }
}