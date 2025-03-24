using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections.Generic;
using Assets.Scripts.Model;


public class FormationSlot : MonoBehaviour
{
    [SerializeField] private Image slotBackground;
    [SerializeField] private Button slotButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Image soldierPortrait;

    public Soldier CurrentSoldier { get; private set; }
    private MissionPreparationUI ui;
    private int slotIndex;

    public void Initialize(MissionPreparationUI uiController, int index)
    {
        ui = uiController;
        slotIndex = index;
        slotButton.onClick.AddListener(OnSlotClicked);
        cancelButton.onClick.AddListener(OnCancelClicked);
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
        }
    }
}