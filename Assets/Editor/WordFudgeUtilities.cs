using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace WordFudge
{
    public static class WordFudgeUtilities
    {
        [MenuItem("Word Fudge/Clear Save Data")]
        private static void ClearSaveData()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Application.persistentDataPath);

            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
            {
                directory.Delete(true);
            }
        }

        [MenuItem("Word Fudge/Open Save Data Folder")]
        private static void OpenSaveDataFolder()
        {
            if(!Directory.Exists(Application.persistentDataPath))
            {
                Directory.CreateDirectory(Application.persistentDataPath);
            }
            Process.Start(Application.persistentDataPath);
        }
    }
}
