using UnityEngine;
using WordFudge.Boards;

namespace WordFudge
{
    public class WordFudgeGameplay : MonoBehaviour
    {
        private TileTray tileTray;

        private void Update()
        {
        
        }

        public void Initialize(TileTray TileTray)
        {
            this.tileTray = TileTray;
            this.tileTray.Initialize();
            this.tileTray.FillTrayWithStartingHand();
        }
    }
}
