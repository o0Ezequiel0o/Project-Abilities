using System.Collections.Generic;
using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "New Item Blacklist", menuName = "ScriptableObjects/Items/Blacklist")]
    public class ItemBlacklist : ScriptableObject
    {
        [SerializeField] private List<ItemData> items;

        public IReadOnlyList<ItemData> Items => items;

        public bool IsBlacklisted(ItemData item) => items.Contains(item);
    }
}