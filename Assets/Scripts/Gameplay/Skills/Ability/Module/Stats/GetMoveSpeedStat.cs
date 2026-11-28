using UnityEngine;

namespace Zeke.Abilities.Modules.Stats
{
    public class GetMoveSpeedStat : GetStatStrategy
    {
        private readonly GetMoveSpeedStatData data;

        public GetMoveSpeedStat(GetMoveSpeedStatData data)
        {
            this.data = data;
        }

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