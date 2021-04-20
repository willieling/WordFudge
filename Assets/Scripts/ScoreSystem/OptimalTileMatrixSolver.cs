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

            Debug.Log($"<color=yellow>Starting search from tile {tile} with {unfinishedMatrices.Count} matrices.</color>");
            return SolveForHighestScoreMatrix();
        }

        public TileMatrixScore CalculateBestScoreMatrixFromUnvisitedTile()
        {
            ClearDataBuffers();
            AddNewMatricesFromGloballyUnvisitedTile();

            Debug.Log($"<color=yellow>Starting search from a removed tile with {unfinishedMatrices.Count} matrices.</color>");

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

            Assert.IsFalse(globalVisitedThisCalculationTiles.Count > gameBoard.TileCount, $"We've reportedly visited more tiles than exist on the gameboard.");

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
            Debug.Log($"<color=magenta>Starting exploration of matrix with last added word: {currentMatrix.LastAddedWord}</color>");

            //remove me
            int iterations = 0;
            Axis oppositeAxis;
            do
            {
                oppositeAxis = currentMatrix.LastAddedWord.Axis.GetOppositeAxis();

                IReadOnlyCollection<WordContainer>[] candidateWords = new IReadOnlyCollection<WordContainer>[currentMatrix.LastAddedWord.Tiles.Count];
                for (int index = 0, lineIndex = currentMatrix.LastAddedWord.FirstTileIndex; index < currentMatrix.LastAddedWord.Tiles.Count; ++index, ++lineIndex)
                {
                    if (currentMatrix.PreviousLastAddedWord == null || lineIndex != currentMatrix.PreviousLastAddedWord.LineIndex)
                    {
                        candidateWords[index] = currentMatrix.LastAddedWord.Tiles[index].GetAssociatedWordsOnAxis(oppositeAxis);
                    }
                    else
                    {
                        //if we just came from here, don't go back
                        candidateWords[index] = new List<WordContainer>();
                    }
                }

                switch (oppositeAxis)
                {
                    case Axis.Horizontal:
                        {
                            bool createdPermutations = CreateAndQueueValidPermutations(candidateWords, currentMatrix.HorizontalWords);

                            // We want to generate every permutation possible but also assign one of the permutations to our current matrix
                            // since we automatically queue N matrices, detach the last one and make it the current matrix
                            // the previous current matrix should be GC'd
                            if (createdPermutations && HasUnfinishedMatrices())
                            {
                                currentMatrix = DetachLastUnfinished();
                            }
                        }
                        break;

                    case Axis.Vertical:
                        {
                            bool createdPermutations = CreateAndQueueValidPermutations(candidateWords, currentMatrix.VerticalWords);

                            if (createdPermutations && HasUnfinishedMatrices())
                            {
                                currentMatrix = DetachLastUnfinished();
                            }
                        }
                        break;
                }

                ++iterations;
            } while (CanCurrentMatrixStillExplore(oppositeAxis) && iterations < 100);

            Debug.Log($"<color=orange>Finished an exploration in {iterations} iterations</color>");
        }

        private bool CanCurrentMatrixStillExplore(Axis lastWordAxis)
        {
            //todo check that the words isn't already added
            switch (lastWordAxis.GetOppositeAxis())
            {
                case Axis.Horizontal:
                    foreach (WorldTile tile in currentMatrix.LastAddedWord.Tiles)
                    {
                        return HasUnvisitedWord(tile.HorizontalWords);
                    }

                    return false;
                case Axis.Vertical:
                default:
                    foreach (WorldTile tile in currentMatrix.LastAddedWord.Tiles)
                    {
                        return HasUnvisitedWord(tile.VerticalWords);
                    }

                    return false;
            }
        }

        private bool HasUnvisitedWord(IReadOnlyCollection<WordContainer> associatedWords)
        {
            if (associatedWords.Count > 0)
            {
                foreach (WordContainer word in associatedWords)
                {
                    if (currentMatrix.HasVisitedWord(word))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        private bool CreateAndQueueValidPermutations(IReadOnlyCollection<WordContainer>[] candidateWords, IReadOnlyCollection<WordContainer> matrixAssociatedWords)
        {
            bool createdPermutations = false;

            foreach (IReadOnlyCollection<WordContainer> candidateWordsLine in candidateWords)
            {
                foreach (WordContainer candidateWord in candidateWordsLine)
                {
                    Debug.Log($"Exploring '{currentMatrix.LastAddedWord}' -> '{candidateWord}'");
                    AddWordToGloballyVisitedTiles(candidateWord);

                    if (!currentMatrix.HasVisitedWord(candidateWord) && !IsGrating(candidateWord, matrixAssociatedWords))
                    {
#if UNITY_EDITOR
                        if (currentMatrix.LastAddedWord == candidateWord)
                        {
                            throw new Exception("Trying to re-add the last added word.");
                        }
#endif
                        Debug.Log($"<color=green>Create permutation.  Base {currentMatrix.LastAddedWord} -> {candidateWord}</color>");

                        TileMatrix newMatrix = currentMatrix.DeepClone();
                        switch (candidateWord.Axis)
                        {
                            case Axis.Horizontal:
                                newMatrix.AddHorizontalWord(candidateWord);
                                break;
                            case Axis.Vertical:
                                newMatrix.AddVerticalWord(candidateWord);
                                break;
                        }
                        QueueTileMatrix(newMatrix);
                        createdPermutations = true;
                    }
                    else
                    {
                        Debug.Log($"<color=red>{candidateWord} rejected for already being visited or grating</color>");
                    }
                }
            }

            return createdPermutations;
        }

        private void AddWordToGloballyVisitedTiles(WordContainer word)
        {
            foreach(WorldTile tile in word.Tiles)
            {
                globalVisitedThisCalculationTiles.Add(tile);
            }
        }

        private bool IsGrating(WordContainer candidateWord, IReadOnlyCollection<WordContainer> associatedWords)
        {
            foreach (WordContainer associatedWord in associatedWords)
            {
                if(IsGrating(candidateWord, associatedWord))
                {
                    Debug.Log($"<color=red>Candidate word '{candidateWord}' grates with associated word '{associatedWord}'</color>");
                    return true;
                }
            }

            Debug.Log($"<color=green>{candidateWord} is a valid word and doesn't grate with anything.</color>");
            return false;
        }

        private bool IsGrating(WordContainer candidateWord, WordContainer associatedWord)
        {
            const int INLINE = 0;
            const int PARLLEL_NOT_INLINE = 1;

            Assert.IsTrue(candidateWord.Axis == associatedWord.Axis, $"{candidateWord} and {associatedWord} have different orientations but are being compared for grating.");

            AddWordToGloballyVisitedTiles(candidateWord);

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
                    WordContainer lowerWord = candidateWord.LineIndex < associatedWord.LineIndex ? candidateWord : associatedWord;

                    switch (candidateWord.Axis)
                    {
                        case Axis.Horizontal:
                            {
                                Vector2Int range = GetRubbingRangeHorizontal(candidateWord, associatedWord);
                                for (int row = range.x; row <= range.y; ++row)
                                {
                                    if (AreTilesGratingVertically(row, lowerWord.LineIndex))
                                    {
                                        return true;
                                    }
                                }
                            }
                            break;
                        case Axis.Vertical:
                            {
                                Vector2Int range = GetRubbingRangeVertical(candidateWord, associatedWord);
                                for (int col = range.x; col <= range.y; ++col)
                                {
                                    if (AreTilesGratingHorizontally(lowerWord.LineIndex, col))
                                    {
                                        return true;
                                    }
                                }
                            }
                            break;
                    }
                    return false;

                default:
                    return false;
            }
        }

        private bool AreTilesGratingHorizontally(int row, int col)
        {
            WorldTile lowerTile = gameBoard.GetTile(row, col);
            Assert.IsNotNull(lowerTile, $"Tried to get tile at ({row},{col})");

            return !lowerTile.ShareAssociatedWordHorizontally(lowerTile.Right);
        }

        private bool AreTilesGratingVertically(int row, int col)
        {
            WorldTile lowerTile = gameBoard.GetTile(row, col);
            Assert.IsNotNull(lowerTile, $"Tried to get tile at ({row},{col})");

            return !lowerTile.ShareAssociatedWordVertically(lowerTile.Up);
        }

        private bool CheckIfNeighboursAreAlreadyVisisted(WorldTile tile)
        {
            return currentMatrix.HasVisitedTile(tile.Up)
                || currentMatrix.HasVisitedTile(tile.Down)
                || currentMatrix.HasVisitedTile(tile.Left)
                || currentMatrix.HasVisitedTile(tile.Right);
        }

        private Vector2Int GetRubbingRangeHorizontal(WordContainer word, WordContainer otherWord)
        {
            // https://scicomp.stackexchange.com/questions/26258/the-easiest-way-to-find-intersection-of-two-intervals
            if (word.FirstTileIndex > otherWord.LastTileIndex
                || otherWord.FirstTileIndex > word.LastTileIndex)
            {
                return Vector2Int.zero;
            }

            return new Vector2Int(Mathf.Max(word.FirstTileIndex, otherWord.FirstTileIndex), Mathf.Min(word.LastTileIndex, otherWord.LastTileIndex));
        }

        private Vector2Int GetRubbingRangeVertical(WordContainer word, WordContainer otherWord)
        {
            // https://scicomp.stackexchange.com/questions/26258/the-easiest-way-to-find-intersection-of-two-intervals
            // vertical words are only valid from top to bottom, so we need to invert how we do the checks.
            // board origin is in the bottom left
            if (word.FirstTileIndex < otherWord.LastTileIndex
                || otherWord.FirstTileIndex < word.LastTileIndex)
            {
                return Vector2Int.zero;
            }

            return new Vector2Int(Mathf.Max(word.LastTileIndex, otherWord.LastTileIndex), Mathf.Min(word.FirstTileIndex, otherWord.FirstTileIndex));
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

/*  todo, try and detect circular matrices to avoid redundant calculations
 *  example:
 *  DOOR
 *  E  O
 *  A  O
 *  LEFT
 *    O
 *    R
 * 
 *  If we dropped the R tile last, our first word would be FOOT and the second would be LEFT.
 *  However, there would be a permutation at DEAL and ROOT, and both matrices would end up circling around.
 *  We need to try and eventually find a way to detect that a matrix has become redundant.
 *  
 *  Maybe create a queue that only stores permutations created per matrix?  At the end, when that matrix is completed,
 *  check any permutations for the LastAddedWord.  If that word exists within the just completed matrix, the permutation is redudant?
 * */

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