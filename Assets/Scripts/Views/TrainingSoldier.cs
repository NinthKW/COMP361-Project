using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class TrainingSoldier : MonoBehaviour
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
            TrainingUI.Instance.currentSelectedSoldier = soldier;
            Debug.Log("Current soldier selected: " + TrainingUI.Instance.currentSelectedSoldier.Name);

            TrainingUI.Instance.updateMenu(soldier);
        }
    }
}