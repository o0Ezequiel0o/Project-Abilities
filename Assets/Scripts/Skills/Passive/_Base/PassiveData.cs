using UnityEngine;

public abstract class PassiveData : ScriptableObject
{
    public abstract IPassive CreatePassive(GameObject source, PassiveController passiveController);
}