using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using WordFudge.DataBase;

namespace WordFudge.ScoreSystem
{
    /// <summary>
    /// Updates the horizontal and vertical lines that intersect the placed tile.
    /// Only updates contiguous lines of tiles.
    /// </summary>
    public static class TileWordUpdater
    {
        private static readonly HashSet<int> wordHashCodeSet = new HashSet<int>();

        public static void CalculateNewlyFormedWords(WorldTile placedTile)
        {
            List<WorldTile> line = GetHorizontalLine(placedTile);
            FindWordsAndAssociateThemWithTiles(placedTile, line, Axis.Horizontal);

            line = GetVerticalLine(placedTile);
            FindWordsAndAssociateThemWithTiles(placedTile, line, Axis.Vertical);
        }

        public static void RemoveDestroyedWords(WorldTile tile)
        {
            foreach(WordContainer word in tile.HorizontalWords)
            {
                if(!wordHashCodeSet.Remove(word))
                {
                    Debug.LogWarning("Trying to remove word that doesn't exist.");
                }
            }

            foreach (WordContainer word in tile.VerticalWords)
            {
                if (!wordHashCodeSet.Remove(word))
                {
                    Debug.LogWarning("Trying to remove word that doesn't exist.");
                }
            }
        }

        private static void FindWordsAndAssociateThemWithTiles(WorldTile placedTile, List<WorldTile> line, Axis axis)
        {
            string lineAsCharacters = GetString(line);
            int placedTileIndex = line.IndexOf(placedTile);

            // we don't want to do the entire line
            // just the potential words that hold the tile
            for (int leftIndex = placedTileIndex; leftIndex >= 0; --leftIndex)
            {
                for (int rightIndex = leftIndex + 1; rightIndex < line.Count; rightIndex++)
                {
                    // left to right INCLUSIVE
                    int length = rightIndex - leftIndex + 1;
                    string word = lineAsCharacters.Substring(leftIndex, length);

                    //we need to check that we're not creating a duplicated WordContainer here
                    //we could compare the Word itself, the first tile and the axis?
                    //validate the wordHashCodeSet works
                    if (Database.IsValidWord(word) && !wordHashCodeSet.Contains(WordContainer.GetWordHashCode(word, line[leftIndex], axis)))
                    {
                        WordContainer container = new WordContainer(word, line.GetRange(leftIndex, length), axis);
                        Debug.Log($"<color=cyan>[Tile {placedTile.Letter}] Found new word {container.Word}</color>");

                        wordHashCodeSet.Add(container);

                        for (int k = leftIndex; k <= rightIndex; ++k)
                        {
                            switch(axis)
                            {
                                case Axis.Horizontal:
                                    line[k].AddHorizontalWord(container);
                                    break;
                                default:
                                    line[k].AddVerticalWord(container);
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private static string GetString(List<WorldTile> subset)
        {
            char[] characters = new char[subset.Count];
            for(int i = 0; i < subset.Count; ++i)
            {
                characters[i] = subset[i].Letter;
            }

            return new string(characters);
        }

        private static List<WorldTile> GetHorizontalLine(WorldTile tile)
        {
            Assert.IsNotNull(tile);
            List<WorldTile> list = new List<WorldTile>();

            WorldTile currentTile = tile;
            while(currentTile.Left != null)
            {
                currentTile = currentTile.Left;
            }

            while(currentTile != null)
            {
                list.Add(currentTile);
                currentTile = currentTile.Right;
            }

            return list;
        }

        private static List<WorldTile> GetVerticalLine(WorldTile tile)
        {
            Assert.IsNotNull(tile);
            List<WorldTile> list = new List<WorldTile>();

            WorldTile currentTile = tile;
            while (currentTile.Up != null)
            {
                currentTile = currentTile.Up;
            }

            while (currentTile != null)
            {
                list.Add(currentTile);
                currentTile = currentTile.Down;
            }

            return list;
        }
    }
}
