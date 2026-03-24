using UnityEngine;
using System;

[Serializable]
public abstract class AbilityModule
{
    public abstract void OnInitialization(ModularAbilityController controller, Transform spawn, GameObject source, ModularAbility ability);

    public abstract AbilityModule CreateDeepCopy();

    public abstract void Activate(bool holding);
    public virtual void Deactivate() { }

    public abstract bool CanDeactivate();
    public abstract bool CanActivate();
    public abstract bool CanUpgrade();

    public virtual void OnDurationFinished() { }

    public virtual void UpdateActive() { }
    public virtual void UpdateUnactive() { }

    public virtual void Update() { }
    public virtual void LateUpdate() { }

    public virtual void Upgrade() { }
    public virtual void Destroy() { }
}