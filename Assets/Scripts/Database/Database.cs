using System.Collections.Generic;

namespace WordFudge.DataBase
{
    public static class Database
    {
        private static readonly HashSet<string> words = new HashSet<string>();

        public static void InitializeWordList(IEnumerable<string> words)
        {
            Database.words.Clear();

            foreach(string word in words)
            {
                Database.words.Add(word);
            }
        }

        public static bool IsValidWord(string word)
        {
            return words.Contains(word);
        }
    }
}
