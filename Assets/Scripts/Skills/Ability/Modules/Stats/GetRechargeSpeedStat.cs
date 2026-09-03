using UnityEngine;

namespace Zeke.Abilities.Modules.Stats
{
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
                stat = abilityController.Abilities[data.AbilityType].RechargeSpeed;
            }

            return stat;
        }
    }
}
