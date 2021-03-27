using System.Collections.Generic;
using UnityEngine;

namespace WordFudge.InputSystem
{
    public abstract class BaseInputDetector : MonoBehaviour
    {
        public enum Inputcount
        {
            None,
            Single,
            Double
        }

        public struct InputData
        {
            public Inputcount count;
            public TouchPhase phase;
            public Vector2 moveDelta;
            public Vector2 position;
        }

        protected InputData currentInputData;

        protected virtual void Start()
        {
            currentInputData = new InputData();
        }

        protected void Update()
        {
            UpdateInputs();

            currentInputData.count = GetTouchCount();
            if (currentInputData.count == Inputcount.None)
            {
                currentInputData.phase = TouchPhase.Ended;

                currentInputData.position = Vector2.zero;
                currentInputData.moveDelta = Vector2.zero;
            }
            else
            {
                currentInputData.phase = GetTouchPhase();

                Vector2 currentPosition = GetTouchPosition();
                currentInputData.moveDelta = UpdateMoveDelta(currentPosition);
                currentInputData.position = currentPosition;
            }
        }

        public InputData GetCurrentInputData()
        {
            return currentInputData;
        }

        protected abstract void UpdateInputs();
        protected abstract Inputcount GetTouchCount();
        protected abstract TouchPhase GetTouchPhase();
        protected abstract Vector2 GetTouchPosition();
        //inline?
        protected abstract Vector2 UpdateMoveDelta(Vector2 currentPosition);

#if UNITY_EDITOR
        [SerializeField]
        bool showDebug = false;

        Rect debugRect = new Rect(Vector2.zero, new Vector2(100, 800));

        void OnGUI()
        {
            if (showDebug)  // or check the app debug flag
            {
                GUI.Label(debugRect,
                    $"InputData\n" +
                    $"Count: {currentInputData.count}\n" +
                    $"Phase: {currentInputData.phase}\n" +
                    $"Position: {currentInputData.position}\n" +
                    $"MoveDelta: {currentInputData.moveDelta}\n");
            }
        }
#endif //UNITY_EDITOR
    }
}