using UnityEngine;
using System;

[Serializable]
public abstract class DeathReward
{
    public abstract void DropRewards(GameObject source, Damageable.DamageEvent deathEvent);
}