using System.Collections.Generic;
using UnityEngine;

public class BoostShieldTest : MonoBehaviour, IProjectileTrigger
{
    [SerializeField] private float slowSpeedMultiplier = 1f;
    [SerializeField] private float boostSpeedMultiplier = 1f;

    private readonly Dictionary<Projectile, float> projectilesSpeedChangeAmount = new Dictionary<Projectile, float>();

    public void OnProjectileEnter(Projectile projectile)
    {
        float oldSpeed = projectile.Speed;

        if (ProjectileEnteredShield(projectile))
        {
            projectile.Speed *= boostSpeedMultiplier;
        }
        else
        {
            projectile.Speed *= slowSpeedMultiplier;
        }


        projectile.onDespawn += RemoveProjectile;
        projectilesSpeedChangeAmount.Add(projectile, projectile.Speed - oldSpeed);
    }

    public void OnProjectileExit(Projectile projectile)
    {
        projectile.onDespawn -= RemoveProjectile;

        projectile.Speed -= projectilesSpeedChangeAmount[projectile];
        projectilesSpeedChangeAmount.Remove(projectile);
    }

    private void RemoveProjectile(Projectile projectile)
    {
        projectilesSpeedChangeAmount.Remove(projectile);
    }

    private bool ProjectileEnteredShield(Projectile projectile)
    {
        float enterDirection = Mathf.Sign((transform.position - projectile.PreviousPosition).normalized.y);
        float shieldDirection = Mathf.Sign(transform.up.y);

        return enterDirection == shieldDirection;
    }
}