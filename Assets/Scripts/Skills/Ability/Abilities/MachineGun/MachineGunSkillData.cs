using UnityEngine;

[CreateAssetMenu(fileName = "Machine Gun", menuName = "ScriptableObjects/Abilities/MachineGun", order = 1)]
public class MachineGunSkillData : GunSkillBaseData
{
    public override IAbility CreateAbility(GameObject source, AbilityController controller)
    {
        return new MachineGunSkill(source, controller, this, CooldownTime);
    }
}