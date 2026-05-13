using UnityEngine;
using Zeke.Items;

public class ItemShrine : ItemGenerator
{
    [Header("Shrine")]
    [SerializeField] private int maxRewards = 3;
    [SerializeField, Range(0, 100)] private int rollNothingChange;

    [Header("Despawn")]
    [SerializeField] private float fadeAwaySeconds;

    private int rewards = 0;

    public override bool CanSelect(GameObject source)
    {
        return rewards < maxRewards;
    }

    public override bool CanInteract(GameObject source)
    {
        return source.TryGetComponent(out MoneyHandler wallet) && wallet.Money >= cost && rewards < maxRewards;
    }

    public override bool Interact(GameObject source)
    {
        if (rewards > maxRewards) return false;

        if (CanInteract(source) && source.TryGetComponent(out MoneyHandler wallet))
        {
            Purchase(wallet);
            Use(source);

            return true;
        }

        return false;
    }

    public override bool Hack(GameObject source)
    {
        if (rewards > maxRewards) return false;

        Use(source);
        return true;
    }

    private void Purchase(MoneyHandler wallet)
    {
        wallet.UseMoney(cost);
    }

    private void Use(GameObject source)
    {
        bool giveReward = Random.Range(1, 100) > rollNothingChange;

        if (giveReward)
        {
            GenerateOptions(source, options);
            rewards += 1;
        }

        if (rewards >= maxRewards)
        {
            Disappear();
        }
        else
        {
            IncreaseCost();
        }
    }

    private void IncreaseCost()
    {
        int newCost = cost * 2;

        if (newCost < 1f && cost >= 1)
        {
            cost = 1;
        }
        else
        {
            cost = newCost;
        }
    }

    private void Disappear()
    {
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        GeneralAnimator2D.FadeOut(this, spriteRenderers, fadeAwaySeconds, DestroyObject);
        DisableColliders();
    }
}