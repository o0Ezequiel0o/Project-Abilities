using System;

namespace Zeke.Abilities.Modules.Summoning
{
    [Serializable]
    public class SetSourceLevelData : SummonModuleData
    {
        public override SummonModule CreateSummonModule()
        {
            return new SetSourceLevel(this);
        }
    }
}