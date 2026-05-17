using UnityEngine;
using static Damageable;

public class SpinnerProjectile : PiercingProjectile
{
    protected override void Hit(GameObject receiver)
    {
        DealDamage(receiver, OnDamageDealt);
    }

    private void OnDamageDealt(DamageEvent damageEvent)
    {
        if (damageEvent.damageRejected) return;

        GameObject receiver = damageEvent.Receiver.gameObject;

        if (SourceUser != null)
        {
            Vector2 direction = (receiver.transform.position - SourceUser.transform.position).normalized;
            ApplyKnockback(receiver, direction);
        }
    }
}