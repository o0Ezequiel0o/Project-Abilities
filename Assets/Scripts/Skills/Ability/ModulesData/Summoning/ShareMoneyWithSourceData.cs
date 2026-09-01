using System;

namespace Zeke.Abilities.Modules.Summoning
{
    [Serializable]
    public class ShareMoneyWithSourceData : SummonModuleData
    {
        public override SummonModule CreateSummonModule()
        {
            return new ShareMoneyWithSource(this);
        }
    }
}