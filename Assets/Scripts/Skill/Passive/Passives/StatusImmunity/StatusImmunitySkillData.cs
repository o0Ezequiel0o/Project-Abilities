using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Status Immunity", menuName = "ScriptableObjects/Passives/StatusImmunity", order = 1)]
public class StatusImmunitySkillData : PassiveData
{
    [field: SerializeField] public List<StatusEffectData> Immunities { get; private set; }

    public override IPassive CreatePassive(GameObject source, PassiveController passiveController)
    {
        return new StatusImmunitySkill(source, passiveController, this);
    }
}