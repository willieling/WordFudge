using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace WordFudge.ScoreSystem
{
    /// <summary>
    /// Class that holds the current best score and matrix of tiles that generated that score.
    /// </summary>
    public class ScoreHolder
    {
        public static event Action<TileMatrixScore> NewHighScore;

        private TileMatrixScore bestMatrix = TileMatrixScore.ZeroScore;

        public void UpdateScore(TileMatrixScore newScoreMatrix)
        {
            Assert.IsNotNull(newScoreMatrix);
            Debug.Log($"Comparing matrix with score: {newScoreMatrix.Score}");

            if (bestMatrix.Score < newScoreMatrix.Score)
            {
                Debug.Log($"New high score of {newScoreMatrix.Score} beats old score of {bestMatrix.Score}");
                UpdateBestScore(newScoreMatrix);
            }
        }

        private void UpdateBestScore(TileMatrixScore newScoreMatrix)
        {
            bestMatrix = newScoreMatrix;
            NewHighScore.Invoke(bestMatrix);
        }

        public void Clear()
        {
            Debug.Log($"Clearing high score.");
            UpdateBestScore(TileMatrixScore.ZeroScore);
        }
    }
}
