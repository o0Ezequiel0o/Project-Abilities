using UnityEngine;

public class GoldDeathReward : DeathReward
{
    [SerializeField] private int gold;

    public override void DropRewards(GameObject source, Damageable.DamageEvent deathEvent)
    {
        if (deathEvent.SourceUser == null) return;

        if (deathEvent.SourceUser.TryGetComponent(out MoneyHandler wallet))
        {
            wallet.GiveMoney(Mathf.FloorToInt(gold * GameInstance.GoldMultiplier));
        }
    }
}