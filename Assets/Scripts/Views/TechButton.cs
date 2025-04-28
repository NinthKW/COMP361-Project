// These are the required Unity components and namespaces
using UnityEngine;
using UnityEngine.UI;        
using TMPro;                
using Assets.Scripts.Model;
using Assets.Scripts.Controller;

public class TechButton : MonoBehaviour
{
    [SerializeField]
    private int techId;              // Unique ID for this tech (e.g., 5 for Laser Rifle)
    [SerializeField]
    private Button button;
    [SerializeField]
    private TextMeshProUGUI techNameText;   // Reference to the name text component
    [SerializeField]
    private TextMeshProUGUI costText;       // Reference to the cost text component
    
    private Tech tech;      // Holds the tech data for this button
    private UnlockButtonController unlockController;  // Reference to the unlock button

    private void Start()
    {
        // Set up what happens when this button is clicked
        button.onClick.AddListener(OnTechSelected);
        
        // Find the unlock button controller in the scene
        unlockController = FindObjectOfType<UnlockButtonController>();
        
        // Update the button's display text
        UpdateTechInfo();
    }

    // Called when the button is clicked
    private void OnTechSelected()
    {
        if (unlockController != null)
        {
            // Tell the unlock button which tech is selected
            unlockController.SetSelectedTech(this);
            AudioManager.Instance.PlaySound("Tech Button Click"); // Play a sound when the button is clicked
        }
    }

    // Called when trying to unlock this tech
    public void TryUnlock()
    {
        if (TechManager.Instance.UnlockTech(techId))
        {
            UpdateTechInfo();
            AudioManager.Instance.PlaySound("Tech Button Unlock"); // Play a sound when the tech is unlocked
        }
        else
        {
            // Add these debug lines
            AudioManager.Instance.PlaySound("Error"); // Play a sound when the unlock fails
            Debug.Log($"Current wood amount: {TechManager.Instance.availableResource.GetAmount(1)}");
            Debug.Log($"Current money: {TechManager.Instance.availableResource.GetAmount(0)}");
        }
    }

    // Updates the button's display information
    private void UpdateTechInfo()
    {
        // Safety check for TechManager
        if (TechManager.Instance == null) return;
        
        // Find this button's tech data in TechManager
        tech = TechManager.Instance.GetAllTechs().Find(t => t.techId == techId);
        
        if (tech != null)
        {
            // Update the display texts
            techNameText.text = tech.techName;
            costText.text = $"Cost: ${tech.costMoney}\n{tech.costResourceAmount} {TechManager.Instance.availableResource.GetName(tech.costResourceId)}";
            
            // Disable the button if tech is already unlocked
            button.interactable = !tech.isUnlocked;
        }
    }
}