using System;
using UnityEngine;
using WordFudge.Boards;
using WordFudge.InputSystem;

namespace WordFudge
{
    public class InputHandler
    {
        struct WordFudgeHitResults
        {
            public WorldTile tile;
            public GameBoard gameBoard;
            public TileTray tileTray;
        }

        private BaseInputDetector inputDetector;
        private WorldTile selectedTile;

        public InputHandler(BaseInputDetector inputDetector, WordFudgeGameplay gameplay)
        {
            this.inputDetector = inputDetector;
        }

        public void Update()
        {
            BaseInputDetector.InputData inputData = inputDetector.GetCurrentInputData();

            switch (inputData.count)
            {
                case BaseInputDetector.Inputcount.Single:
                    HandleSingleInput(inputData);
                    break;
                case BaseInputDetector.Inputcount.Double:
                    HandleDoubleInput(inputData);
                    break;
                default:
                    HandleNoInput(inputData);
                    return;
            }
        }

        private void HandleSingleInput(BaseInputDetector.InputData inputData)
        {
            switch (inputData.phase)
            {
                case TouchPhase.Began:
                    if(selectedTile != null)
                    {
                        //todo error
                    }
                    PickupTile(inputData);
                    break;
                case TouchPhase.Moved:
                    MoveSelectedTile(inputData);
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    DropTile(inputData);
                    break;
            }
        }

        private void PickupTile(BaseInputDetector.InputData inputData)
        {
            WordFudgeHitResults results = DetectHitObjects(inputData);
            if(results.tile == null)
            {
                return;
            }

            selectedTile = results.tile;

            if (results.gameBoard != null)
            {
                results.gameBoard.RemoveTile(selectedTile);
            }
            else if (results.tileTray != null)
            {
                results.tileTray.RemoveTile(selectedTile);
            }
            else
            {
                throw new NotImplementedException("Can't drop a tile onto nothing");
            }

            selectedTile.ShowPickUp();
        }

        private void MoveSelectedTile(BaseInputDetector.InputData inputData)
        {
            if(selectedTile == null)
            {
                return;
            }
            selectedTile.transform.position = new Vector3(inputData.position.x, inputData.position.y, selectedTile.transform.position.z);
        }

        private void DropTile(BaseInputDetector.InputData inputData)
        {
            WordFudgeHitResults results = DetectHitObjects(inputData);
            if(results.tile == null)
            {
                // this can happen when dragging nothing
                // which is a valid case
                return;
            }

            if(results.gameBoard != null)
            {
                results.gameBoard.AddTile(selectedTile);
            }
            else if(results.tileTray != null)
            {
                results.tileTray.AddTile(selectedTile);
            }
            else
            {
                throw new NotImplementedException("Can't drop a tile onto nothing");
            }

            selectedTile.ShowPutDownAndExcluded();
            selectedTile = null;
        }

        private void HandleDoubleInput(BaseInputDetector.InputData inputData)
        {
            throw new NotImplementedException();
        }


        private void HandleNoInput(BaseInputDetector.InputData inputData)
        {
            switch (inputData.phase)
            {
                case TouchPhase.Ended:
                    DropTile(inputData);
                    break;
            }
        }

        private WordFudgeHitResults DetectHitObjects(BaseInputDetector.InputData inputData)
        {
            const int MAX_DISTANCE = 1;

            RaycastHit2D[] hits = Physics2D.RaycastAll(inputData.position, Vector2.right, MAX_DISTANCE);

            WordFudgeHitResults results = new WordFudgeHitResults();
            foreach (RaycastHit2D hit in hits)
            {
                TrySet(hit, ref results.tile);
                TrySet(hit, ref results.gameBoard);
                TrySet(hit, ref results.tileTray);
            }
            return results;
        }

        private void TrySet<TType>(RaycastHit2D hit, ref TType refValue) where TType : MonoBehaviour
        {
            TType casted = hit.collider.GetComponent<TType>();
            if (casted != null)
            {
                refValue = casted;
            }
        }
    }
}
