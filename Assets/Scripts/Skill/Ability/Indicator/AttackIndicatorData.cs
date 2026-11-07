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

        public AttackIndicator CreateInstance(AbilityIndicatorSettings settings)
        {
            AttackIndicator instance = UnityEngine.Object.Instantiate(settings.Prefab);
            instance.Initialize(sprite, size, fillMethod, settings.FillColor, settings.BackgroundColor);
            return instance;
        }
    }
}