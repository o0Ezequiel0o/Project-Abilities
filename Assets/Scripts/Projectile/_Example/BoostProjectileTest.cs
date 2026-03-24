using System.Collections.Generic;
using UnityEngine;

public class BoostProjectileTest : MonoBehaviour, IProjectileTrigger
{
    [SerializeField] private float speedMultiplier = 1f;

    private readonly Dictionary<Projectile, float> projectilesSpeedChangeAmount = new Dictionary<Projectile, float>();

    public void OnProjectileEnter(Projectile projectile)
    {
        projectile.onDespawn += RemoveProjectile;

        float oldSpeed = projectile.Speed;
        projectile.Speed *= speedMultiplier;

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
}