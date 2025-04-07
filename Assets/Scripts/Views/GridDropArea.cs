using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace Assets.Scripts
{


    public class GridDropArea : MonoBehaviour, IDropHandler
    {
        public float occupancyThreshold = 0.1f;
        public float edgeMargin = 100f;
        public TextMeshProUGUI buildingInfoDisplay;

        private Canvas parentCanvas;
        private Camera canvasCamera;

        void Awake()
        {
            parentCanvas = GetComponentInParent<Canvas>();
            if (parentCanvas == null)
            {
                Debug.LogError("No Canvas found in the scene to attach the buildingInfoDisplay!");
                return;
            }

            canvasCamera = parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : parentCanvas.worldCamera;

            if (buildingInfoDisplay == null)
            {
                GameObject textObj = new GameObject("BuildingInfoDisplay", typeof(TextMeshProUGUI));
                textObj.transform.SetParent(parentCanvas.transform, false);
                buildingInfoDisplay = textObj.GetComponent<TextMeshProUGUI>();

                buildingInfoDisplay.fontSize = 25;
                buildingInfoDisplay.alignment = TextAlignmentOptions.Center;
                buildingInfoDisplay.text = "";
                buildingInfoDisplay.color = Color.white;

                RectTransform rect = buildingInfoDisplay.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(0, 0);
                rect.pivot = new Vector2(0, 0);
                rect.anchoredPosition = new Vector2(500, 10);
                rect.sizeDelta = new Vector2(800, 150);
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            DraggableBuilding draggable = eventData.pointerDrag?.GetComponent<DraggableBuilding>();
            if (draggable == null) return;

            RectTransform gridRect = GetComponent<RectTransform>();
            RectTransform droppedRect = draggable.GetComponent<RectTransform>();

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    gridRect,
                    eventData.position,
                    canvasCamera,
                    out Vector2 localPos))
            {
                draggable.ResetToInitialPosition();
                return;
            }

            float halfGridWidth = gridRect.rect.width * 0.5f;
            float halfGridHeight = gridRect.rect.height * 0.5f;
            float halfObjSize = 75f; // 150 / 2

            float clampedX = Mathf.Clamp(localPos.x, -halfGridWidth + halfObjSize - edgeMargin, halfGridWidth - halfObjSize + edgeMargin);
            float clampedY = Mathf.Clamp(localPos.y, -halfGridHeight + halfObjSize - edgeMargin, halfGridHeight - halfObjSize + edgeMargin);
            Vector2 finalPos = new Vector2(clampedX, clampedY);

            bool overlap = false;
            foreach (Transform child in transform)
            {
                if (child == draggable.transform) continue;

                RectTransform childRect = child.GetComponent<RectTransform>();
                if (childRect == null) continue;

                float thresholdW = occupancyThreshold * 150f;
                float thresholdH = occupancyThreshold * 150f;
                float xDiff = Mathf.Abs(childRect.anchoredPosition.x - finalPos.x);
                float yDiff = Mathf.Abs(childRect.anchoredPosition.y - finalPos.y);

                if (xDiff < thresholdW && yDiff < thresholdH)
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
                draggable.transform.SetParent(transform, false);
                draggable.transform.localScale = Vector3.one;

                droppedRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
                droppedRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 150);

                // ✅ Correct the position to where you saw it visually during drag
                Vector2 correctedFinalPos = finalPos - draggable.pointerOffset;
                droppedRect.anchoredPosition = correctedFinalPos;

                if (buildingInfoDisplay != null)
                {
                    buildingInfoDisplay.text = "Building placed: " + draggable.name;
                }
            }
        }
    }
}