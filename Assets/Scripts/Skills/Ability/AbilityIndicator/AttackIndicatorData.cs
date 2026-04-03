using UnityEngine.UI;
using UnityEngine;
using System;

namespace Zeke.Abilities.Indicators
{
    [Serializable]
    public class AttackIndicatorData
    {
        [SerializeField] private Sprite sprite;
        [SerializeField] private Vector2 size = Vector2.one;
        [SerializeField] private Image.FillMethod fillMethod;

        public AttackIndicatorData(AttackIndicatorData original)
        {
            sprite = original.sprite;
            size = original.size;
            fillMethod = original.fillMethod;
        }

        public AttackIndicatorData DeepCopy() => new AttackIndicatorData(this);

        public AttackIndicator CreateInstance(AbilityIndicatorSettings settings)
        {
            AttackIndicator instance = UnityEngine.Object.Instantiate(settings.Prefab);
            instance.Initialize(sprite, size, fillMethod, settings.FillColor, settings.BackgroundColor);
            return instance;
        }
    }
}