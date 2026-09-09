using System.Collections.Generic;
using UnityEngine;
using Zeke.Items;
using System;

[CreateAssetMenu(fileName = "Costs", menuName = "Objects/Duplicator/New Costs", order = 1)]
public class DuplicatorCosts : ScriptableObject
{
    [SerializeField] private Dictionary<ItemRarity, ResourceReward> costs;

    public int GetCost(ItemData itemData)
    {
        return costs[itemData.Rarity].GetCost(itemData);
    }

    [Serializable]
    private class ResourceReward
    {
        [SerializeField] private int cost;

        public int GetCost(ItemData itemData)
        {
            return cost;
        }
    }
}