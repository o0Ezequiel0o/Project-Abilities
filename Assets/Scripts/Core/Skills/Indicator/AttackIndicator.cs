using UnityEngine.UI;
using UnityEngine;

namespace Zeke.Abilities.Indicators
{
    public class AttackIndicator : MonoBehaviour
    {
        [SerializeField] private Transform root;

        [Space]

        [SerializeField] private Image fill;
        [SerializeField] private Image back;

        [Space]

        [SerializeField] private StatusBar indicatorBar;

        public void Initialize(Sprite sprite, Vector2 size, Image.FillMethod fillMethod, Color fillColor, Color backgroundColor)
        {
            root.transform.localScale = size;
            fill.sprite = sprite;
            back.sprite = sprite;
            fill.fillMethod = fillMethod;

            fill.color = fillColor;
            back.color = backgroundColor;
        }

        public void UpdateFill(float ratio)
        {
            indicatorBar.UpdateBar(ratio);
        }
    }
}