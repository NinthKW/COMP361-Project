using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Assets.Scripts.Model;

// SoldierCard.cs
public class SoldierCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image soldierImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    
    public Soldier Soldier { get; private set; }
    private MissionPreparationUI ui;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Initialize(Soldier soldier, MissionPreparationUI uiController)
    {
        Soldier = soldier;
        ui = uiController;
        UpdateVisuals();
    }

    public void SetSelected(bool isSelected)
    {
        canvasGroup.alpha = isSelected ? 0.5f : 1f;
        canvasGroup.blocksRaycasts = !isSelected;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        ui.OnSoldierSelected(Soldier);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        transform.localPosition = Vector3.zero;
    }

    void UpdateVisuals()
    {
        nameText.text = Soldier.Name;
        levelText.text = $"Lv.{Soldier.Level}";
        // TODO: soldierImage.sprite = LoadSoldierSprite(Soldier);
    }
}