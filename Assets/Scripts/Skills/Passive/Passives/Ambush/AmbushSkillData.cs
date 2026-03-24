using UnityEngine;

[CreateAssetMenu(fileName = "Ambush", menuName = "ScriptableObjects/Passives/Ambush", order = 1)]
public class AmbushSkillData : PassiveData
{
    [field: SerializeField] public float DamageMultiplier { get; private set; }
    [field: SerializeField] public float TimeToActivate { get; private set; }

    public override IPassive CreatePassive(GameObject source, PassiveController passiveController)
    {
        return new AmbushSkill(source, passiveController, this);
    }
}