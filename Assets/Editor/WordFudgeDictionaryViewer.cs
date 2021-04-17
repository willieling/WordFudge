using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WordFudge.CloudService;

namespace WordFudge.DataBase
{
    public class WordFudgeDictionaryViewer : EditorWindow
    {
        private static readonly HashSet<string> words = new HashSet<string>();
        private string inputWord = null;
        private bool loaded = false;

        private GUIStyle redTextStyle = new GUIStyle();
        private GUIStyle greenTextStyle = new GUIStyle();

        [MenuItem("Word Fudge/Dictionary")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            WordFudgeDictionaryViewer window = (WordFudgeDictionaryViewer)EditorWindow.GetWindow(typeof(WordFudgeDictionaryViewer));
            window.Show();
        }

        private void OnEnable()
        {
            DatabaseLoader loader = new DatabaseLoader();
            loader.DebugLoadSavedWordList(OnLoadDatabase);

            redTextStyle.normal.textColor = Color.red;
            greenTextStyle.normal.textColor = Color.green;
        }

        private void OnGUI()
        {
            const int WORD_LABEL_WIDTH = 50;

            if (loaded)
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Word: ", GUILayout.Width(WORD_LABEL_WIDTH));
                    inputWord = GUILayout.TextField(inputWord);
                    inputWord = inputWord.ToUpper();
                }
                GUILayout.EndHorizontal();

                if (Database.IsValidWord(inputWord))
                {
                    GUILayout.Label($"'{inputWord}' is a word", greenTextStyle);
                }
                else
                {
                    GUILayout.Label($"'{inputWord}' is NOT a word", redTextStyle);
                }
            }
            else
            {
                GUILayout.Label("Could not load database of words");
            }
        }

        private void OnLoadDatabase(bool loaded)
        {
            this.loaded = loaded;
        }
    }
}
