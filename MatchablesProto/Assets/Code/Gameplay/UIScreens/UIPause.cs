using UnityEngine;
using UnityEngine.UI;

public class UIPause : MonoBehaviour
{
    [SerializeField] Button _returnToGame;
    [SerializeField] Button _returnToMenu;

    private UIGameplay _uiGameplay;

    public void InitMenu(UIGameplay uiGameplay)
    {
        _uiGameplay = uiGameplay;
    }

    private void OnEnable()
    {
        _returnToGame.onClick.AddListener(Hide);
        _returnToMenu.onClick.AddListener(ReturnToMainMenu);
    }

    private void OnDisable()
    {
        _returnToGame.onClick.RemoveListener(Hide);
        _returnToMenu.onClick.RemoveListener(ReturnToMainMenu);
    }

    public void ToggleVisibility(bool show)
    {
        gameObject.SetActive(show);
    }

    private void Hide()
    {
        ToggleVisibility(false);
    }

    private void ReturnToMainMenu()
    {
        _uiGameplay.ReturnToMainMenu();
    }
}
