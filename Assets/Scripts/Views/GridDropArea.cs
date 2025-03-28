using UnityEngine;
using UnityEngine.EventSystems;

public class GridDropArea : MonoBehaviour, IDropHandler
{
    // Grid cell size (useful for occupancy checks)
    public float cellSize = 110f;
    // Threshold to decide if two objects overlap (in units, adjust as needed)
    public float occupancyThreshold = 0.5f; 

    public void OnDrop(PointerEventData eventData)
    {
        // Get the draggable building component.
        DraggableBuilding draggable = eventData.pointerDrag.GetComponent<DraggableBuilding>();
        if (draggable == null)
            return;

        // Get RectTransforms for the grid and the dropped object.
        RectTransform gridRect = GetComponent<RectTransform>();
        RectTransform droppedRect = draggable.GetComponent<RectTransform>();

        // Convert the screen drop position into a local position relative to the grid.
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

        // Get half sizes of the grid and the object.
        float halfGridWidth = gridRect.rect.width * 0.5f;
        float halfGridHeight = gridRect.rect.height * 0.5f;
        float halfObjWidth = droppedRect.rect.width * 0.5f;
        float halfObjHeight = droppedRect.rect.height * 0.5f;

        // Clamp the drop position so the object remains fully inside the grid.
        float clampedX = Mathf.Clamp(localPos.x, -halfGridWidth + halfObjWidth, halfGridWidth - halfObjWidth);
        float clampedY = Mathf.Clamp(localPos.y, -halfGridHeight + halfObjHeight, halfGridHeight - halfObjHeight);
        Vector2 finalPos = new Vector2(clampedX, clampedY);

        // Check if the new position would overlap with any other objects in the grid.
        bool overlap = false;
        foreach (Transform child in transform)
        {
            // Skip the dragged object itself.
            if (child == draggable.transform)
                continue;

            RectTransform childRect = child.GetComponent<RectTransform>();
            // If the distance between centers is less than a fraction of cellSize, consider it overlapping.
            if (Vector2.Distance(childRect.anchoredPosition, finalPos) < occupancyThreshold * cellSize)
            {
                overlap = true;
                break;
            }
        }

        if (overlap)
        {
            // If overlapping another object, revert to the initial starting position.
            draggable.ResetToInitialPosition();
        }
        else
        {
            // Accept the drop:
            // Reparent the dragged object to the grid.
            draggable.transform.SetParent(transform);
            // Set its anchored position to the computed final (clamped) position.
            droppedRect.anchoredPosition = finalPos;
        }
    }
}
