using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StaffData
{
    public List<Staff> staffList;

    public StaffData()
    {
        staffList = new List<Staff>();

        for (int i = 0; i < 5; i++)
        {
            staffList.Add(new Soldier());
        }
    }

    public StaffData(List<Staff> stafflist)
    {
        this.staffList = staffList;
    }
}