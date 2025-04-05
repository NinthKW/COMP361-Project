using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Assets.Scripts.Model
{
    // 基类 Character
    public abstract class Character
    {
        public string Name { get; protected set; }
        public int Health { get; protected set; }
        public int MaxHealth { get; protected set; }
        public int Atk { get; protected set; }
        public int Def { get; protected set; }
        public int Level { get; protected set; }
        public int AttackChances { get; set; }
        public int MaxAttacksPerTurn { get; protected set; }
        public GameObject GameObject { get; private set; }
        public string ObjectTag { get; protected set; }

        protected Character(string name, int health, int level, int attack, int defense, int maxHealth)
        {
            Name = name;
            Health = health;
            MaxHealth = maxHealth;
            Level = level;
            Atk = attack;
            Def = defense;
        }

        public virtual void TakeDamage(int damage)
        {
            damage = Mathf.Max(0, damage);
            Health = Mathf.Max(Health - damage, 0);
            
            if (Health <= 0)
            {
                HandleDeath();
            }
        }

        public virtual void Attack(Character target)
        {
            if (target == null || target.IsDead()) return;
            
            int finalDamage = CalculateDamage();
            Debug.Log($"{Name} attacks {target.Name} with {finalDamage} damage!");
            target.TakeDamage(finalDamage);
        }

        public virtual int GetAttackAmount(Character target)
        {
            if (target == null || target.IsDead()) return 0;

            int finalDamage = CalculateDamage();
            Debug.Log($"{Name} attacks {target.Name} with {finalDamage} damage!");

            return finalDamage;
        }

        protected abstract int CalculateDamage();

        public void SetGameObject(GameObject gameObject)
        {
            GameObject = gameObject;
            UpdateTag();
        }

        public bool IsDead() {
            if (Health <= 0) {
                MaxAttacksPerTurn = 0;
                AttackChances = 0;
                return true;
            }
            return false;
        }

        protected virtual void UpdateTag()
        {
            if (GameObject != null && !string.IsNullOrEmpty(ObjectTag))
            {
                GameObject.tag = ObjectTag;
            }
        }

        protected virtual void HandleDeath()
        {
            Debug.Log($"{Name} has been defeated!");
            // 可以在这里添加死亡动画、物品掉落等逻辑
        }

        public void ResetAttackChances()
        {
            AttackChances = MaxAttacksPerTurn;
        }
    }

    public class Enemy : Character
    {
        public int BaseDamage { get; private set; }
        public int ExperienceReward { get; private set; }

        public Enemy(string name, int health, int damage, int level, int expReward) 
            : base(name, health, level, damage, 0, 0)
        {
            BaseDamage = damage;
            ExperienceReward = expReward;
            ObjectTag = "Enemy";  // 修正原来的错误tag
            AttackChances = 1;
            MaxAttacksPerTurn = 1;
        }

        protected override int CalculateDamage()
        {
            // 可以添加更复杂的伤害计算逻辑（例如暴击、等级修正）
            return BaseDamage + Level * 2;
        }

        protected override void HandleDeath()
        {
            base.HandleDeath();
            // Enemy特有的死亡逻辑（例如触发任务更新）
            Debug.Log($"Dropping {ExperienceReward} experience points!");
        }
    }

    public class Soldier : Character
    {
        private int _experience;
        private bool _hasGun;
        private int _defense;
        private Role _role;

        public Soldier(string name, Role role, int level, int health, int attack, int defense, int maxHealth) 
        : base(name, health, level, attack, defense, maxHealth)
        {
            _role = role;
            AttackChances = role.BaseAttackChance;
            MaxAttacksPerTurn = role.BaseAttackChance;
            _experience = 0;
            ObjectTag = "Soldier";
        }

        protected override int CalculateDamage()
        {
            int baseDamage = 10;  // 基础伤害
            if (_hasGun) baseDamage += 15;
            return baseDamage + Level * 3;
        }

        public string GetRoleName()
        {
            return _role.GetRoleName();
        }

        public void GainExp(int amount)
        {
            _experience += amount;
            CheckLevelUp();
        }

        public void CheckLevelUp()
        {
            if (_experience >= Level * 100)
            {
                Level++;
                MaxHealth += 10;
                Health = MaxHealth;
                Debug.Log($"{Name} has leveled up to {Level}!");
            }
        }

        public override void TakeDamage(int damage)
        {
            int mitigatedDamage = Mathf.Max(0, damage - _defense);
            base.TakeDamage(mitigatedDamage);
        }
    }

    // 随机名称生成器
    public static class NameGenerator
    {
        private static readonly string[] Names = {
            "Alice", "Bob", "Charlie", "David", "Eve", "Frank", "Grace", "Heidi", "Ivan", "Judy"
        };
        
        public static string GetRandomName()
        {
            return Names[UnityEngine.Random.Range(0, Names.Length)];
        }
    }
}