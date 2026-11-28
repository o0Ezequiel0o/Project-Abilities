using UnityEngine;

[CreateAssetMenu(fileName = "Damageable Settings", menuName = "Damageable Settings", order = 1)]
public class DamageableSettings : ScriptableObject
{
    [field: SerializeField] public float RegenInterval { get; private set; } = 1f;
}