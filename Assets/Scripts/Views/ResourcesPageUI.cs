using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Assets.Scripts.Model;
using Assets.Scripts.Controller;

public class ResourcesPageUI : MonoBehaviour
{
    public Text[] resourceAmountTexts;
    public Button confirmButton;

    public int changeAmountPerClick = 10;
    void Start()
    {
        confirmButton.onClick.AddListener(ConfirmButtonClicked);
        UpdateResourceDisplay();
    }

    public void UpArrowClicked(int resourceId)
    {
        GameManager.Instance.ChangeTempResource(resourceId, changeAmountPerClick);
        UpdateResourceDisplay();
    }
    public void DownArrowClicked(int resourceId)
    {
        GameManager.Instance.ChangeTempResource(resourceId, -changeAmountPerClick);
        UpdateResourceDisplay();
    }
    public void ConfirmButtonClicked()
    {
        GameManager.Instance.ConfirmResourceChanges();
        UpdateResourceDisplay();
    }

    public void UpdateResourceDisplay()
    {
        for (int i = 0; i < resourceAmountTexts.Length; i++)
        {
            int displayValue = GameManager.Instance.GetResourceDisplayValue(i);
            resourceAmountTexts[i].text = displayValue.ToString();
        }
    }
}
