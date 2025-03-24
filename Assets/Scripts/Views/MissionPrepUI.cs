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
    [Header("UI Components")]
    [SerializeField] private Transform availableSoldiersPanel;
    [SerializeField] private Transform availableEnemiesPanel;
    [SerializeField] private Transform formationSlotsPanel;
    [SerializeField] private InfoPanel soldierInfoPanel;
    [SerializeField] private InfoPanel enemyInfoPanel;
    [SerializeField] private Button startBattleButton;
    [SerializeField] private Button retreatButton;

    [Header("Prefabs")]
    [SerializeField] private GameObject soldierCardPrefab;
    [SerializeField] private GameObject enemyCardPrefab;
    [SerializeField] private GameObject formationSlotPrefab;

    private List<FormationSlot> formationSlots = new List<FormationSlot>();
    private List<SoldierCard> availableSoldierCards = new List<SoldierCard>();
    private List<CharacterUI> availableEnemyCards = new List<CharacterUI>();

    private Soldier selectedSoldier;
    private Enemy selectedEnemy;
    
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
        int midX = Screen.width / 2;
        int midY = Screen.height / 2;
        int midAllyX = midX;
        List<Vector2> allyPositions = new List<Vector2>
        {
            new(midAllyX - 200, midY - 200),
            new(midAllyX - 200, midY),
            new(midAllyX - 200, midY + 200),
            new(midAllyX, midY - 100),
            new(midAllyX, midY + 100),
        };
        // 初始化5个阵型位置
        for (int i = 0; i < 5; i++)
        {
            var slot = Instantiate(formationSlotPrefab, formationSlotsPanel).GetComponent<FormationSlot>();
            slot.Initialize(i, this);
            formationSlots.Add(slot);
            slot.transform.position = allyPositions[i];
        }

        // 加载可用士兵
        foreach (var soldier in CombatManager.Instance.GetAvailableSoldiers())
        {
            var card = Instantiate(soldierCardPrefab, availableSoldiersPanel).GetComponent<SoldierCard>();
            card.Initialize(soldier, this);
            availableSoldierCards.Add(card);
        }

        // 加载敌人信息
        foreach (var enemy in CombatManager.Instance.GetAvailableEnemies())
        {
            var card = Instantiate(enemyCardPrefab, availableEnemiesPanel).GetComponent<CharacterUI>();
            card.Initialize(enemy, this);
            availableEnemyCards.Add(card);
        }
    }

    void SetupButtons()
    {
        startBattleButton.onClick.AddListener(OnStartBattle);
        retreatButton.onClick.AddListener(OnRetreat);
    }

    public void OnSoldierSelected(Soldier soldier)
    {
        selectedSoldier = soldier;
        soldierInfoPanel.UpdateInfo(soldier);
        
        // 如果是从阵型中选择，显示取消按钮
        formationSlots.ForEach(slot => slot.ToggleCancelButton(slot.ContainsSoldier(soldier)));
    }

    public void OnEnemySelected(Enemy enemy)
    {
        selectedEnemy = enemy;
        enemyInfoPanel.UpdateInfo(enemy);
    }

    public void TryAssignSoldierToSlot(Soldier soldier, int slotIndex)
    {
        // 查找第一个空位或交换位置
        var targetSlot = formationSlots[slotIndex];
        if (targetSlot.CurrentSoldier == null)
        {
            AssignSoldierToSlot(soldier, targetSlot);
        }
        else
        {
            // 交换位置逻辑
            var originalSlot = formationSlots.First(s => s.CurrentSoldier == soldier);
            var tempSoldier = targetSlot.CurrentSoldier;
            
            targetSlot.AssignSoldier(soldier);
            originalSlot.AssignSoldier(tempSoldier);
        }
    }

    private void AssignSoldierToSlot(Soldier soldier, FormationSlot slot)
    {
        // 从原位置移除
        var currentSlot = formationSlots.FirstOrDefault(s => s.CurrentSoldier == soldier);
        currentSlot?.ClearSlot();
        
        // 分配到新位置
        slot.AssignSoldier(soldier);
        
        // 更新卡牌状态
        availableSoldierCards.First(c => c.Soldier == soldier).SetSelected(true);
    }

    public void RemoveSoldierFromFormation(Soldier soldier)
    {
        var slot = formationSlots.First(s => s.CurrentSoldier == soldier);
        slot.ClearSlot();
        availableSoldierCards.First(c => c.Soldier == soldier).SetSelected(false);
    }

    void OnStartBattle()
    {
        var selectedSoldiers = formationSlots
            .Where(s => s.CurrentSoldier != null)
            .Select(s => s.CurrentSoldier)
            .ToList();

        if (selectedSoldiers.Count == 0)
        {
            // Todo: pop up window
            Debug.LogError("至少需要选择一个士兵！");
            return;
        }

        CombatManager.Instance.StartCombat(selectedSoldiers, CombatManager.Instance.GetAvailableEnemies());
        GameManager.Instance.LoadGameState(GameState.CombatPage);
    }

    void OnRetreat()
    {
        GameManager.Instance.LoadGameState(GameState.MissionSelectPage);
    }
}