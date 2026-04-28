using System.Collections.Generic;
using UnityEngine;
using Zeke.Graph;

public class LootGenerator : MonoBehaviour
{
    [Header("Generation")]
    [SerializeField] private int spawnCredits;
    [SerializeField] private int spawnDistance;
    [SerializeField] private List<Spawnable> spawnables;

    [Header("Grid Dimentions")]
    public Vector2 gridWorldSize;
    public float nodeDiameter;

    [Header("Grid Colliders")]
    public LayerMask blockLayer;
    public LayerMask boundsLayer;
    public int erosionIterations;

    [Header("Grid Debug")]
    public bool drawGizmos;

    private Graph graph;

    private int currentSpawnCredits = 0;

    private readonly List<Spawnable> affordableSpawnables = new List<Spawnable>();
    private readonly List<Node> unoccupiedNodes = new List<Node>();

    private void Awake()
    {
        currentSpawnCredits = spawnCredits;

        CreateGraph(transform.position);
        graph.GetValidNodes(unoccupiedNodes);

        GenerateLoot();
    }

    private void GenerateLoot()
    {
        //should update the nodes overlap around the element spawned to see how much space it took etc etc

        affordableSpawnables.AddRange(spawnables);

        UpdateAffordableSpawnables();

        while (affordableSpawnables.Count > 0 && unoccupiedNodes.Count > 0)
        {
            SpawnRandomSpawnable();
            UpdateAffordableSpawnables();
        }
    }

    private void UpdateAffordableSpawnables()
    {
        for (int i = affordableSpawnables.Count - 1; i >= 0; i--)
        {
            if (affordableSpawnables[i].Cost > currentSpawnCredits)
            {
                affordableSpawnables.RemoveAt(i);
            }
        }
    }

    private void SpawnRandomSpawnable()
    {
        Node node = unoccupiedNodes[Random.Range(0, unoccupiedNodes.Count)];
        List<Node> blockedNodes = graph.BlockNode(node, spawnDistance);

        for (int i = 0; i < blockedNodes.Count; i++)
        {
            unoccupiedNodes.Remove(blockedNodes[i]);
        }

        Vector3 position = node.position;
        Spawnable spawnable = WeightedSelect.SelectElement(affordableSpawnables);

        Instantiate(spawnable.Prefab, position, Quaternion.identity, transform);
        currentSpawnCredits -= spawnable.Cost;
    }

    private void CreateGraph(Vector3 worldPosition)
    {
        graph = new Graph(worldPosition, blockLayer, boundsLayer, erosionIterations, gridWorldSize, nodeDiameter);
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos || !Application.isPlaying) return;
        graph.DrawGizmos();
    }
}