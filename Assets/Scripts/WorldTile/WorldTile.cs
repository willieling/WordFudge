using System;
using UnityEngine;
using UnityEngine.UI;

namespace WordFudge
{
    public class WorldTile : MonoBehaviour
    {
        [SerializeField]
        private Text Text;
        [SerializeField]
        private BoxCollider2D Collider;

        private RectTransform rectTransform;

        public char Character { get; private set; }

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            Collider.size = rectTransform.sizeDelta;
        }

        public void Initialize(char character)
        {
            Character = character;
            Text.text = Character.ToString();
        }

        internal void PickUp()
        {
            throw new NotImplementedException();
        }
    }
}