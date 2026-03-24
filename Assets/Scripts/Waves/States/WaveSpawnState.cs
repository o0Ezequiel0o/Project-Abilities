using UnityEngine;

public class WaveSpawnState : State<WaveStateContext>
{
    private readonly WaveStateMachine stateMachine;

    private float timer = 0f;

    public WaveSpawnState(WaveStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public override void DestroyState(WaveStateContext context) { }

    public override void EnterState(WaveStateContext context)
    {
        timer = 0f;
    }

    public override void ExitState(WaveStateContext context) { }

    public override void UpdateState(WaveStateContext context)
    {
        if (context.manager.UnactiveSpawnables == 0)
        {
            stateMachine.ChangeState(stateMachine.waitState);
        }

        UpdateSpawnTimer(context);
    }

    private void UpdateSpawnTimer(WaveStateContext context)
    {
        timer += Time.deltaTime;

        if (timer >= context.manager.LoadedWaveUpdate.SpawnCooldown)
        {
            context.manager.ActivateNextUnactiveInstantiatedSpawnable();
            timer = 0f;
        }
    }
}