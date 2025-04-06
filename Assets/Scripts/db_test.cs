using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;

public class db_test : MonoBehaviour
{
    private string dbName = "URI=file:database.db";
    // Start is called before the first frame update
    void Start()
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Soldier;";
                
                using (IDataReader reader  = command.ExecuteReader())
                {
                    while (reader.Read()) {
                        // Debug.Log("Name: " + reader["hp"]);
                    }

                    reader.Close();
                }
            }

            connection.Close();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
