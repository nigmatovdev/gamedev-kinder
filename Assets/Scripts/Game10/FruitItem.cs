using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class FruitItem : MonoBehaviour
{
    public FruitType fruitType;
    private bool isCollected = false;
    private FruitGameManager gameManager;

    public void Setup(FruitType type, FruitGameManager manager)
    {
        fruitType = type;
        gameManager = manager;
    }

    private void Update()
    {
        if (isCollected) return;

        // Detect Mouse Click or Tap using the New Input System
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            Vector2 screenPosition = Pointer.current.position.ReadValue();
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            
            // Requires a Collider2D on the Fruit Prefab
            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                OnClicked();
            }
        }
    }

    private void OnClicked()
    {
        isCollected = true;
        gameManager.CollectFruit(this);
    }

    public void MoveToBasket(Vector3 basketPosition)
    {
        StartCoroutine(MoveRoutine(basketPosition));
    }

    private IEnumerator MoveRoutine(Vector3 targetPosition)
    {
        float duration = 0.5f;
        float elapsed = 0f;
        Vector3 startPosition = transform.position;

        while (elapsed < duration)
        {
            // Smoothly lerp to the basket
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }
}