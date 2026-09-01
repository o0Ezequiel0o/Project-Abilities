using System.Collections.Generic;
using UnityEngine;

namespace Zeke.Abilities.Modules
{
    public class HealWithNearbyEffects : AbilityModule
    {
        private readonly HealWithNearbyEffectsData data;

        private readonly Stat healingPerStack;
        private readonly Stat radius;

        private GameObject source;
        private Damageable damageable;

        private bool hasRequiredComponents = true;

        private readonly List<Collider2D> hits = new List<Collider2D>();

        public HealWithNearbyEffects(HealWithNearbyEffectsData data, Stat healingPerStack, Stat radius)
        {
            this.data = data;
            this.healingPerStack = healingPerStack;
            this.radius = radius;
        }

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            this.source = source;
            if (!source.TryGetComponent(out damageable)) hasRequiredComponents = false;
        }

        public override bool CanActivate() => true;
        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            if (!hasRequiredComponents) return;

            hits.Clear();

            ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = data.HitLayers, useLayerMask = true };
            Physics2D.OverlapCircle(source.transform.position, radius.Value, contactFilter, hits);

            float healingToReceive = 0f;

            for (int i = 0; i < hits.Count; i++)
            {
                if (hits[i].TryGetComponent(out StatusEffectHandler statusEffectHandler))
                {
                    for (int x = 0; x < data.Effects.Count; x++)
                    {
                        if (statusEffectHandler.TryGetActiveStatusEffect(data.Effects[x], out StatusEffect statusEffect))
                        {
                            healingToReceive += healingPerStack.Value * statusEffect.stacks;

                            if (data.ConsumesEffects) statusEffectHandler.RemoveEffect(statusEffect);
                        }
                    }
                }
            }

            HealInfo heal = new HealInfo(healingToReceive, data.ProcCoefficient);
            damageable.GiveHealing(heal, source, source);
        }

        public override void Upgrade()
        {
            healingPerStack.Upgrade();
            radius.Upgrade();
        }
    }
}