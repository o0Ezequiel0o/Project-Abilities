using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Bloodthirsty Dagger", menuName = "ScriptableObjects/Items/Items/BloodthirstyDagger", order = 1)]
    public class BloodthirstyDaggerItemData : ItemData
    {
        [field: Space]

        [field: SerializeField] public float HealthRatioRequired { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat DamageMultiplier { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new BloodthirstyDaggerItem(this, itemHandler, source);
        }
    }
}