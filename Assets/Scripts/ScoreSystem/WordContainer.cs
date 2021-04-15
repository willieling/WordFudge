using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Assertions;

namespace WordFudge.ScoreSystem
{
    public enum Axis
    {
        Horizontal,
        Vertical
    }

    /// <summary>
    /// Class that contains a word and a reference to all tiles in the word.
    /// </summary>
    [DebuggerDisplay("{Word} - {Axis}")]
    public class WordContainer
    {
        private readonly List<WorldTile> tiles;
        private readonly HashSet<WorldTile> tilesSet;
        private readonly Guid guid;

        public Axis Axis { get; }

        public string Word { get; }

        public IReadOnlyList<WorldTile> Tiles { get { return tiles; } }

        /// <summary>
        /// The index of the row or column this word exists within.
        /// </summary>
        public int LineIndex { get; }
        /// <summary>
        /// The index of the first letter within the line.
        /// </summary>
        public int FirstTileIndex { get; }
        /// <summary>
        /// The index of the last letter within the line.
        /// </summary>
        public int LastTileIndex { get; }

        public WorldTile FirstTile
        {
            get { return tiles[0]; }
        }

        public WorldTile LastTile
        {
            get { return tiles[tiles.Count - 1]; }
        }

        public WordContainer(string word, List<WorldTile> tiles, Axis axis)
        {
            Assert.IsTrue(!string.IsNullOrWhiteSpace(word));
            Assert.IsTrue(word.Length > 0);
            Assert.IsNotNull(tiles);
            Assert.IsTrue(tiles.Count > 0);

            guid = new Guid();

            Word = word;
            this.tiles = tiles;
            tilesSet = new HashSet<WorldTile>(this.tiles);
            Axis = axis;
            switch (axis)
            {
                case Axis.Horizontal:
                    LineIndex = FirstTile.Index.y;
                    FirstTileIndex = FirstTile.Index.x;
                    LastTileIndex = LastTile.Index.x;
                    break;
                case Axis.Vertical:
                    LineIndex = FirstTile.Index.x;
                    FirstTileIndex = FirstTile.Index.y;
                    LastTileIndex = LastTile.Index.y; 
                    break;
            }
        }

        public void ClearAssociations()
        {
            switch (Axis)
            {
                case Axis.Horizontal:
                    foreach (WorldTile tile in tiles)
                    {
                        tile.RemoveHorizontalWord(this);
                    }
                    break;
                default:
                    foreach (WorldTile tile in tiles)
                    {
                        tile.RemoveVerticalWord(this);
                    }
                    break;
            }
        }

        public bool ContainsTile(WorldTile tile)
        {
            return tilesSet.Contains(tile);
        }

        public class Comparer : IEqualityComparer<WordContainer>
        {
            bool IEqualityComparer<WordContainer>.Equals(WordContainer word, WordContainer other)
            {
                return word.guid == other.guid;
            }

            int IEqualityComparer<WordContainer>.GetHashCode(WordContainer word)
            {
                return word.GetHashCode();
            }
        }
    }

    public static class Extensions
    {
        public static Axis GetOppositeAxis(this Axis axis)
        {
            switch(axis)
            {
                case Axis.Horizontal:
                    return Axis.Vertical;
                default:
                    return Axis.Horizontal;
            }
        }
    }
}
