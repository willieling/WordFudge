using UnityEngine;
using UnityEngine.UI;

namespace WordFudge
{
    public class WorldTile : MonoBehaviour
    {
        [SerializeField]
        private Text Text;
        [SerializeField]
        private BoxCollider Collider;

        private RectTransform rectTransform;

        public char Character { get; private set; }

        private void Awake()
        {
            const int DEPTH = 1;

            rectTransform = GetComponent<RectTransform>();
            //Collider.size = new Vector3(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y, DEPTH);
        }

        public void Initialize(char character)
        {
            Character = character;
            Text.text = Character.ToString();
        }
    }
}