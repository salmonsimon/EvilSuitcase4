using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

public class InputsUI : MonoBehaviour
{
    [Header("UI Input Values")]
    public Vector2 point;
    public bool click;
    public bool rightClick;
    public bool rotate;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    public void OnPoint(InputValue value)
    {
        PointInput(value.Get<Vector2>());
    }

    public void OnClick(InputValue value)
    {
        ClickInput(value.isPressed);
    }

    public void OnRightClick(InputValue value)
    {
        RightClickInput(value.isPressed);
    }

    public void OnRotate(InputValue value)
    {
        RotateInput(value.isPressed);
    }

#endif


    public void PointInput(Vector2 newPointPosition)
    {
        point = newPointPosition;
    }

    public void ClickInput(bool newClickState)
    {
        click = newClickState;
    }

    public void RightClickInput(bool newRightClickState)
    {
        rightClick = newRightClickState;
    }

    public void RotateInput(bool newRotateInput)
    {
        rotate = newRotateInput;
    }
}
