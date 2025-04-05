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

    #region Heals
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

    public class HealBuffAbility : Ability
    {
        public int HealAmount { get; private set; }
        public int BuffDefAmount { get; private set; }

        public HealBuffAbility(string name, int cost, int cooldown, int duration, string description, int healAmount, int buffDefAmount) 
            : base(name, cost, cooldown, duration, description, "HealBuff")
        {
            HealAmount = healAmount;
            BuffDefAmount = buffDefAmount;
        }

        public override void Activate(List<Character> targets)
        {
            base.Activate(targets);
            float healPerTurn = (float)HealAmount / Duration;
            foreach (var target in targets)
            {
                if (target != null)
                {
                    // Increase defense immediately
                    target.Def += BuffDefAmount;
                    target.Buffs.Add("HealBuff", this);

                    // Start healing over time (requires a Coroutine runner, e.g., a dedicated MonoBehaviour)
                    CoroutineRunner.Instance.StartCoroutine(HealOverTime(target, healPerTurn));

                    Debug.Log($"{target.Name} will heal for {healPerTurn} per turn for {Duration} turns and defense increased by {BuffDefAmount}!");
                }
            }
        }

        private IEnumerator HealOverTime(Character target, float healPerTurn)
        {
            int turns = Duration;
            while (turns-- > 0)
            {
                if (target.Health < target.MaxHealth)
                {
                    // Apply heal per turn (rounded to an int)
                    int healThisTurn = Mathf.RoundToInt(healPerTurn);
                    target.Health = Mathf.Min(target.Health + healThisTurn, target.MaxHealth);
                    Debug.Log($"{target.Name} heals for {healThisTurn} health.");
                }
                // Wait for 1 second between turns (adjust the duration as needed)
                yield return new WaitForSeconds(1);
            }
        }
    }
    #endregion

    #region buffs
    public class ShieldAbility : Ability
    {
        public int ShieldAmount { get; private set; }

        public ShieldAbility(string name, int cost, int cooldown, int duration, string description, int shieldAmount) 
            : base(name, cost, cooldown, duration, description, "Shield")
        {
            ShieldAmount = shieldAmount;
        }

        public override void Activate(List<Character> targets)
        {
            base.Activate(targets);
            foreach (var target in targets)
            {
                if (target != null)
                {
                    target.Shield += ShieldAmount;
                    target.Buffs.Add("Shield", this);
                    Debug.Log($"{target.Name} gains a shield of {ShieldAmount}!");
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
    #endregion

    #region controls
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
    #endregion
}