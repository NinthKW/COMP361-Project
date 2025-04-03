using Codice.CM.WorkspaceServer.Tree;
using log4net.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model
{
    [System.Serializable]
    public class Base
    {
        public int building_id;
        public string name;
        public string description;
        public int level;
        public int cost;
        public int resource_amount;
        public int resource_type;
        public bool unlocked;

        public Base(int building_id, string name, string description, int level, int cost, int resource_amount, int resource_type, bool unlocked)
        {
            this.building_id = building_id;
            this.name = name;
            this.description = description;
            this.level = level;
            this.cost = cost;
            this.resource_amount = resource_amount;
            this.resource_type = resource_type;
            this.unlocked = unlocked;
        }
    }
}
