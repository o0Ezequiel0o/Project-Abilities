using System.Collections.Generic;
using UnityEngine;
using System;

public class ProjectileDetector : MonoBehaviour, IProjectileTrigger
{
    public Action<Projectile> onProjectileEnter;
    public Action<Projectile> onProjectileExit;

    public List<Projectile> Projectiles { get; private set; } = new List<Projectile>();

    public void OnProjectileEnter(Projectile projectile)
    {
        onProjectileEnter?.Invoke(projectile);
        Projectiles.Add(projectile);
    }

    public void OnProjectileExit(Projectile projectile)
    {
        onProjectileExit?.Invoke(projectile);
        Projectiles.Remove(projectile);
    }
}