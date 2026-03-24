using UnityEngine;

[CreateAssetMenu(fileName = "Flungus", menuName = "ScriptableObjects/Items/Items/Flungus", order = 1)]
public class FlungusItemData : ItemData
{
    [field: Space]
    [field: SerializeField] public float HealCooldown { get; private set; }
    [field: SerializeField] public float ActivationDelay { get; private set; }
    [field: SerializeField] public GameObject Particles { get; private set; }
    [field: SerializeField] public LayerMask HitLayers { get; private set; }
    [field: SerializeField] public ItemStat HealAmount { get; private set; }
    [field: SerializeField] public ItemStat Radius { get; private set; }

    public override Item CreateItem(ItemHandler itemHandler, GameObject source)
    {
        return new FlungusItem(this, itemHandler, source);
    }
}