using UnityEngine;
using System;

public class MoneyHandler : MonoBehaviour
{
    [field: SerializeField] public Stat GoldMultiplier { get; private set; }

    public int Money { get; private set; }

    public Action<int> onReceiveMoney;

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
    }

    public void UseMoney(int money)
    {
        Money = Mathf.Max(Money - money, 0);
    }
}