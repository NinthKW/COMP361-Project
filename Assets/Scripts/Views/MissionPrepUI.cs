using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Assets.Scripts.Model;
using Assets.Scripts.Controller;
using System.Collections.Generic;
using System.Linq;

public class MissionPreparationUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform availableSoldiersPanel;
    [SerializeField] private Transform availableEnemiesPanel;
    [SerializeField] private Transform formationSlotsPanel;
    [SerializeField] private Button assignButton;
    [SerializeField] private Button startBattleButton;
    [SerializeField] private Button retreatButton;

    [Header("Text References")]
    [SerializeField] private InfoPanel soldierInfoPanel;
    [SerializeField] private InfoPanel enemyInfoPanel;

    [Header("Prefabs")]
    [SerializeField] private GameObject soldierCardPrefab;
    [SerializeField] private GameObject formationSlotPrefab;

    private List<CharacterCard> characterCards = new List<CharacterCard>();
    private List<CharacterCard> enemyCards = new List<CharacterCard>();
    private List<FormationSlot> formationSlots = new List<FormationSlot>();
    private CharacterCard selectedCharacterCard;
    private FormationSlot selectedFormationSlot;

    void Start()
    {
        if (CombatManager.Instance == null)
        {
            Debug.Log("Debugging Scene");
            CombatManager.Instance = new GameObject().AddComponent<CombatManager>();
        }
        InitializeUI();
        SetupButtons();
    }

    void InitializeUI()
    {
        List<Vector3> positions = new()
        {
            new Vector3(-200, -100, 0),
            new Vector3(-100, 100, 0),
            new Vector3(0, -100, 0),
            new Vector3(100, 100, 0),
            new Vector3(200, -100, 0)
        };
        // 初始化阵型槽位
        for (int i = 0; i < 5; i++)
        {
            var slot = Instantiate(formationSlotPrefab, formationSlotsPanel).GetComponent<FormationSlot>();
            slot.Initialize(this, i);
            slot.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            
            slot.gameObject.GetComponent<RectTransform>().localPosition = positions[i];
            slot.gameObject.SetActive(true);
            formationSlots.Add(slot);
        }

        // 加载可用士兵
        foreach (var soldier in CombatManager.Instance.GetAvailableSoldiers())
        {
            var card = Instantiate(soldierCardPrefab, availableSoldiersPanel).GetComponent<CharacterCard>();
            card.Initialize(soldier, this);
            characterCards.Add(card);
        }

        // 加载敌人
        foreach (var enemy in CombatManager.Instance.GetAvailableEnemies())
        {
            var card = Instantiate(soldierCardPrefab, availableEnemiesPanel).GetComponent<CharacterCard>();
            card.Initialize(enemy, this);
            enemyCards.Add(card);
        }
    }

    void SetupButtons()
    {
        assignButton.onClick.AddListener(OnAssignButtonClicked);
        startBattleButton.onClick.AddListener(OnStartBattle);
        retreatButton.onClick.AddListener(OnRetreat);
        UpdateButtonStates();
    }

    void OnRetreat()
    {
        // 返回到关卡选择界面
        GameManager.Instance.LoadGameState(GameState.MissionPage);
    }

    void OnStartBattle()
    {
        // 检查是否所有槽位都有士兵
        if (formationSlots.Any(slot => slot.CurrentSoldier == null))
        {
            Debug.Log("请为所有槽位分配士兵");
            return;
        }

        List<Soldier> combatSoldiers = new List<Soldier>();
        List<Enemy> combatEnemies = new List<Enemy>();

        foreach(Soldier soldier in formationSlots.Select(slot => slot.CurrentSoldier).ToList())
        {
            combatSoldiers.Add(soldier);
        }

        // 开始战斗
        CombatManager.Instance.StartCombat(
            formationSlots.Select(slot => slot.CurrentSoldier).ToList(), 
            CombatManager.Instance.GetAvailableEnemies()
        );

        // 进入战斗场景
        GameManager.Instance.LoadGameState(GameState.CombatPage);
    }

    public void OnSoldierSelected(CharacterCard card)
    {
        // 取消之前的选择
        if (selectedCharacterCard != null) 
        {
            selectedCharacterCard.SetSelected(false);
        }

        selectedCharacterCard = card;
        selectedFormationSlot = null;
        
        if (card != null)
        {
            card.SetSelected(true);
        }
        
        soldierInfoPanel.UpdateInfo((Soldier) card.Character);
        UpdateButtonStates();
    }

    public void OnEnemySelected(CharacterCard card)
    {
        // 取消之前的选择
        if (selectedCharacterCard != null) 
        {
            selectedCharacterCard.SetSelected(false);
        }

        selectedCharacterCard = card;
        selectedFormationSlot = null;
        
        if (card != null)
        {
            card.SetSelected(true);
        }
        
        enemyInfoPanel.UpdateInfo((Enemy) card.Character);
        UpdateButtonStates();
    }


    public void OnSlotSelected(FormationSlot slot)
    {
        selectedFormationSlot = slot;
        UpdateButtonStates();
    }

    void UpdateButtonStates()
    {
        // Assign按钮状态
        bool canAssign = selectedCharacterCard != null && selectedFormationSlot != null;
        assignButton.interactable = canAssign;

        // 其他按钮状态...
    }

    void OnAssignButtonClicked()
    {
        if (selectedFormationSlot == null || selectedCharacterCard == null) return;

        // 如果槽位已有士兵，先取消
        if (selectedFormationSlot.CurrentSoldier != null)
        {
            ReturnSoldierToContainer(selectedFormationSlot.CurrentSoldier);
        }

        // 分配新士兵
        AssignSoldierToSlot((Soldier) selectedCharacterCard.Character, selectedFormationSlot);
        
        // 重置选择
        ClearSelection();
    }

    void AssignSoldierToSlot(Soldier soldier, FormationSlot slot)
    {
        slot.AssignSoldier(soldier);
        selectedCharacterCard.SetAssigned(true);
    }

    public void ReturnSoldierToContainer(Soldier soldier)
    {
        var card = characterCards.FirstOrDefault(c => c.Character == soldier);
        if (card != null)
        {
            card.SetAssigned(false);
        }
    }

    void ClearSelection()
    {
        selectedCharacterCard = null;
        selectedFormationSlot = null;
        UpdateButtonStates();
    }
}

public class SoldierInfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI defenseText;
    [SerializeField] private TextMeshProUGUI roleText;

    public void UpdateInfo(Soldier soldier)
    {
        nameText.text = soldier.Name;
        levelText.text = $"Level: {soldier.Level}";
        healthText.text = $"HP: {soldier.Health}/{soldier.MaxHealth}";
        attackText.text = $"ATK: {soldier.Atk}";
        defenseText.text = $"DEF: {soldier.Def}";
        roleText.text = $"Role: {soldier.GetRoleName()}";
    }
}