using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    [System.Serializable]
    public class Combat
    {
        public int playerHealth = 100;
        public int enemyHealth = 50;

        public void AttackEnemy(int damage)
        {
            enemyHealth -= damage;
            if (enemyHealth <= 0)
            {
                enemyHealth = 0;
                Debug.Log("Enemy Defeated!");
            }
        }

        public void ReceiveDamage(int damage)
        {
            playerHealth -= damage;
            if (playerHealth <= 0)
            {
                playerHealth = 0;
                Debug.Log("Game Over!");
            }
        }
    }
}