using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

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
    [DebuggerDisplay("{Word}")]
    public class WordContainer
    {
        private readonly List<WorldTile> tiles;

        public Axis Axis { get; }

        public string Word { get; }

        public IReadOnlyList<WorldTile> Tiles { get { return tiles; } }

        public WordContainer(string word, List<WorldTile> tiles, Axis axis)
        {
            Word = word;
            this.tiles = tiles;
            Axis = axis;
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
    }

    public class WordContainerComparer : IEqualityComparer<WordContainer>
    {
        bool IEqualityComparer<WordContainer>.Equals(WordContainer word, WordContainer other)
        {
            return word.Word == other.Word;
        }

        int IEqualityComparer<WordContainer>.GetHashCode(WordContainer word)
        {
            return word.GetHashCode();
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
