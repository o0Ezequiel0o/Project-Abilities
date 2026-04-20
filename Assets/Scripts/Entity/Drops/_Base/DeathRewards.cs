using UnityEngine;

public class DeathRewards : MonoBehaviour
{
    [SerializeField] private DeathRewardsData deathRewardsData;

    private void Awake()
    {
        if (TryGetComponent(out Damageable damageable))
        {
            damageable.onDeath.Subscribe(OnDeath);
        }
    }

    private void OnDeath(Damageable.DamageEvent damageEvent)
    {
        for (int i = 0; i < deathRewardsData.Rewards.Count; i++)
        {
            deathRewardsData.Rewards[i].DropRewards(gameObject, damageEvent);
        }
    }
}