using UnityEngine.UI;
using UnityEngine;

public class HealthBar : StatusBar
{
    [SerializeField] private Image chipBar;
    [SerializeField] private float chipTime;

    private float chipStartFill;
    private float chipTimer;

    private bool chipping;

    public override void UpdateBar(float percentage)
    {
        if (!chipping)
        {
            StartChipping(bar.fillAmount);
        }

        base.UpdateBar(percentage);
    }

    void Awake()
    {
        chipStartFill = 1f;
        chipTimer = 0f;
    }

    void Update()
    {
        if (chipping)
        {
            UpdateChipBarTimer();
            UpdateChipBarFill();
        }
    }

    void UpdateChipBarTimer()
    {
        chipTimer += Time.deltaTime;
    }

    void StartChipping(float startFill)
    {
        chipStartFill = startFill;
        chipping = true;
    }

    void StopChipping()
    {
        chipping = false;
        chipTimer = 0f;
    }

    void UpdateChipBarFill()
    {
        chipBar.fillAmount = Mathf.Lerp(chipStartFill, bar.fillAmount, chipTimer / chipTime);

        if (chipBar.fillAmount == bar.fillAmount)
        {
            StopChipping();
        }
    }
}