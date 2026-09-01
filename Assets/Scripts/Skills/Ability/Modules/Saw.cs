using System.Collections.Generic;
using UnityEngine;
using Zeke.TeamSystem;

namespace Zeke.Abilities.Modules
{
    public class Saw : AbilityModule
    {
        private readonly SawData data;

        private readonly Stat damage;
        private readonly Stat damageCooldown;

        private Vector3 CastPosition => spawn.position + (spawn.up * data.CastDistance);

        private Transform spawn;
        private GameObject source;
        private GameObject sawInstance;

        private float timer = 0f;

        private readonly List<Collider2D> hits = new List<Collider2D>();

        public Saw(SawData data, Stat damage, Stat damageCooldown)
        {
            this.data = data;
            this.damage = damage;
            this.damageCooldown = damageCooldown;
        }

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            this.spawn = spawn;
            this.source = source;

            if (data.Prefab != null)
            {
                sawInstance = GameObject.Instantiate(data.Prefab, source.transform.position, Quaternion.identity);
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

            ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = data.HitLayers, useLayerMask = true };
            Physics2D.OverlapCircle(CastPosition, data.DamageRadius, contactFilter, hits);

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
            return Physics2D.Linecast(start, end, data.BlockLayers);
        }

        private void OnHit(GameObject gameObject)
        {
            if (TeamManager.IsAlly(source, gameObject)) return;

            if (gameObject.TryGetComponent(out Damageable damageable))
            {
                DamageInfo damageInfo = new DamageInfo(damage.Value, data.ArmorPenetration, data.ProcCoefficient)
                {
                    direction = (damageable.transform.position - source.transform.position).normalized
                };

                damageable.DealDamage(damageInfo, source, source);
            }

            bool statusEffectRollSuccess = data.EffectProcChance > UnityEngine.Random.Range(0, 100);

            if (statusEffectRollSuccess && gameObject.TryGetComponent(out StatusEffectHandler statusEffectHandler))
            {
                statusEffectHandler.ApplyEffect(data.Effect, source);
            }
        }
    }
}