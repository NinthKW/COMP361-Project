using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Assets.Scripts.Model 
{
    public abstract class Ability : MonoBehaviour
    {
        public string Name { get; protected set; }
        public int Cost { get; protected set; }
        public int CooldownCounter { get; protected set; } = 0;
        public int Cooldown { get; protected set; }
        public int DurationCounter { get; protected set; } = 0;
        public int Duration { get; protected set; }
        public string Description { get; protected set; }
        public string Type { get; protected set; }
        
        // Remove constructor and add Initialize method
        public virtual void Initialize(string name, int cost, int cooldown, int duration, string description, string type)
        {
            Name = name;
            Cost = cost;
            Cooldown = cooldown;
            CooldownCounter = 0;
            Duration = duration;
            Description = description;
            Type = type;
        }

        public bool IsOnCooldown => CooldownCounter > 0;
        public bool IsActive => DurationCounter > 0;

        public virtual bool Activate(List<Character> targets)
        {
            if (IsOnCooldown) 
            {
                Debug.LogWarning($"Ability {Name} is on cooldown.");
                return false;
            }
            if (targets == null || targets.Count == 0) 
            {
                Debug.LogWarning($"No targets available for ability {Name}.");
                return false;
            }
            CooldownCounter = Cooldown;
            DurationCounter = Duration;
            return true;
        }
    }

    #region Heals
    public class HealAbility : Ability
    {
        public int HealAmount { get; private set; }
        
        public void Initialize(string name, int cost, int cooldown, string description, int healAmount)
        {
            base.Initialize(name, cost, cooldown, 0, description, "Heal");
            HealAmount = healAmount;
        }

        public override bool Activate(List<Character> targets)
        {
            if (!base.Activate(targets)) return false;
            foreach (var target in targets)
            {
                if (target != null && target.Health < target.MaxHealth)
                {
                    target.Health = Mathf.Min(target.Health + HealAmount, target.MaxHealth);
                    // Debug.Log($"{target.Name} healed for {HealAmount} health!");
                }
            }
            return true;
        }
    }

    public class HealBuffAbility : Ability
    {
        public int HealAmount { get; private set; }
        public int BuffDefAmount { get; private set; }
        private float healPerTurn;
        private Buff healBuff;

        public void Initialize(string name, int cost, int cooldown, int duration, string description, int healAmount, int buffDefAmount)
        {
            base.Initialize(name, cost, cooldown, duration, description, "HealBuff");
            HealAmount = healAmount;
            BuffDefAmount = buffDefAmount;
            healPerTurn = (float) HealAmount / Duration;
            healBuff = new Buff("HealBuff", Duration, true, 
                                effectPerTurn: (target) => 
                                {
                                    target.Health = Mathf.Min(target.Health + (int) healPerTurn, target.MaxHealth);
                                    target.Def += BuffDefAmount;
                                    return 0; // Return value is not used in this context
                                });
        }

        public override bool Activate(List<Character> targets)
        {
            if (!base.Activate(targets)) return false;
            foreach (var target in targets)
            {
                if (target != null)
                {
                    target.Buffs.Remove(this);
                    target.Buffs.Add(this, healBuff);
                    Debug.Log($"{target.Name} will heal for {healPerTurn} per turn for the rest of the combat and defense increased by {BuffDefAmount}!");
                }
            }
            return true;
        }
    }
    #endregion

    #region buff abilities
    public class ShieldAbility : Ability
    {
        public int ShieldAmount { get; private set; }

        public void Initialize(string name, int cost, int cooldown, int duration, string description, int shieldAmount)
        {
            base.Initialize(name, cost, cooldown, duration, description, "Shield");
            ShieldAmount = shieldAmount;
        }

        public override bool Activate(List<Character> targets)
        {
            if (!base.Activate(targets)) return false;
            foreach (var target in targets)
            {
                if (target != null)
                {
                    target.Shield += ShieldAmount;
                    // Debug.Log($"{target.Name} shielded for {ShieldAmount}!");
                }
            }
            return true;
        }
    }
    
    public class BuffAtkAbility : Ability
    {
        public int BuffAtkAmount { get; private set; }
        public int BuffSpeedAmount { get; private set; }
        private Buff atkBuff;

        public void Initialize(string name, int cost, int cooldown, int duration, string description, int buffAtkAmount, int buffSpeedAmount=1)
        {
            base.Initialize(name, cost, cooldown, duration, description, "Buff");
            BuffAtkAmount = buffAtkAmount;
            BuffSpeedAmount = buffSpeedAmount;
            atkBuff = new Buff
            ("BuffAtk", duration, false, 
                effectOnStart: (target) => 
                {
                    target.Atk += BuffAtkAmount;
                    target.AttackChances += BuffSpeedAmount;
                    return 0; // Return value is not used in this context
                },
                effectOnExpire: (target) =>
                {
                    target.Atk -= BuffAtkAmount;
                    return 0; // Return value is not used in this context
                }
            );
        }

        public override bool Activate(List<Character> targets)
        {
            if (!base.Activate(targets)) return false;
            foreach (var target in targets)
            {
                if (target != null)
                {
                    target.Buffs.Remove(this);
                    target.Buffs.Add(this, atkBuff);
                    // Debug.Log($"{target.Name}'s attack increased by {BuffAtkAmount} and speed increased by {BuffSpeedAmount}!");
                }
            }
            return true;
        }
    }
    #endregion

    #region controls
    public class TauntAbility : Ability
    {
        public int TauntDuration { get; private set; }
        public int BuffDefAmount { get; private set; }
        private Buff tauntBuff;

        public void Initialize(string name, int cost, int cooldown, int duration, string description, int buffDefAmount)
        {
            base.Initialize(name, cost, cooldown, duration, description, "TauntAll");
            TauntDuration = duration;
            BuffDefAmount = buffDefAmount;
            tauntBuff = new Buff("Taunt", TauntDuration, false, 
                                (target) => target.Def += BuffDefAmount);
        }

        public override bool Activate(List<Character> targets)
        {
            if (!base.Activate(targets)) return false;
            foreach (var target in targets)
            {
                if (target != null)
                {
                    target.Buffs.Remove(this);
                    target.Buffs.Add(this, tauntBuff);
                    // Debug.Log($"{target.Name} is taunting for {TauntDuration} turns and defense increased by {BuffDefAmount}!");
                }
            }
            return true;
        }
    }
    #endregion


    #region buffs
    public class Buff
    {
        public string Name { get; set; }
        public int Duration { get; set; }
        public bool Permanent { get; set; } = false;
        public Func<Character, int> EffectPerTurn { get; set; } = null;
        public Func<Character, int> EffectOnStart { get; set; } = null;
        public Func<Character, int> EffectOnExpire { get; set; } = null;
        private bool Start;
        public Buff(string name, int duration, bool permanent = false, 
                    Func<Character, int> effectPerTurn = null,
                    Func<Character, int> effectOnStart = null,
                    Func<Character, int> effectOnExpire = null)
        {
            Name = name;
            Duration = duration;
            Permanent = permanent;
            if (Permanent) Duration = 999;
            EffectPerTurn = effectPerTurn;
            EffectOnStart = effectOnStart;
            EffectOnExpire = effectOnExpire;
            Start = true;
        }

        public void UpdateDuration(Character target)
        {
            if (target == null || target.IsDead()) return;
            if (IsExpired()) return; // Buff is expired
            if (Start && EffectOnStart != null)
            {
                EffectOnStart(target);
                Start = false; // Only apply once at the start
            }
            if (EffectPerTurn != null)
            {
                EffectPerTurn(target);
            }
            Duration--;
            if (EffectOnExpire != null && Duration == 0 && !Permanent)
            {
                EffectOnExpire(target);
            }
        }
        public bool IsExpired()
        {
            return !Permanent && Duration <= 0;
        }

        public void Apply(Func<Character, int> effect, Character target)
        {
            if (target == null || target.IsDead()) return;
            if (IsExpired()) return; // Buff is expired
            effect(target);
        }
    }
    #endregion
}