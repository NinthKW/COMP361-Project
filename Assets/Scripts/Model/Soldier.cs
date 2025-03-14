using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model {
    [System.Serializable]
    public class Soldier : Staff
    {
        string name;
        bool gun;
        int bomb;
        bool ak47;
        int ammo;
        bool boots;
        bool vest;

        private static string[] nameList = { "Messi", "Ronaldo", "Joseph.Vyb", "Trump", "Trudeau", "Poilievre", "Legault", "Jagmeet", "Faker", "Zeus", "Drake", "Obama" };

        public Soldier()
        {
            this.name = nameList[UnityEngine.Random.Range(0, 12)];
            this.gun = false;
            this.bomb = 0;
            this.ammo = 0;
            this.boots = false;
            this.vest = false;
        }

        public void SetGun(bool value)
        {
            this.gun = value;
        }
        public void SetBomb(int value)
        {
            this.bomb = value;
        }
        public void SetAk47(bool value)
        {
            this.ak47 = value;
        }
        public void SetAmmo(int value)
        {
            this.ammo = value;
        }
        public void SetBoots(bool value)
        {
            this.boots = value;
        }
        public void SetVest(bool value)
        {
            this.vest = value;
        }
    }
}