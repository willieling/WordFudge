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
            public MonoBehaviour Element;

            public Vector2Int Index { get; private set; }
            public Vector2 Position { get; private set; }

            public Cell(float xPosition, float yPosition, int column, int row)
            {
                Position = new Vector2(xPosition, yPosition);
                Index = new Vector2Int(column, row);
                Element = null;
            }
        }

        [SerializeField]
        private Vector2Int cellSize = new Vector2Int(1,1);
        [SerializeField]
        private Vector2Int cellGap = new Vector2Int(1, 1);
        [SerializeField]
        private CollisionResolution collisionResolution;

        private RectTransform rectTransform;

        // (0,0) is the bottom left corner
        private Cell[][] cells;

        private readonly Dictionary<MonoBehaviour, Vector2Int> elementMap = new Dictionary<MonoBehaviour, Vector2Int>();

        private Vector2Int dimensions;

        public Vector2Int Dimensions { get { return dimensions; } }

        private void Start()
        {
            const int AVOID_ZERO = 1;

            rectTransform = GetComponent<RectTransform>();

            Vector2 worldScale = new Vector2(transform.lossyScale.x, transform.lossyScale.y);

            float width = rectTransform.rect.width * worldScale.x;
            float height = rectTransform.rect.height * worldScale.y;

            float totalCellWidth = (cellSize.x + cellGap.x) * worldScale.x;
            float totalCellHeight = (cellSize.y + cellGap.y) * worldScale.y;

            dimensions.x = Mathf.Max((int)(width / totalCellWidth), AVOID_ZERO);
            dimensions.y = Mathf.Max((int)(height / totalCellHeight), AVOID_ZERO);

            cells = new Cell[dimensions.x][];
            for (int x = 0; x < dimensions.x; ++x)
            {
                cells[x] = new Cell[dimensions.y];
                for (int y = 0; y < dimensions.y; y++)
                {
                    float xPosition = rectTransform.position.x + totalCellWidth * GetXOffset(x);
                    float yPosition = rectTransform.position.y + totalCellHeight * GetYOffset(y);
                    cells[x][y] = new Cell(xPosition, yPosition, x, y);
                }
            }

            float GetXOffset(int x)
            {
                int lastIndex = dimensions.x - 1;
                return x - (float)lastIndex / 2;
            }

            float GetYOffset(int y)
            {
                int lastIndex = dimensions.y - 1;
                return y - (float)lastIndex / 2;
            }
        }

        public Vector2Int AddChild(MonoBehaviour child, CollisionResolution resolution)
        {
            Vector2Int index = GetNextCellIndex(child, resolution);

            child.transform.SetParent(this.transform);
            child.transform.position = cells[index.x][index.y].Position;

            if(elementMap.ContainsKey(child))
            {
                //todo show error, child should not be added twice
                RemoveChild(child);
            }

            cells[index.x][index.y].Element = child;
            elementMap.Add(child, index);

            return index;
        }

        public Vector2Int RemoveChild(MonoBehaviour child)
        {
            if(!elementMap.TryGetValue(child, out Vector2Int index))
            {
                //todo error
            }

            cells[index.x][index.y].Element = null;
            elementMap.Remove(child);

            child.transform.SetParent(null);

            return index;
        }

        public T GetElementAtIndex<T>(int x, int y) where T : MonoBehaviour
        {
            if (0 <= x
                && x < dimensions.x
                && 0 <= y
                && y < dimensions.y)
            {
                return (T)cells[x][y].Element;
            }

            return null;
        }

        private Vector2Int GetNextCellIndex(MonoBehaviour child, CollisionResolution resolution)
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
            foreach (Cell[] column in cells)
            {
                foreach (Cell cell in column)
                {
                    if (cell.Element == null)
                    {
                        return cell.Index;
                    }
                }
            }

            return new Vector2Int(-1, -1);
        }

        private Vector2Int GetClosestFreeIndex(MonoBehaviour child)
        {
            Vector2 childPosition = child.transform.position;
            float minDistance = float.MaxValue;
            Vector2Int? index = null;

            foreach (Cell[] column in cells)
            {
                foreach (Cell cell in column)
                {
                    Vector2 distance = new Vector2(childPosition.x, childPosition.y) - cell.Position;
                    if (cell.Element == null && distance.magnitude < minDistance)
                    {
                        minDistance = distance.magnitude;
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
                foreach(Cell[] column in cells)
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
