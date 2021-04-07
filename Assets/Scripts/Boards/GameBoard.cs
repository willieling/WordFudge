using UnityEngine;

namespace WordFudge.Boards
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class GameBoard : BaseBoard
    {
        public void AddTile(WorldTile tile)
        {
            Vector2Int tileIndex = AddTileToGrid(tile, SnapGrid.CollisionResolution.ClosestFreeCell);

            WorldTile neighbour = grid.GetElementAtIndex<WorldTile>(tileIndex.x + 1, tileIndex.y);
            if(neighbour != null)
            {
                tile.Right = neighbour;
                neighbour.Left = tile;
            }

            neighbour = grid.GetElementAtIndex<WorldTile>(tileIndex.x - 1, tileIndex.y);
            if (neighbour != null)
            {
                tile.Left = neighbour;
                neighbour.Right = tile;
            }

            neighbour = grid.GetElementAtIndex<WorldTile>(tileIndex.x, tileIndex.y + 1);
            if (neighbour != null)
            {
                tile.Up = neighbour;
                neighbour.Down = tile;
            }

            neighbour = grid.GetElementAtIndex<WorldTile>(tileIndex.x, tileIndex.y - 1);
            if (neighbour != null)
            {
                tile.Down = neighbour;
                neighbour.Up = tile;
            }

            //GenerateOptimalBoard(tileIndex)
        }

        public void RemoveTile(WorldTile tile)
        {
            RemoveTileFromGrid(tile);
            tile.ClearNeighbourReferences();
        }
    }
}
