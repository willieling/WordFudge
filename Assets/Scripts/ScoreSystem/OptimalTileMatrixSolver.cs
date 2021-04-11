using System;
using System.Collections.Generic;
using UnityEngine.Assertions;
using WordFudge.Boards;

namespace WordFudge.ScoreSystem
{
    /// <summary>
    /// Scans the entire board and generates the tile matrix with the highest score possible.
    /// </summary>
    public class OptimalTileMatrixSolver
    {
        private readonly LinkedList<TileMatrix> unfinishedMatrices = null;
        private readonly Queue<TileMatrix> finishedMatrices = null;

        /// <summary>
        /// A collection of tiles visited at least once during the entire calculation of the best matrix.
        /// </summary>
        private readonly HashSet<WorldTile> globalVisitedThisCalculationTiles = null;

        private readonly TileMatrixFactory matrixFactory = null;

        private GameBoard gameBoard = null;

        public OptimalTileMatrixSolver()
        {
            unfinishedMatrices = new LinkedList<TileMatrix>();
            finishedMatrices = new Queue<TileMatrix>();
            globalVisitedThisCalculationTiles = new HashSet<WorldTile>();
            matrixFactory = new TileMatrixFactory(globalVisitedThisCalculationTiles);
        }

        public void Initialize(GameBoard gameBoard)
        {
            this.gameBoard = gameBoard;
        }

        public TileMatrixScore CalculateBestScoreMatrix(WorldTile tile)
        {
            ClearDataBuffers();

            List<TileMatrix> initialMatrices = matrixFactory.CreateMatrices(tile);
            foreach (TileMatrix matrix in initialMatrices)
            {
                unfinishedMatrices.AddLast(matrix);
            }

            return SolveForHighestScoreMatrix();
        }

        public TileMatrixScore CalculateBestScoreMatrixFromUnvisitedTile()
        {
            ClearDataBuffers();
            AddNewMatricesFromGloballyUnvisitedTile();
            return SolveForHighestScoreMatrix();
        }

        private void ClearDataBuffers()
        {
            unfinishedMatrices.Clear();
            finishedMatrices.Clear();
            globalVisitedThisCalculationTiles.Clear();
        }

        private TileMatrixScore SolveForHighestScoreMatrix()
        {
            if (unfinishedMatrices.Count == 0)
            {
                return TileMatrixScore.ZeroScore;
            }

            do
            {
                do
                {
                    TileMatrix matrix = DetachFirstUnfinished();

                    FullyExploreAndGenerateAlternateMatrices(matrix);
                    finishedMatrices.Enqueue(matrix);
                } while (unfinishedMatrices.Count > 0);
            } while (AddNewMatricesFromGloballyUnvisitedTile());


            TileMatrixScore highestScore = finishedMatrices.Dequeue().GetTileMatrixScore();
            foreach (TileMatrix matrix in finishedMatrices)
            {
                TileMatrixScore score = matrix.GetTileMatrixScore();
                if (highestScore.Score < score.Score)
                {
                    highestScore = score;
                }
            }

            return highestScore;
        }

        private bool AddNewMatricesFromGloballyUnvisitedTile()
        {
            if(globalVisitedThisCalculationTiles.Count == gameBoard.TileCount)
            {
                return false;
            }

            Assert.IsFalse(globalVisitedThisCalculationTiles.Count > gameBoard.TileCount);

            List<TileMatrix> matrices = null;
            do
            {
                WorldTile unvisited = null;
                foreach (WorldTile onBoard in gameBoard.TilesOnBoard)
                {
                    if (!globalVisitedThisCalculationTiles.Contains(onBoard))
                    {
                        unvisited = onBoard;
                        break;
                    }
                }
                Assert.IsNotNull(unvisited);

                matrices = matrixFactory.CreateMatrices(unvisited);

                //todo in game, put down "rag" then "dog" and make them not touching
                //remove the a and debug why I have two matrices with the same word
                foreach (TileMatrix matrix in matrices)
                {
                    unfinishedMatrices.AddLast(matrix);
                }
            } while (matrices.Count == 0 && globalVisitedThisCalculationTiles.Count < gameBoard.TileCount);

            return matrices.Count > 0;
        }

        private void FullyExploreAndGenerateAlternateMatrices(TileMatrix originalMatrix)
        {
            //foreach (WorldTile lastAddedWordtile in originalMatrix.LastAddedWord.Tiles)
            //{
            //    WordContainer lastWord = originalMatrix.LastAddedWord;

            //    Axis oppositeAxis = lastWord.Axis.GetOppositeAxis();

            //    IReadOnlyCollection<WordContainer>[] perpendicularWords = new IReadOnlyCollection<WordContainer>[lastWord.Tiles.Count];
            //    for(int i = 0; i < lastWord.Tiles.Count; ++i)
            //    {
            //        perpendicularWords[i] = lastWord.Tiles[i].GetAssociatedWordsOnAxis(oppositeAxis);
            //    }

            //    foreach(IReadOnlyCollection<WordContainer> perpendicularLineCollection in perpendicularWords)
            //    {
            //        foreach(WordContainer container in perpendicularLineCollection)
            //        {
            //            //check if the word is rubbing against any parallel and already associated word
            //            foreach(WorldTile tile in container.Tiles)
            //            {

            //            }


            //            unfinishedMatrices.AddLast(permutationMatrix);
            //        }
            //    }

            //    // we want to assign one of the recently created matrices to the current search matrix
            //    originalMatrix.AddWord(DetachLastUnfinished().LastAddedWord);
            //}
        }

        private TileMatrix DetachFirstUnfinished()
        {
            TileMatrix matrix = unfinishedMatrices.Last.Value;
            unfinishedMatrices.RemoveLast();
            return matrix;
        }

        private TileMatrix DetachLastUnfinished()
        {
            TileMatrix matrix = unfinishedMatrices.Last.Value;
            unfinishedMatrices.RemoveLast();
            return matrix;
        }
    }
}

//there's going to be invalid matrices generated

/*  todo an eventual optimization
*
*   super word: a word that completely encompasses another word
*   example:
*       ow   <- base word
*       snow <- super word
*       wind <- NOT a super word because 'o' is not in front of 'w'
*   
*   when scanning a word, any new matrices a word generates can be shared with a super word (assuming the associated word history is the same)
*   this is because the possible permutations of a larger word are a super set of the possible permutations of a smaller word
*/