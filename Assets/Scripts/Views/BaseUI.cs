using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseUI: MonoBehaviour
{
    // Hardcoded grid dimensions
    private int rows = 8;         // 7 rows (y-axis)
    private int columns = 12;     // 12 columns (x-axis)
    private float cellSize = 110f; // Smaller cell size for a reduced overall grid
    private Color lineColor = Color.black;
    private float lineThickness = 2f;

    private RectTransform tableRect;

    void Start()
    {
        // Ensure a Canvas exists in the scene.
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // Create the "Table" panel.
        GameObject tablePanel = new GameObject("GridTable");
        tablePanel.transform.SetParent(canvas.transform, false);

        tableRect = tablePanel.AddComponent<RectTransform>();
        
        // Anchor the table at the top-right corner.
        tableRect.anchorMin = new Vector2(1f, 1f);
        tableRect.anchorMax = new Vector2(1f, 1f);
        tableRect.pivot = new Vector2(1f, 1f);

        // Position the table 50px from the top and right edges.
        tableRect.anchoredPosition = new Vector2(-50f, -50f);

        // Set the overall size of the table based on the grid dimensions.
        float totalWidth = columns * cellSize;
        float totalHeight = rows * cellSize;
        tableRect.sizeDelta = new Vector2(totalWidth, totalHeight);

        // Add a black transparent background to the grid.
        Image tableBg = tablePanel.AddComponent<Image>();
        tableBg.color = new Color(0f, 0f, 0f, 0.5f); // Black with 50% opacity

        // Draw the grid lines.
        DrawGrid(totalWidth, totalHeight);
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
}
