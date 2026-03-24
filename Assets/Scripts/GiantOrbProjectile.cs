using System.Collections.Generic;
using UnityEngine;

public class GiantOrbProjectile : Projectile
{
    [Header("Basic Projectile Settings")]
    [SerializeField] private float armorPenetration = 0f;
    [SerializeField] private float procCoefficient = 1f;
    [SerializeField] private float knockback = 1f;

    [Header("Homing Projectiles")]
    [SerializeField] private HomingOrb homingOrbPrefab;
    [SerializeField] private float fireHomingOrbCooldown = 0.25f;

    [Space]

    [SerializeField] private float findTargetRadius = 10f;
    [SerializeField] private LayerMask findTargetLayer;
    [SerializeField] private LayerMask findTargetBlockLayer;

    private readonly GameObjectPool<HomingOrb> homingOrbs = new GameObjectPool<HomingOrb>();

    private float homingOrbSpeed = 0f;
    private float homingOrbDamage = 0f;
    private float homingOrbRange = 0f;

    private float fireHomingOrbTimer = 0f;

    private readonly List<RaycastHit2D> closeTargets = new List<RaycastHit2D>();

    public void SetHomingOrbsValues(float damage, float speed, float range)
    {
        homingOrbDamage = damage;
        homingOrbSpeed = speed;
        homingOrbRange = range;
    }

    protected override void OnCollision(RaycastHit2D hit)
    {
        GameObject hitObject = hit.transform.gameObject;

        if (hitObject == SourceUser) return;
        if (objectsNotExited.Contains(hitObject)) return;

        OnImpact(hitObject);
    }

    protected override void Update()
    {
        base.Update();

        fireHomingOrbTimer += Time.deltaTime;

        if (fireHomingOrbTimer > fireHomingOrbCooldown)
        {
            FireHomingOrbs();
            fireHomingOrbTimer = 0f;
        }
    }

    private void OnImpact(GameObject hitObject)
    {
        if (TeamManager.IsAlly(SourceUser, hitObject)) return;

        Hit(hitObject);
    }

    private void Hit(GameObject receiver)
    {
        bool damageRejected = false;

        if (receiver.TryGetComponent(out Damageable damageable))
        {
            Damageable.DamageEvent damageEvent = damageable.DealDamage(new DamageInfo(Damage, procCoefficient, armorPenetration), SourceUser, gameObject);

            damageRejected = damageEvent.damageRejected;
        }

        if (!damageRejected && knockback != 0f && receiver.TryGetComponent(out Physics physics))
        {
            physics.AddForce(knockback, Direction);
        }
    }

    private void FireHomingOrbs()
    {
        Vector2 launchDirection = Vector2.Perpendicular(Direction);

        FireHomingOrb(launchDirection);
        FireHomingOrb(-launchDirection);
    }

    private void FireHomingOrb(Vector2 launchDirection)
    {
        HomingOrb homingOrb = homingOrbs.Get(homingOrbPrefab);

        homingOrb.Launch(transform.position, homingOrbSpeed, launchDirection, homingOrbRange, homingOrbDamage, SourceUser);
        Transform target = GetClosestTarget(homingOrb.transform.position, launchDirection);

        homingOrb.SetTarget(target);
        homingOrb.EnableCollider();

        homingOrb.gameObject.SetActive(true);
    }

    private Transform GetClosestTarget(Vector3 position, Vector2 direction)
    {
        closeTargets.Clear();

        ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = findTargetLayer };
        Physics2D.CircleCast(position, findTargetRadius, Vector2.zero, contactFilter, closeTargets, 0f);

        for (int i = 0; i < closeTargets.Count; i++)
        {
            if (closeTargets[i].collider.gameObject == SourceUser) continue;
            if (TeamManager.IsAlly(SourceUser, closeTargets[i].collider.gameObject)) continue;
            if (Physics2D.CircleCast(position, Radius, direction, Vector3.Distance(position, closeTargets[i].transform.position), findTargetBlockLayer)) continue;

            return closeTargets[i].transform;
        }

        return null;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        homingOrbs.Clear();
    }
}