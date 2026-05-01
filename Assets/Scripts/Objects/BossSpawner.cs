using System.Collections.Generic;
using UnityEngine;
using static Damageable;

public class BossSpawner : MonoBehaviour, IInteractable
{
    [Header("Spawning")]
    [field: SerializeField] private List<Spawnable> pool = new List<Spawnable>();

    [Header("Visual")]
    [field: SerializeField] public Sprite InteractOverlay { get; private set; }
    [field: SerializeField] public string InteractTooltip { get; private set; }

    private bool activated = false;

    public class LevelBossDeathEvent : IGlobalEvent { }

    public bool CanInteract(GameObject source) => !activated;
    public bool CanSelect(GameObject source) => !activated;

    public bool Interact(GameObject source)
    {
        Spawnable spawnable = pool[Random.Range(0, pool.Count)];
        GameObject boss = Instantiate(spawnable.Prefab, transform.position, Quaternion.identity);

        if (boss.TryGetComponent(out Damageable damageable))
        {
            damageable.onDeath.Subscribe(OnBossDeath);
        }

        return true;
    }

    private void OnBossDeath(DamageEvent damageEvent)
    {
        GlobalEventBus.Invoke(new LevelBossDeathEvent());
    }
}