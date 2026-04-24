using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Sniper Scope", menuName = "ScriptableObjects/Items/Items/SniperScope", order = 1)]
    public class SniperScopeItemData : ItemData
    {
        [field: SerializeField] public float BaseDamageMultValue { get; private set; } = 1f;
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat DamageMultIncreasePerMeter { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new SniperScopeItem(this, itemHandler, source);
        }
    }
}