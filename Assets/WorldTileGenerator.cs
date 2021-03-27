using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordFudge
{
    public class WorldTileGenerator : MonoBehaviour
    {
        [SerializeField]
        private WorldTile TilePrefab;

        private Transform letterTray;

        private readonly List<WorldTile> tiles = new List<WorldTile>();

        public void Initialize(Transform letterTray)
        {
            this.letterTray = letterTray;
            tiles.Clear();
        }

        public void MakeTiles(char[] letters)
        {
            foreach(char letter in letters)
            {
                MakeTile(letter);
            }
        }

        public void MakeTile(char letter)
        {
            WorldTile tile = Instantiate(TilePrefab, letterTray);
            tile.Initialize(letter);

            tiles.Add(tile);
        }
    }
}
