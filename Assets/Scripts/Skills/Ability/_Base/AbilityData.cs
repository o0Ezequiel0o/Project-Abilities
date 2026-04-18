using System.Collections.Generic;
using UnityEngine;
using Zeke.Abilities.Indicators;
using Zeke.Abilities.Modules;

namespace Zeke.Abilities
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "ScriptableObjects/Ability/Create Ability", order = 1)]
    public class AbilityData : ScriptableObject
    {
        [field: Header("Display")]
        [field: SerializeField] public Sprite Icon { get; private set; }

        [field: Header("Toggling")]
        [field: SerializeField] public AbilityType AbilityType { get; private set; }
        [field: SerializeField] public bool CanManuallyDeactivate { get; private set; }
        [field: SerializeField] public bool CanHold { get; private set; }

        [Header("Casting")]
        [SerializeField] private Stat cooldownTime = new Stat(5f, 0f, 0f, float.PositiveInfinity);
        [SerializeField] private Stat duration = new Stat(0f, 0f, 0f, float.PositiveInfinity);
        [SerializeField] private Stat charges = new Stat(1f, 0f, 1f, float.PositiveInfinity);

        [Header("Modules")]
        [SerializeReferenceDropdown, SerializeReference]
        private List<AbilityModule> modules = new List<AbilityModule>
        {
            new Recharge(),
            new BaseCastCooldown()
        };

        [field: Header("Optional - AI")]
        [field: SerializeField] public AbilityIndicatorData IndicatorData { get; private set; }

        public Ability CreateModularAbility(AbilityController controller, Transform spawn, GameObject source)
        {
            Ability modularAbility = new Ability(source, this, controller, spawn, cooldownTime, duration, charges);
            modularAbility.AddModules(modules);
            return modularAbility;
        }
    }

    public enum AbilityType
    {
        Primary,
        Secondary,
        Utility,
        Ultimate
    }
}