using UnityEngine;

namespace Zeke.Abilities.Modules.Summoning
{
    public class ShareMoneyWithSource : SummonModule
    {
        public override SummonModule DeepCopy() => new ShareMoneyWithSource();

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