public class WaveWaitState : State<WaveStateContext>
{
    public readonly WaveStateMachine stateMachine;

    public WaveWaitState(WaveStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public override void DestroyState(WaveStateContext context) { }

    public override void EnterState(WaveStateContext context) { }

    public override void ExitState(WaveStateContext context) { }

    public override void UpdateState(WaveStateContext context)
    {
        if (context.manager.ActiveSpawnables == 0)
        {
            stateMachine.ChangeState(stateMachine.loadState);
        }
    }
}