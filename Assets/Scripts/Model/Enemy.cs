using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assets.Scripts.Model
{
    public class Enemy
    {
        public string name;
        public int health;
        public int damage;
        public int level;

        public Enemy(string name, int health, int damage, int level)
        {
            this.name = name;
            this.health = health;
            this.damage = damage;
            this.level = level;
        }

        public void TakeDamage(int damage)
        {
            health -= damage;
        }

        public void Attack(Soldier soldier)
        {
            soldier.TakeDamage(damage);
        }
    }
}