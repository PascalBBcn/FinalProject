using UnityEngine;

public class Crosshair : MonoBehaviour
{
    RectTransform rect;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        Cursor.visible = false;
    }

    void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        rect.position = mousePos;
    }
}
