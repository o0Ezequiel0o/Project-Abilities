using System.Collections.Generic;
using UnityEngine;
using System;

public class HealthBarManager : GameService<HealthBarManager>
{
    [SerializeField, Min(1)] private int poolingFramesSpread = 10;

    private readonly Dictionary<HealthBar, Stack<HealthBar>> healthBars = new Dictionary<HealthBar, Stack<HealthBar>>();

    private readonly HashSet<int> pendingIDs = new HashSet<int>(256);

    private Stack<DelayedReturnData>[] returnUpdateBuckets;
    private Stack<DelayedGetData>[] getUpdateBuckets;

    private int currentBucketIndex = 0;

    private int uniqueID = int.MinValue;

    private readonly struct DelayedReturnData
    {
        public readonly HealthBar prefab;
        public readonly HealthBar instance;

        public DelayedReturnData(HealthBar prefab, HealthBar instance)
        {
            this.prefab = prefab;
            this.instance = instance;
        }
    }

    private readonly struct DelayedGetData
    {
        public readonly int id;
        public readonly HealthBar prefab;

        public readonly Action<HealthBar> callback;

        public DelayedGetData(int id, HealthBar prefab, Action<HealthBar> callback)
        {
            this.id = id;
            this.prefab = prefab;

            this.callback = callback;
        }

        public void Execute(HealthBar instance)
        {
            callback?.Invoke(instance);
        }
    }

    public void CancelGet(int id)
    {
        if (pendingIDs.Contains(id))
        {
            pendingIDs.Remove(id);
        }
    }

    public int StaggeredGet(HealthBar prefab, Action<HealthBar> callback)
    {
        int id = GetNewID();
        pendingIDs.Add(id);
        getUpdateBuckets[GetSmallestBucketIndex(getUpdateBuckets)].Push(new DelayedGetData(id, prefab, callback));

        return id;
    }

    public void StaggeredDisableAndReturn(HealthBar prefab, HealthBar instance)
    {
        returnUpdateBuckets[GetSmallestBucketIndex(returnUpdateBuckets)].Push(new DelayedReturnData(prefab, instance));
    }

    public HealthBar Get(HealthBar prefab)
    {
        HealthBar instance;

        if (!healthBars.ContainsKey(prefab))
        {
            healthBars.Add(prefab, new Stack<HealthBar>());
            instance = Instantiate(prefab, Run.Instance.WorldCanvas.transform);
            instance.gameObject.SetActive(false);

            return instance;
        }

        if (healthBars[prefab].TryPop(out instance))
        {
            instance.ResetValues();
            return instance;
        }

        instance = Instantiate(prefab, Run.Instance.WorldCanvas.transform);

        return instance;
    }

    public void Return(HealthBar prefab, HealthBar instance)
    {
        healthBars[prefab].Push(instance);
    }

    private void Awake()
    {
        returnUpdateBuckets = new Stack<DelayedReturnData>[poolingFramesSpread];
        getUpdateBuckets = new Stack<DelayedGetData>[poolingFramesSpread];

        for (int i = 0; i < poolingFramesSpread; i++)
        {
            returnUpdateBuckets[i] = new Stack<DelayedReturnData>(150);
            getUpdateBuckets[i] = new Stack<DelayedGetData>(150);
        }
    }

    private void Update()
    {
        UpdateReturnBucket();
        UpdateGetBucket();

        currentBucketIndex = (currentBucketIndex + 1) % returnUpdateBuckets.Length;
    }

    private void UpdateReturnBucket()
    {
        if (returnUpdateBuckets == null || returnUpdateBuckets.Length == 0) return;

        Stack<DelayedReturnData> currentBucket = returnUpdateBuckets[currentBucketIndex];

        while (currentBucket.Count > 0)
        {
            if (currentBucket.TryPop(out DelayedReturnData data))
            {
                data.instance.gameObject.SetActive(false);
                Return(data.prefab, data.instance);
            }
        }
    }

    private void UpdateGetBucket()
    {
        if (getUpdateBuckets == null || getUpdateBuckets.Length == 0) return;

        Stack<DelayedGetData> currentBucket = getUpdateBuckets[currentBucketIndex];

        while (currentBucket.Count > 0)
        {
            if (currentBucket.TryPop(out DelayedGetData data))
            {
                if (!pendingIDs.Contains(data.id)) continue;

                pendingIDs.Remove(data.id);
                data.Execute(Get(data.prefab));
            }
        }
    }

    private int GetSmallestBucketIndex<T>(Stack<T>[] buckets)
    {
        int smallestIndex = 0;
        int smallestCount = int.MaxValue;

        for (int i = 0; i < buckets.Length; i++)
        {
            if (buckets[i].Count < smallestCount)
            {
                smallestCount = buckets[i].Count;
                smallestIndex = i;
            }
        }
        return smallestIndex;
    }

    private int GetNewID() => ++uniqueID;
}