using System;
using System.Collections.Generic;

namespace WordFudge.ScoreSystem
{
    /// <summary>
    /// A data container that holds visited words
    /// </summary>
    public class TileMatrix
    {
        private WordContainer lastWord = null;

        private readonly HashSet<WorldTile> visitedTiles = new HashSet<WorldTile>();
        private readonly HashSet<WorldTile> globalVisistedTiles = new HashSet<WorldTile>();

        private readonly HashSet<WordContainer> visitedWords = new HashSet<WordContainer>();

        public WordContainer LastAddedWord { get { return lastWord; } }

        public IReadOnlyCollection<WordContainer> VisitedWords { get { return visitedWords; } }

        public TileMatrix(WordContainer word, HashSet<WorldTile> globalVisistedTiles)
        {
            this.globalVisistedTiles = globalVisistedTiles;
            AddWord(word);
        }

        private TileMatrix(HashSet<WorldTile> tiles, HashSet<WorldTile> globalVisistedTiles)
        {
            visitedTiles = new HashSet<WorldTile>(tiles);
            this.globalVisistedTiles = globalVisistedTiles;
        }

        public TileMatrix DeepClone()
        {
            return new TileMatrix(visitedTiles, globalVisistedTiles);
        }

        public void AddWord(WordContainer word)
        {
            if(!visitedWords.Add(word))
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

        public bool ContainsTile(WorldTile tile)
        {
            return visitedTiles.Contains(tile);
        }

        public TileMatrixScore GetTileMatrixScore()
        {
            return new TileMatrixScore(visitedTiles);
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
