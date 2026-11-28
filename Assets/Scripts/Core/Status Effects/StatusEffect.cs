public abstract class StatusEffect
{
    public abstract StatusEffectData Data { get; }

    public int stacks;

    public abstract void Initialize();

    public abstract void OnStacksApplied(int stacks);

    public abstract void OnStacksRemoved(int stacks);

    public abstract void OnUpdate();

    public abstract void OnLateUpdate();

    public abstract void OnRemove();

    public abstract void OnDestroy();
}