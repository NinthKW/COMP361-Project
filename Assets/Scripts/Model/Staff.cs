using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model
{
    [System.Serializable]
    public class Staff
    {
        public List<Staff> staffList;

        public Staff()
        {
            staffList = new List<Staff>();

            for (int i = 0; i < 5; i++)
            {
                staffList.Add(new Soldier ());
            }
        }

        public Staff(List<Staff> staff_list)
        {
            this.staffList = staff_list;
        }
    }
}