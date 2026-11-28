using UnityEngine;

[CreateAssetMenu(fileName = "PassiveTemplate", menuName = "ScriptableObjects/Passives/PassiveTemplate", order = 1)]
public class PassiveTemplateData : PassiveData
{
    public override IPassive CreatePassive(GameObject source, PassiveController passiveController)
    {
        return new PassiveTemplate(source, passiveController, this);
    }
}