using UnityEngine;

public class CinematicStatusBar : StatusBar
{
    [SerializeField] private float duration = 1f;
    [SerializeField] private bool resetAnimation = true;

    private float startFill = 1f;
    private float targetFill = 0f;
    private float animationTimer = 0f;

    private bool animating = false;

    public override void UpdateBar(float percentage)
    {
        (float minFill, float maxFill) = GetFillLimits(mode);

        targetFill = Mathf.Lerp(minFill, maxFill, percentage);

        if (targetFill == bar.fillAmount) return;

        if ((animating && resetAnimation) || !animating)
        {
            StartAnimation(bar.fillAmount);
        }
    }

    protected void StartAnimation(float startFill)
    {
        if (animating)
        {
            StopAnimation();
        }

        this.startFill = startFill;
        animating = true;
    }

    protected void StopAnimation()
    {
        animating = false;
        animationTimer = 0f;
    }

    protected void UpdateBarAnimation()
    {
        animationTimer += Time.deltaTime;

        bar.fillAmount = Mathf.Lerp(startFill, targetFill, animationTimer / duration);

        if (bar.fillAmount == targetFill)
        {
            StopAnimation();
        }
    }

    private void Update()
    {
        if (animating)
        {
            UpdateBarAnimation();
        }
    }
}