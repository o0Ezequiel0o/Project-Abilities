using System.Collections.Generic;
using UnityEngine;

public class EntitySeparationController : GameService<EntitySeparationController>
{
    [Tooltip("Each group is equal to 1 more frame to iterate all entities.\nRecommended: 3")]
    [SerializeField, Min(1)] private int groups = 3;

    private List<EntitySeparation>[] updateBuckets;

    private int currentBucketIndex = 0;

    protected override void InitializeService()
    {
        updateBuckets = new List<EntitySeparation>[groups];

        for (int i = 0; i < groups; i++)
        {
            updateBuckets[i] = new List<EntitySeparation>(500);
        }
    }

    /// <summary>
    /// Registers a new entity into the bucket that currently has the fewest entities.
    /// </summary>
    public int Register(EntitySeparation entity)
    {
        int targetBucket = Instance.GetSmallestBucketIndex();
        Instance.updateBuckets[targetBucket].Add(entity);
        return targetBucket;
    }

    /// <summary>
    /// Removes the entity from the registered bucket.
    /// </summary>
    public void Unregister(EntitySeparation entity, int bucketIndex)
    {
        if (bucketIndex >= 0 && bucketIndex < Instance.updateBuckets.Length)
        {
            Instance.updateBuckets[bucketIndex].Remove(entity);
        }
    }

    private void FixedUpdate()
    {
        if (updateBuckets == null || updateBuckets.Length == 0) return;

        List<EntitySeparation> currentBucket = updateBuckets[currentBucketIndex];

        for (int i = 0; i < currentBucket.Count; i++)
        {
            if (currentBucket[i] != null)
            {
                currentBucket[i].UpdateSeparation();
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