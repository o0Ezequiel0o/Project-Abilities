using System.Collections.Generic;
using UnityEngine;
using System;

public class HealthBarManager : GameService<HealthBarManager>
{
    [SerializeField, Min(1)] private int poolingFramesSpread = 10;

    private readonly Dictionary<SpriteHealthBar, Stack<SpriteHealthBar>> healthBars = new Dictionary<SpriteHealthBar, Stack<SpriteHealthBar>>();

    private readonly HashSet<int> pendingIDs = new HashSet<int>(256);

    private Stack<DelayedReturnData>[] returnUpdateBuckets;
    private Stack<DelayedGetData>[] getUpdateBuckets;

    private int currentBucketIndex = 0;

    private int uniqueID = int.MinValue;

    private readonly struct DelayedReturnData
    {
        public readonly SpriteHealthBar prefab;
        public readonly SpriteHealthBar instance;

        public DelayedReturnData(SpriteHealthBar prefab, SpriteHealthBar instance)
        {
            this.prefab = prefab;
            this.instance = instance;
        }
    }

    private readonly struct DelayedGetData
    {
        public readonly int id;
        public readonly SpriteHealthBar prefab;

        public readonly Action<SpriteHealthBar> callback;

        public DelayedGetData(int id, SpriteHealthBar prefab, Action<SpriteHealthBar> callback)
        {
            this.id = id;
            this.prefab = prefab;

            this.callback = callback;
        }

        public void Execute(SpriteHealthBar instance)
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

    public int StaggeredGet(SpriteHealthBar prefab, Action<SpriteHealthBar> callback)
    {
        int id = GetNewID();
        pendingIDs.Add(id);
        getUpdateBuckets[GetSmallestBucketIndex(getUpdateBuckets)].Push(new DelayedGetData(id, prefab, callback));

        return id;
    }

    public void StaggeredDisableAndReturn(SpriteHealthBar prefab, SpriteHealthBar instance)
    {
        returnUpdateBuckets[GetSmallestBucketIndex(returnUpdateBuckets)].Push(new DelayedReturnData(prefab, instance));
    }

    public SpriteHealthBar Get(SpriteHealthBar prefab)
    {
        SpriteHealthBar instance;

        if (!healthBars.ContainsKey(prefab))
        {
            healthBars.Add(prefab, new Stack<SpriteHealthBar>());
            instance = Instantiate(prefab);
            instance.gameObject.SetActive(false);

            return instance;
        }

        if (healthBars[prefab].TryPop(out instance))
        {
            instance.ResetValues();
            return instance;
        }

        instance = Instantiate(prefab);
        instance.gameObject.SetActive(false);

        return instance;
    }

    public void Return(SpriteHealthBar prefab, SpriteHealthBar instance)
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