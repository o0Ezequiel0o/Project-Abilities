using System.Collections.Generic;
using UnityEngine;
using Zeke.Items;

namespace Zeke.Modding
{
    [CreateAssetMenu(fileName = "Core", menuName = "Modding/Mods/Core", order = -1)]
    public class Core : Mod
    {
        [SerializeField] private List<ItemData> items;

        public override void OnGameInitialization()
        {
            ItemRegistry.Instance.Add(items);
        }
    }
}