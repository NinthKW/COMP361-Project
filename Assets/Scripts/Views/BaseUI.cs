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

    // Hardcoded grid dimensions
    private int rows = 7;         // 7 rows (y-axis)
    private int columns = 12;     // 12 columns (x-axis)
    private float cellSize = 110f; // Smaller cell size for a reduced overall grid
    private Color lineColor = Color.black;
    private float lineThickness = 2f;

    private RectTransform tableRect;


    // Start is called before the first frame update
    void Start()
    {
        PopulateBuildingList();
        backButton.onClick.AddListener(OnBackButtonClicked);

        // Ensure a Canvas exists in the scene.
    //    Canvas canvas = FindObjectOfType<Canvas>();
    //    if (canvas == null)
    //    {
    //        GameObject canvasObj = new GameObject("Canvas");
    //        canvas = canvasObj.AddComponent<Canvas>();
    //        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    //        canvasObj.AddComponent<CanvasScaler>();
    //        canvasObj.AddComponent<GraphicRaycaster>();
    //    }

    //    // Create the "Table" panel.
    //    GameObject tablePanel = new GameObject("GridTable");
    //    tablePanel.transform.SetParent(canvas.transform, false);

    //    tableRect = tablePanel.AddComponent<RectTransform>();

    //    // Anchor the table at the top-right corner.
    //    tableRect.anchorMin = new Vector2(1f, 1f);
    //    tableRect.anchorMax = new Vector2(1f, 1f);
    //    tableRect.pivot = new Vector2(1f, 1f);

    //    // Position the table 50px from the top and right edges.
    //    tableRect.anchoredPosition = new Vector2(-50f, -50f);

    //    // Set the overall size of the table based on the grid dimensions.
    //    float totalWidth = columns * cellSize;
    //    float totalHeight = rows * cellSize;
    //    tableRect.sizeDelta = new Vector2(totalWidth, totalHeight);

    //    // Add a black transparent background to the grid.
    //    Image tableBg = tablePanel.AddComponent<Image>();
    //    tableBg.color = new Color(0f, 0f, 0f, 0.5f); // Black with 50% opacity

    //    // Draw the grid lines.
    //    DrawGrid(totalWidth, totalHeight);
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


    void DrawGrid(float totalWidth, float totalHeight)
    {
        // Draw vertical grid lines.
        for (int i = 0; i <= columns; i++)
        {
            float xPos = i * cellSize;
            CreateLine(new Vector2(xPos, 0), new Vector2(xPos, totalHeight));
        }

        // Draw horizontal grid lines.
        for (int j = 0; j <= rows; j++)
        {
            float yPos = j * cellSize;
            CreateLine(new Vector2(0, yPos), new Vector2(totalWidth, yPos));
        }
    }

    /// <summary>
    /// Creates a thin Image-based line between two points in the UI.
    /// </summary>
    void CreateLine(Vector2 start, Vector2 end)
    {
        GameObject lineObj = new GameObject("GridLine");
        lineObj.transform.SetParent(tableRect, false);

        Image lineImage = lineObj.AddComponent<Image>();
        lineImage.color = lineColor;

        RectTransform lineRect = lineObj.GetComponent<RectTransform>();
        // Set a fixed anchor and pivot for line placement.
        lineRect.anchorMin = new Vector2(0, 0);
        lineRect.anchorMax = new Vector2(0, 0);
        lineRect.pivot = new Vector2(0, 0);

        // Calculate the direction, length, and angle of the line.
        Vector2 direction = end - start;
        float length = direction.magnitude;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Set the line's size, position, and rotation.
        lineRect.sizeDelta = new Vector2(length, lineThickness);
        lineRect.anchoredPosition = start;
        lineRect.localRotation = Quaternion.Euler(0, 0, angle);
    }

    void OnBackButtonClicked()
    {
        GameManager.Instance.ChangeState(GameState.MainMenuPage);
        GameManager.Instance.LoadGameState(GameState.MainMenuPage);
    }
}
