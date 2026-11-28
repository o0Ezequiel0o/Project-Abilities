using System.Collections.Generic;
using UnityEngine;
using Zeke.Abilities;

[CreateAssetMenu(fileName = "Ambush", menuName = "ScriptableObjects/Passives/Ambush", order = 1)]
public class AmbushSkillData : PassiveData
{
    [field: Space]

    [field: SerializeField] public float TimeToActivate { get; private set; }
    [field: SerializeField] public List<AbilityType> CancelTypes { get; private set; }

    [field: Space]

    [field: SerializeField] public AbilityData AbilityOverride { get; private set; }
    [field: SerializeField] public AbilityType UpgradeType { get; private set; }

    public override IPassive CreatePassive(GameObject source, PassiveController passiveController)
    {
        return new AmbushSkill(source, passiveController, this);
    }
}