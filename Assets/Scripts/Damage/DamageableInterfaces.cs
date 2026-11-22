using System.Collections.Generic;
using UnityEngine;
using Zeke.Items;

public partial class Damageable
{
    public interface IHitEvent
    {
        // Context
        public Damageable Receiver { get; }
        public GameObject SourceUser { get; }
        public GameObject SourceObject { get; }
        public Vector2 Direction { get; }
        public float ProcCoefficient { get; }

        // Flags
        public bool IsHit { get; }
        public bool IsLethal { get; }
        public bool IgnoresShield { get; }
        public bool IsRejected { get; set; }

        // Data
        public List<ItemData> ProcChainBranch { get; }

        // Damage
        public float BaseDamage { get; }
        public float Damage { get; }

        // Modifiers
        public float FlatAccumulator { get; set; }
        public Stat Multiplier { get; }
        public float ArmorPenetration { get; set; }
    }

    public interface IDealDamageEvent
    {
        // Context 
        public Damageable Receiver { get; }
        public GameObject SourceUser { get; }
        public GameObject SourceObject { get; }
        public Vector2 Direction { get; }
        public float ProcCoefficient { get; }

        // Flags
        public bool IsHit { get; }
        public bool IsLethal { get; }
        public bool IgnoresShield { get; }
        public bool IsRejected { get; set; }

        // Data
        public List<ItemData> ProcChainBranch { get; }

        // Damage
        public float BaseDamage { get; }
        public float Damage { get; }

        // Modifiers
        public float FlatAccumulator { get; set; }
        public Stat Multiplier { get; }
        public float ArmorPenetration { get; set; }
    }

    public interface IDealtDamageEvent
    {
        // Context
        public Damageable Receiver { get; }
        public GameObject SourceUser { get; }
        public GameObject SourceObject { get; }
        public Vector2 Direction { get; }
        public float ProcCoefficient { get; }

        // Flags
        public bool IsHit { get; }
        public bool IsLethal { get; }
        public bool IgnoresShield { get; }
        public bool IsRejected { get; }

        // Data
        public List<ItemData> ProcChainBranch { get; }

        // Damage
        public float BaseDamage { get; }
        public float Damage { get; }

        // Modifiers
        public float FlatAccumulator { get; }
        public Stat Multiplier { get; }
        public float ArmorPenetration { get; }

        // Results & State
        public float HealthDamageDealt { get; }
        public float ShieldDamageDealt { get; }
        public float OverflowDamage => Mathf.Max(0f, Damage - DamageDealt);
        public float DamageDealt => HealthDamageDealt + ShieldDamageDealt;
        public float UncappedDamageDealt => DamageDealt + OverflowDamage;

        // Output
        public bool DestroyedShield { get; }
        public bool DeathBlow { get; }   
    }

    public interface IKillEvent
    {
        // Context
        public Damageable Receiver { get; }
        public GameObject SourceUser { get; }
        public GameObject SourceObject { get; }
        public Vector2 Direction { get; }
        public float ProcCoefficient { get; }

        // Flags
        public bool IsHit { get; }
        public bool IsLethal { get; }
        public bool IgnoresShield { get; }
        public bool IsRejected { get; }

        // Data
        public List<ItemData> ProcChainBranch { get; }

        // Damage
        public float BaseDamage { get; }
        public float Damage { get; }

        // Modifiers
        public float FlatAccumulator { get; }
        public Stat Multiplier { get; }
        public float ArmorPenetration { get; }

        // Results & State
        public float HealthDamageDealt { get; }
        public float ShieldDamageDealt { get; }
        public float OverflowDamage => Mathf.Max(0f, Damage - DamageDealt);
        public float DamageDealt => HealthDamageDealt + ShieldDamageDealt;
        public float UncappedDamageDealt => DamageDealt + OverflowDamage;

        // Output
        public bool DestroyedShield { get; }
        public bool DeathBlow { get; }
    }

    public interface IDamageEvent
    {
        // Context
        public Damageable Receiver { get; }
        public GameObject SourceUser { get; }
        public GameObject SourceObject { get; }
        public Vector2 Direction { get; }
        public float ProcCoefficient { get; }

        // Flags
        public bool IsHit { get; }
        public bool IsLethal { get; }
        public bool IgnoresShield { get; }
        public bool IsRejected { get; }

        // Data
        public List<ItemData> ProcChainBranch { get; }

        // Damage
        public float BaseDamage { get; }
        public float Damage { get; }

        // Modifiers
        public float FlatAccumulator { get; }
        public Stat Multiplier { get; }
        public float ArmorPenetration { get; }

        // Results & State
        public float HealthDamageDealt { get; }
        public float ShieldDamageDealt { get; }
        public float OverflowDamage => Mathf.Max(0f, Damage - DamageDealt);
        public float DamageDealt => HealthDamageDealt + ShieldDamageDealt;
        public float UncappedDamageDealt => DamageDealt + OverflowDamage;

        // Output
        public bool DestroyedShield { get; }
        public bool DeathBlow { get; }

        public DamageEventSnapshot GetSnapshot();
    }
}