using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Heartbeat Scanner", menuName = "ScriptableObjects/Items/Items/HeartbeatScanner", order = 1)]
    public class HeartbeatScannerItemData : ItemData
    {
        [field: SerializeField] public float MissingHealthRatioForStack { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public float DamageMultiplierPerStack { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new HeartbeatScannerItem(this, itemHandler, source);
        }
    }
}