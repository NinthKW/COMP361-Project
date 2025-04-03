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
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button selectButton;

    public Character Character { get; private set; }
    private MissionPreparationUI ui;
    private bool isAssigned;

    public void Initialize(Character character, MissionPreparationUI uiController)
    {
        Character = character;
        ui = uiController;
        selectButton.onClick.AddListener(OnClicked);
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