using TMPro;
using UnityEngine;

public class PickupText : MonoBehaviour
{
    private TextMeshPro tmpText;
    Color textColor;
    float alphaSpeed = 1.5f;
    float timer = 1.5f;
    float textMoveSpeed = 5f;

    public void Setup(string weaponName)
    {
        tmpText.SetText(weaponName);
    }

    private void Awake()
    {
        tmpText = GetComponent<TextMeshPro>();
        textColor = tmpText.color;
    }

    private void Update()
    {
        // Slowly fade text based on timer
        timer -= Time.deltaTime;
        if (timer < 0f) FadeText();

        transform.Translate(Vector3.up * textMoveSpeed * Time.deltaTime);
    }


    private void FadeText()
    {
        const float alphaThreshold = 5f / 255f;

        // Reduce alpha value 
        textColor.a -= alphaSpeed * Time.deltaTime;
        tmpText.color = textColor;

        // Destroy text
        if (textColor.a < alphaThreshold)
            Destroy(gameObject);
    }
}


