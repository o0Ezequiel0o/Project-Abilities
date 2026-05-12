using UnityEngine;

public class MoneyChest : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    [SerializeField] private int baseReward;

    [Header("Visual")]
    [field: SerializeField] public Sprite InteractOverlay { get; private set; }
    [field: SerializeField] public string InteractTooltip { get; private set; }

    [Header("Despawn")]
    [SerializeField] private float fadeAwaySeconds;

    private int reward = 0;

    private bool used = false;

    public bool CanInteract(GameObject source)
    {
        return source.TryGetComponent(out MoneyHandler wallet) && !used;
    }

    public bool CanSelect(GameObject source)
    {
        return !used;
    }

    public bool Interact(GameObject source)
    {
        if (used) return false;

        if (CanInteract(source) && source.TryGetComponent(out MoneyHandler wallet))
        {
            GiveRewards(source, wallet);
            used = true;
            Disappear();

            return true;
        }

        return false;
    }

    private void Awake()
    {
        int newValue = Mathf.FloorToInt(baseReward * GameInstance.CostMultiplier);

        if (baseReward >= 1 && newValue <= 0)
        {
            newValue = 0;
        }

        reward = newValue;
    }

    private void GiveRewards(GameObject source, MoneyHandler wallet)
    {
        wallet.GiveMoney(reward);
    }

    private void Disappear()
    {
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        GeneralAnimator2D.FadeOut(this, spriteRenderers, fadeAwaySeconds, DestroyObject);
        DisableColliders();
    }

    protected void DisableColliders()
    {
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }
    }

    protected void DestroyObject()
    {
        Destroy(gameObject);
    }
}