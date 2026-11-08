using System.Collections.Generic;
using UnityEngine;
using Zeke.Abilities.Indicators;

public class StatusEffectRendererManagerPool : GameService<StatusEffectRendererManagerPool>
{
    [SerializeField] private StatusEffectInterface prefab;

    private readonly Stack<StatusEffectInterface> interfaces = new Stack<StatusEffectInterface>();

    public StatusEffectInterface Get()
    {
        if (interfaces.Count == 0)
        {
            return Instantiate(prefab, Run.Instance.WorldCanvas.transform);
        }

        return interfaces.Pop();
    }

    public void Return(StatusEffectInterface indicator)
    {
        interfaces.Push(indicator);
    }
}