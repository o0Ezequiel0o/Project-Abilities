using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Stats
{
    [Serializable]
    public class GetRechargeSpeedStat : GetStatStrategy
    {
        private readonly GetRechargeSpeedStatData data;

        public GetRechargeSpeedStat(GetRechargeSpeedStatData data)
        {
            this.data = data;
        }

        public override Stat GetStat(GameObject source)
        {
            Stat stat = null;

            if (source.TryGetComponent(out AbilityController abilityController))
            {
                stat = abilityController.rechargeSpeed[data.AbilityType];
            }

            return stat;
        }
    }
}
