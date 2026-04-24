using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Sniper Scope", menuName = "ScriptableObjects/Items/Items/SniperScope", order = 1)]
    public class SniperScopeItemData : ItemData
    {
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat DamageMultPerMeter { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new SniperScopeItem(this, itemHandler, source);
        }
    }
}