using UnityEngine;

[CreateAssetMenu(fileName = "Magnifying Glass", menuName = "ScriptableObjects/Items/Items/MagnifyingGlass", order = 1)]
public class MagnifyingGlassItemData : ItemData
{
    [field: Space]
    [field: SerializeField] public float MinDistance { get; private set; }
    [field: SerializeReferenceDropdown, SerializeReference] public IStackStat DamageMult { get; private set; }

    public override Item CreateItem(ItemHandler itemHandler, GameObject source)
    {
        return new MagnifyingGlassItem(this, itemHandler, source);
    }
}