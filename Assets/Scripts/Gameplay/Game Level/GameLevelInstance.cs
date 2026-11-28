using UnityEngine;

public class GameLevelInstance : MonoBehaviour
{
    [field: SerializeField] public Collider2D BoundsCollider { get; private set; }
}