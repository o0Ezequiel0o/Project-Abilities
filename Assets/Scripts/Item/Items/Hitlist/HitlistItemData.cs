using System.Collections.Generic;
using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Hitlist", menuName = "ScriptableObjects/Items/Items/Hitlist", order = 1)]
    public class HitlistItemData : ItemData
    {
        [field: Space]

        [field: SerializeField] public GameObject TargetVisual { get; private set; }

        [field: Space]

        [field: SerializeField] public float SearchRadius { get; private set; }
        [field: SerializeField] public LayerMask TargetLayers { get; private set; }
        [field: SerializeField] public List<EntityType> ValidTypes { get; private set; }

        [field: Space]

        [field: SerializeField] public float Duration { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat DamageMultiplier { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new HitlistItem(this, itemHandler, source);
        }
    }
}