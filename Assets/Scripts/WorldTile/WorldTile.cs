using UnityEngine;
using UnityEngine.UI;

namespace WordFudge
{
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

        public WorldTile Up { get; set; }
        public WorldTile Down { get; set; }
        public WorldTile Left { get; set; }
        public WorldTile Right { get; set; }

        public char Character { get; private set; }

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            collider.size = rectTransform.sizeDelta;
            ShowPutDownAndExcluded();
        }

        public void Initialize(char character)
        {
            Character = character;
            text.text = Character.ToString();

#if UNITY_EDITOR
            gameObject.name += $" - {character}";
#endif
        }

        public void ShowPickUp()
        {
            background.color = pickedUp;
        }

        public void ShowPutDownAndExcluded()
        {
            background.color = placedAndExcluded;
        }

        public void ShowAsIncluded()
        {
            background.color = placedAndIncluded;
        }

        public void ClearNeighbourReferences()
        {
            if (Up != null) { Up.Down = null; }
            if (Down != null) { Down.Up = null; }
            if (Left != null) { Left.Right = null; }
            if (Right != null) { Right.Left = null; }

            Up = null;
            Down = null;
            Left = null;
            Right = null;
        }
    }
}