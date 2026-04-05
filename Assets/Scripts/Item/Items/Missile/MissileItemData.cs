using UnityEngine;

[CreateAssetMenu(fileName = "Missile Item", menuName = "ScriptableObjects/Items/Items/MissileItem", order = 1)]
public class MissileItemData : ItemData
{
    [field: Header("Missile")]

    [field: SerializeField] public MissileItemProjectile MissilePrefab { get; private set; }
    [field: SerializeField, Min(0f)] public float ProcChance { get; private set; }
    [field: SerializeField] public Vector2 SpawnDirection { get; private set; }

    [field: Header("Stats")]

    [field: SerializeField] public StackStat DamageMultiplier { get; private set; }
    [field: SerializeField] public float MaxRange { get; private set; }
    [field: SerializeField] public float Speed { get; private set; }

    public override Item CreateItem(ItemHandler itemHandler, GameObject source)
    {
        return new MissileItem(this, itemHandler, source);
    }
}