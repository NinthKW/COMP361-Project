using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResourcesData 
{
    int wood;
    int iron;
    int gas;
    int food;
    int chemicals;
    int leather;
    int gun;
    int bomb;
    int ak47;
    int ammo;
    int boots;
    int vest;

    public ResourcesData() 
    {
        wood = 100;
        iron = 100;
        gas = 100;
        food = 100;
        chemicals = 100;
        leather = 100;
        gun = 5;
        bomb = 0;
        ak47 = 1;
        ammo = 5;
        boots = 5;
        vest = 5;
    }

    public ResourcesData(int wood, int iron, int gas, int food, int chemicals, int leather)
    {
        this.wood = wood;
        this.iron = iron;
        this.gas = gas;
        this.food = food;
        this.chemicals = chemicals;
        this.leather = leather;
        this.gun = gun
        this.bomb = bomb
        this.ak47 = ak47
        this.sniperifle = sniperifle;
        this.boots = boots;
        this.vest = vest;
    }
}

