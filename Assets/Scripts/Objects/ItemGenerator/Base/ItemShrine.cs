using UnityEngine;
using Zeke.Items;

public class ItemShrine : ItemGenerator
{
    [SerializeField] private int maxUses = 3;
    [SerializeField] private float costIncreaseMult = 1f;

    public override bool CanInteract(GameObject source)
    {
        throw new System.NotImplementedException();
    }

    public override bool CanSelect(GameObject source)
    {
        throw new System.NotImplementedException();
    }

    public override bool Interact(GameObject source)
    {
        throw new System.NotImplementedException();
    }
}