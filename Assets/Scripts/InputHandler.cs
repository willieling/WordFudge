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
        private WordFudgeGameplay gameplay;
        private WorldTile selectedTile;

        public InputHandler(BaseInputDetector inputDetector, WordFudgeGameplay gameplay)
        {
            this.inputDetector = inputDetector;
            //needed?
            this.gameplay = gameplay;
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

        private WorldTile TryHitTile(BaseInputDetector.InputData inputData)
        {
            WordFudgeHitResults results = DetectHitObjects(inputData);
            return results.tile;

            //const int MAX_DISTANCE = 1;

            //RaycastHit2D hit = Physics2D.Raycast(inputData.position, Vector2.right, MAX_DISTANCE);
            //if(hit.collider != null)
            //{
            //    return hit.collider.GetComponent<WorldTile>();
            //}

            //return null;

            //WorldTile hitTile = null;
            //GameBoard gameBoard = null;
            //TileTray tileTray = null;
            //GetObjects(, ref hitTile, ref gameBoard, ref tileTray);
            //if (hitTile != null)
            //{
            //    Debug.Log($"Hit tile: {hitTile.Character}");

            //    selectedTile = hitTile;
            //    selectedTile.VisuallySelect();
            //}
        }

        private void PickupTile(BaseInputDetector.InputData inputData)
        {
            WordFudgeHitResults results = DetectHitObjects(inputData);
            if (results.gameBoard != null)
            {
                results.gameBoard.PickupTile(results.tile);
            }
            else
            {
                results.tileTray.RemoveTile(results.tile);
            }

            results.tile.PickUp();
        }

        private void MoveSelectedTile(BaseInputDetector.InputData inputData)
        {
            selectedTile.transform.position = new Vector3(inputData.position.x, inputData.position.y, selectedTile.transform.position.z);
        }

        private void DropTile(BaseInputDetector.InputData inputData)
        {
            WordFudgeHitResults results = DetectHitObjects(inputData);
            if(results.gameBoard != null)
            {
                results.gameBoard.DropTile(results.tile);
            }
            else
            {
                results.tileTray.AddTile(results.tile);
            }
        }

        private void HandleDoubleInput(BaseInputDetector.InputData inputData)
        {
            throw new NotImplementedException();
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
