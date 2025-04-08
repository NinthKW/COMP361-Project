using Assets.Scripts;
using Assets.Scripts.Controller;
using Assets.Scripts.Model;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaffpageUI : MonoBehaviour
{
    public static StaffpageUI Instance;

    // Parent container for the soldier buttons.
    public Transform soldierGrid;
    // Prefab to represent each soldier as a button (should contain both Button and TextMeshProUGUI components).
    public GameObject soldierPrefab;
    // Currently selected soldier.
    public Character currentSelectedSoldier;

    // UI Text fields that will display the detailed soldier information.
    public TextMeshProUGUI soldierNameDisplay;
    public TextMeshProUGUI soldierLevelDisplay;
    public TextMeshProUGUI soldierHealthDisplay;
    public TextMeshProUGUI soldierAttackDisplay;
    public TextMeshProUGUI soldierDefenseDisplay;
    public TextMeshProUGUI soldierRoleDisplay;

    // Dedicated detail area Image to display the soldier's sprite.
    public Image soldierDetailImage;

    // Back button to navigate away from this screen.
    public GameObject backButton;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {   
        PopulateSoldierGrid();
        backButton.GetComponent<Button>().onClick.AddListener(OnBackButtonClicked);
    }

    // Create a button for each soldier in the game's soldiersData list.
    public void PopulateSoldierGrid()
    {
        List<Character> soldiers = StaffManager.Instance.soldiers;
        // Optionally, clear previous grid elements:
        foreach (Transform child in soldierGrid)
        {
            Destroy(child.gameObject);
        }

        foreach (Character soldier in soldiers)
        {
            Debug.Log("Adding soldier: " + soldier.Name);
            // Instantiate the soldier button as a child of the soldierGrid.
            GameObject buttonGameObject = Instantiate(soldierPrefab, soldierGrid);
            Button button = buttonGameObject.GetComponent<Button>();

            // If using a dedicated StaffpageSoldier component, assign the soldier:
            StaffpageSoldier sps = buttonGameObject.GetComponent<StaffpageSoldier>();
            if (sps != null)
            {
                sps.soldier = soldier;
            }

            // If the soldier is a Soldier, load a role-based sprite for the button.
            if (soldier is Soldier soldierData)
            {
                string roleName = soldierData.GetRoleName(); // e.g., "Tank", "Medic", etc.
                // Load sprite from Resources/SoldierImages folder.
                Sprite roleSprite = UnityEngine.Resources.Load<Sprite>(roleName);
                if (roleSprite != null)
                {
                    // Try to get the Image component; first try on the root...
                    Image characterImage = buttonGameObject.GetComponent<Image>();
                    // ...if not found, search in children.
                    if (characterImage == null)
                    {
                        characterImage = buttonGameObject.GetComponentInChildren<Image>();
                    }
                    if (characterImage != null)
                    {
                        characterImage.sprite = roleSprite;
                    }
                    else
                    {
                        Debug.LogWarning("No Image component found on the prefab to display the sprite.");
                    }
                }
                else
                {
                    Debug.LogWarning("No sprite found for role: " + roleName);
                }
            }
        }
    }

    // Navigate back to the BasePage by changing the game state.
    void OnBackButtonClicked()
    {
        GameManager.Instance.ChangeState(GameState.MainMenuPage);
        GameManager.Instance.LoadGameState(GameState.MainMenuPage);
    }

    // Update the detail panel using the soldier's attributes from the game model.
    public void UpdateSoldierDetail(Character soldier)
    {
        if (soldier == null)
        {
            Debug.Log("Received a null soldier in UpdateSoldierDetail.");
            return;
        }

        currentSelectedSoldier = soldier;
        soldierNameDisplay.text = "Name: " + soldier.Name;
        soldierLevelDisplay.text = "Level: " + soldier.Level;
        soldierHealthDisplay.text = "HP: " + soldier.Health + "/" + soldier.MaxHealth;
        soldierAttackDisplay.text = "Atk: " + soldier.Atk;
        soldierDefenseDisplay.text = "Def: " + soldier.Def;

        // Check if the character is a Soldier to display its role and sprite.
        if (soldier is Soldier soldierData)
        {
            soldierRoleDisplay.text = "Role: " + soldierData.GetRoleName();
            // Load sprite for the detail area from the Resources folder.
            string roleName = soldierData.GetRoleName(); // e.g., "Tank", "Medic", etc.
            Sprite roleSprite = UnityEngine.Resources.Load<Sprite>(roleName);
            if (roleSprite != null)
            {
                if (soldierDetailImage != null)
                {
                    soldierDetailImage.sprite = roleSprite;
                }
                else
                {
                    Debug.LogWarning("soldierDetailImage is not assigned in the Inspector.");
                }
            }
            else
            {
                Debug.LogWarning("No sprite found for role: " + roleName);
            }
        }
        else
        {
            soldierRoleDisplay.text = "Role: N/A";
            if (soldierDetailImage != null)
            {
                soldierDetailImage.sprite = null;
            }
        }
    }
}
