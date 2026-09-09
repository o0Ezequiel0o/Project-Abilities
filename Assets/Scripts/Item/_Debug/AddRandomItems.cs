using UnityEngine;
using System;

namespace Zeke.Items
{
    public class AddRandomItems : MonoBehaviour
    {
        [SerializeField] private ItemHandler itemHandler;
        [SerializeField] private ItemSettings items;
        [SerializeField] private int amount;

        private void Start()
        {
            for (int i = 0; i < amount; i++)
            {
                itemHandler.AddItem(items.GetRandomItem((ItemRarity)UnityEngine.Random.Range(0, Enum.GetNames(typeof(ItemRarity)).Length - 1)));
            }
        }
    }
}