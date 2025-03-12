using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Model;
using Assets.Scripts.Controller;

public class CombatUI : MonoBehaviour
{
    public Button attackButton;

    void Start()
    {  
        attackButton.name = "Attack";
        attackButton.onClick.AddListener(PlayerAttack);
    }

    void PlayerAttack()
    {
        CombatManager.Instance.StartCombat();
    }
}