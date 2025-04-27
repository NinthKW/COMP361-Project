using Assets.Scripts.Controller;
using Assets.Scripts.Model;
using UnityEngine;
using UnityEngine.UI;

public class StaffpageSoldier : MonoBehaviour
{
    public Character soldier;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnSoldierSelected);
    }

    // Notify StaffpageUI to update the soldier detail panel.
    void OnSoldierSelected()
    {
        AudioManager.Instance.PlaySound("Building Locking"); // Play a sound when selecting a soldier
        StaffpageUI.Instance.UpdateSoldierDetail(soldier);
    }
}
