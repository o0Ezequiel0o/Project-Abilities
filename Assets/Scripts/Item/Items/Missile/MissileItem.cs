using System.Collections.Generic;
using UnityEngine;
using Zeke.PoolableGameObjects;
using Zeke.TeamSystem;

namespace Zeke.Items
{
    public class MissileItem : Item
    {
        public override ItemData Data => data;
        private readonly MissileItemData data;

        private readonly GameObject source;
        private readonly ItemHandler itemHandler;

        private readonly GameObjectPool<MissileItemProjectile> missilePool = new GameObjectPool<MissileItemProjectile>();

        public MissileItem(MissileItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void OnRemoved()
        {
            missilePool.Clear();
        }

        public override void OnHit(Damageable.DamageEvent damageEvent)
        {
            if (!RollProc(data.ProcChance, damageEvent.ProcCoefficient, itemHandler.Luck)) return;
            if (damageEvent.Receiver != null && damageEvent.Receiver.gameObject == source) return;
            if (damageEvent.ProcChainBranch.Contains(Data)) return;

            float damage = damageEvent.BaseDamage * data.DamageMult.GetValue(stacks);
            Vector2 direction = GetRelativeDirection(data.SpawnDirection, source.transform.up);

            List<ItemData> newProcChainBranch = new List<ItemData>(damageEvent.ProcChainBranch) { Data };

            MissileItemProjectile missile = missilePool.Get(data.MissilePrefab);
            missile.Launch(source.transform.position, data.Speed, direction, data.MaxRange, damage, source, TeamManager.GetTeam(source), newProcChainBranch);

            missile.gameObject.SetActive(true);
        }

        private Vector2 GetRelativeDirection(Vector2 direction, Vector2 relativeTo)
        {
            float angle = Vector2.SignedAngle(Vector2.up, direction);
            return (Quaternion.Euler(0f, 0f, angle) * relativeTo).normalized;
        }
    }
}