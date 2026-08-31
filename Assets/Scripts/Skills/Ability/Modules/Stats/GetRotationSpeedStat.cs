using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Stats
{
    [Serializable]
    public class GetRotationSpeedStat : GetStatStrategy
    {
        private readonly GetRotationSpeedStatData data;

        public GetRotationSpeedStat(GetRotationSpeedStatData data)
        {
            this.data = data;
        }

        public override Stat GetStat(GameObject source)
        {
            Stat stat = null;

            if (source.TryGetComponent(out EntityAim entityAim))
            {
                stat = entityAim.RotationSpeed;
            }

            return stat;
        }
    }
}