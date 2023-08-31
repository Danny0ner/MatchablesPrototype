using System;
using UnityEngine;

//Managers to be used as aggrupation of functionality and be controlled from one place instead of repeating code through the project.
public abstract class Manager : MonoBehaviour
{
    public abstract void InitManager(Action onComplete);

    public abstract void DeinitManager();

    //Resume and stop could be called before and after scene changes to control input and other things while the new scene is loading, not used for the moment.

    //public abstract void ResumeManager(Action onComplete);

    //public abstract void StopManager(Action onComplete);
}
