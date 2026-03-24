using UnityEngine;

[CreateAssetMenu(fileName = "Fire Boomerang", menuName = "ScriptableObjects/Abilities/FireBoomerang", order = 1)]
public class BoomerangSkillData : ProjectileSkillBaseData
{
    [SerializeField] private Stat maxBoomerangs;

    private Stat MaxBoomerangs => maxBoomerangs.DeepCopy();

    public override IAbility CreateAbility(GameObject source, AbilityController controller)
    {
        return new BoomerangSkill(source, controller, this, CooldownTime, MaxBoomerangs);
    }
}