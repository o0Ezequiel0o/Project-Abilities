using UnityEngine;

public class DropRewardsOnDeath : MonoBehaviour
{
    [SerializeField] private int experience;
    [SerializeField] private int money;

    void Awake()
    {
        if (TryGetComponent(out Damageable damageable))
        {
            damageable.onDeath += DropRewards;
        }
    }

    void DropRewards(Damageable.DamageEvent damageEvent)
    {
        if (damageEvent.Receiver.TryGetComponent(out Player player))
        {
            player.GiveMoney(money);
        }

        if (damageEvent.Receiver.TryGetComponent(out LevelHandler levelHandler))
        {
            levelHandler.GiveExperience(experience);
        }
    }
}