using System.Collections.Generic;
using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class HealWithNearbyEffects : AbilityModule
    {
        [SerializeField] private LayerMask hitLayers;
        [SerializeField] private bool consumesEffects = true;
        [SerializeField] private List<StatusEffectData> effects;

        [Space]

        [SerializeField] private Stat healingPerStack;
        [SerializeField] private Stat radius;

        private GameObject source;
        private Damageable damageable;

        private bool hasRequiredComponents = true;

        private readonly List<Collider2D> hits = new List<Collider2D>();

        public HealWithNearbyEffects(HealWithNearbyEffects original)
        {
            consumesEffects = original.consumesEffects;
            hitLayers = original.hitLayers;

            effects = new List<StatusEffectData>(original.effects);

            healingPerStack = original.healingPerStack.DeepCopy();
            radius = original.radius.DeepCopy();
        }

        public override AbilityModule CreateDeepCopy() => new HealWithNearbyEffects(this);

        public override void OnInitialization(ModularAbilityController controller, Transform spawn, GameObject source, ModularAbility ability)
        {
            this.source = source;
            if (!source.TryGetComponent(out damageable)) hasRequiredComponents = false;
        }

        public override bool CanActivate() => true;

        public override bool CanDeactivate() => true;

        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            if (!hasRequiredComponents) return;

            hits.Clear();

            ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = hitLayers };
            Physics2D.OverlapCircle(source.transform.position, radius.Value, contactFilter, hits);

            float healingToReceive = 0f;

            for (int i = 0; i < hits.Count; i++)
            {
                if (hits[i].TryGetComponent(out StatusEffectHandler statusEffectHandler))
                {
                    for (int x = 0; x < effects.Count; x++)
                    {
                        if (statusEffectHandler.TryGetActiveStatusEffect(effects[x], out StatusEffect statusEffect))
                        {
                            healingToReceive += healingPerStack.Value * statusEffect.stacks;

                            if (consumesEffects) statusEffectHandler.RemoveEffect(statusEffect);
                        }
                    }
                }
            }

            damageable.GiveHealing(healingToReceive, source, source);
        }

        public override void Upgrade()
        {
            healingPerStack.Upgrade();
            radius.Upgrade();
        }
    }
}