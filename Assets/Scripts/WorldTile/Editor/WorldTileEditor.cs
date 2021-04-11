using System.Text;
using UnityEditor;
using UnityEngine;
using WordFudge.ScoreSystem;

namespace WordFudge
{
    [CustomEditor(typeof(WorldTile))]
    public class WorldTileEditor : Editor
    {
        private const string NO_WORDS = "[no words]";

        private readonly StringBuilder sb = new StringBuilder();

        private WorldTile targetTile = null;
        private GUIStyle horizontalLine = null;
        private string horizontalWords = null;
        private string verticalWords = null;

        private int horizontalWordCount = 0;
        private int verticalWordCount = 0;

        private Vector2 horizontalWordsScrollPosition;
        private Vector2 verticalWordsScrollPosition;

        public override void OnInspectorGUI()
        {
            const int LINE_HEIGHT = 12;

            base.OnInspectorGUI();

            ParseAssociatedWords();

            // this is still a little bit weird looking
            HorizontalLine(Color.grey);
            EditorGUILayout.LabelField($"Horizontal Words ({targetTile.HorizontalWords.Count})", EditorStyles.boldLabel);
            horizontalWordsScrollPosition = EditorGUILayout.BeginScrollView(horizontalWordsScrollPosition, false, false);
            EditorGUILayout.LabelField(horizontalWords, GUILayout.Height(horizontalWordCount * LINE_HEIGHT));
            EditorGUILayout.EndScrollView();

            HorizontalLine(Color.grey);
            EditorGUILayout.LabelField($"Vertical Words ({targetTile.VerticalWords.Count})", EditorStyles.boldLabel);
            verticalWordsScrollPosition = EditorGUILayout.BeginScrollView(verticalWordsScrollPosition, false, false);
            EditorGUILayout.LabelField(verticalWords, GUILayout.Height(verticalWordCount * LINE_HEIGHT));
            EditorGUILayout.EndScrollView();
        }

        private void ParseAssociatedWords()
        {
            horizontalWordCount = targetTile.HorizontalWords.Count;
            if (horizontalWordCount > 0)
            {
                foreach (WordContainer word in targetTile.HorizontalWords)
                {
                    sb.AppendLine(word.Word);
                }
                horizontalWords = sb.ToString();
                sb.Clear();
            }
            else
            {
                horizontalWords = NO_WORDS;
                horizontalWordCount = 1;
            }

            verticalWordCount = targetTile.VerticalWords.Count;
            if (verticalWordCount > 0)
            {
                sb.Clear();
                foreach (WordContainer word in targetTile.VerticalWords)
                {
                    sb.AppendLine(word.Word);
                }
                verticalWords = sb.ToString();
            }
            else
            {
                verticalWords = NO_WORDS;
                verticalWordCount = 1;
            }
        }

        private void OnEnable()
        {
            targetTile = (WorldTile)target;

            horizontalLine = new GUIStyle();
            horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
            horizontalLine.margin = new RectOffset(0, 0, 4, 4);
            horizontalLine.fixedHeight = 1;
        }

        private void OnDisable()
        {
            horizontalLine = null;
        }

        private void HorizontalLine(Color color)
        {
            var c = GUI.color;
            GUI.color = color;
            GUILayout.Box(GUIContent.none, horizontalLine);
            GUI.color = c;
        }
    }
}
