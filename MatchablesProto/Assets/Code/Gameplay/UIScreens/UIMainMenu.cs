using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] Slider _rowsSlider;
    [SerializeField] Slider _columnsSlider;
    [SerializeField] Slider _elementsSlider;

    [SerializeField] TextMeshProUGUI _rowsValue;
    [SerializeField] TextMeshProUGUI _columnsValue;
    [SerializeField] TextMeshProUGUI _elementsValue;
    
    [SerializeField] Button _startGameButton;

    public void Start()
    {
        InitSliders();
        _startGameButton.onClick.AddListener(StartGame);
    }

    private void InitSliders()
    {
        _rowsSlider.minValue = Engine.Gameplay.MinGameRowsAndCols;
        _rowsSlider.maxValue = Engine.Gameplay.MaxGameRowsAndCols;
        _rowsSlider.onValueChanged.AddListener(OnRowsSliderChanged);
        _rowsSlider.value = _rowsSlider.minValue;
        OnRowsSliderChanged(_rowsSlider.minValue);


        _columnsSlider.minValue = Engine.Gameplay.MinGameRowsAndCols;
        _columnsSlider.maxValue = Engine.Gameplay.MaxGameRowsAndCols;
        _columnsSlider.onValueChanged.AddListener(OnColumnsSliderChanged);
        _columnsSlider.value = _columnsSlider.minValue;
        OnColumnsSliderChanged(_columnsSlider.minValue);


        _elementsSlider.minValue = Engine.Gameplay.MinGameElements;
        _elementsSlider.maxValue = Engine.Gameplay.MaxGameElements;
        _elementsSlider.onValueChanged.AddListener(OnElementsSliderChanged);
        _elementsSlider.value = _elementsSlider.minValue;
        OnElementsSliderChanged(_elementsSlider.minValue);
    }

    private void OnDestroy()
    {
        _rowsSlider.onValueChanged.RemoveListener(OnRowsSliderChanged);
        _columnsSlider.onValueChanged.RemoveListener(OnColumnsSliderChanged);
        _elementsSlider.onValueChanged.RemoveListener(OnElementsSliderChanged);
        _startGameButton.onClick.RemoveListener(StartGame);
    }

    private void OnRowsSliderChanged(float value)
    {
        _rowsValue.SetText(value.ToString());
        Engine.Gameplay.SetGameRows((int)value);
    }

    private void OnColumnsSliderChanged(float value)
    {
        _columnsValue.SetText(value.ToString()); 
        Engine.Gameplay.SetGameColumns((int)value);
    }

    private void OnElementsSliderChanged(float value)
    {
        _elementsValue.SetText(value.ToString());
        Engine.Gameplay.SetGameElements((int)value);
    }

    private void StartGame()
    {
        Engine.Scene.GoToGameplayScene();
    }
}
