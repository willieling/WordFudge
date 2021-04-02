using UnityEngine;
using WordFudge.Boards;
using WordFudge.InputSystem;

namespace WordFudge
{
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField]
        private TileTray letterTray;
        [SerializeField]
        private GameBoard gameBoard;
        [SerializeField]
        private WorldTile tilePrefab;
        [SerializeField]
        private WordFudgeGameplay gameplay;

        private BaseInputDetector inputDetector;
        private InputHandler inputHandler;

        private void Start()
        {
#if UNITY_EDITOR
            inputDetector = gameObject.AddComponent<MouseInputDetector>();
#else
            InputDetector = gameObject.AddComponent<TouchInputDetector>();
#endif //UNITY_EDITOR

            StartGame();
        }

        private void StartGame()
        {
            gameplay.Initialize(letterTray);

            inputHandler = new InputHandler(inputDetector, gameplay);
        }

        private void Update()
        {
            inputHandler.Update();
        }
    }
}
