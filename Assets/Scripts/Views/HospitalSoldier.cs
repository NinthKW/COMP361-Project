using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class HospitalSoldier : MonoBehaviour
    {
        public Model.Character soldier;

        private void Awake()
        {
            this.GetComponent<Button>().onClick.AddListener(selectSoldier);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void selectSoldier()
        {
            HospitalUI.Instance.currentSelectedSoldier = soldier;
            Debug.Log("Current soldier selected: " + HospitalUI.Instance.currentSelectedSoldier.Name);

            HospitalUI.Instance.updateMenu(soldier);
        }
    }
}