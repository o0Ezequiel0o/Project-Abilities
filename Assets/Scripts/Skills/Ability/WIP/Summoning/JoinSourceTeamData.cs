using System;

namespace Zeke.Abilities.Modules.Summoning
{
    [Serializable]
    public class JoinSourceTeamData : SummonModuleData
    {
        public override SummonModule CreateSummonModule()
        {
            return new JoinSourceTeam(this);
        }
    }
}