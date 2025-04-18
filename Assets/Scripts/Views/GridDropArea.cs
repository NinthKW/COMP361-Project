// Assets/Scripts/GridDropArea.cs
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace Assets.Scripts
{
    public class GridDropArea : MonoBehaviour, IDropHandler
    {
        [Header("Bounds Settings")]
        public float padding = 10f;       // space from the very edge
        public bool freeDrop = false;     // if true, skips clamping

        public TextMeshProUGUI buildingInfoDisplay;

        private Canvas parentCanvas;
        private Camera canvasCamera;

        void Awake()
        {
            parentCanvas = GetComponentInParent<Canvas>();
            if (parentCanvas == null)
            {
                Debug.LogError("No Canvas found in parent hierarchy!");
                return;
            }
            canvasCamera = parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : parentCanvas.worldCamera;

            // auto-create info text if you forgot to assign one
            if (buildingInfoDisplay == null)
            {
                var txtGO = new GameObject("BuildingInfoDisplay", typeof(TextMeshProUGUI));
                txtGO.transform.SetParent(parentCanvas.transform, false);
                buildingInfoDisplay = txtGO.GetComponent<TextMeshProUGUI>();
                buildingInfoDisplay.fontSize = 25;
                buildingInfoDisplay.alignment = TextAlignmentOptions.Center;
                buildingInfoDisplay.color = Color.white;

                var rt = buildingInfoDisplay.rectTransform;
                rt.anchorMin = rt.anchorMax = rt.pivot = Vector2.zero;
                rt.anchoredPosition = new Vector2(0, 10);
                rt.sizeDelta = new Vector2(800, 150);
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            var draggable = eventData.pointerDrag?.GetComponent<DraggableBuilding>();
            if (draggable == null) return;

            var gridRect = GetComponent<RectTransform>();
            var droppedRect = draggable.GetComponent<RectTransform>();

            // 1) get drop point in grid‑local coords
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    gridRect,
                    eventData.position,
                    canvasCamera,
                    out Vector2 localPos))
            {
                draggable.ResetToInitialPosition();
                return;
            }

            // 2) compute half‑sizes
            float halfGridW = gridRect.rect.width  * 0.5f;
            float halfGridH = gridRect.rect.height * 0.5f;
            float halfObjW  = droppedRect.rect.width  * 0.5f;
            float halfObjH  = droppedRect.rect.height * 0.5f;

            // 3) clamp or free
            Vector2 finalPos;
            if (freeDrop)
            {
                finalPos = localPos;
            }
            else
            {
                float minX = -halfGridW + halfObjW + padding;
                float maxX =  halfGridW - halfObjW - padding;
                float minY = -halfGridH + halfObjH + padding;
                float maxY =  halfGridH - halfObjH - padding;

                finalPos = new Vector2(
                    Mathf.Clamp(localPos.x, minX, maxX),
                    Mathf.Clamp(localPos.y, minY, maxY)
                );
            }

            // 4) precise overlap check
            Rect newRect = new Rect(
                finalPos - new Vector2(halfObjW, halfObjH),
                new Vector2(halfObjW * 2, halfObjH * 2)
            );

            foreach (Transform child in transform)
            {
                if (child == draggable.transform) continue;
                var cr = child.GetComponent<RectTransform>();
                if (cr == null) continue;

                float cW = cr.rect.width  * 0.5f;
                float cH = cr.rect.height * 0.5f;
                Vector2 cPos = cr.anchoredPosition;
                Rect childRect = new Rect(
                    cPos - new Vector2(cW, cH),
                    new Vector2(cW * 2, cH * 2)
                );

                if (newRect.Overlaps(childRect))
                {
                    draggable.ResetToInitialPosition();
                    return;
                }
            }

            // 5) commit drop — **center** the building at finalPos
            draggable.transform.SetParent(transform, false);
            draggable.transform.localScale = Vector3.one;

            // make sure pivot/anchors are centered
            droppedRect.pivot     = new Vector2(0.5f, 0.5f);
            droppedRect.anchorMin = droppedRect.anchorMax = new Vector2(0.5f, 0.5f);

            // lock size (if you still want 150×150)
            droppedRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
            droppedRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   150);

            // **no pointerOffset subtraction** — this lines up the center
            droppedRect.anchoredPosition = finalPos;

            if (buildingInfoDisplay != null)
                buildingInfoDisplay.text = "Building placed: " + draggable.name;
        }
    }
}
