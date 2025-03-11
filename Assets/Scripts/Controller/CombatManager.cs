using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;
    public Combat combatData;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        combatData = new Combat();
    }

    public void PlayerAttack()
    {
        combatData.AttackEnemy(20);
    }

    public void EnemyAttack()
    {
        combatData.ReceiveDamage(10);
        CheckBattleEnd();
    }

    void CheckBattleEnd()
    {
        if (combatData.enemyHealth <= 0)
        {
            Debug.Log("Enemy Defeated! Returning to Mission Select...");
            GameManager.Instance.ChangeState(GameState.MissionSelectPage);
            UnityEngine.SceneManagement.SceneManager.LoadScene("MissionSelect");
        }
        else if (combatData.playerHealth <= 0)
        {
            Debug.Log("Game Over! Returning to Main Menu...");
            GameManager.Instance.ChangeState(GameState.MainMenuPage);
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}
