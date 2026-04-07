using UnityEngine;
using Zeke.PoolableGameObjects;

public class ParticleController : MonoBehaviour, IPoolableGameObjectConfirmator
{
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private Vector3 offset;

    [Space]

    [SerializeField] private StartTrigger start = StartTrigger.OnEnable;
    [SerializeField] private StopTrigger stop = StopTrigger.Finished;

    public bool CanGetPoolable
    {
        get
        {
            if (particleInstance == null)
            {
                return true;
            }

            return !particleInstance.IsAlive();
        }
    }

    private ParticleSystem particleInstance;

    public void OnRetrievedFromPool() { }

    private void Awake()
    {
        particleInstance = Instantiate(particles);
        particleInstance.gameObject.SetActive(false);

        ParticleSystem.MainModule main = particleInstance.main;
        main.stopAction = ParticleSystemStopAction.Disable;
    }

    private void Start()
    {
        if (particleInstance == null) return;
        TriggerStartEvent(StartTrigger.Start);
        TriggerStopEvent(StopTrigger.Start);
    }

    private void OnEnable()
    {
        if (particleInstance == null) return;
        TriggerStartEvent(StartTrigger.OnEnable);
        TriggerStopEvent(StopTrigger.OnEnable);
    }

    private void OnDisable()
    {
        if (particleInstance == null) return;
        TriggerStartEvent(StartTrigger.OnDisable);
        TriggerStopEvent(StopTrigger.OnDisable);
    }

    private void OnDestroy()
    {
        if (particleInstance == null) return;
        TriggerStartEvent(StartTrigger.OnDestroy);
        TriggerStopEvent(StopTrigger.OnDestroy);

        ParticleSystem.MainModule main = particleInstance.main;
        main.stopAction = ParticleSystemStopAction.Destroy;
    }

    private void LateUpdate()
    {
        if (particleInstance == null) return;
        particleInstance.transform.position = transform.position + offset;
    }

    private void TriggerStartEvent(StartTrigger eventType)
    {
        if (start == eventType)
        {
            particleInstance.gameObject.SetActive(true);
            particleInstance.Play();
        }
    }

    private void TriggerStopEvent(StopTrigger eventType)
    {
        if (stop == eventType)
        {
            particleInstance.Stop();
            particleInstance.gameObject.SetActive(false);
        }
    }

    private enum StartTrigger
    {
        Start,
        OnEnable,
        OnDisable,
        OnDestroy,
    }

    private enum StopTrigger
    {
        Start,
        OnEnable,
        OnDisable,
        OnDestroy,
        Finished
    }
}