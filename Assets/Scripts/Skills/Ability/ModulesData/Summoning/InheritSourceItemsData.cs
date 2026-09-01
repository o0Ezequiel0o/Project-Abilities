using System;

namespace Zeke.Abilities.Modules.Summoning
{
    [Serializable]
    public class InheritSourceItemsData : SummonModuleData
    {
        public override SummonModule CreateSummonModule()
        {
            return new InheritSourceItems(this);
        }
    }
}