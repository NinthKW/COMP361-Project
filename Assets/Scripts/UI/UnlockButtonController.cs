using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Model;
using TMPro;

/// Controls the unlock button functionality for tech tree items
/// Handles the interaction between the UI button and selected tech items
public class UnlockButtonController : MonoBehaviour
{
    // Reference to the Button component this script is attached to
    private Button unlockButton;
    // Reference to currently selected tech button that can be unlocked
    private TechButton selectedTechButton;
    
    /// Initializes the component and sets up button listeners
    void Awake()
    {
        unlockButton = GetComponentInParent<Button>();
        
        // Make sure to check if the button was found
        if (unlockButton == null)
        {
            Debug.LogWarning("Button component not found. Please add a Button component or update reference.");
        }
        
        // Add click listener and disable button until a tech is selected
        unlockButton.onClick.AddListener(OnUnlockClicked);
        unlockButton.interactable = false;
    }

    /// Sets the currently selected tech button and updates button interactability
    public void SetSelectedTech(TechButton techButton)
    {
        selectedTechButton = techButton;
        // Only enable the button if a tech is selected
        unlockButton.interactable = (techButton != null);
    }

    /// Handles the unlock button click event
    /// Attempts to unlock the selected tech and clears the selection
    private void OnUnlockClicked()
    {
        if (selectedTechButton != null)
        {
            selectedTechButton.TryUnlock();
            SetSelectedTech(null);
        }
    }
} 