using System.Collections.Generic;
using UnityEngine;
using Zeke.Items;
using System;

[CreateAssetMenu(fileName = "Drops", menuName = "Objects/Item Generator/New Drops", order = 1)]
public class ItemGeneratorDrops : ScriptableObject
{
    [field: SerializeField] public List<ItemRaritySlot> RaritySlots { get; private set; }

    [Serializable]
    public class ItemRaritySlot : IWeighted
    {
        public ItemRarity rarity;
        public int weight;

        public int Weight => weight;

        public ItemRaritySlot() { }

        public ItemRaritySlot(ItemRarity rarity, int weight)
        {
            this.rarity = rarity;
            this.weight = weight;
        }
    }
}