using UnityEngine;

public class CollisionDamage : MonoBehaviour, IUpgradable
{
    [SerializeField] private Stat damage;
    [SerializeField] private float armorPenetration = 0f;
    [SerializeField] private float procCoefficient = 0f;

    public void DealDamage(GameObject target)
    {
        if (target.TryGetComponent(out Damageable damageable))
        {
            Vector2 direction = (transform.position - target.transform.position).normalized;
            DamageInfo damageInfo = new DamageInfo(damage.Value, armorPenetration, procCoefficient) { hit = true, direction = direction };

            damageable.DealDamage(damageInfo, gameObject, gameObject);
        }
    }

    public void Upgrade()
    {
        damage.Upgrade();
    }
}