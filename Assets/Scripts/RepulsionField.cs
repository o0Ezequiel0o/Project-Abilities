using System.Collections.Generic;
using UnityEngine;

public class RepulsionField : MonoBehaviour, IProjectileTrigger
{
    [SerializeField] private float returnSpeed;
    [SerializeField] private float duration;

    private readonly List<Projectile> projectiles = new List<Projectile>();

    private Collider2D coll;
    private float timer = 0f;

    private void Awake()
    {
        coll = GetComponent<Collider2D>();
        coll.enabled = true;
    }

    public void OnProjectileEnter(Projectile projectile)
    {
        projectile.Speed = 0f;
        projectiles.Add(projectile);
    }

    public void OnProjectileExit(Projectile projectile)
    {
        projectiles.Remove(projectile);
    }

    public void Update()
    {
        timer += Time.deltaTime;

        if (timer > duration)
        {
            for (int i = 0; i < projectiles.Count; ++i)
            {
                Projectile projectile = projectiles[i];

                projectile.Launch(projectile.transform.position, returnSpeed, -projectile.Direction, projectile.MaxRange, projectile.Damage);
            }

            coll.enabled = false;

            Destroy(gameObject);
        }
    }
}