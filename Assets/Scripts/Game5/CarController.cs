using UnityEngine;

namespace BridgeBuilder
{
    public class CarController : MonoBehaviour
    {
        public float speed = 5f;
        private bool isDriving = false;
        private Rigidbody2D rb;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public void StartDriving()
        {
            isDriving = true;
            if (rb) rb.bodyType = RigidbodyType2D.Dynamic;
            
            // Ensure sprite faces right if speed is positive
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.flipX = speed < 0; // If speed is positive, flipX is false (assuming original faces right)
                // If original faces left, then sr.flipX = speed > 0;
            }
        }

        void FixedUpdate()
        {
            if (isDriving)
            {
                rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Finish"))
            {
                isDriving = false;
                GameManager.Instance.Win();
            }
            else if (other.CompareTag("DeadZone"))
            {
                isDriving = false;
                GameManager.Instance.Fail();
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            // Optional: detect if car flipped or something
        }
    }
}
