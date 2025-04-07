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
        private TextMeshProUGUI woodText;

        void Awake()
        {
            // Remove any existing UnlockButtonController components from the entire hierarchy
            var controllers = GetComponentsInChildren<UnlockButtonController>(true);
            foreach (var controller in controllers)
            {
                DestroyImmediate(controller);
            }
            
            // Add PlayerResources if it doesn't exist
            if (PlayerResources.Instance == null)
            {
                GameObject resourceManager = new GameObject("PlayerResources");
                resourceManager.AddComponent<PlayerResources>();
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
            if (PlayerResources.Instance != null)
            {
                moneyText.text = $"Money: ${PlayerResources.Instance.GetMoney()}";
                woodText.text = $"Resources: {PlayerResources.Instance.GetResource(1)}";
            }
        }

        //  Update the display every frame or change when new tech is unlocked
        void Update()
        {
            if (PlayerResources.Instance != null && moneyText != null && woodText != null)
            {
                moneyText.text = $"Money: ${PlayerResources.Instance.GetMoney()}";
                woodText.text = $"Resources: {PlayerResources.Instance.GetResource(1)}";
            }
        }

        private void OnExitButtonClicked()
        {
            GameManager.Instance.ChangeState(GameState.MainMenuPage);
            GameManager.Instance.LoadGameState(GameState.MainMenuPage);
        }
    }
}