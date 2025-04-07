using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.Scripts.Controller;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Assets.Scripts.Model
{
    // 基类 Character
    [Serializable]
    public abstract class Character
    {
        public string Name { get; protected set; }
        public int Health { get; set; }
        public int MaxHealth { get; protected set; }
        public int Atk { get; set; }
        public int Def { get; set; }
        public int Shield { get; set; } = 0;
        public int Experience { get; set; } = 0;
        public int Level { get; protected set; }
        public int AttackChances { get; set; }
        public int MaxAttacksPerTurn { get; set; }
        public GameObject GameObject { get; private set; }
        public string ObjectTag { get; protected set; }
        public Dictionary<Ability, Buff> Buffs { get; private set; } = new Dictionary<Ability, Buff>();

        protected Character(string name, int health, int level, int attack, int defense, int maxHealth)
        {
            Name = name;
            Health = health;
            MaxHealth = maxHealth;
            Experience = level * 100;
            Level = level;
            Atk = attack;
            Def = defense;
        }

        public virtual void TakeDamage(int damage)
        {
            int mitigatedDamage = Mathf.Max(0, damage - Def);  // 根据防御力减少伤害
            mitigatedDamage = Mathf.Max(1, mitigatedDamage);   // 至少造成1点伤害

            if (Shield > 0)
            {
                if (mitigatedDamage > Shield)
                {
                    mitigatedDamage -= Shield;
                    Shield = 0;
                }
                else
                {
                    Shield -= mitigatedDamage;
                    mitigatedDamage = 0;
                }
            }

            Health = Mathf.Max(Health - mitigatedDamage, 0);

            if (Health <= 0)
            {
                HandleDeath();
            }
        }
    

        public virtual void Attack(Character target)
        {
            if (target == null || target.IsDead()) return;
            
            int finalDamage = CalculateDamage(target);  // 修改此处为传入 target
            Debug.Log($"{Name} attacks {target.Name} with {finalDamage} damage!");
            target.TakeDamage(finalDamage);
        }

        public virtual int GetAttackAmount(Character target)
        {
            if (target == null || target.IsDead()) return 0;

            int finalDamage = CalculateDamage(target);  // 修改此处为传入 target
            return finalDamage;
        }

        protected virtual int CalculateDamage(Character target)
        {
            // 基础伤害 = 攻击者的攻击力 - 目标的防御力
            int damage = Atk - target.Def;
            damage = Mathf.Max(1, damage);  // 确保伤害至少为 1
            return damage;
        }

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

        protected virtual void HandleDeath(Character killer = null)
        {
            Debug.Log($"{Name} has died!");
            // 可以在这里添加死亡动画、物品掉落等逻辑
            GameObject.SetActive(false);
            GameObject.tag = "Untagged";  // 清除tag
            GameObject = null;  // 清除引用
        }


        public void ResetAttackChances()
        {
            AttackChances = MaxAttacksPerTurn;
        }
    }

    [Serializable]
    public class Enemy : Character
    {
        public int BaseDamage { get; private set; }
        public int ExperienceReward { get; private set; }

        public Enemy(string name, int health, int damage, int defense, int maxHealth, int level, int expReward) 
            : base(name, health, level, damage, defense, maxHealth)
        {
            BaseDamage = damage;
            ExperienceReward = expReward;
            ObjectTag = "Enemy";  // 修正原来的错误tag
            AttackChances = 1;
            MaxAttacksPerTurn = 1;
        }

        protected override int CalculateDamage(Character target)
        {
            int damage = BaseDamage + Level * 2 - target.Def;
            damage = Mathf.Max(1, damage);  // 确保至少造成 1 点伤害
            return damage;
        }

        protected override void HandleDeath(Character killer = null)
        {
            base.HandleDeath();
            // Enemy特有的死亡逻辑（例如触发任务更新）
            Debug.Log($"Dropping {ExperienceReward} experience points!");
        }
    }

    [Serializable]
    public class Soldier : Character
    {
        private int _experience;
        private readonly bool _hasGun;
        private readonly Role _role;
        public List<Ability> Abilities { get; private set; } = new List<Ability>();


        public Soldier(string name, Role role, int level, int health, int attack, int defense, int maxHealth) 
        : base(name, health, level, attack, defense, maxHealth)
        {
            _role = role;
            AttackChances = 0;
            MaxAttacksPerTurn = role.BaseAttackChance + (level / 5 + 1);
            _experience = 0;
            ObjectTag = "Soldier";
            Def = defense;
        }

        protected override int CalculateDamage(Character target)
        {
            int baseDamage = Atk;  // 基础攻击力
            if (_hasGun) baseDamage += 15;

            // 根据攻击者与目标的防御力差异计算伤害
            int damage = baseDamage + Level * 3 - target.Def;
            damage = Mathf.Max(1, damage);  // 确保至少造成 1 点伤害

            return damage;
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
                _experience -= Level * 100;
                Level++;
                MaxHealth += 10;
                Health = MaxHealth;
                Atk += 5;
                Def += 2;
                if (Level % 5 == 0)
                {
                    AttackChances++;
                    MaxAttacksPerTurn++;
                }
                AudioManager.Instance.PlaySound("LevelUp");
                UpdateAbilityValues();
                Debug.Log($"{Name} has leveled up to {Level}!");
            }
        }

        public override void TakeDamage(int damage)
        {
            int mitigatedDamage = Mathf.Max(0, damage - Def);
            base.TakeDamage(mitigatedDamage);
        }

        public void ModifyAttack(int amount)
        {
            Atk += amount;
            Debug.Log($"{Name}'s Attack modified by {amount}. New Attack: {Atk}");
        }

        public void ModifyDefense(int amount)
        {
            Def += amount;
            Debug.Log($"{Name}'s Defense modified by {amount}. New Defense: {Def}");
        }

        public void ModifyHP(int amount)
        {
            Health = Mathf.Clamp(Health + amount, 0, MaxHealth);
            Debug.Log($"{Name}'s HP modified by {amount}. New HP: {Health}/{MaxHealth}");
        }

        public void UpdateAbilityValues()
        {
            foreach (var ability in Abilities)
            {
                ability.UpdateAbilityValues(this);
            }
        }
    }
}