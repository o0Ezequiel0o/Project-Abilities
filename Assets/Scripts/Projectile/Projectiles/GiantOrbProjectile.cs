using System.Collections.Generic;
using UnityEngine;
using Zeke.PoolableGameObjects;
using Zeke.TeamSystem;

public class GiantOrbProjectile : DamageProjectileBase
{
    [Header("Homing Projectiles")]
    [SerializeField] private HomingOrbProjectile homingOrbPrefab;
    [SerializeField] private float fireHomingOrbCooldown = 0.25f;

    [Space]

    [SerializeField] private float findTargetRadius = 10f;
    [SerializeField] private LayerMask findTargetLayer;
    [SerializeField] private LayerMask findTargetBlockLayer;

    private readonly GameObjectPool<HomingOrbProjectile> homingOrbs = new GameObjectPool<HomingOrbProjectile>();

    private float homingOrbSpeed = 0f;
    private float homingOrbDamage = 0f;
    private float homingOrbRange = 0f;
    private int homingOrbPierce = 0;

    private float fireHomingOrbTimer = 0f;

    private readonly List<RaycastHit2D> closeTargets = new List<RaycastHit2D>();

    public void Launch(Vector3 position, float speed, Vector2 direction, float maxRange, float damage, float smallOrbDamage, float smallOrbSpeed, float smallOrbRange, int smallOrbPierce, GameObject source, Teams team)
    {
        homingOrbDamage = smallOrbDamage;
        homingOrbSpeed = smallOrbSpeed;
        homingOrbRange = smallOrbRange;
        homingOrbPierce = smallOrbPierce;

        Launch(position, speed, direction, maxRange, damage, source, team);
    }

    protected override void OnCollision(RaycastHit2D hit)
    {
        GameObject hitObject = hit.transform.gameObject;

        if (hitObject == SourceUser) return;
        if (objectsNotExited.Contains(hitObject)) return;

        Hit(hitObject);
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

    private void Hit(GameObject receiver)
    {
        if (TeamManager.IsAlly(Team, receiver)) return;

        bool damageRejected = DealDamage(receiver);

        if (damageRejected) return;

        ApplyKnockback(receiver, Direction);
    }

    private void FireHomingOrbs()
    {
        Vector2 launchDirection = Vector2.Perpendicular(Direction);

        FireHomingOrb(launchDirection);
        FireHomingOrb(-launchDirection);
    }

    private void FireHomingOrb(Vector2 launchDirection)
    {
        HomingOrbProjectile homingOrb = homingOrbs.Get(homingOrbPrefab);
        Transform target = GetClosestTarget(homingOrb.transform.position, launchDirection);

        homingOrb.Launch(transform.position, homingOrbSpeed, launchDirection, homingOrbRange, homingOrbDamage, homingOrbPierce, target, SourceUser, Team);
        homingOrb.gameObject.SetActive(true);
    }

    private Transform GetClosestTarget(Vector3 position, Vector2 direction)
    {
        closeTargets.Clear();

        ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = findTargetLayer, useLayerMask = true };
        Physics2D.CircleCast(position, findTargetRadius, Vector2.zero, contactFilter, closeTargets, 0f);

        for (int i = 0; i < closeTargets.Count; i++)
        {
            if (closeTargets[i].collider.gameObject == SourceUser) continue;
            if (TeamManager.IsAlly(Team, closeTargets[i].collider.gameObject)) continue;
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