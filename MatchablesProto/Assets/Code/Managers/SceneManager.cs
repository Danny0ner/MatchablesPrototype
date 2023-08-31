using System;

//I'm using manager interfaces to limit the amount of functionality from a Manager that the rest of the project can use.
public interface ISceneManager
{
    public void GoToMainMenu();

    public void GoToGameplayScene();
}

public class SceneManager : Manager, ISceneManager
{
    private string _mainMenuScene = "MainMenu";
    public string _gameplayScene = "Gameplay";

    public override void InitManager(Action onComplete)
    {
        onComplete?.Invoke();
    }

    public override void DeinitManager()
    {

    }

    public void GoToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(_mainMenuScene);
    }

    public void GoToGameplayScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(_gameplayScene);
    }
}
