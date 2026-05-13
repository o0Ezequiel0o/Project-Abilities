using UnityEngine;
using Zeke.Items;

public class ItemChest : ItemGenerator
{
    [Header("Despawn")]
    [SerializeField] private float fadeAwaySeconds;

    private bool used = false;

    public override bool CanSelect(GameObject source)
    {
        return !used;
    }

    public override bool CanInteract(GameObject source)
    {
        return source.TryGetComponent(out MoneyHandler wallet) && wallet.Money >= cost && !used;
    }

    public override bool CanHack(GameObject source)
    {
        return !used;
    }

    public override bool Interact(GameObject source)
    {
        if (used) return false;

        if (CanInteract(source) && source.TryGetComponent(out MoneyHandler wallet))
        {
            Purchase(wallet);
            Use(source);
            Disappear();

            return true;
        }

        return false;
    }

    public override bool Hack(GameObject source)
    {
        if (used) return false;

        Use(source);
        Disappear();

        return true;
    }

    private void Purchase(MoneyHandler wallet)
    {
        wallet.UseMoney(cost);
    }

    private void Use(GameObject source)
    {
        GenerateOptions(source, options);
        used = true;
    }

    private void Disappear()
    {
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        GeneralAnimator2D.FadeOut(this, spriteRenderers, fadeAwaySeconds, DestroyObject);
        DisableColliders();
    }
}