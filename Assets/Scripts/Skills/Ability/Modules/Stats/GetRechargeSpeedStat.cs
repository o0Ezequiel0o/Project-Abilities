using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Stats
{
    [Serializable]
    public class GetRechargeSpeedStat : GetStatStrategy
    {
        [SerializeField] private AbilityType abilityType;

        public GetRechargeSpeedStat() { }

        public GetRechargeSpeedStat(GetRechargeSpeedStat original)
        {
            abilityType = original.abilityType;
        }

        public override GetStatStrategy DeepCopy() => new GetRechargeSpeedStat(this);

        public override Stat GetStat(GameObject source)
        {
            Stat stat = null;

            if (source.TryGetComponent(out AbilityController abilityController))
            {
                stat = abilityController.rechargeSpeed[abilityType];
            }

            return stat;
        }
    }
}
