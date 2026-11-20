using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Item Settings", menuName = "ScriptableObjects/Items/ItemSettings", order = 1)]
    public class ItemSettings : ScriptableObject
    {
        [Header("Colors")]
        [SerializeField] private Color commonColor;
        [SerializeField] private Color rareColor;
        [SerializeField] private Color epicColor;
        [SerializeField] private Color legendaryColor;
        [SerializeField] private Color uniqueColor;

        public Color GetRarityColor(ItemRarity rarity)
        {
            return rarity switch
            {
                ItemRarity.Common => commonColor,
                ItemRarity.Rare => rareColor,
                ItemRarity.Epic => epicColor,
                ItemRarity.Legendary => legendaryColor,
                ItemRarity.Unique => uniqueColor,
                _ => throw new System.ArgumentOutOfRangeException(),
            };
        }
    }
}