using UnityEngine;
using UnityEngine.EventSystems;

public class AnimalDropZone : MonoBehaviour, IDropHandler
{
    public GameManager gameManager;

    public void OnDrop(PointerEventData eventData)
    {
        // Check if the dropped object has the DraggableFood script
        if (eventData.pointerDrag != null)
        {
            DraggableFood droppedFood = eventData.pointerDrag.GetComponent<DraggableFood>();
            
            if (droppedFood != null)
            {
                // Send the food type to the GameManager to check if it's correct
                gameManager.CheckMatch(droppedFood);
            }
        }
    }
}