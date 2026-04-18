using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class StatusEffectDisplaySlot : MonoBehaviour
{
    [Header("Dependency")]
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI stacksText;

    public Sprite Icon
    {
        get
        {
            return image.sprite;
        }
        set
        {
            image.sprite = value;
        }
    }

    public void UpdateStacksAmount(int stacks)
    {
        if (stacks == 1)
        {
            stacksText.text = string.Empty;
        }
        else
        {
            stacksText.text = stacks.ToString();
        }
    }
}