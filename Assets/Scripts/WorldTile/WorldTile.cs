using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using WordFudge.ScoreSystem;

namespace WordFudge
{
    [DebuggerDisplay("{Letter} - {Index}")]
    public class WorldTile : MonoBehaviour
    {
        [SerializeField]
        private Text text;
        [SerializeField]
        private Image background;
        [SerializeField]
        private new BoxCollider2D collider;

        [Header("Visuals")]
        [SerializeField]
        private Color placedAndExcluded;
        [SerializeField]
        private Color placedAndIncluded;
        [SerializeField]
        private Color pickedUp;

        private RectTransform rectTransform;

        private HashSet<WordContainer> horizontalWords = new HashSet<WordContainer>(new WordContainer.Comparer());
        private HashSet<WordContainer> verticalWords = new HashSet<WordContainer>(new WordContainer.Comparer());

        public WorldTile Up { get; set; }
        public WorldTile Down { get; set; }
        public WorldTile Left { get; set; }
        public WorldTile Right { get; set; }

        public Vector2Int Index { get; private set; }

        public IReadOnlyCollection<WordContainer> HorizontalWords { get { return horizontalWords; } }
        public IReadOnlyCollection<WordContainer> VerticalWords { get { return verticalWords; } }

        public char Letter { get; private set; }

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            collider.size = rectTransform.sizeDelta;
            SetVisualsAsPutDownAndExcluded();
        }

        public void Initialize(char character)
        {
            Letter = char.ToUpper(character);
            text.text = Letter.ToString();

#if UNITY_EDITOR
            gameObject.name += $" - {Letter}";
#endif
        }

        public void RegisterToGameplayEvents()
        {
            ScoreHolder.NewHighScore += OnNewHighScore;
        }

        public void UnregisterFromGameplayEvents()
        {
            ScoreHolder.NewHighScore -= OnNewHighScore;
        }

        public void SetVisualsAsPickedUp()
        {
            background.color = pickedUp;
        }

        public void SetVisualsAsPutDownAndExcluded()
        {
            background.color = placedAndExcluded;
        }

        public void SetVisualsAsIncluded()
        {
            background.color = placedAndIncluded;
        }

        public void SetIndex(Vector2Int index)
        {
            Index = index;
        }

        public void ClearIndex()
        {
            Index = new Vector2Int(-1, -1);
        }

        public void ClearNeighbourReferencesAndAssociatedWords()
        {
            if (Up != null) { Up.Down = null; }
            if (Down != null) { Down.Up = null; }
            if (Left != null) { Left.Right = null; }
            if (Right != null) { Right.Left = null; }

            Up = null;
            Down = null;
            Left = null;
            Right = null;

            // ClearAssocations modifies the horizontal word collection
            // we can't use foreach on the original collection
            foreach (WordContainer word in horizontalWords.ToArray())
            {
                word.ClearAssociations();
            }

            foreach (WordContainer word in verticalWords.ToArray())
            {
                word.ClearAssociations();
            }
        }

        public void AddHorizontalWord(WordContainer word)
        {
            if(!horizontalWords.Add(word))
            {
                UnityEngine.Debug.LogWarning($"\'{name}\' already has the horizontal word {word.Word}");
            }
        }

        public void RemoveHorizontalWord(WordContainer word)
        {
            if(!horizontalWords.Remove(word))
            {
                //todo error
            }
        }

        public void AddVerticalWord(WordContainer word)
        {
            if (!verticalWords.Add(word))
            {
                //todo error
                UnityEngine.Debug.LogWarning($"\'{name}\' already has the vertical word {word.Word}");
            }
        }

        public void RemoveVerticalWord(WordContainer word)
        {
            if (!verticalWords.Remove(word))
            {
                //todo error
                UnityEngine.Debug.LogWarning($"\'{name}\' already has the vertical word {word.Word}");
            }
        }

        public IReadOnlyCollection<WordContainer> GetAssociatedWordsOnAxis(Axis axis)
        {
            switch(axis)
            {
                case Axis.Horizontal:
                    return horizontalWords;
                default:
                    return verticalWords;
            }
        }

        public bool ShareAssociatedWordHorizontally(WorldTile otherTile)
        {
            foreach (WordContainer word in HorizontalWords)
            {
                if (word.ContainsTile(otherTile))
                {
                    return true;
                }
            }

            return false;
        }

        public bool ShareAssociatedWordVertically(WorldTile otherTile)
        {
            foreach (WordContainer word in VerticalWords)
            {
                if (word.ContainsTile(otherTile))
                {
                    return true;
                }
            }

            return false;
        }

        private void OnNewHighScore(TileMatrixScore score)
        {
            if (score.ContainsTile(this))
            {
                SetVisualsAsIncluded();
            }
            else
            {
                SetVisualsAsPutDownAndExcluded();
            }
        }
    }
}