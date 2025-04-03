using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model
{
    [System.Serializable]
    public class Base
    {
        public List<Base> bases;
        public Base()
        {
            bases = new List<Base>();
        }
    }
}
