using UnityEngine;

public class DebugApplyStatusEffect : MonoBehaviour
{
    [SerializeField] private StatusEffectData statusEffect;
    [SerializeField] private int stacksToApply = 1;

    private void Start()
    {
        if (TryGetComponent(out StatusEffectHandler statusEffectHandler))
        { 
            statusEffectHandler.ApplyEffect(statusEffect, gameObject, stacksToApply);
        }
    }
}