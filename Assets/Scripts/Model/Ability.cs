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

        public virtual void OnTurnEnd(List<Character> targets)
        {
            if (Duration > 0)
            {
                DurationCounter = Mathf.Max(0, DurationCounter - 1);
                if (DurationCounter <= 0)
                {
                    // TODO: add to UI
                    Debug.Log($"{Name} has expired.");
                }
            }
            if (IsOnCooldown)
            {
                Cooldown--;
                if (Cooldown <= 0)
                {
                    // TODO: add to UI
                    Debug.Log($"{Name} is ready to use again.");
                }
            }
        }
    }

    #region Heals
    public class HealAbility : Ability
    {
        public int HealAmount { get; private set; }
        
        public void Initialize(string name, int cost, int cooldown, int duration, string description, int healAmount)
        {
            base.Initialize(name, cost, cooldown, duration, description, "Heal");
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
                    Debug.Log($"{target.Name} healed for {HealAmount} health!");
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

        public void Initialize(string name, int cost, int cooldown, int duration, string description, int healAmount, int buffDefAmount)
        {
            base.Initialize(name, cost, cooldown, duration, description, "HealBuff");
            HealAmount = healAmount;
            BuffDefAmount = buffDefAmount;
        }

        public override bool Activate(List<Character> targets)
        {
            if (!base.Activate(targets)) return false;
            healPerTurn = (float) HealAmount / Duration;
            foreach (var target in targets)
            {
                if (target != null)
                {
                    // Increase defense immediately
                    target.Def += BuffDefAmount;
                    target.Buffs.Add("HealBuff", this);

                    StartCoroutine(HealOverTime(target, healPerTurn));

                    Debug.Log($"{target.Name} will heal for {healPerTurn} per turn for {Duration} turns and defense increased by {BuffDefAmount}!");
                }
            }
            return true;
        }

        public override void OnTurnEnd(List<Character> targets)
        {
            base.OnTurnEnd(targets);
            if (IsActive)
            {
                Duration--;
                Cooldown--;
                if (Cooldown <= 0)
                {
                    IsOnCooldown = false;
                    Debug.Log($"{Name} is ready to use again.");
                }
                if (Duration <= 0)
                {
                    IsActive = false;
                    Debug.Log($"{Name} has expired.");
                    foreach (var target in targets)
                    {
                        if (target != null && target.Buffs.ContainsKey("HealBuff"))
                        {
                            target.Def -= BuffDefAmount;
                            target.Buffs.Remove("HealBuff");
                            Debug.Log($"{target.Name}'s defense decreased by {BuffDefAmount}!");
                        }
                    }
                }
            }
        }
    }
    #endregion

    #region buffs
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
                    target.Buffs.Add("Shield", this);
                    Debug.Log($"{target.Name} gains a shield of {ShieldAmount}!");
                }
            }
            return true;
        }
    }
    
    public class BuffAtkAbility : Ability
    {
        public int BuffAtkAmount { get; private set; }
        public int BuffSpeedAmount { get; private set; }

        public void Initialize(string name, int cost, int cooldown, int duration, string description, int buffAtkAmount, int buffSpeedAmount=1)
        {
            base.Initialize(name, cost, cooldown, duration, description, "Buff");
            BuffAtkAmount = buffAtkAmount;
            BuffSpeedAmount = buffSpeedAmount;
        }

        public override bool Activate(List<Character> targets)
        {
            if (!base.Activate(targets)) return false;
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
            return true;
        }
    }
    #endregion

    #region controls
    public class TauntAbility : Ability
    {
        public int TauntDuration { get; private set; }
        public int BuffDefAmount { get; private set; }

        public void Initialize(string name, int cost, int cooldown, int duration, string description, int buffDefAmount)
        {
            base.Initialize(name, cost, cooldown, duration, description, "Control");
            TauntDuration = duration;
            BuffDefAmount = buffDefAmount;
        }

        public override bool Activate(List<Character> targets)
        {
            if (!base.Activate(targets)) return false;
            foreach (var target in targets)
            {
                if (target != null)
                {
                    target.Buffs.Add("Taunt", this);
                    target.Def += BuffDefAmount;
                    target.Shield += target.Atk;
                    TauntDuration = this.Duration;

                    Debug.Log($"{target} taunts enemies for {TauntDuration} turns!");
                }
            }
            return true;
        }

        public override void OnTurnEnd(List<Character> targets)
        {
            base.OnTurnEnd(targets);
            if (IsActive)
            {
                TauntDuration--;
                Cooldown--;
                if (Cooldown <= 0)
                {
                    IsOnCooldown = false;
                    Debug.Log($"{Name} is ready to use again.");
                }
                if (TauntDuration <= 0)
                {
                    IsActive = false;
                    Debug.Log($"{Name} has expired.");
                    foreach (var target in targets)
                    {
                        if (target != null && target.Buffs.ContainsKey("Taunt"))
                        {
                            target.Def -= BuffDefAmount;
                            if (target.Shield > 0) {
                                target.Shield -= target.Atk;
                            }
                            else {
                                target.Shield = 0;
                            }
                            target.Buffs.Remove("Taunt");
                            Debug.Log($"{target} is no longer taunting.");
                        }
                    }
                }
            }
        }
    }
    #endregion
}