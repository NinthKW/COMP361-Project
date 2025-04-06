using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections.Generic;
using Assets.Scripts.Model;
using TMPro;


public class FormationSlot : MonoBehaviour
{
    [SerializeField] private Image slotBackground;
    [SerializeField] private Button slotButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Image soldierPortrait;
    [SerializeField] private TextMeshProUGUI soldierNameText;
    [SerializeField] private TextMeshProUGUI slotIndexText;
    public int SlotIndex { get; set; }

    public Soldier CurrentSoldier { get; private set; }
    private MissionPreparationUI ui;
    private CombatUI combatUI;
    private int slotIndex;

    public void Initialize(MissionPreparationUI uiController, int index)
    {
        ui = uiController;
        slotIndex = index;
        slotButton.onClick.AddListener(OnSlotClicked);
        cancelButton.onClick.AddListener(OnCancelClicked);
        slotIndexText.text = $"Slot {index + 1}";
        UpdateVisuals();
    }
    public void Initialize(CombatUI uiController, int index)
    {
        combatUI = uiController;
        slotIndex = index;
        slotButton.onClick.AddListener(OnSlotClicked);
        cancelButton.onClick.AddListener(OnCancelClicked);
        slotIndexText.text = $"Slot {index + 1}";
        UpdateVisuals();
    }

    void OnSlotClicked()
    {
        ui.OnSlotSelected(this);
    }

    void OnCancelClicked()
    {
        if (CurrentSoldier != null)
        {
            ui.ReturnSoldierToContainer(CurrentSoldier);
            CurrentSoldier = null;
            UpdateVisuals();
        }
    }

    public void AssignSoldier(Soldier soldier)
    {
        CurrentSoldier = soldier;
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        // soldierPortrait.gameObject.SetActive(CurrentSoldier != null);
        cancelButton.gameObject.SetActive(CurrentSoldier != null);

        if (CurrentSoldier != null)
        {
            // 加载士兵头像
            // soldierPortrait.sprite = ...
            soldierNameText.text = CurrentSoldier.Name;
            slotBackground.color = Color.magenta; // 选中状态的颜色
        }
        else
        {
            soldierNameText.text = "";
            slotBackground.color = Color.white; // 默认颜色
        }
    }
}