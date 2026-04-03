using UnityEngine;
using System;
using Zeke.TeamSystem;

namespace Zeke.Abilities.Modules.Summoning
{
    [Serializable]
    public class JoinSourceTeam : SummonModule
    {
        public override SummonModule DeepCopy() => new JoinSourceTeam();

        public override void OnSummonSpawn(GameObject summon, GameObject source)
        {
            TeamManager.ChangeTeams(summon, source);
        }

        public override void OnDestroy(GameObject summon, GameObject source) { }
    }
}