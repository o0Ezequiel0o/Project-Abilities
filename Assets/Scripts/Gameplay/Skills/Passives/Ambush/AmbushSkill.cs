using UnityEngine;
using Zeke.Abilities;

public class AmbushSkill : PassiveBase
{
    public override PassiveData Data => data;
    private readonly AmbushSkillData data;

    private readonly GameObject source;

    private float timer = 0f;

    private IAbility abilityInstance;
    private AbilityController abilityController;

    private bool hasRequiredComponents = false;

    public AmbushSkill(GameObject source, PassiveController passiveController, AmbushSkillData data) : base(passiveController)
    {
        this.source = source;
        this.data = data;
    }

    public override void Awake()
    {
        hasRequiredComponents = source.TryGetComponent(out abilityController);

        if (hasRequiredComponents)
        {
            abilityController.onAbilityUsed.Subscribe(OnAbilityUsed);
        }
    }

    public override void Update()
    {
        if (!hasRequiredComponents) return;

        AbilityController.AbilityTypeSlot abilityTypeSlot = abilityController.AbilityTypeSlots[data.UpgradeType];

        if (abilityTypeSlot.CurrentAbilitySlot != null && abilityTypeSlot.CurrentAbilitySlot != abilityInstance)
        {
            timer += Time.deltaTime;

            if (timer > data.TimeToActivate)
            {
                AddAbility();
            }
        }
    }

    public override void OnRemove()
    {
        if (!hasRequiredComponents || abilityInstance == null) return;
        abilityController.RemoveAbility(abilityInstance);
    }

    private void AddAbility()
    {
        abilityInstance = abilityController.AddAbility(data.AbilityOverride, data.UpgradeType);
    }

    private void OnThisAbilityUsed()
    {
        if (abilityInstance.Charges == 0)
        {
            abilityController.RemoveAbility(abilityInstance);
            abilityInstance.Destroy();

            if (abilityController.TryGetAbility(data.UpgradeType, out IAbility currentAbility))
            {
                currentAbility.SetCharges(currentAbility.Charges - 1);
            }

            abilityInstance = null;
            timer = 0f;
        }
    }

    private void OnAbilityUsed(IAbility ability)
    {
        if (ability == abilityInstance)
        {
            OnThisAbilityUsed();
        }
        else if (data.CancelTypes.Contains(ability.Type))
        {
            timer = 0f;
        }
    }
}