using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using WordFudge.ScoreSystem;

namespace WordFudge
{
    public class WorldTile : MonoBehaviour
    {
        public struct Neighbors
        {
            public readonly WorldTile Up;
            public readonly WorldTile Down;
            public readonly WorldTile Left;
            public readonly WorldTile Right;

            public Neighbors(WorldTile up, WorldTile down, WorldTile left, WorldTile right)
            {
                Up = up;
                Down = down;
                Left = left;
                Right = right;
            }
        }

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

        private HashSet<WordContainer> horizontalWords = new HashSet<WordContainer>();
        private HashSet<WordContainer> verticalWords = new HashSet<WordContainer>();

        public WorldTile Up { get; set; }
        public WorldTile Down { get; set; }
        public WorldTile Left { get; set; }
        public WorldTile Right { get; set; }

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
            Letter = character;
            text.text = Letter.ToString();

#if UNITY_EDITOR
            gameObject.name += $" - {character}";
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
            horizontalWords.Add(word);
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
            verticalWords.Add(word);
        }

        public void RemoveVerticalWord(WordContainer word)
        {
            if (!verticalWords.Remove(word))
            {
                //todo error
            }
        }

        public Neighbors GetNeighbours()
        {
            return new Neighbors(Up, Down, Left, Right);
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

        public bool ShareAssociatedWord(WorldTile otherTile)
        {
            throw new System.Exception("not implemented");
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