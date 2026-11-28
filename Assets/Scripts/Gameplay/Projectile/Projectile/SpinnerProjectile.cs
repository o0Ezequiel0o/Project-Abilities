using UnityEngine;

public class SpinnerProjectile : PiercingProjectile
{
    protected override void Hit(GameObject receiver)
    {
        bool damageRejected = DealDamage(receiver);

        if (damageRejected) return;

        if (SourceUser != null)
        {
            Vector2 direction = (receiver.transform.position - SourceUser.transform.position).normalized;
            ApplyKnockback(receiver, direction);
        }
    }
}