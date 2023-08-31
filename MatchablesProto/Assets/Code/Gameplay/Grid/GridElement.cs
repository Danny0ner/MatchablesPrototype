using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class GridElement : MonoBehaviour
{
    [SerializeField] Image _icon;
    [SerializeField] Button _button;
    [SerializeField] RectTransform _rectTrans;

    [Space, Header("Falling settings")]
    [SerializeField] float _fallingAcceleration = 0.005f;
    [SerializeField] float _shakeDuration = 2f;
    [SerializeField] float _shakeStrength = 0.1f;

    [SerializeField] float _destroyTime = 0.2f;

    [HideInInspector] public ElementType Type;

    public bool IsCheckedThisTurn => _checkedThisTurn;
    private bool _checkedThisTurn = false;

    private int _gridPosX, _gridPosY;
    private GridController _controller;

    private Action<int, int> _onElementClicked = null;
    private Action _onElementFallEnded;

    private Coroutine _fallingRoutine = null;
    private Coroutine _destroyRoutine = null;

    private Vector3 _shakeDirection;

    private void Awake()
    {
        _shakeDirection = new Vector3(default, _shakeStrength, default);
    }

    private void OnDestroy()
    {
        if(_fallingRoutine != null)
        {
            StopCoroutine(_fallingRoutine);
            _fallingRoutine = null;
        }

        if(_destroyRoutine != null)
        {
            StopCoroutine(_destroyRoutine);
            _destroyRoutine = null;
        }
    }

    //Initializing element
    public void InitElement(GridController controller, int gridPosX, int gridPosY, float elementSize, ElementType type, Action<int, int> onElementClicked, Action onFallEnded)
    {
        _controller = controller;
        _gridPosX = gridPosX;
        _gridPosY = gridPosY;
        Type = type;
        _onElementClicked = onElementClicked;
        _onElementFallEnded = onFallEnded;

        _rectTrans.sizeDelta = new Vector2(elementSize, elementSize);

        _icon.sprite = _controller.ElementSettings.GetElementSprite(type);
        _button.onClick.AddListener(ElementClicked);
    }

    public void StartFall()
    {
        _fallingRoutine = StartCoroutine(FallingToPosition());
    }

    public void UpdateElementPosition(int gridPosX, int gridPosY)
    {
        _gridPosX = gridPosX;
        _gridPosY = gridPosY;
    }

    public void SetElementToDestroy()
    {
        _checkedThisTurn = true;
    }

    public void DestroyElement()
    {
        _destroyRoutine = StartCoroutine(DestroyRoutine());
    }

    private void ElementClicked()
    {
        _onElementClicked?.Invoke(_gridPosX, _gridPosY);
    }

    private IEnumerator FallingToPosition()
    {
        Vector3 originalPosition = transform.localPosition;
        float finalPosition = -_controller.GridHalfHeight - _controller.HalfElementSize + (_controller.Rows - _gridPosY) * _controller.ElementSize - ((_controller.Rows - _gridPosY) * -_controller.ElementSepparation);

        float fallDuration = CalculateFallDuration(originalPosition.y - finalPosition, default, _fallingAcceleration);

        transform.DOLocalMoveY(finalPosition, fallDuration)
            .SetEase(Ease.InSine);

        yield return new WaitForSeconds(fallDuration);

        transform.localPosition = new Vector3(transform.localPosition.x, finalPosition);
        transform.DOShakePosition(_shakeDuration, _shakeDirection);
        yield return new WaitForSeconds(_shakeDuration);

        _onElementFallEnded?.Invoke();
    }

    private IEnumerator DestroyRoutine()
    {
        transform.DOScale(0, _destroyTime);

        yield return new WaitForSeconds(_destroyTime);
        
        SelfDestroy();
    }

    private void SelfDestroy()
    {
        Destroy(this.gameObject);
    }

    private float CalculateFallDuration(float distance, float initialSpeed, float acceleration)
    {
        return Mathf.Sqrt(2f * distance / acceleration) + initialSpeed;
    }
}
