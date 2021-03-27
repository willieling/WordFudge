using System;
using System.Collections.Generic;
using UnityEngine;

namespace WordFudge.InputSystem
{
    public class TouchInputDetector : BaseInputDetector
    {
        protected const int MAX_TOUCHES = 2;

        protected List<Touch> Touches;

        protected override void Start()
        {
            Touches = new List<Touch>(MAX_TOUCHES);
        }

        protected override void UpdateInputs()
        {
            Touches.Clear();

            int index = 0;
            for (index = 0; index < Input.touchCount; ++index)
            {
                Touches[index] = Input.GetTouch(index);
            }

            Touches.RemoveRange(index, Touches.Count);
        }

        protected override Inputcount GetTouchCount()
        {
            switch (Input.touchCount)
            {
                case 1:
                    return Inputcount.Single;
                case 2:
                    return Inputcount.Double;
                default:
                    return Inputcount.None;
            }
        }

        protected override TouchPhase GetTouchPhase()
        {
            return Input.GetTouch(0).phase;
        }

        protected override Vector2 GetTouchPosition()
        {
            return Input.GetTouch(0).position;
        }

        protected override Vector2 UpdateMoveDelta(Vector2 currentPosition)
        {
            switch (currentInputData.phase)
            {
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    return currentPosition - currentInputData.position;
                default:
                    return Vector2.zero;
            }
        }
        }
}