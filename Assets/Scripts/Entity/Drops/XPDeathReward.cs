using UnityEngine;
using System;

[Serializable]
public class XPDeathReward : DeathReward
{
    [SerializeField] private int experience;

    public override void DropRewards(GameObject source, Damageable.DamageEvent deathEvent)
    {
        if (deathEvent.SourceUser == null) return;

        if (deathEvent.SourceUser.TryGetComponent(out LevelHandler levelHandler))
        {
            levelHandler.GiveExperience(experience);
        }
    }
}