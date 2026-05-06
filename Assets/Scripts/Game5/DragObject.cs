using UnityEngine;
using UnityEngine.InputSystem;

namespace BridgeBuilder
{
    public class DragObject : MonoBehaviour
    {
        private bool isDragging = false;
        private Vector2 offset;
        private Rigidbody2D rb;
        private Vector3 startPos;
        private Camera mainCam;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            startPos = transform.position;
            mainCam = Camera.main;
        }

        public void OnDrag(InputAction.CallbackContext context)
        {
            if (!isDragging || GameManager.Instance.IsTesting) return;
            
            Vector2 mousePos = Pointer.current.position.ReadValue();
            Vector3 worldPos = mainCam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0));
            transform.position = new Vector3(worldPos.x + offset.x, worldPos.y + offset.y, 0);
        }

        void OnMouseDown()
        {
            if (GameManager.Instance && !GameManager.Instance.IsTesting)
            {
                isDragging = true;
                Vector2 mousePos = Pointer.current.position.ReadValue();
                Vector3 worldPos = mainCam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0));
                offset = (Vector2)transform.position - (Vector2)worldPos;
                if (rb) rb.bodyType = RigidbodyType2D.Kinematic;
            }
        }

        void OnMouseUp()
        {
            isDragging = false;
        }

        void Update()
        {
            if (isDragging)
            {
                Vector2 mousePos = Pointer.current.position.ReadValue();
                Vector3 worldPos = mainCam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0));
                transform.position = new Vector3(worldPos.x + offset.x, worldPos.y + offset.y, 0);
            }
        }

        public void ResetPosition()
        {
            transform.position = startPos;
            if (rb) 
            {
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0;
            }
        }

        public void SetPhysics(bool enabled)
        {
            if (rb)
            {
                rb.bodyType = enabled ? RigidbodyType2D.Dynamic : RigidbodyType2D.Kinematic;
                if (!enabled)
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.angularVelocity = 0;
                }
            }
        }
    }
}
