using UnityEditor;
using UnityEngine;

public class CircleSpawnpoint : Spawnpoint
{
    [Header("Bounds")]
    [SerializeField] private float radius;

    private Vector3 RandomDirection => new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    private float RandomDistanceFromCenter => Random.Range(0f, radius);

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