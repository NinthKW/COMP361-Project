using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Model;


namespace Assets.Scripts.Model
{
    [System.Serializable]
    public class Game
    {
         
        public Resources resourcesData;
        public List <Mission> MissionsData;
        public List <Soldier> soldiersData;
        public List <Base> basesData;
        public Tech techData;

        public Game()
        {
            this.resourcesData = new Resources();
            this.MissionsData = new List <Mission> ();
            this.soldiersData = new List <Soldier> ();
            this.basesData = new List <Base> ();
            this.basesData.Add(new Base());
            this.techData = new Tech();
        }        
    }
 
    
}