using UnityEngine;
using WordFudge.InputSystem;

namespace WordFudge
{
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField]
        private Transform letterTray;
        [SerializeField]
        private Transform gameBoard;
        [SerializeField]
        private TileHandGenerator handGenerator;
        [SerializeField]
        private WorldTileGenerator tileGenerator;
        [SerializeField]
        private WordFudgeGameplay gameplay;

        public BaseInputDetector InputDetector { get; private set; }

        private void Start()
        {
#if UNITY_EDITOR
            InputDetector = gameObject.AddComponent<MouseInputDetector>();
#else
            InputDetector = gameObject.AddComponent<TouchInputDetector>();
#endif //UNITY_EDITOR

            StartGame();
        }

        private void StartGame()
        {
            handGenerator.Initialize();
            tileGenerator.Initialize(letterTray);
            gameplay.Initialize(handGenerator, tileGenerator);

        }
    }
}
