using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Assets.Scripts.Model;

public class CharacterCard : MonoBehaviour
{
    /// <summary>
    /// Represents a character card in the mission preparation UI.
    /// </summary>
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button selectButton;
    [SerializeField] private Image roleImage;


    public Character Character { get; private set; }
    private MissionPreparationUI ui;
    private bool isAssigned;

    public void Initialize(Character character, MissionPreparationUI uiController)
    {
        Character = character;
        ui = uiController;
        selectButton.onClick.AddListener(OnClicked);
          // Set a role-specific sprite for soldiers
    if (Character is Soldier soldier)
    {
        string roleName = soldier.GetRoleName(); // e.g., "Tank", "Medic", etc.
        // Load sprite from Resources (adjust path if your images are in a subfolder)
        Sprite roleSprite = UnityEngine.Resources.Load<Sprite>(roleName);
        if (roleSprite != null)
        {
            roleImage.sprite = roleSprite;
        }
        else
        {
            Debug.LogWarning("No sprite found for role: " + roleName);
        }
    }
    
    UpdateVisuals();
        UpdateVisuals();
    }

    void OnClicked()
    {
        if (!isAssigned)
        {
            if (Character is Soldier) ui.OnSoldierSelected(this);
            else if (Character is Enemy) ui.OnEnemySelected(this);
        }
    }

    public void SetSelected(bool isSelected)
    {
        background.color = isSelected ? Color.yellow : Color.white;
    }

    public void SetAssigned(bool assigned)
    {
        isAssigned = assigned;
        background.color = assigned ? new Color(0.5f, 0.5f, 0.5f) : Color.white;
        selectButton.interactable = !assigned;
    }

    void UpdateVisuals()
    {
        nameText.text = Character.Name;
        levelText.text = $"Lv.{Character.Level}";
    }
}