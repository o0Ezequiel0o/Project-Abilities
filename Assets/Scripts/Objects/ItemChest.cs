using UnityEngine;

public class ItemChest : ItemDropper
{
    [Header("Despawn")]
    [SerializeField] private float fadeAwaySeconds;

    private Player player;
    private bool used = false;

    public override bool CanSelect(GameObject source)
    {
        return !used;
    }

    public override bool CanInteract(GameObject source)
    {
        return source.TryGetComponent(out player) && player.Money >= cost && !used;
    }

    public override bool Interact(GameObject source)
    {
        if (used) return false;

        if (CanInteract(source))
        {
            Use(player);
            Disappear();
            return true;
        }

        return false;
    }

    private void Use(Player player)
    {
        player.UseMoney(cost);
        SpawnRandomItem();
        used = true;
    }

    private void Disappear()
    {
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        GeneralAnimator2D.FadeOut(this, spriteRenderers, fadeAwaySeconds, DestroyObject);
        DisableColliders();
    }
}