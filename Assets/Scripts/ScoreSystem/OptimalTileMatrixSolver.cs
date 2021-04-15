using System;
using System.Collections.Generic;
using UnityEngine;
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
        private TileMatrix currentMatrix = null;

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
                    currentMatrix = DetachFirstUnfinished();

                    FullyExploreAndGenerateAlternateMatrices();
                    finishedMatrices.Enqueue(currentMatrix);
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
            if (globalVisitedThisCalculationTiles.Count == gameBoard.TileCount)
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

        private void FullyExploreAndGenerateAlternateMatrices()
        {
            Axis oppositeAxis = currentMatrix.LastAddedWord.Axis.GetOppositeAxis();

            //for this iteration, we're just looking at the immediate perpendicular words to the first word in the matrix
            IReadOnlyCollection<WordContainer>[] candidateWords = new IReadOnlyCollection<WordContainer>[currentMatrix.LastAddedWord.Tiles.Count];
            for (int i = 0; i < currentMatrix.LastAddedWord.Tiles.Count; ++i)
            {
                candidateWords[i] = currentMatrix.LastAddedWord.Tiles[i].GetAssociatedWordsOnAxis(oppositeAxis);
            }

            switch (oppositeAxis)
            {
                case Axis.Horizontal:
                    {
                        CreateAndQueueValidPermutations(candidateWords, currentMatrix.HorizontalWords);

                        // We want to generate every permutation possible but also assign one of the permutations to our current matrix
                        // since we automatically queue N matrices, detach the last one and give it's data to the original matrix
                        // the last matrix should be GC'd
                        if (HasUnfinishedMatrices())
                        {
                            TileMatrix lastMatrix = DetachLastUnfinished();
                            currentMatrix.AddHorizontalWord(lastMatrix.LastAddedWord);
                        }
                    }
                    break;

                case Axis.Vertical:
                    {
                        CreateAndQueueValidPermutations(candidateWords, currentMatrix.VerticalWords);

                        if (HasUnfinishedMatrices())
                        {
                            TileMatrix lastMatrix = DetachLastUnfinished();
                            currentMatrix.AddVerticalWord(lastMatrix.LastAddedWord);
                        }
                    }
                    break;
            }
        }

        private void CreateAndQueueValidPermutations(IReadOnlyCollection<WordContainer>[] candidateWords, IReadOnlyCollection<WordContainer> matrixAssociatedWords)
        {
            foreach (IReadOnlyCollection<WordContainer> candidateWordsLine in candidateWords)
            {
                foreach (WordContainer candidateWord in candidateWordsLine)
                {
                    //need to create permutations here
                    if (!IsGrating(candidateWord, matrixAssociatedWords))
                    {
                        TileMatrix newMatrix = currentMatrix.DeepClone();
                        newMatrix.AddHorizontalWord(candidateWord);
                        QueueTileMatrix(newMatrix);
                    }
                }
            }
        }

        private bool IsGrating(WordContainer candidateWord, IReadOnlyCollection<WordContainer> associatedWords)
        {
            foreach (WordContainer associatedWord in associatedWords)
            {
                if(IsGrating(candidateWord, associatedWord))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsGrating(WordContainer candidateWord, WordContainer associatedWord)
        {
            const int INLINE = 0;
            const int PARLLEL_NOT_INLINE = 1;

            Assert.IsTrue(candidateWord.Axis == associatedWord.Axis);

            int parallelDistance = Math.Abs(associatedWord.LineIndex - candidateWord.LineIndex);
            switch (parallelDistance)
            {
                case INLINE:
                    if (CheckIfNeighboursAreAlreadyVisisted(candidateWord.FirstTile)
                        || CheckIfNeighboursAreAlreadyVisisted(candidateWord.LastTile))
                    {
                        return true;
                    }

                    return false;

                case PARLLEL_NOT_INLINE:
                    Vector2Int range = GetRubbingRange(candidateWord, associatedWord);

                    WordContainer higherWord;
                    WordContainer lowerWord;
                    bool candidateLower = candidateWord.LineIndex < associatedWord.LineIndex;
                    if(candidateLower)
                    {
                        higherWord = associatedWord;
                        lowerWord = candidateWord;
                    }
                    else
                    {
                        higherWord = candidateWord;
                        lowerWord = associatedWord;
                    }

                    switch (candidateWord.Axis)
                    {
                        case Axis.Horizontal:
                            for(int col = range.x; col <= range.y; ++col)
                            {
                                if (AreTilesGrating(lowerWord.LineIndex, col, higherWord))
                                {
                                    return true;
                                }
                            }

                            break;
                        case Axis.Vertical:
                            for (int row = range.x; row <= range.y; ++row)
                            {
                                if (AreTilesGrating(row, lowerWord.LineIndex, higherWord))
                                {
                                    return true;
                                }
                            }

                            break;
                    }
                    return false;

                default:
                    return false;
            }
        }

        private bool AreTilesGrating(int row, int col, WordContainer higherWord)
        {
            WorldTile lowerTile = gameBoard.GetTile(row, col);
            Assert.IsNotNull(lowerTile);
            Assert.IsTrue(higherWord.ContainsTile(lowerTile.Up));

            return !lowerTile.ShareAssociatedWord(lowerTile.Up, Axis.Horizontal);
        }

        private bool CheckIfNeighboursAreAlreadyVisisted(WorldTile tile)
        {
            return currentMatrix.HasVisitedTile(tile.Up)
                || currentMatrix.HasVisitedTile(tile.Down)
                || currentMatrix.HasVisitedTile(tile.Left)
                || currentMatrix.HasVisitedTile(tile.Right);
        }

        private Vector2Int GetRubbingRange(WordContainer word, WordContainer otherWord)
        {
            // https://scicomp.stackexchange.com/questions/26258/the-easiest-way-to-find-intersection-of-two-intervals
            if (word.FirstTileIndex > otherWord.LastTileIndex
                || otherWord.FirstTileIndex > word.LastTileIndex)
            {
                return Vector2Int.zero;
            }

            return new Vector2Int(Mathf.Max(word.FirstTileIndex, otherWord.FirstTileIndex), Mathf.Max(word.FirstTileIndex, otherWord.FirstTileIndex));
        }

        private bool HasUnfinishedMatrices()
        {
            return unfinishedMatrices.Count > 0;
        }

        private void QueueTileMatrix(TileMatrix matrix)
        {
            unfinishedMatrices.AddLast(matrix);
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

/* Two words are grating if are
* - parallel
* -- not co-linear (they do not exist on the same line)
* - touching
* - for all pairs of touching tiles there exists one pair that is not part of a word
* 
* grating ex:
* is grating
*   TRY
*    DOG
*    
*  not grating
*   CAT
*    TOP
* */