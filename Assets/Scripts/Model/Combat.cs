using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model
{
    [System.Serializable]
    public class Combat
    {
        private readonly List<Enemy> enemies;
        private readonly List<Soldier> soldiers;
        public Combat(List<Enemy> enemies, List<Soldier> soldiers)
        {
            Debug.Log("Combat Created");
            this.enemies = enemies;
            this.soldiers = soldiers;
        }

        public bool CombatStart()
        {
            Debug.Log("Combat Start");
            while (true)
            {
                // Player Turn
                foreach (Soldier soldier in soldiers)
                {
                    // TODO: Player select movement
                    // ...

                    // Choose a random enemy to attack (should be replaced by human interaction)
                    int randomIndex = Random.Range(0, enemies.Count);
                    Enemy targetEnemy = enemies[randomIndex];
                    targetEnemy.TakeDamage(soldier.GetAttack());

                    if (enemies.Count == 0)
                    {
                        Debug.Log("Player Wins");
                        return true;
                    }
                }

                // Enemy Turn
                foreach (Enemy enemy in enemies)
                {
                    // Choose a random soldier to attack (To be replaced AI logic)
                    int randomIndex = Random.Range(0, soldiers.Count);
                    Soldier targetSoldier = soldiers[randomIndex];

                    Debug.Log($"Enemy attacks a soldier");
                    // TODO: Here you would implement the actual attack logic
                    // Example: enemy.Attack(targetSoldier);

                    // After attack, check if the soldier was defeated
                    // Example: if (!targetSoldier.IsAlive()) soldiers.Remove(targetSoldier);

                    // Check if all soldiers are defeated
                    if (soldiers.Count == 0)
                    {
                        Debug.Log("Enemy Wins");
                        return false;
                    }
                }
            }
        }
    }
}