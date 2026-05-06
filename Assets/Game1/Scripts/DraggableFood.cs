using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game1
{
    [RequireComponent(typeof(Image), typeof(CanvasGroup))]
    public class DraggableFood : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public FoodType foodType;
        
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private Vector2 originalPosition;
        private Transform originalParent;
        private Canvas canvas;
        private Vector3 originalScale;

        private Vector2 originalAnchorMin;
        private Vector2 originalAnchorMax;
        private Vector2 originalPivot;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            canvas = GetComponentInParent<Canvas>();
            originalScale = transform.localScale;
            originalAnchorMin = rectTransform.anchorMin;
            originalAnchorMax = rectTransform.anchorMax;
            originalPivot = rectTransform.pivot;
        }

        public void SetFood(FoodType type, Sprite sprite)
        {
            foodType = type;
            Transform iconTransform = transform.Find("Icon");
            if (iconTransform != null)
            {
                Image iconImg = iconTransform.GetComponent<Image>();
                if (iconImg != null) iconImg.sprite = sprite;
            }
            else
            {
                Image img = GetComponent<Image>();
                if (img != null) img.sprite = sprite;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (canvas == null) canvas = GetComponentInParent<Canvas>();
            
            originalPosition = rectTransform.anchoredPosition;
            originalParent = transform.parent;
            
            // Lock position before changing parent
            Vector3 worldPos = transform.position;
            transform.SetParent(canvas.transform, true);
            transform.position = worldPos;

            // Reset anchors and pivot to center for dragging consistency
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            
            canvasGroup.alpha = 0.8f;
            canvasGroup.blocksRaycasts = false;

            // Visual feedback: Scale up
            transform.localScale = originalScale * 1.2f;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)canvas.transform, 
                eventData.position, 
                eventData.pressEventCamera, 
                out Vector2 localPoint))
            {
                rectTransform.anchoredPosition = localPoint;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            transform.localScale = originalScale;

            if (transform.parent == canvas.transform)
            {
                ReturnToTray();
            }
        }

        public void ReturnToTray()
        {
            transform.SetParent(originalParent);
            
            // Restore original layout properties
            rectTransform.anchorMin = originalAnchorMin;
            rectTransform.anchorMax = originalAnchorMax;
            rectTransform.pivot = originalPivot;
            
            rectTransform.anchoredPosition = originalPosition;
        }
}
}
