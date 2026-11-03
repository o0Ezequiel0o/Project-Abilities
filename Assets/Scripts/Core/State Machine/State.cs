public abstract class State<T>
{
    public abstract void ExitState(T context);

    public abstract void EnterState(T context);

    public abstract void UpdateState(T context);

    public abstract void LateUpdateState(T context);

    public abstract void DestroyState(T context);
}