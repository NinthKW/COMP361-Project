using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Model;

public class CombatUI : MonoBehaviour
{
    public Button attackButton;

    void Start()
    {
        attackButton.onClick.AddListener(PlayerAttack);
    }

    void PlayerAttack()
    {
        CombatManager.Instance.PlayerAttack();
    }
}