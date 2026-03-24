using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class SummonFireBarrier : GenericSummon<FireBarrier>
    {
        [SerializeField] private Stat duration;
        [SerializeField] private Vector2 size;
        [SerializeField] private float scale;

        public SummonFireBarrier(SummonFireBarrier original) : base(original)
        {
            size = original.size;
            scale = original.scale;

            duration = original.duration.DeepCopy();
        }

        public override AbilityModule DeepCopy() => new SummonFireBarrier(this);

        public override void Activate(bool holding)
        {
            if (TrySpawnSummon(prefab, out FireBarrier fireBarrier))
            {
                fireBarrier.SetValues(fireBarrier.transform.position, source, spawn.up, size * scale, duration.Value);
            }
        }

        public override void Upgrade()
        {
            base.Upgrade();
            duration.Upgrade();
        }
    }
}