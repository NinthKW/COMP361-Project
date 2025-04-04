using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model
{
    [System.Serializable]
    public class Base
    {
        private Dictionary<int, int> buildings = new Dictionary <int, int>();
         
        public class Building 
        {
            public Dictionary<int, (string name, string description)> building = new Dictionary <int, (string, string)>
            {
                { 0, ("Living Quarters", "Housing for soldiers")}, 
                { 1, ("Hospital", "You can heal soldiers in Hospital")},
                { 2, ("Lab", "Research and development")},
                { 3, ("Hangar", "Facility for aircraft and vehicles")}
            };

        }
 
        public Base()
        {
            buildings.Add(0, 1);
        }
 
        public Base (List <int> ids, List <int> lvls)
        {
            for (int i = 0; i < 4; i++)
            {
                if (ids[i] != 0)
                {
                    buildings.Add(i, lvls[i]);
                }
            }
        }
    }
}
