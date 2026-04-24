using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Copper Wires", menuName = "ScriptableObjects/Items/Items/CopperWires", order = 1)]
    public class CopperWiresItemData : ItemData
    {
        [field: Space]

        [field: SerializeField] public float ProcChance { get; private set; }
        [field: SerializeField] public float ProcCoefficient { get; private set; }
        [field: SerializeField] public float ArmorPenetration { get; private set; }

        [field: Space]

        [field: SerializeField] public LayerMask HitLayers { get; private set; }
        [field: SerializeField] public LayerMask BlockLayers { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat MaxTargets { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat Radius { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new CopperWiresItem(this, itemHandler, source);
        }
    }
}