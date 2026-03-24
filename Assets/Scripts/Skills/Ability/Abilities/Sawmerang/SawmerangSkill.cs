using UnityEngine;

public class SawmerangSkill : ProjectileSkillBase<Projectile>
{
    public override AbilityData Data => data;

    private readonly SawmerangSkillData data;
    private readonly GameObject source;

    private bool projectileReturned = true;
    private readonly AbilityLock abilityLock;

    public SawmerangSkill(GameObject source, AbilityController controller, SawmerangSkillData data, Stat cooldownTime) : base(data, controller, cooldownTime)
    {
        this.source = source;
        this.data = data;

        if (data.AbilityToLock != null)
        {
            abilityLock = new AbilityLock(data.AbilityToLock.AbilityType);
        }
    }

    public override bool CanActivate()
    {
        return !DurationActive && projectileReturned;
    }

    public override bool CanDeactivate()
    {
        return DurationActive;
    }

    public override void OnDestroy()
    {
        if (!projectileReturned)
        {
            UnlockAbility();
        }
    }

    protected override void OnActivation()
    {
        Projectile projectile = LaunchAndGetProjectile(controller.CastWorldPosition, controller.CastDirection, source);
        projectile.onDespawn += OnProjectileDespawn;
        projectileReturned = false;

        if (abilityLock != null)
        {
            LockAbility();
            SubscribeToEvents();
        }
    }

    protected override void OnDeactivation() { }

    protected override void UpdateActive()
    {
        TryDeactivate();
    }

    protected override bool CanUpgrade()
    {
        return true;
    }

    protected override void UpgradeInternal()
    {
        UpgradeProjectileStats();
    }

    private void OnProjectileDespawn(Projectile projectile)
    {
        projectile.onDespawn -= OnProjectileDespawn;
        projectileReturned = true;

        if (abilityLock != null)
        {
            UnlockAbility();
            UnsubscribeFromEvents();
        }
    }

    private void SubscribeToEvents()
    {
        controller.onAbilityRemoved += OnAbilityRemoved;
        controller.onAbilityAdded += OnAbilityAdded;
    }

    private void UnsubscribeFromEvents()
    {
        controller.onAbilityRemoved -= OnAbilityRemoved;
        controller.onAbilityAdded -= OnAbilityAdded;
    }

    private void LockAbility()
    {
        if (controller.TryGetAbility(data.AbilityToLock.AbilityType, out IAbility ability))
        {
            if (ability.Data == data.AbilityToLock)
            {
                controller.AddAbilityLock(abilityLock);
            }
        }
    }

    private void UnlockAbility()
    {
        if (controller.TryGetAbility(data.AbilityToLock.AbilityType, out IAbility ability))
        {
            if (ability.Data == data.AbilityToLock)
            {
                controller.RemoveAbilityLock(abilityLock);
            }
        }
    }

    private void OnAbilityAdded(IAbility ability)
    {
        if (ability.Data == data.AbilityToLock && !projectileReturned)
        {
            controller.AddAbilityLock(abilityLock);
        }
    }

    private void OnAbilityRemoved(IAbility ability)
    {
        if (ability.Data == data.AbilityToLock && !projectileReturned)
        {
            controller.RemoveAbilityLock(abilityLock);
        }
    }
}