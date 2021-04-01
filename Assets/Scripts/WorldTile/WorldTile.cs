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

        public char Character { get; private set; }

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            collider.size = rectTransform.sizeDelta;
            ShowPutDown();
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

        public void ShowPutDown()
        {
            background.color = placedAndExcluded;
        }
    }
}