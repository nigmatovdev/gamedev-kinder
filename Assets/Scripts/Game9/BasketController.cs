using UnityEngine;
using UnityEngine.InputSystem;

public class BasketController : MonoBehaviour
{
    const float MoveSpeed = 9f;

    float _halfLimit; // max |x| the basket centre can reach

    void Start()
    {
        var cam = Camera.main;
        float camHalfW = cam != null ? cam.orthographicSize * cam.aspect : 5f;
        _halfLimit = camHalfW - 0.9f;
    }

    void Update()
    {
        float input = ReadInput();
        if (Mathf.Abs(input) < 0.001f) return;

        float newX = transform.position.x + input * MoveSpeed * Time.deltaTime;
        newX = Mathf.Clamp(newX, -_halfLimit, _halfLimit);
        transform.position = new Vector3(newX, transform.position.y, 0f);
    }

    float ReadInput()
    {
        float axis = 0f;

        // Keyboard
        var kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.leftArrowKey.isPressed  || kb.aKey.isPressed) axis -= 1f;
            if (kb.rightArrowKey.isPressed || kb.dKey.isPressed) axis += 1f;
        }

        // Touch – drag toward touch X position
        var ts = Touchscreen.current;
        if (ts != null && ts.primaryTouch.press.isPressed)
        {
            var screenPt = ts.primaryTouch.position.ReadValue();
            var worldPt  = Camera.main.ScreenToWorldPoint(
                               new Vector3(screenPt.x, screenPt.y, 10f));
            float diff = worldPt.x - transform.position.x;
            axis = Mathf.Clamp(diff * 3f, -1f, 1f);
        }

        return axis;
    }
}
