using UnityEngine;

public class HealthBar : ModularBar
{
    [SerializeField] private HealthBarSettings settings;
    [SerializeField] private StatusBar chip;

    private float chipStartFill = 1f;
    private float chipTimer = 0f;

    private bool chipping;

    private readonly BarID healthBarID = new BarID();
    private readonly BarID shieldBarID = new BarID();

    public override void UpdateBar(BarID id, float current, float max)
    {
        float combinedOldValue = CombinedValue;
        float combinedOldMaxValue = CombinedMaxValue;

        base.UpdateBar(id, current, max);

        if (chipping) return;

        if (combinedOldValue > CombinedValue)
        {
            StartChipping(combinedOldValue / CombinedMaxValue);
        }
        else if (CombinedMaxValue > combinedOldMaxValue)
        {
            chip.UpdateBar(CombinedValue / CombinedMaxValue);
        }
    }

    public void UpdateBar(float health, float maxHealth, float shield, float maxShield)
    {
        UpdateBar(healthBarID, health, maxHealth);
        UpdateBar(shieldBarID, shield, maxShield);
    }

    private void Awake()
    {
        AddBar(healthBarID, settings.HealthColor);
        AddBar(shieldBarID, settings.ShieldColor);
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
        float combinedValue = CombinedValue;
        float targetFill = combinedValue / CombinedMaxValue;

        chip.UpdateBar(Mathf.Lerp(chipStartFill, targetFill, chipTimer / settings.ChipTime));

        if (chip.Fill == targetFill)
        {
            StopChipping();
        }
    }
}