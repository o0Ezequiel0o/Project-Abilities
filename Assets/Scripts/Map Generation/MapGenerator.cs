using System.Collections.Generic;
using UnityEngine;
using Zeke.Graph;
using System;

public class MapGenerator : MonoBehaviour
{
    [Header("Generation")]
    [SerializeField] private int spawnCredits;
    [SerializeField] private List<MapSpawnable> spawnables;
    [SerializeField] private List<RequiredSpawnable> requiredSpawnables;

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

    private readonly List<MapSpawnable> affordableSpawnables = new List<MapSpawnable>();

    private void Awake()
    {
        currentSpawnCredits = spawnCredits;

        CreateGraph();
        GenerateLoot();
    }

    private void GenerateLoot()
    {
        affordableSpawnables.AddRange(spawnables);

        SpawnRequiredSpawnables();

        while (affordableSpawnables.Count > 0)
        {
            SpawnRandomSpawnable();
        }
    }

    private void SpawnRandomSpawnable()
    {
        MapSpawnable mapSpawnable = WeightedSelect.SelectElement(affordableSpawnables);

        if (mapSpawnable.spawnable.Cost > currentSpawnCredits)
        {
            affordableSpawnables.Remove(mapSpawnable);
            return;
        }

        List<Graph.Area> areas = graph.GetValidAreas(mapSpawnable.area);
        
        if (areas.Count <= 0)
        {
            affordableSpawnables.Remove(mapSpawnable);
            return;
        }

        Graph.Area area = areas[UnityEngine.Random.Range(0, areas.Count)];
        SpawnSpawnable(area, mapSpawnable);
    }

    private void SpawnRequiredSpawnables()
    {
        for (int i = 0; i < requiredSpawnables.Count; i++)
        {
            for (int j = 0; j < requiredSpawnables[i].amount; j++)
            {
                MapSpawnable mapSpawnable = requiredSpawnables[i].mapSpawnable;
                List<Graph.Area> areas = graph.GetValidAreas(mapSpawnable.area);

                if (areas.Count <= 0)
                {
                    Debug.LogWarning($"Can't spawn required spawnable {mapSpawnable.spawnable.name}", mapSpawnable.spawnable);
                    return;
                }

                Graph.Area area = areas[UnityEngine.Random.Range(0, areas.Count)];
                SpawnSpawnable(area, mapSpawnable);
            }
        }
    }

    private void SpawnSpawnable(Graph.Area area, MapSpawnable mapSpawnable)
    {
        Instantiate(mapSpawnable.spawnable.Prefab, area.center, Quaternion.identity, transform);
        currentSpawnCredits -= mapSpawnable.spawnable.Cost;
        graph.BlockArea(area);
    }

    private void CreateGraph()
    {
        graph = new Graph(transform.position, blockLayer, boundsLayer, erosionIterations, gridWorldSize, nodeDiameter);
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos || !Application.isPlaying) return;
        graph.DrawGizmos();
    }

    [Serializable]
    private struct MapSpawnable : IWeighted
    {
        public Spawnable spawnable;
        public Vector2Int area;

        public int Weight => spawnable.Weight;
    }

    [Serializable]
    private struct RequiredSpawnable
    {
        public MapSpawnable mapSpawnable;
        public int amount;
    }
}