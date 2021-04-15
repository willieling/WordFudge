using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace WordFudge.ScoreSystem
{
    /// <summary>
    /// A data container that holds visited words
    /// </summary>
    [DebuggerDisplay("Last word: {lastWord.Word} - {lastWord.Axis}")]
    public class TileMatrix
    {
        private WordContainer lastWord = null;

        private readonly HashSet<WorldTile> visitedTiles = new HashSet<WorldTile>();
        private readonly HashSet<WorldTile> globalVisistedTiles = new HashSet<WorldTile>();

        private readonly HashSet<WordContainer> horizontalWords = new HashSet<WordContainer>();
        private readonly HashSet<WordContainer> verticalWords = new HashSet<WordContainer>();

        public WordContainer LastAddedWord { get { return lastWord; } }

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

        private TileMatrix(HashSet<WorldTile> tiles, HashSet<WorldTile> globalVisistedTiles, HashSet<WordContainer> horizontalWords, HashSet<WordContainer> verticalWords)
        {
            visitedTiles = new HashSet<WorldTile>(tiles);
            this.globalVisistedTiles = globalVisistedTiles;
            this.horizontalWords = new HashSet<WordContainer>(horizontalWords);
            this.verticalWords = new HashSet<WordContainer>(verticalWords);
        }

        /// <summary>
        /// Deep clone the data structures declared within this class but not their elements.
        /// </summary>
        /// <returns></returns>
        public TileMatrix DeepClone()
        {
            return new TileMatrix(visitedTiles, globalVisistedTiles, horizontalWords, verticalWords);
        }

        public void AddHorizontalWord(WordContainer word)
        {
            AddWord(word, horizontalWords);
        }

        public void AddVerticalWord(WordContainer word)
        {
            AddWord(word, verticalWords);
        }

        public bool HasVisitedTile(WorldTile tile)
        {
            return visitedTiles.Contains(tile);
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

            lastWord = word;

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
