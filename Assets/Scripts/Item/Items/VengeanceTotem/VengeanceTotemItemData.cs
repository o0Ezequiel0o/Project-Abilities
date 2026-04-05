using UnityEngine;

[CreateAssetMenu(fileName = "Vengeance Totem", menuName = "ScriptableObjects/Items/Items/VengeanceTotem", order = 1)]
public class VengeanceTotemItemData : ItemData
{
    [field: SerializeField] public StackStat Damage { get; private set; }
    [field: SerializeField] public float ProcCoefficient { get; private set; }
    [field: SerializeField] public float ArmorPenetration { get; private set; }

    public override Item CreateItem(ItemHandler itemHandler, GameObject source)
    {
        return new VengeanceTotemItem(this, itemHandler, source);
    }
}