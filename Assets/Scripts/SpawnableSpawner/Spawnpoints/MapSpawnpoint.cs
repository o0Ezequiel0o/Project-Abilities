using System.Collections.Generic;
using UnityEngine;
using Zeke.Graph;

public class MapSpawnpoint : Spawnpoint
{
    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private float blockRadius = 3f;

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
            Debug.LogWarning($"{nameof(selectedNode)} is null, node should have been assigned before calling spawn.", this);
        }

        return Instantiate(prefab, selectedNode.position, Quaternion.identity);
    }

    protected override bool IsBlocked(ContactFilter2D contactFilter)
    {
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
                return true;
            }

            Node randomNode = avaibleNodes[Random.Range(0, avaibleNodes.Count)];

            validSpawn = Physics2D.OverlapCircle(randomNode.position, blockRadius, contactFilter, hits) == 0;

            if (!validSpawn)
            {
                avaibleNodes.Remove(randomNode);
                blockedNodesNow.Push(randomNode);
            }
            else
            {
                selectedNode = randomNode;
            }
        }

        return false;
    }
}