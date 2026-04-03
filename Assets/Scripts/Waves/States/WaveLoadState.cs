using UnityEngine;

public class WaveLoadState : State<WaveStateContext>
{
    private readonly WaveStateMachine stateMachine;

    private float intermissionTimer = 0f;

    public WaveLoadState(WaveStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public override void DestroyState(WaveStateContext context) { }

    public override void EnterState(WaveStateContext context)
    {
        context.manager.LoadNextWave();
        context.manager.GenerateWave();

        intermissionTimer = context.manager.IntermissionLength;
    }

    public override void UpdateState(WaveStateContext context)
    {
        intermissionTimer -= Time.deltaTime;

        int spawnablesToPrepareThisFrame = GetSpawnableAmountToPrepareThisFrame(context);

        for (int i = 0; i < spawnablesToPrepareThisFrame; i++)
        {
            context.manager.InstantiateUnactiveNextSpawnableInQueue();
        }

        if (intermissionTimer <= 0f)
        {
            stateMachine.ChangeState(stateMachine.spawnState);
        }
    }

    public override void LateUpdateState(WaveStateContext context) { }

    public override void ExitState(WaveStateContext context) { }

    private int GetSpawnableAmountToPrepareThisFrame(WaveStateContext context)
    {
        int framesLeft = Mathf.Max(1, Mathf.CeilToInt(intermissionTimer / Time.deltaTime));

        int averageSpawnablesToPrepare = Mathf.CeilToInt(1 / Time.deltaTime) / 30;
        int minSpawnablesToPrepare = Mathf.CeilToInt((float)context.manager.SpawnablesLeftToSpawn / framesLeft);

        if (averageSpawnablesToPrepare * framesLeft > context.manager.SpawnablesLeftToSpawn)
        {
            return averageSpawnablesToPrepare;
        }
        else
        {
            return minSpawnablesToPrepare;
        }
    }
}