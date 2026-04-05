using UnityEngine;

[CreateAssetMenu(fileName = "Green Crystal", menuName = "ScriptableObjects/Items/Items/GreenCrystal", order = 1)]
public class GreenCrystalItemData : ItemData
{
    [field: SerializeField] public StackStat ExtraHealth { get; private set; }

    public override Item CreateItem(ItemHandler itemHandler, GameObject source)
    {
        return new GreenCrystalItem(this, itemHandler, source);
    }
}