using System.Collections.Generic;
using UnityEngine;

public class EntityTypeIdentifier : MonoBehaviour
{
    private static readonly Dictionary<GameObject, EntityTypeIdentifier> entityTypeCache = new Dictionary<GameObject, EntityTypeIdentifier>();

    [SerializeField] private EntityType type;

    public EntityType Type => type;

    public static EntityType GetEntityType(GameObject gameObject)
    {
        if (entityTypeCache.TryGetValue(gameObject, out EntityTypeIdentifier entityTypeIdentifier))
        {
            return entityTypeIdentifier.Type;
        }
        else
        {
            return EntityType.Normal;
        }
    }

    private void OnEnable()
    {
        entityTypeCache.Add(gameObject, this);
    }

    private void OnDisable()
    {
        entityTypeCache.Remove(gameObject);
    }
}