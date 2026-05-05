using System.Collections.Generic;
using UnityEngine;
using Zeke.TeamSystem;

public class DefenseSystemSkill : PassiveBase
{
    public override PassiveData Data => data;
    private readonly DefenseSystemSkillData data;

    private readonly GameObject source;
    private Damageable damageable;

    private bool hasRequiredComponents = false;

    private readonly Stat radius;

    private GameObject visualInstance;

    private float timer = 0f;

    private readonly List<Collider2D> hits = new List<Collider2D>();

    public DefenseSystemSkill(GameObject source, PassiveController passiveController, DefenseSystemSkillData data, Stat radius) : base(passiveController)
    {
        this.source = source;
        this.data = data;

        this.radius = radius;
    }

    public override void Awake()
    {
        LookForComponents();
        CreateVisual();
        AdjustVisualSize();
    }

    public override void Update()
    {
        if (!hasRequiredComponents) return;

        timer += Time.deltaTime;

        if (timer >= data.DamageInterval)
        {
            DealDamage();
            timer = 0f;
        }
    }

    public override void LateUpdate()
    {
        if (visualInstance == null) return;
        visualInstance.transform.position = source.transform.position;
    }

    public override void OnRemove()
    {
        if (visualInstance == null) return;
        GameObject.Destroy(visualInstance);
    }

    protected override void UpgradeInternal()
    {
        radius.Upgrade();
        AdjustVisualSize();
    }

    private void LookForComponents()
    {
        hasRequiredComponents = source.TryGetComponent(out damageable);
    }

    private void CreateVisual()
    {
        visualInstance = GameObject.Instantiate(data.AreaVisual, source.transform.position, Quaternion.identity);
    }

    private void AdjustVisualSize()
    {
        if (visualInstance == null) return;
        visualInstance.transform.localScale = 2f * radius.Value * Vector3.one;
    }

    private void DealDamage()
    {
        float damage = Mathf.Max(data.MinDamage, damageable.MaxShield.Value * data.MaxShieldDamageRatio);

        ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = data.HitLayers, useLayerMask = true };
        for (int i = 0; i < Physics2D.OverlapCircle(source.transform.position, radius.Value, contactFilter, hits); i++)
        {
            GameObject receiver = hits[i].gameObject;

            if (receiver == source) continue;
            if (TeamManager.IsAlly(source, receiver)) continue;

            if (receiver.TryGetComponent(out Damageable damageable))
            {
                Vector2 direction = (receiver.transform.position - source.transform.position).normalized;
                DamageInfo damageInfo = new DamageInfo(damage, data.ArmorPenetration, data.ProcCoefficient)
                {
                    hit = true,
                    direction = direction
                };
                damageable.DealDamage(damageInfo, source, source);
            }
        }
    }
}