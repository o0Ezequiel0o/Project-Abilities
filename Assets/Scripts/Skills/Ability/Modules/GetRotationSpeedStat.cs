using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class GetRotationSpeedStat : GetStatStrategy
    {
        public GetRotationSpeedStat() { }

        public override GetStatStrategy DeepCopy() => new GetRotationSpeedStat();

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