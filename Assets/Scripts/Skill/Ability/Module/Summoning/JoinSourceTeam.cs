using UnityEngine;
using Zeke.TeamSystem;

namespace Zeke.Abilities.Modules.Summoning
{
    public class JoinSourceTeam : SummonModule
    {
        private readonly JoinSourceTeamData data;

        public JoinSourceTeam(JoinSourceTeamData data)
        {
            this.data = data;
        }

        public override void OnSummonSpawn(GameObject summon, GameObject source)
        {
            TeamManager.ChangeTeams(summon, source);
        }

        public override void OnDestroy(GameObject summon, GameObject source) { }
    }
}