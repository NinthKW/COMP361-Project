using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Assets.Scripts.Model
{
    public class Role
    {
        public enum RoleType
        {
            Tank,
            Engineer,
            Medic,
            Sniper,
            Scout,
            Assault,        
            HeavyGunner,    
            Recon,          
            SpecialForces,
            Infantry,
        }

        public RoleType Type { get; }
        public int MaxHealth { get; }
        public int BaseAtk { get; }
        public int BaseDef { get; }
        public int BaseAttackChance { get; }

        public Role(string roleType)
        {
            switch (roleType.ToLower())
            {
                case "tank":
                    Type = RoleType.Tank;
                    MaxHealth = 100;
                    BaseAtk = 10;
                    BaseDef = 8;
                    BaseAttackChance = 1;
                    break;
                case "engineer":
                    Type = RoleType.Engineer;
                    MaxHealth = 70;
                    BaseAtk = 15;
                    BaseDef = 5;
                    BaseAttackChance = 2;
                    break;
                case "medic":
                    Type = RoleType.Medic;
                    MaxHealth = 80;
                    BaseAtk = 12;
                    BaseDef = 6;
                    BaseAttackChance = 1;
                    break;
                case "sniper":
                    Type = RoleType.Sniper;
                    MaxHealth = 75;
                    BaseAtk = 18;
                    BaseDef = 4;
                    BaseAttackChance = 1;
                    break;
                case "scout":
                    Type = RoleType.Scout;
                    MaxHealth = 90;
                    BaseAtk = 8;
                    BaseDef = 9;
                    BaseAttackChance = 2;
                    break;
                case "assault":
                    Type = RoleType.Assault;
                    MaxHealth = 85;
                    BaseAtk = 14;
                    BaseDef = 7;
                    BaseAttackChance = 2;
                    break;
                case "heavygunner":
                    Type = RoleType.HeavyGunner;
                    MaxHealth = 110;
                    BaseAtk = 20;
                    BaseDef = 6;
                    BaseAttackChance = 1;
                    break;
                case "recon":
                    Type = RoleType.Recon;
                    MaxHealth = 80;
                    BaseAtk = 16;
                    BaseDef = 5;
                    BaseAttackChance = 3;
                    break;
                case "specialforces":
                    Type = RoleType.SpecialForces;
                    MaxHealth = 95;
                    BaseAtk = 18;
                    BaseDef = 7;
                    BaseAttackChance = 2;
                    break;
                case "infantry":
                    Type = RoleType.Infantry;
                    MaxHealth = 95;
                    BaseAtk = 18;
                    BaseDef = 7;
                    BaseAttackChance = 2;
                    break;
                default:
                    throw new ArgumentException($"Invalid role type: {roleType}");
            }
        }

        public string GetRoleName() => Type.ToString();
    }
}