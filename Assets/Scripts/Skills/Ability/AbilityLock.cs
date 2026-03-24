using System;
using UnityEngine;

[Serializable]
public class AbilityLock
{
    [SerializeField] public AbilityType abilityType;

    public AbilityLock(AbilityType abilityType)
    {
        this.abilityType = abilityType;
    }

    public AbilityLock() { }
}