using System;
using System.Collections.Generic;
using UnityEngine;
using WordFudge.ScoreSystem;

namespace WordFudge.Boards
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class GameBoard : BaseBoard
    {
        private readonly ScoreHolder scoreHolder = new ScoreHolder();
        private readonly OptimalTileMatrixSolver solver = new OptimalTileMatrixSolver();

        private readonly HashSet<WorldTile> tilesOnBoard = new HashSet<WorldTile>();

        public IReadOnlyCollection<WorldTile> TilesOnBoard { get { return tilesOnBoard; } }


        //todo, store the history of placed tiles so we can easily handle removing a tile?

        protected override void Awake()
        {
            base.Awake();
            solver.Initialize(this);
        }

        public bool HasTile(WorldTile tile)
        {
            return tilesOnBoard.Contains(tile);
        }

        public void AddTile(WorldTile tile)
        {
            if(!tilesOnBoard.Add(tile))
            {
                Debug.LogError("Trying to add a tile to the board when it's already on the board.");
                return;
            }

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

            TileWordUpdater.UpdateLinesIntersectingAddedTile(tile);

            tile.RegisterToGameplayEvents();

            //make this two steps?
            //first steps set a tile
            //second step performs the calculations?
            //or one step, sets tile which internally calls the public function which performs the calcultions?
            TileMatrixScore scoreMatrix = solver.CalculateBestScoreMatrix(tile);

            scoreHolder.UpdateScore(scoreMatrix);
        }

        public void RemoveTile(WorldTile tile)
        {
            if (!tilesOnBoard.Remove(tile))
            {
                Debug.LogError("Trying to remove a tile from the board when it's not on the board.");
                return;
            }

            RemoveTileFromGrid(tile);
            tile.UnregisterFromGameplayEvents();

            WorldTile.Neighbors neighbours = tile.GetNeighbours();
            tile.ClearNeighbourReferencesAndAssociatedWords();

            scoreHolder.Clear();

            TileMatrixScore scoreMatrix = solver.CalculateBestScoreMatrixFromUnvisitedTile();
            scoreHolder.UpdateScore(scoreMatrix);
        }
    }
}
