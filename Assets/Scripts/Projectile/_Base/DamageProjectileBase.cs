using UnityEngine;
using Zeke.TeamSystem;

public abstract class DamageProjectileBase : Projectile
{
    [Header("Impact")]
    [SerializeField] protected float armorPenetration = 0f;
    [SerializeField] protected float procCoefficient = 1f;
    [SerializeField] protected float knockback = 1f;

    public float Damage { get; private set; }

    public GameObject SourceUser { get; protected set; }
    public Teams Team { get; private set; }

    public override void OnRetrievedFromPool()
    {
        base.OnRetrievedFromPool();
        Damage = 0f;

        SourceUser = null;
        Team = Teams.IgnoreTeam;
    }

    public void Launch(Vector3 position, float speed, Vector2 direction, float maxRange, float damage, GameObject source, Teams team)
    {
        Team = team;
        Damage = damage;
        SourceUser = source;
        Launch(position, speed, direction, maxRange);
        OnLaunch(position, speed, direction, maxRange, damage, source, team);
    }

    public virtual void OnLaunch(Vector3 position, float speed, Vector2 direction, float maxRange, float damage, GameObject source, Teams team) { }

    protected bool DealDamage(GameObject receiver)
    {
        if (receiver.TryGetComponent(out Damageable damageable))
        {
            Damageable.DamageEvent damageEvent = damageable.DealDamage(new DamageInfo(Damage, armorPenetration, procCoefficient), SourceUser, gameObject);
            return damageEvent.damageRejected;
        }

        return true;
    }

    protected void ApplyKnockback(GameObject receiver, Vector2 direction)
    {
        if (knockback != 0f && receiver.TryGetComponent(out Physics physics))
        {
            physics.AddForce(knockback, direction);
        }
    }
}