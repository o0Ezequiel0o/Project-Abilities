using UnityEngine;

public class FlankingShotProjectile : PiercingProjectile
{
    [Header("Flanking Shot")]
    [SerializeField] private float pierceDamageMultiplier;

    protected override void OnHit(GameObject receiver)
    {
        Damage *= pierceDamageMultiplier;
    }
}