using System.Collections.Generic;
using UnityEngine;

namespace WordFudge.Boards
{
    /// <summary>
    /// This class represents the bag of tiles.
    /// Most functions for this class can thought of as removing tiles from the bag.
    /// </summary>
    public class TileBag
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
        private uint startingLettersCount;
        private uint refillLettersCount;

        public void Initialize(uint startingLettersCount, uint refillLettersCount)
        {
            this.startingLettersCount = startingLettersCount;
            this.refillLettersCount = refillLettersCount;

            remainingLetters = new Dictionary<char, uint>(letterCounts);
        }

        public char[] GetStartingHandLetters()
        {
#if UNITY_EDITOR
            if(BootstrapCheats.Instance.HasStartingLetters)
            {
                return BootstrapCheats.Instance.StartingLetters.ToCharArray();
            }
#endif // UNITY_EDITOR
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
