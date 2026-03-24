using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public Sprite Outline { get; private set; }
    [field: Space]
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField, TextArea(3, 10)] public string Description { get; private set; }
    [field: SerializeField] public ItemRarity Rarity { get; private set; }

    public abstract Item CreateItem(ItemHandler itemHandler, GameObject source);
}