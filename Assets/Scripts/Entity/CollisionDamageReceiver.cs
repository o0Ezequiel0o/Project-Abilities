using System.Collections.Generic;
using UnityEngine;
using Zeke.TeamSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class CollisionDamageReceiver : MonoBehaviour
{
    [Header("Dependency")]
    [SerializeField] private Collider2D collider;

    [Header("Settings")]
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float interval = 0.2f;

    private readonly List<Collider2D> hits = new List<Collider2D>(32);

    private float damageTimer;

    private void Reset()
    {
        collider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        damageTimer += Time.deltaTime;

        if (damageTimer >= interval)
        {
            damageTimer = 0f;
            CheckCollisions();
        }
    }

    private void CheckCollisions()
    {
        ContactFilter2D contactFilter = new ContactFilter2D() { useLayerMask = true, layerMask = layerMask };

        int amount = collider.Overlap(contactFilter, hits);

        for (int i = 0; i < amount; i++)
        {
            GameObject hit = hits[i].gameObject;

            if (TeamManager.IsEnemy(gameObject, hit) && hit.TryGetComponent(out CollisionDamage collisionDamage))
            {
                collisionDamage.DealDamage(gameObject);
            }
        }
    }
}