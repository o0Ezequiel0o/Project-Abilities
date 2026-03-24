using UnityEngine;

[CreateAssetMenu(fileName = "Machine Guns", menuName = "ScriptableObjects/Abilities/Machine Guns", order = 1)]
public class MachineGunsSkillData : ProjectileSkillBaseData
{
    [field: Header("Visual")]
    [field: SerializeField] public GameObject MachineGunsPrefab { get; private set; }
    [field: SerializeField] public float DistanceFromCenter { get; private set; }
    [field: SerializeField] public float AngleOffset { get; private set; }

    public override IAbility CreateAbility(GameObject source, AbilityController controller)
    {
        return new MachineGunsSkill(source, controller, this, CooldownTime);
    }
}