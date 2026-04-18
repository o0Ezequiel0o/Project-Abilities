using UnityEngine;

[CreateAssetMenu(fileName = "Gasoline", menuName = "ScriptableObjects/Items/Items/Gasoline", order = 1)]
public class GasolineItemData : ItemData
{
    [field: Space]
    [field: SerializeReferenceDropdown, SerializeReference] public IStackStat Radius { get; private set; }
    [field: SerializeField] public StatusEffectData StatusEffectToApply { get; private set; }
    [field: SerializeField] public LayerMask HitLayers { get; private set; }

    public override Item CreateItem(ItemHandler itemHandler, GameObject source)
    {
        return new GasolineItem(this, itemHandler, source);
    }
}