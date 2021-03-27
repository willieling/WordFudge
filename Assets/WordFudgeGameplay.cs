using UnityEngine;

namespace WordFudge
{
    public class WordFudgeGameplay : MonoBehaviour
    {
        private TileHandGenerator tileHandGenerator;
        private WorldTileGenerator tileGenerator;

        private void Update()
        {
        
        }

        public void Initialize(TileHandGenerator tileHandGenerator, WorldTileGenerator tileGenerator)
        {
            this.tileHandGenerator = tileHandGenerator;
            this.tileGenerator = tileGenerator;

            var hand = this.tileHandGenerator.GetStartingHandLetters();
            this.tileGenerator.MakeTiles(hand);
        }
    }
}
