using UnityEngine;
using TMPro;

public class FallingNumber : MonoBehaviour
{
    public int  Value     { get; private set; }
    public bool WasCaught { get; private set; }

    float _speed;
    float _rotationSpeed;

    public void Setup(int value, float speed)
    {
        Value  = value;
        _speed = speed;
        _rotationSpeed = Random.Range(-40f, 40f);
        transform.localScale = Vector3.one * Random.Range(0.9f, 1.2f);
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));

        // World-space text label centred on the ball
var textGO = new GameObject("Lbl");
        textGO.transform.SetParent(transform, false);
        textGO.transform.localPosition = Vector3.zero;

        var tmp = textGO.AddComponent<TextMeshPro>();
        tmp.text      = value.ToString();
        tmp.fontSize  = 3.8f;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color     = Color.white;
        tmp.fontStyle = FontStyles.Bold;
        tmp.sortingOrder = 6;

        tmp.rectTransform.sizeDelta = new Vector2(1.2f, 1.2f);
    }

    public void Catch()
    {
        WasCaught = true;
        Destroy(gameObject);
    }

    void Update()
    {
        if (WasCaught) return;
        transform.Translate(Vector3.down * _speed * Time.deltaTime, Space.World);
        transform.Rotate(Vector3.forward, _rotationSpeed * Time.deltaTime);
        
        // Keep the label upright
        var label = transform.Find("Lbl");
        if (label != null) label.rotation = Quaternion.identity;

        if (transform.position.y < -8f) Destroy(gameObject);
    }
}
