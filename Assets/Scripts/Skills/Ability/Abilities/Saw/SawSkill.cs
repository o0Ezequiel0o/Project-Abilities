using UnityEngine;

public class SawSkill : AbilityBase
{
    public override AbilityData Data => data;

    private readonly SawSkillData data;
    private readonly GameObject source;

    private readonly Stat damage;
    private readonly Stat damageCooldown;

    private Collider2D[] hits;
    private float timer = 0f;

    private GameObject sawInstance;

    private bool enabledThisFrame = false;
    private bool showSawThisFrame = false;

    public SawSkill(GameObject source, AbilityController controller, SawSkillData data, Stat cooldownTime, Stat damage, Stat damageCooldown) : base(controller, cooldownTime)
    {
        this.source = source;
        this.data = data;

        this.damage = damage;
        this.damageCooldown = damageCooldown;
    }

    public override bool CanActivate()
    {
        return !DurationActive;
    }

    public override bool CanDeactivate()
    {
        return DurationActive;
    }

    public override void OnDestroy()
    {
        if (sawInstance == null) return;
        GameObject.Destroy(sawInstance);
    }

    protected override void Awake()
    {
        if (data.SawPrefab != null)
        {
            sawInstance = GameObject.Instantiate(data.SawPrefab, source.transform.position, Quaternion.identity);
        }
    }

    protected override void OnActivation()
    {
        if (enabledThisFrame) return;

        showSawThisFrame = true;

        if (timer > damageCooldown.Value)
        {
            enabledThisFrame = true;
            UpdateSawCollision();
            timer = 0f;
        }
    }

    protected override void OnDeactivation()
    {
        HideSawVisual();
    }

    protected override void UpdateAll()
    {
        timer += Time.deltaTime;
    }

    protected override void LateUpdateAll()
    {
        enabledThisFrame = false;

        if (showSawThisFrame)
        {
            DisplaySawVisual();
            UpdateSawPosition();
            showSawThisFrame = false;
        }
        else
        {
            HideSawVisual();
        }
    }

    protected override void UpdateActive()
    {
        TryDeactivate();
    }

    protected override bool CanUpgrade()
    {
        return !DurationActive;
    }

    protected override void UpgradeInternal()
    {
        UpgradeStats();
    }

    private void UpdateSawCollision()
    {
        hits = Physics2D.OverlapCircleAll(GetCastPosition(), data.DamageRadius, data.HitLayers);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].gameObject == source) continue;

            if (!IsBlockedByObstacle(controller.CastWorldPosition, hits[i].transform.position))
            {
                HitIfEnemy(hits[i].gameObject);
            }
        }
    }

    private bool IsBlockedByObstacle(Vector3 start, Vector3 end)
    {
        return Physics2D.Linecast(start, end, data.BlockLayers);
    }

    private void HitIfEnemy(GameObject gameObject)
    {
        if (TeamManager.IsAlly(source, gameObject)) return;

        if (gameObject.TryGetComponent(out Damageable damageable))
        {
            damageable.DealDamage(new DamageInfo(damage.Value, data.ArmorPenetration, data.ProcCoefficient), source, source);
        }

        bool statusEffectRollSuccess = data.StatusEffectProcChance > Random.Range(0, 100);

        if (statusEffectRollSuccess && gameObject.TryGetComponent(out StatusEffectHandler statusEffectHandler))
        {
            statusEffectHandler.ApplyEffect(data.StatusEffectToApply, source);
        }
    }

    private void UpdateSawPosition()
    {
        sawInstance.transform.position = GetCastPosition();
    }

    private void DisplaySawVisual()
    {
        if (sawInstance == null) return;
        if (sawInstance.activeSelf) return;

        sawInstance.SetActive(true);
    }

    private void HideSawVisual()
    {
        if (sawInstance == null) return;
        if (!sawInstance.activeSelf) return;

        sawInstance.SetActive(false);
    }

    private Vector3 GetCastPosition()
    {
        return controller.CastWorldPosition + (controller.CastDirection * data.CastDistanceAway);
    }

    private void UpgradeStats()
    {
        damage.Upgrade();
        damageCooldown.Upgrade();
    }
}