using UnityEngine;

[CreateAssetMenu(fileName = "Mega Fireball", menuName = "ScriptableObjects/Abilities/MegaFireball", order = 1)]
public class MegaFireballSkillData : ProjectileSkillBaseData
{
    [field: Header("Mega Fireball Stats")]
    [field: SerializeField] public int FireballsAmount { get; private set; }
    [SerializeField] private Stat damageRadius;

    private Stat DamageRadius => damageRadius.DeepCopy();

    public override IAbility CreateAbility(GameObject source, AbilityController controller)
    {
        return new MegaFireballSkill(source, controller, this, CooldownTime, DamageRadius);
    }
}