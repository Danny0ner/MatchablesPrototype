using System;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    [SerializeField] Manager[] _managers;
    [SerializeField] bool _loadMenuAfterInit = false;

    private readonly List<Manager> _managerInstances = new List<Manager>();

    //Creates and initializes managers, if a scene needs to be played by itself, add a MainController to the scene
    private void Awake()
    {
        Application.targetFrameRate = 60;

        MainController[] mainControllers = FindObjectsOfType<MainController>();
        if (mainControllers.Length == 1)
        {
            DontDestroyOnLoad(gameObject);
            InstantiateManagers();
        }
        else
            Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        for (int i = 0; i < _managerInstances.Count; i++)
        {
            _managerInstances[i].DeinitManager();
        }
    }

    private void InstantiateManagers()
    {
        for (int i = 0; i < _managers.Length; i++)
        {
            Manager manager = Instantiate(_managers[i], transform);

            if (manager is IInputManager inputMan)
                Engine.Input = inputMan;
            else if (manager is ISceneManager sceneMan)
                Engine.Scene = sceneMan;
            else if (manager is IGameplayManager gameMan)
                Engine.Gameplay = gameMan;

            _managerInstances.Add(manager);
        }

        InitializeManagers();
    }

    private void InitializeManagers()
    {
        if(_managerInstances.Count > 0)
        {
            InitializeManagersByIndex(default, () =>
            {
                AfterInitManagers();
            });
        }
        else
        {
            AfterInitManagers();
        }
    }

    private void InitializeManagersByIndex(int index, Action onComplete)
    {
        if (index < _managerInstances.Count)
        {
            _managerInstances[index].InitManager(() =>
            {
                InitializeManagersByIndex(++index, onComplete);
            });
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    private void AfterInitManagers()
    {
        if (_loadMenuAfterInit)
            Engine.Scene.GoToMainMenu();
    }
}
