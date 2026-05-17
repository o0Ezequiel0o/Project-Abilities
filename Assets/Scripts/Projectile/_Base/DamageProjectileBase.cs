using System;
using UnityEngine;
using Zeke.TeamSystem;
using static Damageable;

public abstract class DamageProjectileBase : Projectile
{
    public float Damage { get; protected set; }

    public GameObject SourceUser { get; protected set; }
    public Teams Team { get; private set; }

    protected float armorPenetration = 0f;
    protected float procCoefficient = 1f;

    protected float knockback = 0f;

    public readonly struct DamageData
    {
        public readonly float armorPenetration;
        public readonly float procCoefficient;
        public readonly float damage;

        public DamageData(float damage, float armorPenetration, float procCoefficient)
        {
            this.damage = damage;
            this.armorPenetration = armorPenetration;
            this.procCoefficient = procCoefficient;
        }
    }

    public override void OnRetrievedFromPool()
    {
        base.OnRetrievedFromPool();
        Damage = 0f;

        SourceUser = null;
        Team = Teams.IgnoreTeam;
    }

    public void Launch(Vector3 position, float speed, Vector2 direction, float maxRange, DamageData damageData, float knockback, GameObject source, Teams team)
    {
        Team = team;
        SourceUser = source;
        Damage = damageData.damage;
        armorPenetration = damageData.armorPenetration;
        procCoefficient = damageData.procCoefficient;

        this.knockback = knockback;

        Launch(position, speed, direction, maxRange);
        OnLaunch(position, speed, direction, maxRange, damageData, knockback, source, team);
    }

    public virtual void OnLaunch(Vector3 position, float speed, Vector2 direction, float maxRange, DamageData damageData, float knockback, GameObject source, Teams team) { }

    protected void DealDamage(GameObject receiver)
    {
        DealDamage(receiver, OnDamageDealt);
    }

    protected void DealDamage(GameObject receiver, Action<DamageEvent> callback)
    {
        if (receiver.TryGetComponent(out Damageable damageable))
        {
            DamageInfo damageInfo = new DamageInfo(Damage, armorPenetration, procCoefficient)
            {
                direction = Direction,
            };

            damageable.DealDamage(damageInfo, SourceUser, gameObject, callback);
        }
    }

    private void OnDamageDealt(DamageEvent damageEvent)
    {
        if (!damageEvent.damageRejected)
        {
            ApplyKnockback(damageEvent.Receiver.gameObject, damageEvent.Direction);
        }
    }

    protected void ApplyKnockback(GameObject receiver, Vector2 direction)
    {
        if (knockback != 0f && receiver.TryGetComponent(out Physics physics))
        {
            physics.AddForce(knockback, direction);
        }
    }

    protected Vector2 GetHitDirection(GameObject receiver)
    {
        return (receiver.transform.position - lastPosition).normalized;
    }
}