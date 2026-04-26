using UnityEditor;
using UnityEngine;
using Zeke.TeamSystem;

public class CircleSpawnpoint : Spawnpoint
{
    [Header("Bounds")]
    [SerializeField] private float radius;

    private Vector3 RandomDirection => new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    private float RandomDistanceFromCenter => Random.Range(0f, radius);

    protected override bool IsBlocked(ContactFilter2D contactFilter)
    {
        Physics2D.OverlapCircle(transform.position, radius, contactFilter, hits);

        for (int i = 0; i < hits.Count; i++)
        {
            if (TeamManager.IsEnemy(team, hits[i].gameObject))
            {
                return true;
            }
        }

        return false;
    }

    public override GameObject Spawn(GameObject prefab)
    {
        return Instantiate(prefab, GetRandomPositionInBounds(), Quaternion.identity);
    }

    Vector3 GetRandomPositionInBounds()
    {
        return transform.position + RandomDirection * RandomDistanceFromCenter;
    }

    #if UNITY_EDITOR

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(255f, 0f, 0f);
        Gizmos.DrawWireSphere(transform.position, radius);
        Handles.Label(transform.position, name);
    }

    #endif
}