public abstract class StatusEffect
{
    public abstract StatusEffectData Data { get; }

    public int stacks;

    public abstract void OnApply();

    public abstract void OnStackApply();

    public abstract void OnUpdate();

    public abstract void OnLateUpdate();

    public abstract void OnRemove();

    public abstract void OnDestroy();
}