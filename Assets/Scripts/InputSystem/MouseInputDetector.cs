using System;
using UnityEngine;

namespace WordFudge.InputSystem
{
    public class MouseInputDetector : BaseInputDetector
    {
        const Int16 LEFT_MOUSE_BUTTON = 0;
        const Int16 RIGHT_MOUSE_BUTTON = 1;

        protected override void UpdateInputs()
        {
        }

        protected override Inputcount GetTouchCount()
        {
            Inputcount count = Inputcount.None;
            if (Input.GetMouseButton(LEFT_MOUSE_BUTTON))
            {
                count = Inputcount.Single;
            }
            if (count == Inputcount.Single && Input.GetMouseButton(RIGHT_MOUSE_BUTTON))
            {
                count = Inputcount.Double;
            }

            return count;
        }

        protected override TouchPhase GetTouchPhase()
        {
            if(Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON) || Input.GetMouseButtonDown(RIGHT_MOUSE_BUTTON))
            {
                return TouchPhase.Began;
            }
            else if (Input.GetMouseButton(LEFT_MOUSE_BUTTON) || Input.GetMouseButton(RIGHT_MOUSE_BUTTON))
            {
                return TouchPhase.Moved;
            }

            return TouchPhase.Ended;
        }

        protected override Vector2 GetTouchPosition()
        {
            return Input.mousePosition;
        }

        protected override Vector2 UpdateMoveDelta(Vector2 currentPosition)
        {
            switch (currentInputData.phase)
            {
                case TouchPhase.Moved:
                    return currentPosition - currentInputData.position;
                default:
                    return Vector2.zero;
            }
        }
    }
}