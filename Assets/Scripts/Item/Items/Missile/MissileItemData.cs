using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Missile Item", menuName = "ScriptableObjects/Items/Items/MissileItem", order = 1)]
    public class MissileItemData : ItemData
    {
        [field: Header("Missile")]

        [field: SerializeField] public MissileItemProjectile MissilePrefab { get; private set; }
        [field: SerializeField, Min(0f)] public float ProcChance { get; private set; }
        [field: SerializeField] public Vector2 SpawnDirection { get; private set; }

        [field: Header("Stats")]

        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat DamageMult { get; private set; }
        [field: SerializeField] public float MaxRange { get; private set; }
        [field: SerializeField] public float Speed { get; private set; }

        [field: Space]

        [field: SerializeField] public float ArmorPenetration { get; private set; } = 0f;
        [field: SerializeField] public float ProcCoefficient { get; private set; } = 1f;
        [field: SerializeField] public float Knockback { get; private set; } = 1f;

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new MissileItem(this, itemHandler, source);
        }
    }
}