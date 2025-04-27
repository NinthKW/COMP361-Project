using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Model;
using Assets.Scripts.Controller;
using TMPro;
// Manages the tech tree UI, including resource display and button interactions
namespace Assets.Scripts.Views
{
    public class TechUI : MonoBehaviour
    {
        public Button exitButton;
        public Button unlockButton;
        public TechButton[] techButtons; 
        private UnlockButtonController unlockButtonController;
        
        [SerializeField]
        private TextMeshProUGUI moneyText;
        [SerializeField]
        private TextMeshProUGUI ironText;
        [SerializeField]
        private TextMeshProUGUI woodText;
        [SerializeField]
        private TextMeshProUGUI titaniumText;

        void Awake()
        {
            // Remove any existing UnlockButtonController components from the entire hierarchy
            var controllers = GetComponentsInChildren<UnlockButtonController>(true);
            foreach (var controller in controllers)
            {
                DestroyImmediate(controller);
            }
        }

        void Start()
        {
            exitButton.onClick.AddListener(OnExitButtonClicked);
            
            // Make sure TechManager is initialized
            if (TechManager.Instance == null)
            {
                Debug.LogError("TechManager not found in scene!");
            }

            // Start updating the resource display
            UpdateResourceDisplay();
        }

        private void UpdateResourceDisplay()
        {
            if (TechManager.Instance != null)
            {
                moneyText.text = $"Money: ${TechManager.Instance.availableResource.GetAmount(1)}";
                ironText.text = $"Iron: ${TechManager.Instance.availableResource.GetAmount(2)}";
                woodText.text = $"Wood: ${TechManager.Instance.availableResource.GetAmount(3)}";
                titaniumText.text = $"Titanium: ${TechManager.Instance.availableResource.GetAmount(4)}";
            }
        }

        //  Update the display every frame or change when new tech is unlocked
        void Update()
        {
            if (TechManager.Instance != null && moneyText != null && woodText != null)
            {
                moneyText.text = $"Money: ${TechManager.Instance.availableResource.GetAmount(1)}";
                ironText.text = $"Iron: ${TechManager.Instance.availableResource.GetAmount(2)}";
                woodText.text = $"Wood: ${TechManager.Instance.availableResource.GetAmount(3)}";
                titaniumText.text = $"Titanium: ${TechManager.Instance.availableResource.GetAmount(4)}";
            }
        }

        private void OnExitButtonClicked()
        {
            GameManager.Instance.ChangeState(GameState.MainMenuPage);
            GameManager.Instance.LoadGameState(GameState.MainMenuPage);
        }
    }
}