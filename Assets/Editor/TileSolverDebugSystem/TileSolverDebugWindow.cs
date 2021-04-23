using UnityEditor;
using WordFudge.Boards;
using WordFudge.ScoreSystem;

namespace WordFudgeEditor.ScoreSystem
{
    public class TileSolverDebugWindow : EditorWindow
    {
        private OptimalTileMatrixSolverRuntimeDebugger debugger = null;

        [MenuItem("Word Fudge/Optimal Tile Solver Debug")]
        private static void Init()
        {
            // Get existing open window or if none, make a new one:
            TileSolverDebugWindow window = (TileSolverDebugWindow)EditorWindow.GetWindow(typeof(TileSolverDebugWindow));
            window.Show();
        }

        private void OnEnable()
        {
            GameBoard gameBoard = FindObjectOfType<GameBoard>();
            debugger = gameBoard.Solver.Debugger;
        }

        private void OnGUI()
        {
            //draw list of finished matrices
        }
    }
}
