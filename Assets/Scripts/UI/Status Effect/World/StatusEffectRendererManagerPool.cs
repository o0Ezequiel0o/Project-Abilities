using System.Collections.Generic;
using UnityEngine;

public class StatusEffectRendererManagerPool : GameService<StatusEffectRendererManagerPool>
{
    [SerializeField] private StatusEffectInterface prefab;
    [SerializeField] private int poolingFramesSpread = 10;

    private readonly Stack<StatusEffectInterface> interfaces = new Stack<StatusEffectInterface>();

    private Stack<StatusEffectInterface>[] updateBuckets;

    private int currentBucketIndex = 0;

    public void StaggeredDisableAndReturn(StatusEffectInterface statusEffectInterface)
    {
        updateBuckets[GetSmallestBucketIndex()].Push(statusEffectInterface);
    }

    public StatusEffectInterface Get()
    {
        if (interfaces.Count == 0)
        {
            return Instantiate(prefab, Run.Instance.WorldCanvas.transform);
        }

        StatusEffectInterface statusEffectInterface = interfaces.Pop();
        statusEffectInterface.Clear();

        return statusEffectInterface;
    }

    public void Return(StatusEffectInterface statusEffectInterface)
    {
        interfaces.Push(statusEffectInterface);
    }

    private void Awake()
    {
        updateBuckets = new Stack<StatusEffectInterface>[poolingFramesSpread];

        for (int i = 0; i < poolingFramesSpread; i++)
        {
            updateBuckets[i] = new Stack<StatusEffectInterface>(500);
        }
    }

    private void Update()
    {
        if (updateBuckets == null || updateBuckets.Length == 0) return;

        Stack<StatusEffectInterface> currentBucket = updateBuckets[currentBucketIndex];

        while (currentBucket.Count > 0)
        {
            if (currentBucket.TryPop(out StatusEffectInterface statusEffectInterface))
            {
                statusEffectInterface.gameObject.SetActive(false);
                Return(statusEffectInterface);
            }
        }

        currentBucketIndex = (currentBucketIndex + 1) % updateBuckets.Length;
    }

    private int GetSmallestBucketIndex()
    {
        int smallestIndex = 0;
        int smallestCount = int.MaxValue;

        for (int i = 0; i < updateBuckets.Length; i++)
        {
            if (updateBuckets[i].Count < smallestCount)
            {
                smallestCount = updateBuckets[i].Count;
                smallestIndex = i;
            }
        }
        return smallestIndex;
    }
}