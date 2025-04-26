using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts.Model;
using Assets.Scripts.Controller;

public class InventoryPageUI : MonoBehaviour
{
    [SerializeField] Button backButton;
    [SerializeField] Transform weaponsContainer, equipmentsContainer;
    [SerializeField] GameObject headerPrefab, listItemPrefab;
    [SerializeField] UnlockPopupUI popup;
    [SerializeField] string weaponsHeaderText = "Weapons";
    [SerializeField] string equipmentsHeaderText = "Equipment";

    void Start()
    {
        PopulateInventory();
        backButton.onClick.AddListener(OnBack);
    }

    public void PopulateInventory()
    {
        Clear(weaponsContainer); Clear(equipmentsContainer);

        if (headerPrefab)
        {
            SetHeader(Instantiate(headerPrefab, weaponsContainer), weaponsHeaderText);
            SetHeader(Instantiate(headerPrefab, equipmentsContainer), equipmentsHeaderText);
        }

        foreach (var w in InventoryManager.Instance.GetWeapons()) SpawnRow(w);
        foreach (var e in InventoryManager.Instance.GetEquipments()) SpawnRow(e);
    }

    void SpawnRow(Weapon w)    => CreateRow(w.isUnlocked, weaponsContainer,
                                            $"{w.name} | DMG:{w.damage} | Cost:{w.cost}",
                                            () => popup.Show(w));
    void SpawnRow(Equipment e) => CreateRow(e.isUnlocked, equipmentsContainer,
                                            $"{e.name} | HP:{e.hp} DEF:{e.def} | Cost:{e.cost}",
                                            () => popup.Show(e));

    void CreateRow(bool unlocked, Transform parent, string label, UnityEngine.Events.UnityAction click)
    {
        var row = Instantiate(listItemPrefab, parent);
        row.GetComponentInChildren<TextMeshProUGUI>().text = label;
        row.GetComponent<CanvasGroup>().alpha = unlocked ? 1f : 0.4f;

        var btn = row.GetComponent<Button>();
        btn.interactable = !unlocked;
        if (!unlocked) btn.onClick.AddListener(click);
    }

    void Clear(Transform t) { foreach (Transform c in t) Destroy(c.gameObject); }

    void SetHeader(GameObject h, string txt)
    {
        var tmp = h.GetComponent<TextMeshProUGUI>(); 
        if (tmp) tmp.text = txt; 
        else h.GetComponent<Text>().text = txt;
    }

    void OnBack()
    {
        GameManager.Instance.ChangeState(GameState.MainMenuPage);
        GameManager.Instance.LoadGameState(GameState.MainMenuPage);
    }
}
