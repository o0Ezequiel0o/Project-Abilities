using System.Collections.Generic;
using UnityEngine;

namespace Zeke.Graph
{
    public class Graph
    {
        private readonly Vector3 center;

        private readonly LayerMask blockLayer;
        private readonly LayerMask boundsLayer;

        private readonly int erosionIterations;

        private readonly float nodeRadius;
        private readonly float nodeDiameter;

        private readonly Vector2 gridWorldSize;
        private readonly Vector2Int gridSize;

        private readonly Node[,] grid;

        public int MaxSize
        {
            get
            {
                return gridSize.x * gridSize.y;
            }
        }

        private readonly Collider2D[] cachedOverlap = new Collider2D[1];

        private readonly List<Node> updatingNodesCache = new List<Node>();
        private readonly List<Node> updateNodesCache = new List<Node>();

        public Graph(Vector3 center, LayerMask blockLayer, LayerMask boundsLayer, int erosionIterations, Vector2 gridWorldSize, float nodeDiameter)
        {
            this.center = center;

            this.blockLayer = blockLayer;
            this.boundsLayer = boundsLayer;

            this.erosionIterations = erosionIterations;

            this.gridWorldSize = gridWorldSize;
            this.nodeDiameter = nodeDiameter;

            nodeRadius = nodeDiameter * 0.5f;

            gridSize.x = Mathf.Max(0, Mathf.RoundToInt(gridWorldSize.x / nodeDiameter));
            gridSize.y = Mathf.Max(0, Mathf.RoundToInt(gridWorldSize.y / nodeDiameter));

            grid = new Node[gridSize.x, gridSize.y];

            InitializeGrid();
            CalculateErosion();
        }

        public struct Area
        {
            public List<Node> nodes;
            public Vector2 center;

            public Area(List<Node> nodes, Vector2 center)
            {
                this.nodes = nodes;
                this.center = center;
            }
        }

        public List<Area> GetValidAreas(Vector2 size)
        {
            Vector2Int gridSize = new Vector2Int()
            {
                x = Mathf.CeilToInt(size.x / nodeDiameter),
                y = Mathf.CeilToInt(size.y / nodeDiameter)
            };

            return GetValidAreas(gridSize);
        }

        public List<Area> GetValidAreas(Vector2Int gridSize)
        {
            List<Area> areas = new List<Area>();

            for (int x = 0; x < this.gridSize.x; x++)
            {
                for (int y = 0; y < this.gridSize.y; y++)
                {
                    Vector2Int gridPosition = new Vector2Int(x, y);

                    if (!IsAreaValid(gridPosition, gridSize))
                    {
                        continue;
                    }
                    else
                    {
                        areas.Add(GetArea(gridPosition, gridSize));
                    }
                }
            }

            return areas;
        }

        public void BlockArea(Area area)
        {
            for (int i = 0; i < area.nodes.Count; i++)
            {
                BlockNode(area.nodes[i]);
            }
        }

        public List<Node> GetValidNodes()
        {
            List<Node> validNodes = new List<Node>();

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    if (grid[x, y].blocked) continue;
                    validNodes.Add(grid[x, y]);
                }
            }

            return validNodes;
        }

        public void GetValidNodes(List<Node> list)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    if (grid[x, y].blocked) continue;
                    list.Add(grid[x, y]);
                }
            }
        }

        public Node BlockNode(Node node)
        {
            node.blocked = true;
            return node;
        }

        public List<Node> BlockNode(Node node, int erosion)
        {
            node.blocked = true;

            updatingNodesCache.Clear();
            updateNodesCache.Clear();

            updatingNodesCache.Add(node);

            List<Node> nodesBlocked = new List<Node>();

            for (int i = 0; i < erosion; i++)
            {
                updatingNodesCache.AddRange(updateNodesCache);

                for (int j = 0; j < updatingNodesCache.Count; j++)
                {
                    foreach (Node neighbour in GetNeighbours(grid[updatingNodesCache[j].gridPosition.x, updatingNodesCache[j].gridPosition.y]))
                    {
                        if (!neighbour.blocked)
                        {
                            neighbour.blocked = true;
                            nodesBlocked.Add(neighbour);
                        }

                        updateNodesCache.Add(neighbour);
                    }
                }

                updatingNodesCache.Clear();
            }

            return nodesBlocked;
        }

        public List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (TryGetNeighbour(node, x, y, out Node neighbour))
                    {
                        neighbours.Add(neighbour);
                    }
                }
            }

            return neighbours;
        }

        public bool TryGetNeighbour(Node node, int directionX, int directionY, out Node neighbour)
        {
            neighbour = GetNeighbour(node, directionX, directionY);

            return neighbour != null;
        }

        public bool TryGetNeighbour(Node node, Vector2Int direction, out Node neighbour)
        {
            return TryGetNeighbour(node, direction.x, direction.y, out neighbour);
        }

        public Node GetNeighbour(Node node, int directionX, int directionY)
        {
            if (directionX != 0 || directionY != 0)
            {
                int checkX = node.gridPosition.x + directionX;
                int checkY = node.gridPosition.y + directionY;

                if (NodeInBounds(checkX, checkY))
                {
                    return grid[checkX, checkY];
                }
            }

            return null;
        }

        public Node GetNeighbour(Node node, Vector2Int direction)
        {
            return GetNeighbour(node, direction.x, direction.y);
        }

        public Node WorldPositionToNode(Vector3 nodeWorldPosition)
        {
            if (MaxSize == 0) return null;

            Vector2 rawGridPosition = Vector2.zero;
            Vector2Int gridPosition = Vector2Int.zero;

            rawGridPosition.x = (nodeWorldPosition.x - center.x + gridWorldSize.x * 0.5f) / nodeDiameter;
            rawGridPosition.y = (nodeWorldPosition.y - center.y + gridWorldSize.y * 0.5f) / nodeDiameter;

            gridPosition.x = Mathf.FloorToInt(Mathf.Clamp(rawGridPosition.x, 0, gridSize.x - 1));
            gridPosition.y = Mathf.FloorToInt(Mathf.Clamp(rawGridPosition.y, 0, gridSize.y - 1));

            return grid[gridPosition.x, gridPosition.y];
        }

        public void DrawGizmos()
        {
            Gizmos.DrawWireCube(center, new Vector3(gridWorldSize.x, gridWorldSize.y, 0f));

            foreach (Node node in grid)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(new Vector3(node.position.x, node.position.y, 0f), Vector3.one * nodeDiameter);

                if (node.blocked)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(new Vector3(node.position.x, node.position.y, 0f), 0.5f);
                }
            }
        }

        private void InitializeGrid()
        {
            Vector3 worldBottomLeft = Vector3.zero;
            Vector3 worldPoint = Vector3.zero;

            worldBottomLeft.x = center.x - gridWorldSize.x / 2;
            worldBottomLeft.y = center.y - gridWorldSize.y / 2;

            for (int x = 0; x < gridSize.x; x++)
            {
                worldPoint.x = worldBottomLeft.x + x * nodeDiameter + nodeRadius;

                for (int y = 0; y < gridSize.y; y++)
                {
                    worldPoint.y = worldBottomLeft.y + y * nodeDiameter + nodeRadius;

                    Vector2 boxSize = new Vector2(nodeDiameter, nodeDiameter);

                    ContactFilter2D blockContactFilter = new ContactFilter2D() { layerMask = blockLayer, useLayerMask = true, useTriggers = true };
                    bool blocked = Physics2D.OverlapBox(worldPoint, boxSize, 0f, blockContactFilter, cachedOverlap) > 0;

                    if (boundsLayer.value != 0 && !blocked)
                    {
                        ContactFilter2D boundsContactFilter = new ContactFilter2D() { layerMask = boundsLayer, useLayerMask = true, useTriggers = true };
                        blocked = Physics2D.OverlapBox(worldPoint, boxSize, 0f, boundsContactFilter, cachedOverlap) == 0;
                    }

                    grid[x, y] = new Node(blocked, worldPoint, new Vector2Int(x, y));
                }
            }
        }

        private void CalculateErosion()
        {
            for (int i = 0; i < erosionIterations; i++)
            {
                List<Node> nodesToUpdate = new List<Node>();

                foreach (Node node in grid)
                {
                    if (!node.blocked) continue;

                    foreach (Node neighbour in GetNeighbours(grid[node.gridPosition.x, node.gridPosition.y]))
                    {
                        if (!neighbour.blocked)
                        {
                            nodesToUpdate.Add(neighbour);
                        }
                    }
                }

                foreach (Node nodeToUpdate in nodesToUpdate)
                {
                    nodeToUpdate.blocked = true;
                }
            }
        }

        private bool NodeInBounds(int x, int y)
        {
            if (x >= 0 && x < gridSize.x && y >= 0 && y < gridSize.y)
            {
                return true;
            }

            return false;
        }

        private Area GetArea(Vector2Int corner, Vector2Int size)
        {
            List<Node> nodes = new List<Node>();

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    nodes.Add(grid[corner.x + x, corner.y + y]);
                }
            }

            Vector3 cornerPosition = grid[corner.x, corner.y].position;

            Vector2 centerOffset = new Vector2()
            {
                x = ((nodeDiameter * size.x) * 0.5f) - nodeRadius,
                y = ((nodeDiameter * size.y) * 0.5f) - nodeRadius
            };

            Vector2 center = new Vector2()
            {
                x = cornerPosition.x + centerOffset.x,
                y = cornerPosition.y + centerOffset.y
            };

            return new Area(nodes, center);
        }

        private bool IsAreaValid(Vector2Int corner, Vector2Int size)
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    Vector2Int gridPosition = new Vector2Int(corner.x + x, corner.y + y);

                    if (!NodeInBounds(gridPosition.x, gridPosition.y) || grid[gridPosition.x, gridPosition.y].blocked)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}