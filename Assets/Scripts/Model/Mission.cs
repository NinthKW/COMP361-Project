using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model {
    [System.Serializable]
    public class Mission
    {
        public int id;
        public string name;
        public string description;
        public bool isCompleted;

        public Mission(int id, string name, string description)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.isCompleted = false;
        }
    }

}