using UnityEngine;
using System;

namespace Zeke.Items
{
    public class AddRandomItems : MonoBehaviour
    {
        [SerializeField] private ItemHandler itemHandler;
        [SerializeField] private ItemSettings items;
        [SerializeField] private int amount;

        //private float timer = 0f;

        private void Start()
        {
            for (int i = 0; i < amount; i++)
            {
                itemHandler.AddItem(items.GetRandomItem((ItemRarity)UnityEngine.Random.Range(0, Enum.GetNames(typeof(ItemRarity)).Length - 1)));
            }
        }

        //private void Update()
        //{
        //    timer += Time.deltaTime;

        //    if (timer >= 0.25f)
        //    {
        //        itemHandler.AddItem(items.GetRandomItem((ItemRarity)UnityEngine.Random.Range(0, Enum.GetNames(typeof(ItemRarity)).Length - 1)));
        //        timer = 0f;
        //    }
        //}
    }
}