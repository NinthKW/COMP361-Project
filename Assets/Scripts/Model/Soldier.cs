using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model
{
    [System.Serializable]
    public class Soldier 
    {
        readonly string name;
        bool gun;
        int bomb;
        int ammo;
        bool boots;
        bool vest;
        int health;
        int maxHealth;
        int exp;
        int level;
        int maxLevel;
        int attack;
        int defense;
        Role role;

        private static readonly string[] nameList = { "Messi", "Ronaldo", "Joseph.Vyb", "Trump", "Trudeau", "Poilievre", "Legault", "Jagmeet", "Faker", "Zeus", "Drake", "Obama" };

        public Soldier(Role role)
        {
            this.name = nameList[UnityEngine.Random.Range(0, 11)];
            this.gun = false;
            this.bomb = 0;
            this.ammo = 0;
            this.boots = false;
            this.vest = false;
            this.role = role;
            this.health = role.maxHealth;
            this.maxHealth = role.maxHealth;
            this.exp = 0;
            this.level = 1;
            this.maxLevel = role.maxLevel;
            this.attack = role.base_attack;
            this.defense = role.base_defense;
        }

        public Soldier()
        {
            this.name = nameList[UnityEngine.Random.Range(0, 11)];
            this.gun = false;
            this.bomb = 0;
            this.ammo = 0;
            this.boots = false;
            this.vest = false;
            this.role = new Role(RoleType.Army);
            this.health = role.maxHealth;
            this.maxHealth = role.maxHealth;
            this.exp = 0;
            this.level = 1;
            this.maxLevel = role.maxLevel;
            this.attack = role.base_attack;
            this.defense = role.base_defense;
        }

        public void SetGun(bool value)
        {
            this.gun = value;
        }
        public void SetBomb(int value)
        {
            this.bomb = value;
        }
        public void SetAmmo(int value)
        {
            this.ammo = value;
        }
        public void SetBoots(bool value)
        {
            this.boots = value;
        }
        public void SetVest(bool value)
        {
            this.vest = value;
        }
        public void SetRole(Role value)
        {
            this.role = value;
        }

        public bool GetGun()
        {
            return this.gun;
        }
        public int GetBomb()
        {
            return this.bomb;
        }
        public int GetAmmo()
        {
            return this.ammo;
        }
        public bool GetBoots()
        {
            return this.boots;
        }
        public bool GetVest()
        {
            return this.vest;
        }
        public Role GetRole()
        {
            return this.role;
        }

        public string GetName()
        {
            return this.name;
        }

        public int GetHealth()
        {
            return this.health;
        }

        public int GetMaxHealth()
        {
            return this.maxHealth;
        }

        public int GetExp()
        {
            return this.exp;
        }

        public int GetLevel()
        {
            return this.level;
        }

        public int GetMaxLevel()
        {
            return this.maxLevel;
        }

        public int GetAttack()
        {
            return this.attack;
        }

        public int GetDefense()
        {
            return this.defense;
        }

        public void SetHealth(int value)
        {
            this.health = value;
        }

        public void SetMaxHealth(int value)
        {
            this.maxHealth = value;
        }

        public void GainExp(int value)
        {
            this.exp += value;
            CheckLevelUp();
        }

        public void SetAttack(int value)
        {
            this.attack = value;
        }

        public void SetDefense(int value)
        {
            this.defense = value;
        }

        public void CheckLevelUp()
        {
            if (this.level < this.maxLevel)
            {
                if (this.exp >= 100)
                // TODO: add a formula to calculate the exp needed to level up
                {
                    this.level++;
                    this.exp = 0;
                    this.maxHealth += 10;
                    this.health = this.maxHealth;
                    this.attack += 2;
                    this.defense += 1;
                    Debug.Log(this.name + " has leveled up! Level: " + this.level);
                }
            }
        }

        private int ComputeDamage()
        {
            // TODO: Compute damage based on soldier's equipment
            return 10;
        }

        public void AttackEnemy()
        {
            // TODO: Implement attack logic
            int damage = ComputeDamage();
            Debug.Log(this.name + " attacks the enemy!" + " Damage: " + damage);
        }

        public void Defend()
        {
            // TODO: Implement defend logic
            Debug.Log(this.name + " defends against the enemy!");
        }

        public void TakeDamage(int damage)
        {
            damage -= this.defense;
            if (damage < 0)
            {
                damage = 0;
            }
            this.health -= damage;
            if (this.health <= 0)
            {
                Debug.Log(this.name + " has died!");
            }
        }

        public void UseSkill(Skill skill)
        {
            // TODO: Implement skill logic
            Debug.Log(this.name + " uses " + skill.name + "!");
        }

        public void UseItem(string item)
        {
            // TODO: Implement item logic
            Debug.Log(this.name + " uses " + item + "!");
        }
    }
}