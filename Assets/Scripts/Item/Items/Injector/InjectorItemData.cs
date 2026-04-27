using UnityEngine;
using Zeke.Abilities;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Injector", menuName = "ScriptableObjects/Items/Items/Injector", order = 1)]
    public class InjectorItemData : ItemData
    {
        [field: Space]

        [field: SerializeField] public AbilityType AbilityType { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat ExtraChargeSpeed { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new InjectorItem(this, itemHandler, source);
        }
    }
}