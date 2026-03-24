using UnityEngine;

[CreateAssetMenu(fileName = "Spawn Engineer Turret", menuName = "ScriptableObjects/Abilities/SpawnEngineerTurret", order = 1)]
public class SpawnEngineerTurretSkillData : SummonSkillBaseData
{
    public override IAbility CreateAbility(GameObject source, AbilityController controller)
    {
        return new SpawnEngineerTurretSkill(source, controller, this, CooldownTime, MaxSummons);
    }
}