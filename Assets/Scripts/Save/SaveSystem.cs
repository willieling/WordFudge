using System.IO;
using UnityEngine;

namespace WordFudge.Save
{
    public static class SaveSystem
    {
        public static string Load(string key)
        {
            string path = GetFullPath(key);
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }

            return string.Empty;
        }

        public static string[] LoadMultiLine(string key)
        {
            string path = GetFullPath(key);
            if (File.Exists(path))
            {
                return File.ReadAllLines(path);
            }

            return new string[0];
        }

        public static void Save(string key, string value)
        {
            string fullPath = GetFullPath(key);
            if (!Directory.Exists(fullPath))
            {
                string folderPath = Path.GetDirectoryName(fullPath);
                Directory.CreateDirectory(folderPath);
            }
            File.WriteAllText(fullPath, value);
        }

        private static string GetFullPath(string relativePath)
        {
            return Path.Combine(Application.persistentDataPath, $"{relativePath}.gd");
        }
    }
}
