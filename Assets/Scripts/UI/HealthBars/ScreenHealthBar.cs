using UnityEngine;
using TMPro;

public class ScreenHealthBar : HealthBar
{
    [Space]

    [SerializeField] private TextMeshProUGUI amount;

    public void UpdateBar(float health, float maxHealth, float shield, float maxShield, float combinedHealth)
    {
        UpdateBar(health / maxHealth, shield / maxShield, combinedHealth);
    }

    public void UpdateBar(float healthRatio, float shieldRatio, float combinedHealth)
    {
        UpdateBar(healthRatio, shieldRatio);
        amount.text = NumberFormatter.FormatNumber(Mathf.Ceil(combinedHealth));
    }
}