using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using Assets.Scripts.Model;

[System.Serializable]
public class SoldierEquipment 
{
    public Character soldier;
    public Weapon weapon;
    public Equipment equipment;

    public SoldierEquipment(Character soldier, Weapon weapon, Equipment equipment) {
        this.soldier = soldier;
        this.weapon = weapon;
        this.equipment = equipment;
    }
}
