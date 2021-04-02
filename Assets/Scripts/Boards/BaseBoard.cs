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

        protected void AddTileToGrid(WorldTile tile, SnapGrid.CollisionResolution resolution)
        {
            grid.AddChild(tile.gameObject, resolution);
        }

        protected void RemoveTileToGrid(WorldTile tile)
        {
            grid.RemoveChild(tile.gameObject);
            tile.transform.SetParent(dragSpace);
        }
    }
}
