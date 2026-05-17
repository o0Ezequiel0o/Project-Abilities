using System.Collections.Generic;
using UnityEngine;
using Zeke.PoolableGameObjects;
using Zeke.TeamSystem;

public class GiantOrbProjectile : DamageProjectileBase
{
    [SerializeField] protected bool allyCollision;

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

    public void Launch(Vector3 position, float speed, Vector2 direction, float maxRange, DamageData damageData, float knockback, float smallOrbDamage, float smallOrbSpeed, float smallOrbRange, int smallOrbPierce, GameObject source, Teams team)
    {
        homingOrbDamage = smallOrbDamage;
        homingOrbSpeed = smallOrbSpeed;
        homingOrbRange = smallOrbRange;
        homingOrbPierce = smallOrbPierce;

        Launch(position, speed, direction, maxRange, damageData, knockback, source, team);
    }

    protected override void OnCollision(RaycastHit2D hit)
    {
        GameObject receiver = hit.transform.gameObject;

        if (receiver == SourceUser) return;
        if (objectsNotExited.Contains(receiver)) return;

        Hit(receiver);
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

        DealDamage(receiver);
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

        DamageData homingOrbDamageData = new DamageData(homingOrbDamage, armorPenetration, procCoefficient);
        homingOrb.Launch(transform.position, homingOrbSpeed, launchDirection, homingOrbRange, homingOrbDamageData, knockback, homingOrbPierce, target, SourceUser, Team);
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