using System;
using UnityEngine;

namespace Zeke.Abilities.Modules.Summoning
{
    [Serializable]
    public class ShareExperienceWithSource : SummonModule
    {
        public override SummonModule DeepCopy() => new ShareExperienceWithSource();

        public override void OnSummonSpawn(GameObject summon, GameObject source)
        {
            if (summon.TryGetComponent(out LevelHandler summonLevelHandler) && source.TryGetComponent(out LevelHandler sourceLevelHandler))
            {
                summonLevelHandler.onExperienceReceived += sourceLevelHandler.GiveExperience;
            }
        }

        public override void OnDestroy(GameObject summon, GameObject source)
        {
            if (summon.TryGetComponent(out LevelHandler summonLevelHandler) && source.TryGetComponent(out LevelHandler sourceLevelHandler))
            {
                summonLevelHandler.onExperienceReceived -= sourceLevelHandler.GiveExperience;
            }
        }
    }
}