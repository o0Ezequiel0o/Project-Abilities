using UnityEngine;

public class Player : MonoBehaviour
{
    public int Money { get; private set; }
    public int Kills { get; private set; }

    public void GiveMoney(int money)
    {
        Money += money;
    }

    public void UseMoney(int money)
    {
        Money = Mathf.Max(Money - money, 0);
    }

    private void Awake()
    {
        Damageable.DamageEvent.onKill.Subscribe(gameObject, IncreaseKills);
    }

    private void OnEnable()
    {
        GameInstance.AddPlayer(this);
    }

    private void OnDisable()
    {
        GameInstance.RemovePlayer(this);
    }

    private void IncreaseKills(Damageable.DamageEvent _)
    {
        Kills += 1;
    }
}