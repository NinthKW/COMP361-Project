using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Model;
using Assets.Scripts.Controller;
using TMPro;

public class InventoryPageUI : MonoBehaviour
{
    // Back button to return to the Main Menu.
    public Button backButton;
    
    // These are the Content objects from your two Scroll Views.
    public Transform weaponsScrollContent;
    public Transform equipmentsScrollContent;
    
    // Prefab references for list items and headers.
    public GameObject listItemPrefab;
    public GameObject headerPrefab;
    
    // Header texts for each scroll view.
    public string weaponsHeaderText = "Weapons";
    public string equipmentsHeaderText = "Equipments";

    void Start()
    {
        // Register the back button click event.
        backButton.onClick.AddListener(OnBackButtonClicked);
        // Populate the UI with inventory items.
        PopulateInventory();
    }

    void OnBackButtonClicked()
    {
        GameManager.Instance.ChangeState(GameState.MainMenuPage);
        GameManager.Instance.LoadGameState(GameState.MainMenuPage);
    }

    void PopulateInventory()
    {
        // Clear existing items.
        foreach (Transform child in weaponsScrollContent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in equipmentsScrollContent)
        {
            Destroy(child.gameObject);
        }

        // Instantiate headers.
        if (headerPrefab != null)
        {
            GameObject weaponsHeader = Instantiate(headerPrefab, weaponsScrollContent);
            SetHeaderText(weaponsHeader, weaponsHeaderText);

            GameObject equipmentsHeader = Instantiate(headerPrefab, equipmentsScrollContent);
            SetHeaderText(equipmentsHeader, equipmentsHeaderText);
        }

        // Populate the weapons list.
        List<Weapon> weapons = InventoryManager.Instance.GetWeapons();
        foreach (Weapon weapon in weapons)
        {
            GameObject newItem = Instantiate(listItemPrefab, weaponsScrollContent);
            TextMeshProUGUI tmp = newItem.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                // Removed "Weapon:" prefix.
                tmp.text = $"{weapon.name} | Damage: {weapon.damage} | Cost: {weapon.cost}";
            }
            else
            {
                Text txt = newItem.GetComponent<Text>();
                if (txt != null)
                {
                    txt.text = $"{weapon.name} | Damage: {weapon.damage} | Cost: {weapon.cost}";
                }
            }
        }

        // Populate the equipment list.
        List<Equipment> equipments = InventoryManager.Instance.GetEquipments();
        foreach (Equipment equipment in equipments)
        {
            GameObject newItem = Instantiate(listItemPrefab, equipmentsScrollContent);
            TextMeshProUGUI tmp = newItem.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                // Removed "Equipment:" prefix.
                tmp.text = $"{equipment.name} | HP: {equipment.hp} | DEF: {equipment.def} | ATK: {equipment.atk} | Cost: {equipment.cost}";
            }
            else
            {
                Text txt = newItem.GetComponent<Text>();
                if (txt != null)
                {
                    txt.text = $"{equipment.name} | HP: {equipment.hp} | DEF: {equipment.def} | ATK: {equipment.atk} | Cost: {equipment.cost}";
                }
            }
        }
    }

    void SetHeaderText(GameObject headerObject, string headerText)
    {
        TextMeshProUGUI tmp = headerObject.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text = headerText;
        }
        else
        {
            Text txt = headerObject.GetComponent<Text>();
            if (txt != null)
            {
                txt.text = headerText;
            }
        }
    }
}
