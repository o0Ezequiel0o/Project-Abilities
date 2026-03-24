using UnityEngine;

public class AbilityTemplate : AbilityBase
{
    public override AbilityData Data => data;

    private bool hasRequiredComponents = true;

    private readonly AbilityTemplateData data;
    private readonly GameObject source;

    public AbilityTemplate(GameObject source, AbilityController controller, AbilityTemplateData data, Stat cooldownTime) : base(controller, cooldownTime)
    {
        this.source = source;
        this.data = data;
    }

    public override bool CanActivate()
    {
        return !DurationActive && hasRequiredComponents;
        //If the ability is in a state where it can be activated
    }

    public override bool CanDeactivate()
    {
        return DurationActive && hasRequiredComponents;
        //If the ability is in a state where it can be deactivated
    }

    protected override void Awake()
    {
        LookForComponents();
        //Called after Initialization, when the ability is created
    }

    public override void OnDestroy()
    {
        //When the ability is destroyed, delete world objects and other data instantiated by the ability
    }

    private void LookForComponents()
    {
        //Look for components required by this ability
        hasRequiredComponents = true;
    }

    protected override void OnActivation()
    {
        //Called when the ability is activated
    }

    protected override void OnDeactivation()
    {
        //Called when the ability is deactivated
    }

    protected override void OnDurationFinished()
    {
        //Called when the ability duration finishes
    }

    protected override void UpdateAll()
    {
        //Called every frame
    }

    protected override void UpdateActive()
    {
        TryDeactivate();
        //Called every frame the ability is active
    }
    
    protected override void UpdateUnactive()
    {
        //Called every frame the ability is unactive
    }

    protected override bool CanUpgrade()
    {
        return !DurationActive;
        //If the ability is in a state where it can be upgraded
    }

    protected override void UpgradeInternal()
    {
        //Called when the ability is upgraded
        UpgradeStats();
    }

    private void UpgradeStats()
    {
        //Upgrade local stats
    }
}