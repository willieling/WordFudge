using System;
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

            public InputData(TouchPhase phase)
            {
                count = Inputcount.None;
                this.phase = phase;
                moveDelta = Vector2.zero;
                position = Vector2.zero;
            }
        }

        protected InputData currentInputData;

        protected virtual void Start()
        {
            //We interpret TouchPhase.Cancelled to mean no input
            currentInputData = new InputData(TouchPhase.Canceled);
        }

        protected void Update()
        {
            UpdateInputs();

            currentInputData.count = GetTouchCount();
            if (currentInputData.count == Inputcount.None)
            {
                switch (currentInputData.phase)
                {
                    case TouchPhase.Began:
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        currentInputData.phase = TouchPhase.Ended;
                        break;
                    case TouchPhase.Ended:
                        currentInputData.phase = TouchPhase.Canceled;
                        break;
                    case TouchPhase.Canceled:
                        //do nothing
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                currentInputData.phase = GetTouchPhase();
            }

            Vector2 currentFramePosition = GetTouchPosition();
            currentInputData.moveDelta = UpdateMoveDelta(currentFramePosition);
            currentInputData.position = currentFramePosition;
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