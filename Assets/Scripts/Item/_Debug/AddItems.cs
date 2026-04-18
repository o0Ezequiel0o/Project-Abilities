using System.Collections.Generic;
using UnityEngine;

namespace Zeke.Items
{
    public class AddItems : MonoBehaviour
    {
        [SerializeField] private List<ItemData> items;

        void Awake()
        {
            if (TryGetComponent(out ItemHandler itemHandler))
            {
                for (int i = 0; i < items.Count; i++)
                {
                    itemHandler.AddItem(items[i]);
                }
            }
        }
    }
}