using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Weak-Point Analyzer", menuName = "ScriptableObjects/Items/Items/WeakPointAnalyzer", order = 1)]
    public class WeakPointAnalyzerItemData : ItemData
    {
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat Chance { get; private set; }
        [field: SerializeField] public float DamageMultiplier { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new WeakPointAnalyzerItem(this, itemHandler, source);
        }
    }
}