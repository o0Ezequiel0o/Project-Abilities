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
        [field: SerializeField] public string Name { get; private set; }

        [field: SerializeField, TextArea(3, 3)] public string Description { get; private set; } 

        [field: Header("Toggling")]
        [field: SerializeField] public bool CanManuallyDeactivate { get; private set; }
        [field: SerializeField] public bool CanHold { get; private set; }

        [Header("Casting")]
        [SerializeField] private Stat cooldownTime = new Stat(5f, 0f, 0f, float.PositiveInfinity);
        [SerializeField] private Stat duration = new Stat(0f, 0f, 0f, float.PositiveInfinity);
        [SerializeField] private Stat charges = new Stat(1f, 0f, 1f, float.PositiveInfinity);

        [Header("Modules")]
        [SerializeReferenceDropdown, SerializeReference]
        private List<AbilityModuleData> modules = new List<AbilityModuleData>
        {
            new RechargeData(),
            new BaseCastCooldownData()
        };

        [field: Header("Optional - AI")]
        [field: SerializeField] public AbilityIndicatorData IndicatorData { get; private set; }

        public float CooldownTime => cooldownTime.Value;

        public Ability CreateModularAbility(AbilityController controller, Transform spawn, GameObject source, AbilityType type)
        {
            Ability modularAbility = new Ability(source, this, controller, spawn, type, cooldownTime, duration, charges);

            for (int i = 0; i < modules.Count; i++)
            {
                modularAbility.AddModule(modules[i].CreateModule());
            }

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