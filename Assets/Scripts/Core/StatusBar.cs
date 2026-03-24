using UnityEngine.UI;
using UnityEngine;

public class StatusBar : MonoBehaviour
{
    [SerializeField] protected Image bar;
    [SerializeField] protected Mode mode;

    public virtual void UpdateBar(float value, float maxValue)
    {
        UpdateBar(value / maxValue);
    }

    public virtual void UpdateBar(float percentage)
    {
        (float minFill, float maxFill) = GetFillLimits(mode);
        bar.fillAmount = Mathf.Lerp(minFill, maxFill, Mathf.Clamp01(percentage));
    }

    (float, float) GetFillLimits(Mode mode)
    {
        float minFill = 0f;
        float maxFill = 0f;

        switch (mode)
        {
            case Mode.Normal:
                minFill = 0f; maxFill = 1f;
                break;

            case Mode.Reverse:
                minFill = 1f; maxFill = 0f;
                break;
        }

        return (minFill, maxFill);
    }

    protected enum Mode
    {
        Normal,
        Reverse
    }
}