using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour, IInteractable, IInteractableTooltipRenderer
{
    [Header("Spawning")]
    [SerializeField] private Transform portalSpawn;
    [SerializeField] private GameObject portalPrefab;

    [Space]

    [field: SerializeField] private List<Spawnable> pool = new List<Spawnable>();

    [Header("Visual")]
    [field: SerializeField] public Sprite InteractOverlay { get; private set; }
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField, TextArea(4,4)] public string Description { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }

    public string Cost => "No Cost";

    private bool activated = false;

    public bool CanInteract(GameObject source) => !activated;
    public bool CanSelect(GameObject source) => !activated;

    public bool Interact(GameObject source)
    {
        Spawnable spawnable = pool[Random.Range(0, pool.Count)];
        GameObject boss = Instantiate(spawnable.Prefab, transform.position, Quaternion.identity);

        if (boss.TryGetComponent(out Damageable damageable))
        {
            damageable.onDespawn += OnBossDeath;
        }

        activated = true;

        return true;
    }

    private void OnBossDeath(Damageable _)
    {
        Instantiate(portalPrefab, portalSpawn.position, Quaternion.identity);
        GlobalEventBus.Invoke(new GameLevelHandler.LevelEndEvent());
    }
}