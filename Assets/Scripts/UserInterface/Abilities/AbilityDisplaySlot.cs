using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class AbilityDisplaySlot : MonoBehaviour
{
    [Header("Dependency")]
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI chargesText;

    [Header("Overlays")]
    [SerializeField] private Image cooldownOverlay;
    [SerializeField] private Image durationOverlay;
    [SerializeField] private Image usableOverlay;

    [Space]

    [SerializeField] private StatusBar cooldownBar;
    [SerializeField] private StatusBar durationBar;

    public Sprite Background
    {
        get
        {
            return background.sprite;
        }
        set
        {
            background.sprite = value;
        }
    }

    public Sprite CooldowSprite
    {
        get
        {
            return cooldownOverlay.sprite;
        }
        set
        {
            cooldownOverlay.sprite = value;
        }
    }

    public Sprite DurationSprite
    {
        get
        {
            return durationOverlay.sprite;
        }
        set
        {
            durationOverlay.sprite = value;
        }
    }

    public Sprite UsableSprite
    {
        get
        {
            return usableOverlay.sprite;
        }
        set
        {
            usableOverlay.sprite = value;
        }
    }

    private int chargesTextIntCache = -1;
    private int chargesUsableIntCache = -1;
    private bool durationActiveCache = false;

    public void UpdateCooldownBar(float fillAmount)
    {
        cooldownBar.UpdateBar(fillAmount);
    }

    public void UpdateDurationBar(float fillAmount)
    {
        durationBar.UpdateBar(fillAmount);
    }

    public void UpdateUseState(int charges, bool durationActive)
    {
        if (chargesUsableIntCache == charges && durationActiveCache == durationActive) return;

        bool activeState = charges <= 0 && !durationActive;

        usableOverlay.gameObject.SetActive(activeState);

        chargesUsableIntCache = charges;
        durationActiveCache = durationActive;
    }

    public void UpdateChargesText(int amount)
    {
        if (chargesTextIntCache == amount) return;

        chargesText.text = amount.ToString();
        chargesTextIntCache = amount;
    }

    public void ClearChargesText()
    {
        chargesText.text = null;
        chargesTextIntCache = -1;
    }
}