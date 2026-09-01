using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Zeke.Graph;

public class MapSpawnpoint : Spawnpoint
{
    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private float blockRadius = 3f;

    [Header("Performance")]
    [SerializeField] private float msFrameBudget = 1f;

    private readonly List<Node> avaibleNodes = new List<Node>(128);
    private readonly Stack<Node> blockedNodesNow = new Stack<Node>(128);
    private readonly List<Node> blockedNodesStored = new List<Node>(128);

    private Node selectedNode = null;

    private void Awake()
    {
        if (mapGenerator == null) return;

        if (mapGenerator.LootGenerated)
        {
            OnLootGenerated();
        }
        else
        {
            mapGenerator.onLootGenerated += OnLootGenerated;
        }
    }

    private void OnLootGenerated()
    {
        mapGenerator.Graph.GetValidNodes(avaibleNodes);
    }

    public override GameObject Spawn(GameObject prefab)
    {
        if (selectedNode == null)
        {
            UnityEngine.Debug.LogWarning($"{nameof(selectedNode)} is null, node should have been assigned before calling spawn.", this);
        }

        return Instantiate(prefab, selectedNode.position, Quaternion.identity);
    }

    protected override void ProcessBlockState(Action<bool> onFinishedProcessing, ContactFilter2D contactFilter)
    {
        StartCoroutine(ProcessBlockStateRoutine(onFinishedProcessing, contactFilter));
    }

    private IEnumerator ProcessBlockStateRoutine(Action<bool> onFinishedProcessing, ContactFilter2D contactFilter)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        bool validSpawn = false;

        while (!validSpawn)
        {
            if (avaibleNodes.Count == 0 && blockedNodesStored.Count > 0)
            {
                avaibleNodes.AddRange(blockedNodesStored);
                blockedNodesStored.Clear();
            }

            if (avaibleNodes.Count == 0)
            {
                avaibleNodes.AddRange(blockedNodesNow);
                blockedNodesStored.AddRange(blockedNodesNow);
                blockedNodesNow.Clear();

                onFinishedProcessing?.Invoke(true);
                yield break;
            }

            int randomIndex = UnityEngine.Random.Range(0, avaibleNodes.Count);
            Node randomNode = avaibleNodes[randomIndex];

            validSpawn = Physics2D.OverlapCircle(randomNode.position, blockRadius, contactFilter, hits) == 0;

            if (!validSpawn)
            {
                avaibleNodes[randomIndex] = avaibleNodes[^1];
                avaibleNodes.RemoveAt(avaibleNodes.Count - 1);
                blockedNodesNow.Push(randomNode);
            }
            else
            {
                selectedNode = randomNode;
            }

            if (stopwatch.Elapsed.TotalMilliseconds >= msFrameBudget)
            {
                yield return null;
                stopwatch.Restart();
                UnityEngine.Debug.Log("mapSpawnpoint processing frame budget reached");
            }
        }

        stopwatch.Stop();
        onFinishedProcessing?.Invoke(false);
    }
}