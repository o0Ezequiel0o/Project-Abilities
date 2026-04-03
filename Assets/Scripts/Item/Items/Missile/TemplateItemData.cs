using UnityEngine;

[CreateAssetMenu(fileName = "Missile Item", menuName = "ScriptableObjects/Items/Items/MissileItem", order = 1)]
public class MissileItemData : ItemData
{
    [field: SerializeField] public Missile MissilePrefab { get; private set; }
    [field: SerializeField] public float ProcChance { get; private set; }
    [field: SerializeField] public ItemStat DamageMultiplier { get; private set; }

    public override Item CreateItem(ItemHandler itemHandler, GameObject source)
    {
        return new MissileItem(this, itemHandler, source);
    }
}