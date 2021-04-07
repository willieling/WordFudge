using UnityEngine;

namespace WordFudge.Boards
{
    [RequireComponent(typeof(SnapGrid))]
    public class BaseBoard : MonoBehaviour
    {
        //private?
        [SerializeField]
        protected Transform dragSpace;

        protected SnapGrid grid;

        protected virtual void Awake()
        {
            grid = GetComponent<SnapGrid>();
        }

        protected virtual void Start()
        {
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            RectTransform rectTransform = GetComponent<RectTransform>();

            collider.size = rectTransform.rect.size;
        }

        protected Vector2Int AddTileToGrid(WorldTile tile, SnapGrid.CollisionResolution resolution)
        {
            return grid.AddChild(tile, resolution);
        }

        protected Vector2Int RemoveTileFromGrid(WorldTile tile)
        {
            Vector2Int index = grid.RemoveChild(tile);
            tile.transform.SetParent(dragSpace);
            return index;
        }
    }
}
