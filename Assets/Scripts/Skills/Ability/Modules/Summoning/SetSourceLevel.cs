using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Summoning
{
    [Serializable]
    public class SetSourceLevel : SummonModule
    {
        private readonly SetSourceLevelData data;

        public SetSourceLevel(SetSourceLevelData data)
        {
            this.data = data;
        }

        public override void OnSummonSpawn(GameObject summon, GameObject source)
        {
            if (source.TryGetComponent(out LevelHandler sourceLevelHandler) && summon.TryGetComponent(out LevelHandler summonLevelHandler))
            {
                int targetLevel = sourceLevelHandler.Level;
                summonLevelHandler.IncreaseLevel(targetLevel - 1);
            }
        }

        public override void OnDestroy(GameObject summon, GameObject source) { }
    }
}