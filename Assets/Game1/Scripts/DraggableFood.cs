using UnityEngine;
using UnityEngine.EventSystems;

namespace Game1 { // This keeps this script separate from other games
    public class DraggableFood : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
        public FoodType foodType; 
        private Vector2 startPos;
        private CanvasGroup canvasGroup;

        void Awake() {
            canvasGroup = GetComponent<CanvasGroup>();
            startPos = transform.position;
        }

        public void OnBeginDrag(PointerEventData eventData) {
            canvasGroup.blocksRaycasts = false; 
            // Optional: make it transparent while dragging
            canvasGroup.alpha = 0.6f;
        }

        public void OnDrag(PointerEventData eventData) {
            transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData) {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;

            if (eventData.pointerEnter == null || !eventData.pointerEnter.CompareTag("Animal")) {
                ReturnToTray();
            }
        }

        public void ReturnToTray() => transform.position = startPos;
    }
}