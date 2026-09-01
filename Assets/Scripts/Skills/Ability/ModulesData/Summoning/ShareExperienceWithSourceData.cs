using System;

namespace Zeke.Abilities.Modules.Summoning
{
    [Serializable]
    public class ShareExperienceWithSourceData : SummonModuleData
    {
        public override SummonModule CreateSummonModule()
        {
            return new ShareExperienceWithSource(this);
        }
    }
}