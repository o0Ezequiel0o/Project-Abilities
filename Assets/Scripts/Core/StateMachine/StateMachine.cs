public abstract class StateMachine<T> where T : class
{
    protected State<T> currentState;

    public abstract void Update();

    public abstract void LateUpdate();

    public abstract void ChangeState(State<T> newState);

    public abstract void Destroy();
}