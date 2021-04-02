using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace WordFudge
{
    /// <summary>
    /// This class represents a grid of cells.
    /// It will snap parented children to the center of the closest cell.
    /// (0,0) should be in the bottom left
    /// </summary>
    public class SnapGrid : MonoBehaviour
    {
        public enum CollisionResolution
        {
            /// <summary>
            /// Choose the next free cell in by scanning the grid linearly
            /// </summary>
            NextFreeCell,
            /// <summary>
            /// Choose the next free cell based on distance from the added object
            /// </summary>
            ClosestFreeCell,
            /// <summary>
            /// Put added object in the closest cell and shift the next cells linearly to the end
            /// </summary>
            SwapObjects
        }

        [System.Diagnostics.DebuggerDisplay("{Index}: {Position} - {(Object != null ? Object.name : \"null\")}")]
        private struct Cell
        {
            public GameObject Object;
            public int x;
            public Vector2Int Index { get; private set; }
            public Vector2 Position { get; private set; }

            public Cell(float xPosition, float yPosition, int column, int row)
            {
                Position = new Vector2(xPosition, yPosition);
                Index = new Vector2Int(column, row);
                Object = null;
                x = 0;
            }
        }

        [SerializeField]
        private Vector2Int cellSize = new Vector2Int(1,1);
        [SerializeField]
        private Vector2Int cellGap = new Vector2Int(1, 1);
        [SerializeField]
        private CollisionResolution collisionResolution;

        private RectTransform rectTransform;

        private Cell[][] Cells;
        private Vector2Int Dimensions;

        private readonly Dictionary<GameObject, Vector2Int> objectMap = new Dictionary<GameObject, Vector2Int>();

        private void Start()
        {
            const int AVOID_ZERO = 1;

            rectTransform = GetComponent<RectTransform>();

            Vector2 worldScale = new Vector2(transform.lossyScale.x, transform.lossyScale.y);

            float width = rectTransform.rect.width * worldScale.x;
            float height = rectTransform.rect.height * worldScale.y;

            float xTotalWidth = (cellSize.x + cellGap.x) * worldScale.x;
            float yTotalWidth = (cellSize.y + cellGap.y) * worldScale.y;

            Dimensions.x = Mathf.Max((int)(width / xTotalWidth), AVOID_ZERO);
            Dimensions.y = Mathf.Max((int)(height / yTotalWidth), AVOID_ZERO);

            Cells = new Cell[Dimensions.x][];
            for (int x = 0; x < Dimensions.x; ++x)
            {
                Cells[x] = new Cell[Dimensions.y];
                for (int y = 0; y < Dimensions.y; y++)
                {
                    float xPosition = rectTransform.position.x + xTotalWidth * GetXOffset(x);
                    float yPosition = rectTransform.position.y + yTotalWidth * GetYOffset(y);
                    Cells[x][y] = new Cell(xPosition, yPosition, x, y);
                }
            }

            float GetXOffset(int x)
            {
                int lastIndex = Dimensions.x - 1;
                return x - (float)lastIndex / 2;
            }

            float GetYOffset(int y)
            {
                int lastIndex = Dimensions.y - 1;
                return y - (float)lastIndex / 2;
            }
        }

        public void AddChild(GameObject child, CollisionResolution resolution)
        {
            Vector2Int index = GetNextCellIndex(child, resolution);

            child.transform.SetParent(this.transform);
            child.transform.position = Cells[index.x][index.y].Position;

            if(objectMap.ContainsKey(child))
            {
                //todo show error, child should not be added twice
                RemoveChild(child);
            }

            Cells[index.x][index.y].Object = child;
            objectMap.Add(child, index);
        }

        public void RemoveChild(GameObject child)
        {
            if(!objectMap.TryGetValue(child, out Vector2Int index))
            {
                //todo error
            }

            Cells[index.x][index.y].Object = null;
            objectMap.Remove(child);

            child.transform.SetParent(null);
        }

        private Vector2Int GetNextCellIndex(GameObject child, CollisionResolution resolution)
        {
            switch(resolution)
            {
                case CollisionResolution.ClosestFreeCell:
                    return GetClosestFreeIndex(child);
                case CollisionResolution.SwapObjects:
                    return GetClosestIndex();
                case CollisionResolution.NextFreeCell:
                default:
                    return GetNextCellIndex();
            }
        }

        private Vector2Int GetNextCellIndex()
        {
            foreach (Cell[] column in Cells)
            {
                foreach (Cell cell in column)
                {
                    if (cell.Object == null)
                    {
                        return cell.Index;
                    }
                }
            }

            return new Vector2Int(-1, -1);
        }

        private Vector2Int GetClosestFreeIndex(GameObject child)
        {
            float minDistance = float.MaxValue;
            Vector2Int? index = null;
            foreach (Cell[] column in Cells)
            {
                foreach (Cell cell in column)
                {
                    Vector2 distance = new Vector2(child.transform.position.x, child.transform.position.y) - cell.Position;
                    if (cell.Object == null && distance.magnitude < minDistance)
                    {
                        index = cell.Index;
                    }
                }
            }

            //we'll need to handle overflow but just do this for now
            Assert.IsTrue(index.HasValue, "Index should never be null?");

            return index.Value;
        }

        private Vector2Int GetClosestIndex()
        {
            throw new NotImplementedException();
        }

#if UNITY_EDITOR
        [SerializeField]
        bool showDebug = false;

        private void OnDrawGizmos()
        {
            if(showDebug)
            {
                foreach(Cell[] column in Cells)
                {
                    foreach(Cell cell in column)
                    {
                        Gizmos.color = Color.magenta;
                        Gizmos.DrawSphere(cell.Position, 5);
                    }
                }
            }
        }
#endif
    }
}
