using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public bool pause;
    public bool inventory;
    public bool autoSort;
    public bool next;
    public bool previous;
    public bool enter;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED

    public void ResetInputs() 
    {
        point = Vector2.zero;
        click = false;
        rightClick = false;
        rotate = false;
        pause = false;
        inventory = false;
        autoSort = false;
        next = false;
        previous = false;
        enter = false;
    }


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

    public void OnPause(InputValue value)
    {
        PauseInput(value.isPressed);
    }

    public void OnInventory(InputValue value)
    {
        InventoryInput(value.isPressed);
    }


    public void OnAutoSort(InputValue value)
    {
        AutoSortInput(value.isPressed);
    }

    public void OnNext(InputValue value)
    {
        NextInput(value.isPressed);
    }

    public void OnPrevious(InputValue value)
    {
        PreviousInput(value.isPressed);
    }

    public void OnEnter(InputValue value)
    {
        EnterInput(value.isPressed);
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

    public void RotateInput(bool newRotateState)
    {
        rotate = newRotateState;
    }

    public void PauseInput(bool newPauseState)
    {
        pause = newPauseState;
    }

    public void InventoryInput(bool newInventoryState)
    {
        inventory = newInventoryState;
    }

    public void AutoSortInput(bool newAutoSortState)
    {
        autoSort = newAutoSortState;
    }

    public void NextInput(bool newNextState)
    {
        next = newNextState;
    }

    public void PreviousInput(bool newPreviousState)
    {
        previous = newPreviousState;
    }

    public void EnterInput(bool newEnterState)
    {
        enter = newEnterState;
    }
}
