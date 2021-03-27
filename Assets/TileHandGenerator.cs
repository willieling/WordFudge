using System.Collections.Generic;
using UnityEngine;

namespace WordFudge
{
    //make this a non-mono behaviour?
    public class TileHandGenerator : MonoBehaviour
    {
        private readonly Dictionary<char, uint> letterCounts = new Dictionary<char, uint>()
        {
            { 'A', 9 },
            { 'B', 2 },
            { 'C', 2 },
            { 'D', 4 },
            { 'E', 12 },
            { 'F', 2 },
            { 'G', 3 },
            { 'H', 2 },
            { 'I', 9 },
            { 'J', 1 },
            { 'K', 1 },
            { 'L', 4 },
            { 'M', 2 },
            { 'N', 6 },
            { 'O', 8 },
            { 'P', 2 },
            { 'Q', 1 },
            { 'R', 6 },
            { 'S', 4 },
            { 'T', 6 },
            { 'U', 4 },
            { 'V', 2 },
            { 'W', 2 },
            { 'X', 1 },
            { 'Y', 2 },
            { 'Z', 1 },
            { '*', 2 }
        };

        private Dictionary<char, uint> remainingLetters;

        [SerializeField]
        private uint startingLettersCount = 20;
        [SerializeField]
        private uint refillLettersCount = 5;

        public void Initialize()
        {
            remainingLetters = new Dictionary<char, uint>(letterCounts);
        }

        public char[] GetStartingHandLetters()
        {
            return GetRandomLetters(startingLettersCount);
        }

        public char[] GetRefillLetters()
        {
            return GetRandomLetters(refillLettersCount);
        }

        private char[] GetRandomLetters(uint amount)
        {
            char[] letters = new char[amount];
            for(int i = 0; i < amount; ++i)
            {
                int randomIndex = Random.Range(0, remainingLetters.Count);

                char chosenCharacter = '\0';
                int index = 0;
                foreach (KeyValuePair<char, uint> kvp in remainingLetters)
                {
                    if(index == randomIndex)
                    {
                        chosenCharacter = kvp.Key;
                        break;
                    }

                    ++index;
                }

                letters[i] = chosenCharacter;
                --remainingLetters[chosenCharacter];
                if(remainingLetters[chosenCharacter] <= 0)
                {
                    remainingLetters.Remove(chosenCharacter);
                }
            }
            return letters;
        }
    }
}
