public abstract class PassiveBase : IPassive
{
    public abstract PassiveData Data { get; }

    public int Level { get; private set; } = 1;

    protected PassiveController controller;

    public PassiveBase(PassiveController passiveController)
    {
        controller = passiveController;
    }

    public void Initialize()
    {
        Awake();
    }

    public virtual void Awake() { }

    public virtual void OnRemove() { }

    public virtual void Update() { }

    public void Upgrade()
    {
        Level += 1;
        UpgradeInternal();
    }

    public virtual void UpgradeInternal() { }
}