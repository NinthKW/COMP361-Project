using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Assets.Scripts.Model 
{
    public abstract class Ability
    {
        public string Name { get; protected set; }
        public int Cost { get; protected set; }
        public int Cooldown { get; protected set; } = 0;
        public int Duration { get; protected set; }
        public string Description { get; protected set; }
        public string Type { get; protected set; }
        public bool IsActive { get; private set; } = false;
        public bool IsOnCooldown { get; private set; } = false;
        
        public Ability(string name, int cost, int cooldown, int duration, string description, string type)
        {
            Name = name;
            Cost = cost;
            Cooldown = cooldown;
            Duration = duration;
            Description = description;
            Type = type;
        }

        public virtual void Activate(List<Character> targets)
        {
            if (IsOnCooldown) 
            {
                Debug.LogWarning($"Ability {Name} is on cooldown.");
                return;
            }
            if (targets == null || targets.Count == 0) 
            {
                Debug.LogWarning($"No targets available for ability {Name}.");
                return;
            }
            IsActive = true;
            IsOnCooldown = true;
        }

        public virtual void ResetCooldown()
        {
            IsOnCooldown = false;
        }
    }

    public class HealAbility : Ability
    {
        public int HealAmount { get; private set; }
        public HealAbility(string name, int cost, int cooldown, int duration, string description, int healAmount) 
            : base(name, cost, cooldown, duration, description, "Heal")
        {
            HealAmount = healAmount;
        }

        public override void Activate(List<Character> targets)
        {
            base.Activate(targets);
            foreach (var target in targets)
            {
                if (target != null && target.Health < target.MaxHealth)
                {
                    target.Health = Mathf.Min(target.Health + HealAmount, target.MaxHealth);
                    Debug.Log($"{target.Name} healed for {HealAmount} health!");
                }
            }
        }
    }

    public class BuffAtkAbility : Ability
    {
        public int BuffAtkAmount { get; private set; }
        public int BuffSpeedAmount { get; private set; }

        public BuffAtkAbility(string name, int cost, int cooldown, int duration, string description, int buffAtkAmount, int buffSpeedAmount=1) 
            : base(name, cost, cooldown, duration, description, "Buff")
        {
            BuffAtkAmount = buffAtkAmount;
            BuffSpeedAmount = buffSpeedAmount;
        }

        public override void Activate(List<Character> targets)
        {
            base.Activate(targets);
            foreach (var target in targets)
            {
                if (target != null)
                {
                    target.Atk += BuffAtkAmount;
                    target.AttackChances += BuffSpeedAmount;
                    target.Buffs.Add("BuffAtk", this);
                    Debug.Log($"{target.Name}'s attack increased by {BuffAtkAmount} and speed increased by {BuffSpeedAmount}!");
                }
            }
        }
    }

    public class TauntAbility : Ability
    {
        public int TauntDuration { get; private set; }
        public int BuffDefAmount { get; private set; }

        public TauntAbility(string name, int cost, int cooldown, int duration, string description, int buffDefAmount) 
            : base(name, cost, cooldown, duration, description, "Control")
        {
            TauntDuration = duration;
            BuffDefAmount = buffDefAmount;
        }

        public override void Activate(List<Character> targets)
        {
            base.Activate(targets);
            foreach (var target in targets)
            {
                if (target != null)
                {
                    target.Buffs.Add("Taunt", this);
                    target.Def += BuffDefAmount;
                    Debug.Log($"{target} taunts enemies for {TauntDuration} turns!");
                }
            }
        }
    }
}