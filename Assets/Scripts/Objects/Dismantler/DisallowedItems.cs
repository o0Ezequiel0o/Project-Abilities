using System.Collections.Generic;
using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Disallowed Items", menuName = "ScriptableObjects/Items/DisallowedItems", order = 1)]
    public class DisallowedItems : ScriptableObject
    {
        [SerializeField] private List<ItemData> disallowedItems;

        public bool IsDisallowed(ItemData item) => disallowedItems.Contains(item);
    }
}