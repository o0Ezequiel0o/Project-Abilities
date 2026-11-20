using System.Collections.Generic;
using UnityEngine;
using Zeke.Items;

using static Damageable;

public class DamageableEventPool : GameService<DamageableEventPool>
{
    private readonly Stack<DamageEvent> damageEvents = new Stack<DamageEvent>();
    private readonly Stack<HealEvent> healEvents = new Stack<HealEvent>();

    public DamageEvent GetDamage(DamageInfo damageInfo, Damageable receiver, GameObject userSource, GameObject sourceObject, List<ItemData> procChain)
    {
        if (damageEvents.Count == 0)
        {
            return new DamageEvent().Initialize(damageInfo, receiver, userSource, sourceObject, procChain);
        }

        return damageEvents.Pop().Initialize(damageInfo, receiver, userSource, sourceObject, procChain);
    }

    public void ReturnDamage(DamageEvent damageEvent)
    {
        damageEvents.Push(damageEvent);
    }

    public HealEvent GetHeal(HealInfo healInfo, Damageable receiver, GameObject userSource, GameObject sourceObject, List<ItemData> procChain)
    {
        if (healEvents.Count == 0)
        {
            return new HealEvent().Initialize(healInfo, receiver, userSource, sourceObject, procChain);
        }

        return healEvents.Pop().Initialize(healInfo, receiver, userSource, sourceObject, procChain);
    }

    public void ReturnHeal(HealEvent healEvent)
    {
        healEvents.Push(healEvent);
    }
}