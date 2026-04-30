using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Glass Ball", menuName = "ScriptableObjects/Items/Items/GlassBall", order = 1)]
    public class GlassBallItemData : ItemData
    {
        [field: Space]

        [field: SerializeField] public GameObject ActiveVisual { get; private set; }

        [field: Space]

        [field: SerializeField] public float Cooldown { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat Armor { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new GlassBallItem(this, itemHandler, source);
        }
    }
}