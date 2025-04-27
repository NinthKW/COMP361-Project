using Assets.Scripts.Controller;
using Assets.Scripts.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGenerationBuilding 
{
    public bool active;
    public int resourceID;
    public int increaseAmount;

    public ResourceGenerationBuilding(int resourceID, int increase)
    {
        active = false;
        this.resourceID = resourceID;
        increaseAmount = increase;
    }
}
