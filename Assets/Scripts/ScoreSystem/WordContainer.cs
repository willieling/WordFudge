using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Assertions;

namespace WordFudge.ScoreSystem
{
    public enum Axis
    {
        Horizontal = -1,
        Vertical = 1
    }

    /// <summary>
    /// Class that contains a word and a reference to all tiles in the word.
    /// </summary>
    [DebuggerDisplay("{Word}.{hashCode} - {Axis} - Line {LineIndex}")]
    public class WordContainer
    {
        private readonly List<WorldTile> tiles;
        private readonly HashSet<WorldTile> tilesSet;
        private readonly int hashCode;

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

            hashCode = GetWordHashCode(Word, FirstTile, Axis);
        }

        public static int GetWordHashCode(string word, WorldTile firstTile, Axis axis)
        {
            return word.GetHashCode() * firstTile.GetHashCode() * (int)axis;
        }

        //todo make horizontal and vertical versions?
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

        public override string ToString()
        {
            return $"{Word} ({Axis})";
        }

        public override bool Equals(object obj)
        {
            WordContainer other = obj as WordContainer;
            if(other == null)
            {
                return false;
            }

            return hashCode == other.hashCode;
        }

        public override int GetHashCode()
        {
            return hashCode;
        }

        public static bool operator ==(WordContainer word, WordContainer other)
        {
            if(word is null ^ other is null)
            {
                return false;
            }

            if(word is null && other is null)
            {
                return true;
            }

            return word.GetHashCode() == other.GetHashCode();
        }

        public static bool operator !=(WordContainer word, WordContainer other)
        {
            return !(word == other);
        }

        public static implicit operator int(WordContainer word)
        {
            return word.hashCode;
        }

        public class Comparer : IEqualityComparer<WordContainer>
        {
            bool IEqualityComparer<WordContainer>.Equals(WordContainer word, WordContainer other)
            {
                return word.hashCode == other.hashCode;
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
