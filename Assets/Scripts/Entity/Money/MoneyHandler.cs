using UnityEngine;
using Zeke.Collections;

public class MoneyHandler : MonoBehaviour
{
    [field: SerializeField] public Stat GoldMultiplier { get; private set; }

    public int Money { get; private set; }

    public OrderedAction<int> onReceiveMoney = new OrderedAction<int>();
    public OrderedAction<int> onReceivedMoney = new OrderedAction<int>();

    public OrderedAction<int> onUsedMoney = new OrderedAction<int>();

    public void GiveMoney(int money)
    {
        int oldMoney = money;

        money = Mathf.FloorToInt(money * GoldMultiplier.Value);

        if (oldMoney > 0 && money <= 0 && GoldMultiplier.Value > 0f)
        {
            money = 1;
        }

        onReceiveMoney?.Invoke(money);

        Money += Mathf.FloorToInt(money * GoldMultiplier.Value);

        onReceivedMoney?.Invoke(money);
    }

    public void UseMoney(int money)
    {
        Money = Mathf.Max(Money - money, 0);
        onUsedMoney?.Invoke(money);
    }
}