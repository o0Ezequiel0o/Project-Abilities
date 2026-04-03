using System.Collections.Generic;
using UnityEngine;
using System;

public static class Avoidance
{
    private static readonly List<Vector2> targetDirections = new List<Vector2>(12);
    private static readonly List<Collider2D> hits = new List<Collider2D>(64);

    public static bool TryGetAvoidanceDirection(Vector2 position, float radius, LayerMask layerMask, out Vector2 direction)
    {
        return TryGetAvoidanceDirection(position, radius, layerMask, _ => true, out direction);
    }

    public static bool TryGetAvoidanceDirection(Vector2 position, float radius, LayerMask layerMask, Predicate<GameObject> filter, out Vector2 direction)
    {
        direction = GetAvoidanceDirection(position, radius, layerMask, filter);
        return direction != Vector2.zero;
    }

    public static Vector2 GetAvoidanceDirection(Vector2 position, float radius, LayerMask layerMask)
    {
        return GetAvoidanceDirection(position, radius, layerMask, _ => true);
    }

    public static Vector2 GetAvoidanceDirection(Vector2 position, float radius, LayerMask layerMask, Predicate<GameObject> filter)
    {
        UpdateTargetsDirections(position, radius, layerMask, filter);

        if (targetDirections.Count == 0)
        {
            return Vector2.zero;
        }
        else if (targetDirections.Count == 1)
        {
            return -targetDirections[0];
        }

        targetDirections.Sort((v1, v2) => OrderVectorsClockwise(v1, v2));

        float biggestAngle = float.NegativeInfinity;
        float biggestAngleStartAngle = float.NegativeInfinity;

        for (int i = 0; i < targetDirections.Count; i++)
        {
            int nextIndex = (i + 1) % targetDirections.Count;

            float angle = GetClockwiseAngle(targetDirections[i], targetDirections[nextIndex]);
            float startAngle = GetClockwiseAngle(Vector2.up, targetDirections[i]);

            if (angle > biggestAngle)
            {
                biggestAngle = angle;
                biggestAngleStartAngle = startAngle;
            }

            if (i == targetDirections.Count - 1) break;
        }

        float bestMiddleAngle = biggestAngleStartAngle + (biggestAngle * 0.5f);
        Vector2 bestAvoidanceDirection = AngleToDirection(bestMiddleAngle);

        return bestAvoidanceDirection;
    }

    private static void UpdateTargetsDirections(Vector2 position, float radius, LayerMask layerMask, Predicate<GameObject> filter)
    {
        hits.Clear();
        targetDirections.Clear();

        ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = layerMask, useLayerMask = true };
        Physics2D.OverlapCircle(position, radius, contactFilter, hits);

        for (int i = 0; i < hits.Count; i++)
        {
            if ((Vector2)hits[i].transform.position == position) continue;
            if (!filter(hits[i].gameObject)) continue;
            targetDirections.Add(GetDirection(hits[i].transform.position, position));
        }
    }

    private static float GetClockwiseAngle(Vector2 d1, Vector2 d2)
    {
        float angle = -Vector2.SignedAngle(d1, d2);
        if (angle < 0) angle += 360;
        return angle;
    }

    private static Vector2 GetDirection(Vector3 v1, Vector3 v2)
    {
        return (v1 - v2).normalized;
    }

    private static Vector2 AngleToDirection(float angle)
    {
        float radians = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Sin(radians), Mathf.Cos(radians)).normalized;
    }

    private static int OrderVectorsClockwise(Vector2 v1, Vector2 v2)
    {
        if (v1.x >= 0 && v2.x >= 0)
        {
            return v2.y.CompareTo(v1.y);
        }
        else if (v1.x < 0 && v2.x < 0)
        {
            return v1.y.CompareTo(v2.y);
        }
        else if (v1.x >= 0 && v2.x < 0)
        {
            return -1;
        }
        else
        {
            return 1;
        }
    }
}