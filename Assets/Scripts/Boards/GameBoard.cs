using UnityEngine;

namespace WordFudge.Boards
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class GameBoard : BaseBoard
    {
        public void AddTile(WorldTile tile)
        {
            AddTileToGrid(tile, SnapGrid.CollisionResolution.ClosestFreeCell);
        }

        public void RemoveTile(WorldTile tile)
        {
            RemoveTileToGrid(tile);
        }
    }
}
