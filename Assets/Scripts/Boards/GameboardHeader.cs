using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using WordFudge.ScoreSystem;

namespace WordFudge.Board
{
    public class GameboardHeader : MonoBehaviour
    {
        [SerializeField]
        private Text scoreText = null;

        private void Start()
        {
            Assert.IsNotNull(scoreText);
            scoreText.text = "0";

            ScoreHolder.NewHighScore += OnNewHighScore;
        }

        private void OnDestroy()
        {
            ScoreHolder.NewHighScore -= OnNewHighScore;
        }

        private void OnNewHighScore(TileMatrixScore score)
        {
            scoreText.text = score.Score.ToString();
        }
    }
}
