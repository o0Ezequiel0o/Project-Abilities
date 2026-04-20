using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private StatusBar health;
    [SerializeField] private StatusBar shield;
    [SerializeField] private StatusBar chip;

    [Space]

    [SerializeField] private float chipTime;

    private float chipStartFill = 1f;
    private float chipTimer = 0f;

    private bool chipping;

    public void UpdateBar(float health, float maxHealth, float shield, float maxShield)
    {
        UpdateBar(health / maxHealth, shield / maxShield);
    }

    public void UpdateBar(float healthRatio, float shieldRatio)
    {
        if (IsInfiniteOrUndefined(shieldRatio))
        {
            shieldRatio = 0f;
        }

        if (!chipping && healthRatio < 1f)
        {
            StartChipping(health.Fill);
        }

        health.UpdateBar(healthRatio);
        shield.UpdateBar(shieldRatio);
    }

    private void Update()
    {
        if (chipping)
        {
            chipTimer += Time.deltaTime;
            UpdateChipBarFill();
        }
    }

    protected void StartChipping(float startFill)
    {
        chipStartFill = startFill;
        chipping = true;
    }

    protected void StopChipping()
    {
        chipping = false;
        chipTimer = 0f;
    }

    protected void UpdateChipBarFill()
    {
        chip.UpdateBar(Mathf.Lerp(chipStartFill, health.Fill, chipTimer / chipTime));

        if (chip.Fill == health.Fill)
        {
            StopChipping();
        }
    }

    protected bool IsInfiniteOrUndefined(float value)
    {
        return float.IsNaN(value) || float.IsInfinity(value);
    }
}