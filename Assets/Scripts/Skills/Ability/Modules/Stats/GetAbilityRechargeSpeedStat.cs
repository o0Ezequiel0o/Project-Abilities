using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Stats
{
    [Serializable]
    public class GetRecha : GetStatStrategy
    {
        [SerializeField] private AbilityType abilityType;

        public GetRecha() { }

        public GetRecha(GetRecha original)
        {
            abilityType = original.abilityType;
        }

        public override GetStatStrategy DeepCopy() => new GetRecha(this);

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
