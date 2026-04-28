using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Dead Man's Switch", menuName = "ScriptableObjects/Items/Items/DeadMansSwitch", order = 1)]
    public class DeadMansSwitchItemData : ItemData
    {
        [field: Space]

        [field: SerializeField] public float ArmorPenetration { get; private set; } = 0f;
        [field: SerializeField] public float ProcCoefficient { get; private set; } = 1f;
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat MinDamage { get; private set; }

        [field: Space]

        [field: SerializeField] public LayerMask HitLayers { get; private set; }
        [field: SerializeField] public LayerMask BlockLayers { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat Radius { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new DeadMansSwitchItem(this, itemHandler, source);
        }
    }
}