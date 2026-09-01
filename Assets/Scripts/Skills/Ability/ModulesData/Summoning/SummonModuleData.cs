using System;

namespace Zeke.Abilities.Modules.Summoning
{
    [Serializable]
    public abstract class SummonModuleData
    {
        public abstract SummonModule CreateSummonModule();
    }
}