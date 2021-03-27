using UnityEngine;
using UnityEngine.UI;

namespace WordFudge
{
    public class WorldTile : MonoBehaviour
    {
        [SerializeField]
        private Text Text;

        public string Letter { get; private set; }

        public void Initialize(char character)
        {
            Text.text = character.ToString();
        }
    }
}