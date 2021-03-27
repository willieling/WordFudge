using System;
using UnityEngine;
using WordFudge.Boards;
using WordFudge.InputSystem;

namespace WordFudge
{
    public class InputHandler
    {
        private BaseInputDetector inputDetector;
        private WordFudgeGameplay gameplay;

        public InputHandler(BaseInputDetector inputDetector, WordFudgeGameplay gameplay)
        {
            this.inputDetector = inputDetector;
            this.gameplay = gameplay;
        }

        public void Update()
        {
            BaseInputDetector.InputData inputData = inputDetector.GetCurrentInputData();

            switch (inputData.count)
            {
                case BaseInputDetector.Inputcount.Single:
                    HandleTileDrag(inputData);
                    break;
                case BaseInputDetector.Inputcount.Double:
                    HandleGridDrag(inputData);
                    break;
                default:
                    return;
            }
        }

        private void HandleTileDrag(BaseInputDetector.InputData inputData)
        {
            const int MAX_DISTANCE = 1;

            WorldTile tile = null;
            GameBoard gameBoard = null;
            TileTray tileTray = null;
            GetObjects(Physics2D.RaycastAll(inputData.position, Vector2.right, MAX_DISTANCE), ref tile, ref gameBoard, ref tileTray);
            if(tile == null)
            {
                return;
            }

            Debug.Log($"Hit tile: {tile.Character}");

            //if(gameBoard != null)
            //{
            //    tile.transform.SetParent(gameBoard.transform);
            //}

            //if (tileTray != null)
            //{
            //    tile.transform.SetParent(tileTray.transform);
            //}

        }

        private void GetObjects(RaycastHit2D[] hits, ref WorldTile tile, ref GameBoard gameBoard, ref TileTray tileTray)
        {
            foreach(RaycastHit2D hit in hits)
            {
                TrySet<WorldTile>(hit, ref tile);
                TrySet<GameBoard>(hit, ref gameBoard);
                TrySet<TileTray>(hit, ref tileTray);
            }
        }

        private void TrySet<TType>(RaycastHit2D hit, ref TType refValue) where TType : MonoBehaviour
        {
            TType casted = hit.collider.GetComponent<TType>();
            if(casted != null)
            {
                refValue = casted;
            }
        }

        private void HandleGridDrag(BaseInputDetector.InputData inputData)
        {
            throw new NotImplementedException();
        }
    }
}
