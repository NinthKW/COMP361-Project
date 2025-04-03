using UnityEngine;
using UnityEngine.EventSystems;

public class GridDropArea : MonoBehaviour, IDropHandler
{
    // Grid cell size (useful for occupancy checks)
    public float cellSize = 110f;

    // Threshold to decide if two objects overlap (in units, adjust as needed)
    public float occupancyThreshold = 0.7f; // Was 0.5f — more generous now

    // Margin to allow drops near or slightly outside the grid edges
    public float edgeMargin = 20f;

    public void OnDrop(PointerEventData eventData)
    {
        DraggableBuilding draggable = eventData.pointerDrag.GetComponent<DraggableBuilding>();
        if (draggable == null)
            return;

        RectTransform gridRect = GetComponent<RectTransform>();
        RectTransform droppedRect = draggable.GetComponent<RectTransform>();

        Vector2 localPos;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                gridRect,
                eventData.position,
                eventData.pressEventCamera,
                out localPos))
        {
            draggable.ResetToInitialPosition();
            return;
        }

        float halfGridWidth = gridRect.rect.width * 0.5f;
        float halfGridHeight = gridRect.rect.height * 0.5f;
        float halfObjWidth = droppedRect.rect.width * 0.5f;
        float halfObjHeight = droppedRect.rect.height * 0.5f;

        // More flexible clamping with a margin
        float clampedX = Mathf.Clamp(localPos.x, -halfGridWidth + halfObjWidth - edgeMargin, halfGridWidth - halfObjWidth + edgeMargin);
        float clampedY = Mathf.Clamp(localPos.y, -halfGridHeight + halfObjHeight - edgeMargin, halfGridHeight - halfObjHeight + edgeMargin);
        Vector2 finalPos = new Vector2(clampedX, clampedY);

        // Overlap detection with higher tolerance
        bool overlap = false;
        foreach (Transform child in transform)
        {
            if (child == draggable.transform)
                continue;

            RectTransform childRect = child.GetComponent<RectTransform>();
            if (Vector2.Distance(childRect.anchoredPosition, finalPos) < occupancyThreshold * cellSize)
            {
                overlap = true;
                break;
            }
        }

        if (overlap)
        {
            draggable.ResetToInitialPosition();
        }
        else
        {
            draggable.transform.SetParent(transform);
            droppedRect.anchoredPosition = finalPos;
        }
    }
}
