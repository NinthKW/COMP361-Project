using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assets.Scripts.Model
{
    public class Role
    {
        public RoleType roleType;
        public int maxHealth;
        public int exp;
        public int level;
        public int maxLevel;
        public int base_attack;
        public int base_defense;
        List<Skill> skills = new List<Skill>();
        List<Skill> availableSkills = new List<Skill>();

        public Role(RoleType type)
        {
            this.roleType = type;
            exp = 0;
            level = 1;

            switch (type)
            {
                case RoleType.Army:
                    maxHealth = 100;
                    maxLevel = 10;
                    base_attack = 10;
                    base_defense = 8;
                    skills.Add(new Skill("Skill1", 10, 0, 1));
                    break;
                case RoleType.Sniper:
                    maxHealth = 70;
                    maxLevel = 10;
                    base_attack = 15;
                    base_defense = 5;
                    break;
                case RoleType.Engineer:
                    maxHealth = 80;
                    maxLevel = 10;
                    base_attack = 12;
                    base_defense = 6;
                    break;
                case RoleType.Medic:
                    maxHealth = 75;
                    maxLevel = 10;
                    base_attack = 6;
                    base_defense = 7;
                    break;
                case RoleType.Scott:
                    maxHealth = 90;
                    maxLevel = 10;
                    base_attack = 8;
                    base_defense = 9;
                    break;
                default:
                    throw new ArgumentException($"Invalid role type: {type}", nameof(type));
            }
        }

        public void AddSkill(Skill skill)
        {
            skills.Add(skill);
        }

        public string GetRoleName()
        {
            return roleType.ToString();
        }
    }

}