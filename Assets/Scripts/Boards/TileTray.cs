using System;
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

        private TileBag tileBag = new TileBag();

        public void Initialize()
        {
            tileBag.Initialize(startingLettersCount, refillLettersCount);
        }

        public void ShowStartingHand()
        {
            MakeTiles(tileBag.GetStartingHandLetters());
        }

        public void MakeTiles(char[] letters)
        {
            foreach (char letter in letters)
            {
                MakeTile(letter);
            }
        }

        public void MakeTile(char letter)
        {
            WorldTile tile = Instantiate(tilePrefab, this.transform);
            tile.Initialize(letter);
        }
    }
}
