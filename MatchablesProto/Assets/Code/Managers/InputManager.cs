using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

//I'm using manager interfaces to limit the amount of functionality from a Manager that the rest of the project can use.
public interface IInputManager
{
    public void BlockInput(bool block);
}

//Input manager controls things relative to the game's input. 
public class InputManager : Manager, IInputManager
{
    [SerializeField] InputSystemUIInputModule _inputModule;

    public override void InitManager(Action onComplete)
    {
        onComplete?.Invoke();
    }

    public override void DeinitManager()
    {

    }

    public void BlockInput(bool block)
    {
        _inputModule.enabled = !block;
    }
}
