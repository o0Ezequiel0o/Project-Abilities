using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Summoning
{
    [Serializable]
    public class ShareMoneyWithSource : SummonModule
    {
        private readonly ShareMoneyWithSourceData data;

        public ShareMoneyWithSource(ShareMoneyWithSourceData data)
        {
            this.data = data;
        }

        public override void OnSummonSpawn(GameObject summon, GameObject source)
        {
            if (summon.TryGetComponent(out MoneyHandler summonMoneyHandler) && source.TryGetComponent(out MoneyHandler sourceMoneyHandler))
            {
                summonMoneyHandler.onReceiveMoney.Subscribe(sourceMoneyHandler.GiveMoney);
            }
        }

        public override void OnDestroy(GameObject summon, GameObject source)
        {
            if (summon.TryGetComponent(out MoneyHandler summonMoneyHandler) && source.TryGetComponent(out MoneyHandler sourceMoneyHandler))
            {
                summonMoneyHandler.onReceiveMoney.Unsubscribe(sourceMoneyHandler.GiveMoney);
            }
        }
    }
}