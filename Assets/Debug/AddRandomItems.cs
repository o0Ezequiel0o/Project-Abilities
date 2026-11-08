using UnityEngine;
using System;

namespace Zeke.Items
{
    public class AddRandomItems : MonoBehaviour
    {
        [SerializeField] private ItemHandler itemHandler;
        [SerializeField] private ItemSettings items;
        [SerializeField] private int amount;

        [Space]

        [SerializeField] private bool onUpdate;
        [SerializeField] private float cooldown;

        private float timer = 0f;

        private void Start()
        {
            for (int i = 0; i < amount; i++)
            {
                itemHandler.AddItem(items.GetRandomItem((ItemRarity)UnityEngine.Random.Range(0, Enum.GetNames(typeof(ItemRarity)).Length - 1)));
            }
        }

        private void Update()
        {
            if (!onUpdate) return;

            timer += Time.deltaTime;

            if (timer >= cooldown)
            {
                itemHandler.AddItem(items.GetRandomItem((ItemRarity)UnityEngine.Random.Range(0, Enum.GetNames(typeof(ItemRarity)).Length - 1)));
                timer = 0f;
            }
        }
    }
}