using UnityEditor;
using UnityEngine;

public class AreaSpawnpoint : Spawnpoint
{
    [Header("Bounds")]
    [SerializeField] private Vector2 bounds;

    private float RandomXinBounds => Random.Range(-bounds.x * .5f, bounds.x * .5f);
    private float RandomYinBounds => Random.Range(-bounds.y * .5f, bounds.y * .5f);

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