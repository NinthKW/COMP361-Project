using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Assets.Scripts.Controller;
using UnityEngine.UI;
using System.Drawing;

public class BaseUI : MonoBehaviour
{
    public Transform missionButtonContainer;
    public GameObject missionButtonPrefab;
    [SerializeField] int size;

    public TextMeshProUGUI buildingName;
    private Base selectedBuilding;
    // Start is called before the first frame update
    void Start()
    {
        PopulateBuildingList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Create all selectable building objects
    void PopulateBuildingList()
    {
        Debug.Log("PopulateBuildingList called");
        if (BaseManager.Instance == null)
        {
            Debug.LogError("BaseManager.Instance is NULL");
            return;
        }

        if (BaseManager.Instance.buildingList == null)
        {
            Debug.LogError("building list is NULL");
            return;
        }

        Debug.Log("Building count: " + BaseManager.Instance.buildingList.Count);

        foreach (var building in BaseManager.Instance.buildingList)
        {
            Debug.Log("Adding building: " + building.name);

            GameObject buttonObj = Instantiate(missionButtonPrefab, missionButtonContainer);

            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            buttonText.text = building.name;

            Button btn = buttonObj.GetComponent<Button>();
            btn.onClick.AddListener(() => OnSelectedBuilding(building));
        }
    }

    //Update selected building attribute
    void OnSelectedBuilding(Base building)
    {
        buildingName.text = building.name;
        selectedBuilding = building;
    }
}
