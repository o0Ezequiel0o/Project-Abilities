using UnityEngine;

[DisallowMultipleComponent]
public class DestroyOnDeath : MonoBehaviour
{
    private bool alreadyDestroying = false;
    private bool destroyThisFrame = false;

    private void Awake()
    {
        if (TryGetComponent(out Damageable damageable))
        {
            damageable.onDeath += DestroyThisFrame;
        }
    }

    private void LateUpdate()
    {
        if (!destroyThisFrame) return;
        if (alreadyDestroying) return;

        alreadyDestroying = true;
        Destroy(gameObject);
    }

    private void DestroyThisFrame(Damageable.DamageEvent _)
    {
        destroyThisFrame = true;
    }
}