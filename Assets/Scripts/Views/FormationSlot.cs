using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Assets.Scripts.Model;
public class FormationSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private Image slotImage;
    [SerializeField] private Button cancelButton;
    [SerializeField] private GameObject characterUIPrefab;

    public Soldier CurrentSoldier { get; private set; }
    private int slotIndex;
    private MissionPreparationUI ui;

    public void Initialize(int index, MissionPreparationUI uiController)
    {
        slotIndex = index;
        ui = uiController;
        cancelButton.gameObject.SetActive(false);
    }

    public void AssignSoldier(Soldier soldier)
    {
        CurrentSoldier = soldier;
        var characterCard = Instantiate(characterUIPrefab, transform);
        characterCard.GetComponent<SoldierCard>().Initialize(soldier, ui);
        UpdateVisuals();
    }

    public bool ContainsSoldier(Soldier soldier)
    {
        return CurrentSoldier == soldier;
    }

    public void ClearSlot()
    {
        CurrentSoldier = null;
        foreach (Transform child in transform)
        {
            if (child.GetComponent<SoldierCard>()) Destroy(child.gameObject);
        }
        UpdateVisuals();
    }

    public void OnDrop(PointerEventData eventData)
    {
        var draggedPortrait = eventData.pointerDrag.GetComponent<SoldierCard>();
        if (draggedPortrait != null)
        {
            ui.TryAssignSoldierToSlot(draggedPortrait.Soldier, slotIndex);
        }
    }

    public void ToggleCancelButton(bool show)
    {
        cancelButton.gameObject.SetActive(show);
        cancelButton.onClick.RemoveAllListeners();
        if (show)
        {
            cancelButton.onClick.AddListener(() => ui.RemoveSoldierFromFormation(CurrentSoldier));
        }
    }

    void UpdateVisuals()
    {
        slotImage.color = CurrentSoldier == null ? Color.gray : Color.white;
    }
}