using System.Collections.Generic;
using UnityEngine;
using Zeke.Abilities.Modules;

[CreateAssetMenu(fileName = "Modular Ability", menuName = "ScriptableObjects/Test/Modular Ability", order = 1)]
public class ModularAbilityData : ScriptableObject
{
    [field: Header("Display")]
    [field: SerializeField] public Sprite Icon { get; private set; }

    [field: Header("Toggling")]
    [field: SerializeField] public AbilityType AbilityType { get; private set; }
    [field: SerializeField] public bool ManualDeactivation { get; private set; }
    [field: SerializeField] public bool CanHold { get; private set; }

    [Header("Casting")]
    [SerializeField] private Stat cooldownTime = new Stat(5f, 0f, 0f, float.PositiveInfinity);
    [SerializeField] private Stat duration = Stat.Zero;
    [SerializeField] private Stat charges = new Stat(1f, 0f, 1f, float.PositiveInfinity);

    [Header("Modules")]
    [SerializeReferenceDropdown, SerializeReference] private List<AbilityModule> modules = new List<AbilityModule> { new DeltaTimeCooldown() };

    public ModularAbility CreateModularAbility(ModularAbilityController controller, Transform spawn, GameObject source)
    {
        ModularAbility modularAbility = new ModularAbility(source, this, controller, spawn, cooldownTime, duration, charges);
        modularAbility.AddModules(modules);
        return modularAbility;
    }
}