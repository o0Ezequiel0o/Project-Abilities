using System;
using UnityEditor;
using UnityEngine;
using Zeke.TeamSystem;

public class AreaSpawnpoint : Spawnpoint
{
    [Header("Bounds")]
    [SerializeField] private Vector2 bounds;

    private float RandomXinBounds => UnityEngine.Random.Range(-bounds.x * .5f, bounds.x * .5f);
    private float RandomYinBounds => UnityEngine.Random.Range(-bounds.y * .5f, bounds.y * .5f);

    protected override void ProcessBlockState(Action<bool> onFinishedProcessing, ContactFilter2D contactFilter)
    {
        Physics2D.OverlapBox(transform.position, bounds, Quaternion.identity.eulerAngles.z, contactFilter, hits);

        for (int i = 0; i < hits.Count; i++)
        {
            if (TeamManager.IsEnemy(team, hits[i].gameObject))
            {
                onFinishedProcessing?.Invoke(true);
            }
        }

        onFinishedProcessing?.Invoke(false);
    }

    public override GameObject Spawn(GameObject prefab)
    {
        return Instantiate(prefab, GetRandomPositionInBounds(), Quaternion.identity);
    }

    Vector3 GetRandomPositionInBounds()
    {
        return transform.position + new Vector3(RandomXinBounds, RandomYinBounds, 0f);
    }

    #if UNITY_EDITOR

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(255f, 0f, 0f);
        Gizmos.DrawWireCube(transform.position, bounds);
        Handles.Label(transform.position, name);
    }

    #endif
}