using System.Collections.Generic;
using UnityEngine;
using System;
using Zeke.TeamSystem;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class Saw : AbilityModule
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private float castDistance;
        [SerializeField] private float damageRadius;

        [Space]

        [SerializeField] private float procCoefficient;
        [SerializeField] private float armorPenetration;

        [Space]

        [SerializeField] private LayerMask hitLayers;
        [SerializeField] private LayerMask blockLayers;

        [Space]

        [SerializeField] private Stat damage;
        [SerializeField] private Stat damageCooldown;

        [Space]

        [SerializeField] private StatusEffectData effect;
        [SerializeField] private int effectProcChance;

        private Vector3 CastPosition => spawn.position + (spawn.up * castDistance);

        private Transform spawn;
        private GameObject source;
        private GameObject sawInstance;

        private float timer = 0f;

        private readonly List<Collider2D> hits = new List<Collider2D>();

        public Saw() { }

        public Saw(Saw original)
        {
            prefab = original.prefab;
            castDistance = original.castDistance;
            damageRadius = original.damageRadius;

            procCoefficient = original.procCoefficient;
            armorPenetration = original.armorPenetration;

            hitLayers = original.hitLayers;
            blockLayers = original.blockLayers;

            effect = original.effect;
            effectProcChance = original.effectProcChance;

            damage = original.damage.DeepCopy();
            damageCooldown = original.damageCooldown.DeepCopy();
        }

        public override AbilityModule DeepCopy() => new Saw(this);

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            this.spawn = spawn;
            this.source = source;

            if (prefab != null)
            {
                sawInstance = GameObject.Instantiate(prefab, source.transform.position, Quaternion.identity);
                sawInstance.SetActive(false);
            }
        }

        public override bool CanActivate() => true;
        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            if (sawInstance == null) return;

            if (!sawInstance.activeSelf)
            {
                sawInstance.SetActive(true);
            }
        }

        public override void Deactivate()
        {
            if (sawInstance == null) return;

            if (sawInstance.activeSelf)
            {
                sawInstance.SetActive(false);
            }
        }

        public override void UpdateActive()
        {
            timer += Time.deltaTime;

            if (timer > damageCooldown.Value)
            {
                UpdateSawCollision();
                timer = 0f;
            }
        }

        public override void LateUpdate()
        {
            sawInstance.transform.position = CastPosition;
        }

        public override void Upgrade()
        {
            base.Upgrade();
            damage.Upgrade();
            damageCooldown.Upgrade();
        }

        public override void Destroy()
        {
            if (sawInstance == null) return;
            GameObject.Destroy(sawInstance);
        }

        private void UpdateSawCollision()
        {
            hits.Clear();

            ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = hitLayers, useLayerMask = true };
            Physics2D.OverlapCircle(CastPosition, damageRadius, contactFilter, hits);

            for (int i = 0; i < hits.Count; i++)
            {
                if (hits[i].gameObject == source) continue;

                if (!IsBlockedByObstacle(spawn.position, hits[i].transform.position))
                {
                    OnHit(hits[i].gameObject);
                }
            }
        }

        private bool IsBlockedByObstacle(Vector3 start, Vector3 end)
        {
            return Physics2D.Linecast(start, end, blockLayers);
        }

        private void OnHit(GameObject gameObject)
        {
            if (TeamManager.IsAlly(source, gameObject)) return;

            if (gameObject.TryGetComponent(out Damageable damageable))
            {
                DamageInfo damageInfo = new DamageInfo(damage.Value, armorPenetration, procCoefficient)
                {
                    direction = (damageable.transform.position - source.transform.position).normalized
                };

                damageable.DealDamage(damageInfo, source, source);
            }

            bool statusEffectRollSuccess = effectProcChance > UnityEngine.Random.Range(0, 100);

            if (statusEffectRollSuccess && gameObject.TryGetComponent(out StatusEffectHandler statusEffectHandler))
            {
                statusEffectHandler.ApplyEffect(effect, source);
            }
        }
    }
}