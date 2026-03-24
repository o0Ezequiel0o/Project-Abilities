using UnityEngine;

[CreateAssetMenu(fileName = "Fire Sawmerang", menuName = "ScriptableObjects/Abilities/FireSawmerang", order = 1)]
public class SawmerangSkillData : ProjectileSkillBaseData
{
    [field: SerializeField] public AbilityData AbilityToLock { get; private set; }

    public override IAbility CreateAbility(GameObject source, AbilityController controller)
    {
        return new SawmerangSkill(source, controller, this, CooldownTime);
    }
}