using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI display;
    [SerializeField] private bool roundToCeil = true;

    public float Value { get; private set; }

    public void Initialize(float value, float size, Color color)
    {
        display.transform.localScale = Vector2.one * size;
        display.color = color;
        UpdateValue(value);
    }

    public void UpdateAlpha(float alpha)
    {
        display.color = new Color(display.color.r, display.color.g, display.color.b, alpha);
    }

    public void UpdateValue(float value)
    {
        if (roundToCeil) value = Mathf.Ceil(value);
        display.text = value.ToString();
        Value = value;
    }
}