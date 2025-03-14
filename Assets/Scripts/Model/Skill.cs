using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assets.Scripts.Model
{
    public class Skill
    {
        public string name;
        public int damage;
        public int cost;
        public int level;

        public Skill(string name, int damage, int cost, int level)
        {
            this.name = name;
            this.damage = damage;
            this.cost = cost;
            this.level = level;
        }
    }
}