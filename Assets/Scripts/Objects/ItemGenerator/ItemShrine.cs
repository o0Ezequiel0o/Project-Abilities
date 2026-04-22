using UnityEngine;
using Zeke.Items;

public class ItemShrine : ItemGenerator
{
    [Header("Shrine")]
    [SerializeField] private int maxRewards = 3;
    [SerializeField] private IStackStat costMultiplier;
    [SerializeField, Range(0, 100)] private int rollNothingChange;

    [Header("Despawn")]
    [SerializeField] private float fadeAwaySeconds;

    private int rewards = 0;
    private int uses = 0;

    public override bool CanSelect(GameObject source)
    {
        return rewards < maxRewards;
    }

    public override bool CanInteract(GameObject source)
    {
        return source.TryGetComponent(out MoneyHandler wallet) && wallet.Money >= ScaledCost && rewards < maxRewards;
    }

    public override bool Interact(GameObject source)
    {
        if (rewards > maxRewards) return false;

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
        bool giveReward = Random.Range(1, 100) > rollNothingChange;

        if (giveReward)
        {
            wallet.UseMoney(ScaledCost);
            GenerateOptions(source, options);
            rewards += 1;
        }

        uses += 1;
        IncreaseCost();
    }

    private void IncreaseCost()
    {
        float newCost = cost * costMultiplier.GetValue(uses);

        if (newCost < 1f && cost >= 1)
        {
            cost = 1;
        }
        else
        {
            cost = Mathf.FloorToInt(cost * costMultiplier.GetValue(uses));
        }
    }

    private void Disappear()
    {
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        GeneralAnimator2D.FadeOut(this, spriteRenderers, fadeAwaySeconds, DestroyObject);
        DisableColliders();
    }
}