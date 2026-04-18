using UnityEngine;

[CreateAssetMenu(fileName = "Glass Shards", menuName = "ScriptableObjects/Items/Items/GlassShards", order = 1)]
public class GlassShardsItemData : ItemData
{
    [field: Space]

    [field: SerializeField] public StatusEffectData StatusEffect { get; private set; }
    [field: SerializeReferenceDropdown, SerializeReference] public IStackStat ProcChance { get; private set; }

    public override Item CreateItem(ItemHandler itemHandler, GameObject source)
    {
        return new GlassShardsItem(this, itemHandler, source);
    }
}