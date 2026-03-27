using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class GetMoveSpeedStat : GetStatStrategy
    {
        public GetMoveSpeedStat() { }

        public override GetStatStrategy DeepCopy() => new GetMoveSpeedStat();

        public override Stat GetStat(GameObject source)
        {
            Stat stat = null;

            if (source.TryGetComponent(out EntityMove entityMove))
            {
                stat = entityMove.MoveSpeed;
            }

            return stat;
        }
    }
}