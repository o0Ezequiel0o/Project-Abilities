using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Projectiles
{
    [Serializable]
    public class FireSniperData : FireBasicProjectileData
    {
        [SerializeField] private Stat doubleDamageChance;

        public override FireProjectileType CreateProjectileTypeModule()
        {
            return new FireSniper(this, Damage.DeepCopy(), doubleDamageChance.DeepCopy());
        }
    }
}