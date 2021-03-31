using System.Collections.Generic;
using UnityEngine;

namespace WordFudge.Boards
{
    public class TileTray : MonoBehaviour
    {
        [SerializeField]
        private WorldTile tilePrefab;
        [SerializeField]
        private uint startingLettersCount = 20;
        [SerializeField]
        private uint refillLettersCount = 5;
        [SerializeField]
        private SnapGrid grid;

        private TileBag tileBag = new TileBag();

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
                grid.AddChild(tile.gameObject, SnapGrid.CollisionResolution.NextFreeCell);
            }
        }

        public void RemoveTile(WorldTile tile)
        {
            if(!heldTiles.Remove(tile))
            {
                //todo error
            }

            if(heldTiles.Count == 0)
            {
                RefillTray();
            }
        }

        public void AddTile(WorldTile tile)
        {
            if(!heldTiles.Add(tile))
            {
                //todo error
            }
        }

        private void RefillTray()
        {
            MakeTiles(tileBag.GetRefillLetters());
        }
    }
}
