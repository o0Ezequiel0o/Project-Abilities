using UnityEngine;
using TMPro;

public class ScreenHealthBar : HealthBar
{
    [Space]

    [SerializeField] private TextMeshProUGUI amount;

    public void UpdateBar(float health, float maxHealth, float shield, float maxShield, float combinedHealth)
    {
        UpdateBar(health, maxHealth, shield, maxShield);
        amount.text = NumberFormatter.FormatNumber(Mathf.Ceil(combinedHealth));
    }
}