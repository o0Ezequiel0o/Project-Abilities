using UnityEngine;

public class PassiveTemplate : PassiveBase
{
    public override PassiveData Data => data;
    private readonly PassiveTemplateData data;

    private readonly GameObject source;

    private bool hasRequiredComponents = false;

    public PassiveTemplate(GameObject source, PassiveController passiveController, PassiveTemplateData data) : base(passiveController)
    {
        this.source = source;
        this.data = data;
    }

    public override void Awake()
    {
        LookForComponents();
    }

    public override void Update()
    {
        if (!hasRequiredComponents) return;
    }

    public override void OnRemove() { }

    public override void UpgradeInternal() { }

    void LookForComponents()
    {
        hasRequiredComponents = true;
    }
}