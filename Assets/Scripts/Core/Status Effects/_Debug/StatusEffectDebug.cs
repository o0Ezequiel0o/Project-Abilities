using UnityEngine;

public class StatusEffectDebug : MonoBehaviour
{
    [SerializeField] private StatusEffectData statusEffectToApply;
    [SerializeField] private int stacks;
    [SerializeField] private GameObject source;

    void Start()
    {
        if (TryGetComponent(out StatusEffectHandler statusEffectHandler))
        {
            statusEffectHandler.ApplyEffect(statusEffectToApply, gameObject, stacks);
        }
    }
}