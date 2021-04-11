using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WordFudge
{
    public class BootstrapCheats : MonoBehaviour
    {
#if UNITY_EDITOR
        private static BootstrapCheats instance = null;

        [SerializeField]
        private bool useCheatLetters = false;
        [SerializeField]
        private string startingLetters = string.Empty;

        public static BootstrapCheats Instance { get { return instance; } }

        public string StartingLetters { get { return startingLetters; } }

        public bool HasStartingLetters { get { return useCheatLetters; } }

        // Start is called before the first frame update
        private void Awake()
        {
            instance = this;

            List<char> letters = startingLetters.ToCharArray().ToList();
            for(int i = 0; i < startingLetters.Length; ++i)
            {
                if (!char.IsLetter(letters[i]))
                {
                    letters.RemoveAt(i);
                }
            }
            startingLetters = new string(letters.ToArray());
        }
#endif // UNITY_EDITOR
    }
}
