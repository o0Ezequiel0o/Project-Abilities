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
        bool activeState = charges <= 0 && durationActive;
        usableOverlay.gameObject.SetActive(activeState);
    }

    public void UpdateChargesText(int amount)
    {
        chargesText.text = amount.ToString();
    }

    public void ClearChargesText()
    {
        chargesText.text = null;
    }
}