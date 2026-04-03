using System.Collections.Generic;
using UnityEngine;
using System;

public static class TargetAwareness
{
    private static readonly List<RaycastHit2D> rayHits = new List<RaycastHit2D>(16);
    private static readonly List<Collider2D> hits = new List<Collider2D>(32);

    public static bool HasLineOfSight(Vector3 position, Vector3 endPosition, LayerMask blockLayers)
    {
        rayHits.Clear();

        ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = blockLayers, useLayerMask = true };
        Physics2D.Linecast(position, endPosition, contactFilter, rayHits);

        for (int i = 0; i < rayHits.Count; i++)
        {
            if (rayHits[i].transform.position == position) continue;
            return false;
        }

        return true;
    }

    public static bool HasLineOfSight(Vector3 position, Collider2D targetCollider, LayerMask blockLayers)
    {
        return HasLineOfSight(position, targetCollider, blockLayers, _ => true);
    }

    public static bool HasLineOfSight(Vector3 position, Collider2D targetCollider, LayerMask blockLayers, Predicate<GameObject> filter)
    {
        Vector2 closestPoint = GetClosestTargetPoint(position, targetCollider);
        closestPoint += Mathf.Epsilon * GetTargetDirection(position, closestPoint);

        return HasLineOfSight(position, closestPoint, targetCollider.transform, blockLayers, filter);
    }

    public static bool HasLineOfSight(Vector3 position, Transform target, LayerMask blockLayers)
    {
        return HasLineOfSight(position, target.position, target, blockLayers, _ => true);
    }

    public static bool HasLineOfSight(Vector3 position, Transform target, LayerMask blockLayers, Predicate<GameObject> filter)
    {
        return HasLineOfSight(position, target.position, target, blockLayers, filter);
    }

    private static bool HasLineOfSight(Vector3 position, Vector3 closestTargetPosition, Transform target, LayerMask blockLayers, Predicate<GameObject> filter)
    {
        rayHits.Clear();

        ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = blockLayers, useLayerMask = true };
        Physics2D.Linecast(position, closestTargetPosition, contactFilter, rayHits);

        for (int i = 0; i < rayHits.Count; i++)
        {
            if (rayHits[i].transform.position == position) continue;
            if (!filter(rayHits[i].transform.gameObject)) continue;
            return rayHits[i].transform == target;
        }

        return true;
    }

    public static bool AnyTargetInLineOfSight(Vector3 position, Vector2 direction, float range, LayerMask targetLayers)
    {
        return AnyTargetInLineOfSight(position, direction, range, targetLayers, _ => true);
    }

    public static bool AnyTargetInLineOfSight(Vector3 position, Vector2 direction, float range, LayerMask targetLayers, Predicate<GameObject> filter)
    {
        rayHits.Clear();

        ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = targetLayers, useLayerMask = true };
        Physics2D.Raycast(position, direction, contactFilter, rayHits, range);

        for (int i = 0; i < rayHits.Count; i++)
        {
            if (rayHits[i].transform.position == position) continue;
            return filter(rayHits[i].transform.gameObject);
        }

        return false;
    }

    public static bool TryGetClosestTargetToDirection(Vector3 position, Vector2 direction, float range, LayerMask targetLayers, LayerMask blockLayers, out Transform target)
    {
        target = GetClosestTargetToDirection(position, direction, range, targetLayers, blockLayers);
        return target != null;
    }

    public static bool TryGetClosestTargetToDirection(Vector3 position, Vector2 direction, float range, LayerMask targetLayers, LayerMask blockLayers, Predicate<GameObject> filter, out Transform target)
    {
        target = GetClosestTargetToDirection(position, direction, range, targetLayers, blockLayers, filter);
        return target != null;
    }

    public static Transform GetClosestTargetToDirection(Vector3 position, Vector2 direction, float range, LayerMask targetLayers, LayerMask blockLayers)
    {
        return GetClosestTargetToDirection(position, direction, range, targetLayers, blockLayers, _ => true);
    }

    public static Transform GetClosestTargetToDirection(Vector3 position, Vector2 direction, float range, LayerMask targetLayers, LayerMask blockLayers, Predicate<GameObject> filter)
    {
        hits.Clear();

        Transform closestTarget = null;
        float distance = float.MaxValue;

        ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = targetLayers, useLayerMask = true };
        Physics2D.OverlapCircle(position, range, contactFilter, hits);

        for (int i = 0; i < hits.Count; i++)
        {
            if (position == hits[i].transform.position) continue;
            if (!filter(hits[i].gameObject)) continue;

            Vector2 targetDirection = GetTargetDirection(position, hits[i].transform.position);
            float directionDistance = (direction - targetDirection).magnitude;

            if (directionDistance < distance)
            {
                closestTarget = hits[i].transform;
                distance = directionDistance;
            }
        }

        return closestTarget;
    }

    private static Vector2 GetTargetDirection(Vector2 position, Vector2 targetPosition)
    {
        return (targetPosition - position).normalized;
    }

    private static Vector2 GetClosestTargetPoint(Vector2 position, Collider2D targetCollider)
    {
        return targetCollider.ClosestPoint(position);
    }
}