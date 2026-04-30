using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Magnetic Boots", menuName = "ScriptableObjects/Items/Items/MagneticBoots", order = 1)]
    public class MagneticBootsItemData : ItemData
    {
        [field: Space]

        [field: SerializeField] public HomingOrbProjectile Prefab { get; private set; }
        [field: SerializeField] public float DistanceRequired { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat OrbDamage { get; private set; }

        [field: Space]

        [field: SerializeField] public int OrbPierce { get; private set; }
        [field: SerializeField] public float OrbSpeed { get; private set; }
        [field: SerializeField] public float OrbRange { get; private set; }

        [field: Space]
        [field: SerializeField] public LayerMask TargetLayers { get; private set; }
        [field: SerializeField] public float FindTargetRange { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new MagneticBootsItem(this, itemHandler, source);
        }
    }
}