using UnityEngine;

namespace Zeke.Graph
{
    public class Node
    {
        public bool blocked;

        public Vector2 position;
        public Vector2Int gridPosition;

        public Node(bool blocked, Vector3 position, Vector2Int gridPosition)
        {
            this.blocked = blocked;
            this.position = position;
            this.gridPosition = gridPosition;
        }
    }
}