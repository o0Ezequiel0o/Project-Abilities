using UnityEngine;

public class EntityTypeIdentifier : MonoBehaviour
{
    [SerializeField] private EntityType type;

    public EntityType Type => type;
}