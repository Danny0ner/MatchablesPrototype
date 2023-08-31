using UnityEngine;
using UnityEngine.UI;

public class UIGameplay : MonoBehaviour
{
    [SerializeField] GridController _gridController;
    [SerializeField] UIPause _uiPause;
    [SerializeField] UIScoreMarker _uiScore;

    [Header("Buttons")]
    [SerializeField] Button _pauseButton;

    public UIScoreMarker UiScore => _uiScore;

    private void Awake()
    {
        _uiPause.InitMenu(this);
        _uiPause.ToggleVisibility(false);
    }

    private void Start()
    {
        _uiScore.InitScore();
        _gridController.Init(this);
        _pauseButton.onClick.AddListener(OpenPauseMenu); 
    }

    public void ReturnToMainMenu()
    {
        Engine.Scene.GoToMainMenu();
    }

    private void OpenPauseMenu()
    {
        _uiPause.ToggleVisibility(true);
    }
}
