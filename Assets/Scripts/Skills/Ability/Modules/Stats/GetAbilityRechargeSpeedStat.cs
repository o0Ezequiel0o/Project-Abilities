using UnityEngine;

namespace Zeke.Abilities.Modules.Stats
{
    public class GetAbilityRechargeSpeedStat : GetStatStrategy
    {
        private GetAbilityRechargeSpeedStatData data;

        public GetAbilityRechargeSpeedStat(GetAbilityRechargeSpeedStatData data)
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
