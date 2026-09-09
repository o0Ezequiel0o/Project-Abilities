using System.Collections.Generic;
using UnityEngine;
using Zeke.Items;
using System;

[CreateAssetMenu(fileName = "Dismantler Rewards", menuName = "Objects/Dismantler/New Rewards", order = 1)]
public class DismantlerRewards : ScriptableObject
{
    [SerializeField] private Dictionary<ItemRarity, ResourceReward> rewards;

    public void GiveRewards(ItemData itemData, ItemHandler itemHandler)
    {
        rewards[itemData.Rarity].GiveRewards(itemHandler);
    }

    [Serializable]
    private class ResourceReward
    {
        [SerializeField] private ItemData reward;
        [SerializeField] private int stacks;

        public void GiveRewards(ItemHandler itemHandler)
        {
            itemHandler.AddItem(reward, stacks);
        }
    }
}