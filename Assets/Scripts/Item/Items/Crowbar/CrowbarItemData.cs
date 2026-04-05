using UnityEngine;

[CreateAssetMenu(fileName = "Crowbar", menuName = "ScriptableObjects/Items/Items/Crowbar", order = 1)]
public class CrowbarItemData : ItemData
{
    [field: Space]
    [field: SerializeField] public float HealthThreshold { get; private set; }
    [field: SerializeField] public StackStat DamageMultiplier { get; private set; }

    public override Item CreateItem(ItemHandler itemHandler, GameObject source)
    {
        return new CrowbarItem(this, itemHandler, source);
    }
}