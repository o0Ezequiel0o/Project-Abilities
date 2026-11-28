using System.Collections.Generic;
using UnityEngine;
using Zeke.Items;

namespace Zeke.Modding
{
    [CreateAssetMenu(fileName = "Core", menuName = "Modding/Mods/Core", order = -1)]
    public class Core : Mod
    {
        [SerializeField] private List<ItemData> items;
        [SerializeField] private List<Spawnable> enemies;
        [SerializeField] private List<GameLevel> levels;

        public override void OnRunStart()
        {
            RunLevelHandler.Instance.AddLevels(levels);

            ItemRegistry.Instance.Add(items);
            EnemyRegistry.Instance.Add(enemies);
        }
    }
}