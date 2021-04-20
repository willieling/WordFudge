using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Assertions;

namespace WordFudge.ScoreSystem
{
    /// <summary>
    /// A data container that holds visited words
    /// </summary>
    [DebuggerDisplay("Last word: {lastWord}")]
    public class TileMatrix
    {
        private WordContainer lastWord = null;
        private WordContainer previousLastWord = null;

        private readonly HashSet<WorldTile> visitedTiles = new HashSet<WorldTile>();
        private readonly HashSet<WorldTile> globalVisistedTiles = new HashSet<WorldTile>();

        private readonly HashSet<WordContainer> horizontalWords = new HashSet<WordContainer>();
        private readonly HashSet<WordContainer> verticalWords = new HashSet<WordContainer>();

        public WordContainer LastAddedWord
        {
            get { return lastWord; } 
            set 
            {
                previousLastWord = lastWord;
                lastWord = value;
            }
        }
        public WordContainer PreviousLastAddedWord { get { return previousLastWord; } }

        public IReadOnlyCollection<WordContainer> HorizontalWords { get { return horizontalWords; } }
        public IReadOnlyCollection<WordContainer> VerticalWords { get { return verticalWords; } }

        public TileMatrix(WordContainer word, HashSet<WorldTile> globalVisistedTiles)
        {
            this.globalVisistedTiles = globalVisistedTiles;
            switch (word.Axis)
            {
                case Axis.Horizontal:
                    AddHorizontalWord(word);
                    break;
                case Axis.Vertical:
                    AddVerticalWord(word);
                    break;
            }
        }

        private TileMatrix(WordContainer lastWord, HashSet<WorldTile> globalVisistedTiles, HashSet<WorldTile> visitedTiles, HashSet<WordContainer> horizontalWords, HashSet<WordContainer> verticalWords)
        {
            this.lastWord = lastWord;
            this.globalVisistedTiles = globalVisistedTiles;
            this.visitedTiles = new HashSet<WorldTile>(visitedTiles);
            this.horizontalWords = new HashSet<WordContainer>(horizontalWords);
            this.verticalWords = new HashSet<WordContainer>(verticalWords);
        }

        /// <summary>
        /// Deep clone the data structures declared within this class but not their elements.
        /// </summary>
        /// <returns></returns>
        public TileMatrix DeepClone()
        {
            return new TileMatrix(lastWord, visitedTiles, globalVisistedTiles, horizontalWords, verticalWords);
        }

        public void AddHorizontalWord(WordContainer word)
        {
            Assert.IsTrue(word.Axis == Axis.Horizontal, $"Trying to add {word} to the horizontal words.");
            AddWord(word, horizontalWords);
        }

        public void AddVerticalWord(WordContainer word)
        {
            Assert.IsTrue(word.Axis == Axis.Vertical, $"Trying to add {word} to the vertical words.");
            AddWord(word, verticalWords);
        }

        public bool HasVisitedTile(WorldTile tile)
        {
            return visitedTiles.Contains(tile);
        }

        public bool HasVisitedWord(WordContainer word)
        {
            return horizontalWords.Contains(word)
                || verticalWords.Contains(word);
        }

        public TileMatrixScore GetTileMatrixScore()
        {
            return new TileMatrixScore(visitedTiles);
        }

        private void AddWord(WordContainer word, HashSet<WordContainer> wordHashSet)
        {
            if (!wordHashSet.Add(word))
            {
                throw new Exception("We should never re-vist a word.");
            }

            LastAddedWord = word;

            foreach (WorldTile tile in word.Tiles)
            {
                visitedTiles.Add(tile);
                globalVisistedTiles.Add(tile);
            }
        }
    }

    /// <summary>
    /// A container holding a Score value and collection of tiles representing a 
    /// valid grouping of contiguous horizontal and vertical WorldTiles.
    /// </summary>
    public class TileMatrixScore
    {
        private static TileMatrixScore zeroScore = new TileMatrixScore(new HashSet<WorldTile>());

        private readonly HashSet<WorldTile> validWordTiles = null;

        public int Score { get { return validWordTiles.Count; } }

        public TileMatrixScore(HashSet<WorldTile> tiles)
        {
            validWordTiles = new HashSet<WorldTile>(tiles);
        }

        public static TileMatrixScore ZeroScore { get { return zeroScore; } }

        public bool ContainsTile(WorldTile tile)
        {
            return validWordTiles.Contains(tile);
        }
    }
}
