using UnityEngine;

[DisallowMultipleComponent]
public class DestroyOnDeath : MonoBehaviour
{
    private void Awake()
    {
        if (TryGetComponent(out Damageable damageable))
        {
            damageable.onDeath += Destroy;
        }
    }

    private void Destroy(Damageable.DamageEvent _)
    {
        Destroy(gameObject);
    }
}