using UnityEngine;

[CreateAssetMenu(fileName = "AbilityTemplate", menuName = "ScriptableObjects/Abilities/AbilityTemplate", order = 1)]
public class AbilityTemplateData : AbilityData
{
    public override IAbility CreateAbility(GameObject source, AbilityController controller)
    {
        return new AbilityTemplate(source, controller, this, CooldownTime);
    }
}