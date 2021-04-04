using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordFudge.DataBase
{
    public static class Database
    {
        private static readonly HashSet<string> words = new HashSet<string>();

        public static void InitializeDictionary(HashSet<string> words)
        {
            Database.words.Clear();

            foreach(string word in words)
            {
                Database.words.Add(word);
            }
        }

        public static bool IsWord(char[] word)
        {
            return words.Contains(new string(word));
        }
    }
}
