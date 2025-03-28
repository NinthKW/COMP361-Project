using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Assets.Scripts.Controller;
using UnityEngine.UI;

public class BaseUI : MonoBehaviour
{
    public Transform missionButtonContainer;
    public GameObject missionButtonPrefab;

    public TextMeshProUGUI buildingName;
    private Base selectedBuilding;

    public Button backButton;

    private RectTransform tableRect;


    // Start is called before the first frame update
    void Start()
    {
        PopulateBuildingList();
        backButton.onClick.AddListener(OnBackButtonClicked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
        
        // Adjust the button's RectTransform to increase its height
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        // For example, double the current height:
        buttonRect.sizeDelta = new Vector2(buttonRect.sizeDelta.x, buttonRect.sizeDelta.y * 2);
        
        // If your prefab has a LayoutElement component that controls its size, update it too:
        LayoutElement layoutElement = buttonObj.GetComponent<LayoutElement>();
        if (layoutElement != null)
        {
            layoutElement.preferredHeight = buttonRect.sizeDelta.y;
        }
        
        Sprite buttonTexture = Resources.Load<Sprite>("base_" + building.name.ToLower());
        buttonObj.GetComponent<Image>().sprite = buttonTexture;

        if (buttonTexture == null)
        {
            Debug.LogError("Sprite not found: " + "base_" + building.name.ToLower());
        }

        TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = building.name;

        Button btn = buttonObj.GetComponent<Button>();
        btn.onClick.AddListener(() => OnSelectedBuilding(building));

        buttonObj.SetActive(true);
    }

    // If missionButtonContainer has a GridLayoutGroup that is forcing cell sizes, adjust it as well:
    GridLayoutGroup grid = missionButtonContainer.GetComponent<GridLayoutGroup>();
    if (grid != null)
    {
        grid.cellSize = new Vector2(grid.cellSize.x, grid.cellSize.y * 2);
    }
}


    //Update selected building attribute
    void OnSelectedBuilding(Base building)
    {
        buildingName.text = building.name;
        selectedBuilding = building;
    }



    
}
