using UnityEngine;
using UnityEngine.EventSystems;

namespace Game1
{
    public class AnimalDropZone : MonoBehaviour, IDropHandler
    {
        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null)
            {
                DraggableFood droppedFood = eventData.pointerDrag.GetComponent<DraggableFood>();
                if (droppedFood != null)
                {
                    if (GameManager1.Instance.CheckFood(droppedFood.foodType))
                    {
                        Debug.Log("Correct Food!");
                        Destroy(droppedFood.gameObject);
                    }
                    else
                    {
                        Debug.Log("Wrong Food! Game Over.");
                        // The food will remain where it was dropped (or return if we handle it in OnEndDrag)
                        // But CheckFood triggers the Game Over panel.
                    }
                }
            }
        }
    }
}
