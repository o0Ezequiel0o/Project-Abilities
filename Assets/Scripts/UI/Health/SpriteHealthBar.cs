using UnityEngine;

public class SpriteHealthBar : SpriteModularBar
{
    [Space]

    [SerializeField] private HealthBarSettings settings;
    [SerializeField] private Transform chip;

    [Space]

    [SerializeField] private Sprite sprite;

    private float chipStart = 1f;
    private float chipTimer = 0f;

    private bool chipping = false;

    private readonly BarID healthBarID = new BarID();
    private readonly BarID shieldBarID = new BarID();

    public override void ResetValues()
    {
        base.ResetValues();
        chip.transform.localScale = new Vector3(1f, chip.transform.localScale.x, chip.transform.localScale.z);
        chip.transform.localPosition = Vector2.zero;
        StopChipping();
    }

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
            UpdateChip(CombinedValue / CombinedMaxValue);
        }
    }

    public void UpdateBar(float health, float maxHealth, float shield, float maxShield)
    {
        UpdateBar(healthBarID, health, maxHealth);
        UpdateBar(shieldBarID, shield, maxShield);
    }

    protected override void Awake()
    {
        base.Awake();

        AddBar(healthBarID, settings.HealthColor, sprite);
        AddBar(shieldBarID, settings.ShieldColor, sprite);
    }

    private void Update()
    {
        if (chipping)
        {
            chipTimer += Time.deltaTime;
            UpdateChipBarFill();
        }
    }

    private void UpdateChip(float percentage)
    {
        chip.transform.localScale = new Vector2(percentage, 1f);
        chip.transform.localPosition = new Vector2((percentage - 1f) * 0.5f, 0f);
    }

    protected void StartChipping(float start)
    {
        chipStart = start;
        chipping = true;
    }

    protected void StopChipping()
    {
        chipping = false;
        chipTimer = 0f;
    }

    protected void UpdateChipBarFill()
    {
        float targetFill = CombinedValue / CombinedMaxValue;
        UpdateChip(Mathf.Lerp(chipStart, targetFill, chipTimer / settings.ChipTime));

        if (chipTimer >= settings.ChipTime || chip.transform.localScale.x < targetFill)
        {
            StopChipping();
        }
    }
}