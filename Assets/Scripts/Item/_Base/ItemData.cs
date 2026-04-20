using UnityEngine;

namespace Zeke.Items
{
    public abstract class ItemData : ScriptableObject
    {
        [Space]

        [SerializeField] private ItemSettings settings;
        [SerializeField] private ItemDrop worldPrefab;

        [field: Space]

        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public Sprite Outline { get; private set; }

        [field: Space]

        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField, TextArea(3, 10)] public string Description { get; private set; }
        [field: SerializeField] public ItemRarity Rarity { get; private set; }

        public abstract Item CreateItem(ItemHandler itemHandler, GameObject source);

        public int TriggerOrder => settings.GetTriggerOrder(this);

        public GameObject CreateInWorld(Vector3 position, Quaternion rotation)
        {
            if (worldPrefab == null)
            {
                Debug.LogError("ItemData does not have a world prefab", this);
            }

            ItemDrop itemDrop = Instantiate(worldPrefab, position, rotation);
            itemDrop.LoadItemData(this);
            itemDrop.name = Name;

            return itemDrop.gameObject;
        }
    }
}