using System.Collections.Generic;
using UnityEngine;
using Zeke.TeamSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(Physics))]
[RequireComponent(typeof(EntityMove))]
public class EntitySeparation : MonoBehaviour
{
    [SerializeField] private EntityPhysics physics;
    [SerializeField] private EntityMove entityMove;

    [Space]

    [Min(0f), SerializeField] private float separationDistance = 0.6f;
    [Min(0f), SerializeField] private float separationForce = 1f;
    [SerializeField] private LayerMask layers;

    private readonly List<Collider2D> hits = new List<Collider2D>(4);

    private void FixedUpdate()
    {
        if (physics.MoveForces == Vector2.zero) return;
        if (entityMove.MoveDirection == Vector2.zero) return;

        hits.Clear();

        ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = layers, useLayerMask = true };
        Physics2D.OverlapCircle(transform.position, separationDistance, contactFilter, hits);

        Vector3 averageDirection = Vector3.zero;

        for (int i = 0; i < hits.Count; i++)
        {
            if (TeamManager.IsAlly(gameObject, hits[i].gameObject))
            {
                averageDirection += (transform.position - hits[i].transform.position).normalized;
            }
        }

        if (averageDirection != Vector3.zero)
        {
            averageDirection = (averageDirection + (Vector3)physics.MoveForces).normalized;
            physics.AddForce(separationForce, averageDirection);
        }
    }
}