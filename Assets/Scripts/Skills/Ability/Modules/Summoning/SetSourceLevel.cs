using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Summoning
{
    [Serializable]
    public class SetSourceLevel : SummonModule
    {
        public SetSourceLevel() { }

        public SetSourceLevel(SetSourceLevel original) { }

        public override SummonModule DeepCopy() => new SetSourceLevel(this);

        public override void OnSummonSpawn(GameObject summon, GameObject source)
        {
            if (source.TryGetComponent(out LevelHandler sourceLevelHandler) && summon.TryGetComponent(out LevelHandler summonLevelHandler))
            {
                for (int i = summonLevelHandler.Level; summonLevelHandler.Level < sourceLevelHandler.Level; i++)
                {
                    summonLevelHandler.GiveExperience(summonLevelHandler.ExperienceRequired);
                }
            }
        }

        public override void OnDestroy(GameObject summon, GameObject source) { }
    }
}