using System.Collections.Generic;
using UnityEngine;

namespace WordFudge.Boards
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class TileTray : BaseBoard
    {
        [SerializeField]
        private WorldTile tilePrefab;
        [SerializeField]
        private uint startingLettersCount = 20;
        [SerializeField]
        private uint refillLettersCount = 5;

        private readonly TileBag tileBag = new TileBag();

        private readonly HashSet<WorldTile> heldTiles = new HashSet<WorldTile>();

        public void Initialize()
        {
            tileBag.Initialize(startingLettersCount, refillLettersCount);

            foreach (WorldTile tile in heldTiles)
            {
                Destroy(tile);
            }
            heldTiles.Clear();
        }

        public void FillTrayWithStartingHand()
        {
            MakeTiles(tileBag.GetStartingHandLetters());
        }

        public void MakeTiles(char[] letters)
        {
            foreach (char letter in letters)
            {
                WorldTile tile = Instantiate(tilePrefab, this.transform);
                tile.Initialize(letter);

                heldTiles.Add(tile);
                grid.AddChild(tile, SnapGrid.CollisionResolution.NextFreeCell);
            }
        }

        public void AddTile(WorldTile tile)
        {
            if (!heldTiles.Add(tile))
            {
                //todo error
            }

            AddTileToGrid(tile, SnapGrid.CollisionResolution.ClosestFreeCell);
        }

        public void RemoveTile(WorldTile tile)
        {
            if(!heldTiles.Remove(tile))
            {
                //todo error
                return;
            }

            if(heldTiles.Count == 0)
            {
                RefillTray();
            }

            RemoveTileFromGrid(tile);
        }

        private void RefillTray()
        {
            MakeTiles(tileBag.GetRefillLetters());
        }
    }
}
