using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentBonus 
{
    public int atk { get; set; }
    public int def { get; set; }

    public EquipmentBonus(int atk, int def) { 
        this.atk = atk;
        this.def = def;
    }
}
