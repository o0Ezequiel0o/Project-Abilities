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

    public override bool Interact(GameObject source)
    {
        if (used) return false;

        if (CanInteract(source) && source.TryGetComponent(out MoneyHandler wallet))
        {
            Purchase(source, wallet);

            Disappear();
            return true;
        }

        return false;
    }

    private void Purchase(GameObject source, MoneyHandler wallet)
    {
        wallet.UseMoney(cost);
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