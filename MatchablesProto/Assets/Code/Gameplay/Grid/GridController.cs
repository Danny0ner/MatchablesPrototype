using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    [Space, Header("Grid")]
    private int _rows = 5;
    private int _columns = 5;
    private int _differentElementTypes = 3;
    public int Rows => _rows;

    [SerializeField] RectTransform _gridContent;
    [SerializeField] RectTransform _gridBackground;
    public RectTransform GridContent => _gridContent;

    [Header("Elements")]
    [SerializeField] GridElement _elementPrefab;
    [SerializeField] ElementSettings _elementSettings;
    public ElementSettings ElementSettings => _elementSettings;

    [HideInInspector] public float ElementSize = 55;
    [HideInInspector] public float HalfElementSize;
    public float ElementSepparation = 1f;

    //Grid logic array
    private GridElement[,] _grid;

    private float _gridHalfWidth = 0f;
    public float GridHalfHeight { get; private set; }

    //variables to controll elements fall and cheking after selecting one
    private int _elementsToFall = 0;
    private int _fallsEnded = 0;

    private List<(int, int)> _elementsToCheck = new List<(int, int)>();
    private List<(int, int)> _elementsToDestroy = new List<(int, int)>();
    private int[] _elementsFalling;

    private UIGameplay _uiGameplay;

    public void Init(UIGameplay uiGameplay)
    {
        _uiGameplay = uiGameplay;
    }

    private void Start()
    {
        CreateGrid();

        Rect gridRect = GridContent.rect;
        Vector2 sizeDelta = GridContent.sizeDelta;

        ElementSize = Mathf.Min((gridRect.width - Mathf.Abs(sizeDelta.x)) / _columns, (gridRect.height - Mathf.Abs(sizeDelta.y)) / _rows) - ElementSepparation;
        HalfElementSize = ElementSize / 2f;

        float gridWidth = _columns * (ElementSize + 1);
        float gridHeight = _rows * (ElementSize + 1);

        _gridBackground.sizeDelta = new Vector2(gridWidth, gridHeight);

        _gridHalfWidth = gridWidth * .5f;
        GridHalfHeight = gridHeight * .5f;

        InitGrid();
    }

    //Creates grid logic based on selected rows, columns and number of elements
    private void CreateGrid()
    {
        _rows = Engine.Gameplay.GameRows;
        _columns = Engine.Gameplay.GameColumns;
        _differentElementTypes = Engine.Gameplay.GameElements;

        _grid = new GridElement[_rows, _columns];
        _elementsFalling = new int[_columns];
    }

    //Creating grid elements
    private void InitGrid()
    {
        Engine.Input.BlockInput(true);

        for (int i = _rows - 1; i >= 0; i--)
        {
            for (int x = 0; x < _columns; x++)
            {
                _grid[i, x] = CreateRandomElement(x, i);
            }
        }

        StartCoroutine(StartFallingGrid());
    }

    //Creates random element for hte grid 
    private GridElement CreateRandomElement(int posX, int posY)
    {
        _elementsToFall++;
        _elementsFalling[posX]++;

        GridElement newElement = Instantiate(_elementPrefab, _gridBackground);

        //the initial position of the element is calculated based on it's X position relative to the grid.
        //The Y position depends on the quantity of elements falling, they're instantiated one above the last one to make them fall correctly
        newElement.transform.localPosition = new Vector3(-_gridHalfWidth + HalfElementSize + (posX * ElementSize) + (ElementSepparation * posX),  GridHalfHeight + _elementsFalling[posX]* ElementSize);
        
        ElementType elemType = (ElementType)UnityEngine.Random.Range(1, _differentElementTypes + 1);
        newElement.InitElement(this, posX, posY, ElementSize, elemType,  ElementClicked, OnElementFallEnded);

        return newElement;
    }

    //Once the grid is full created, we make them fall row by row, giving the grid a more interesting creation animation
    private IEnumerator StartFallingGrid()
    {
        for (int x = _rows - 1; x >= 0; x--)
        { 
            for (int i = 0; i < _columns; i++)
                _grid[x, i].StartFall();

            yield return new WaitForSeconds(0.1f);
        }
    }


    //Called when an element is clicked, calls function to destroy the element and check the ones next to it. 
    public void ElementClicked(int posX, int posY)
    {
        Engine.Input.BlockInput(true);

        _grid[posY, posX].SetElementToDestroy();
        
        _elementsToDestroy.Clear();
        _elementsToDestroy.Add((posY, posX));

        _elementsToCheck.Clear();
        _elementsToCheck.Add((posY, posX));

        CheckElementsToDestroy(_grid[posY, posX].Type);

        UpdateGrid();
    }

    //Recursive function that checks if elements has to be destroyed. The inside methods will add new elements to the _elementsToCheckList and it will keep going till the list is empty
    private void CheckElementsToDestroy(ElementType type)
    {
        int elementsToCheck = _elementsToCheck.Count;

        for (int i = elementsToCheck - 1; i >= 0; i--)
            CheckElementsAround(_elementsToCheck[i].Item1, _elementsToCheck[i].Item2, type);

        for (int i = 0; i < elementsToCheck; i++)
            _elementsToCheck.RemoveAt(0);

        if (_elementsToCheck.Count > 0)
            CheckElementsToDestroy(type);
    }

    //Checks elements around
    private void CheckElementsAround(int posX, int posY, ElementType type)
    {
        int toCheckX = posX - 1;
        int toCheckY = posY;

        CheckElement(toCheckX, toCheckY, type);

        toCheckX = posX + 1;
        toCheckY = posY;

        CheckElement(toCheckX, toCheckY, type);

        toCheckX = posX;
        toCheckY = posY - 1;

        CheckElement(toCheckX, toCheckY, type);

        toCheckX = posX;
        toCheckY = posY + 1;

        CheckElement(toCheckX, toCheckY, type);
    }

    //If the element is the correc type, it will be added to the toCheck and toDestroy lists.
    private void CheckElement(int posX, int posY, ElementType type)
    {
        GridElement tempElement = null;

        if (posX >= 0 && posX < _rows && posY >= 0 && posY < _columns) 
        {
            tempElement = _grid[posX, posY];
            if (tempElement.Type == type)
            {
                if (!tempElement.IsCheckedThisTurn)
                {
                    tempElement.SetElementToDestroy();
                    _elementsToDestroy.Add((posX, posY));
                    _elementsToCheck.Add((posX, posY));
                }
            }
        }
    }

    //Once every element to be destroy is found, we destroy them and update the elements position in the grid
    private void UpdateGrid()
    {
        int extraPoints = _elementsToDestroy.Count * Engine.Gameplay.ScorePerElement;
        _uiGameplay.UiScore.AddScore(extraPoints);

        int toDestroyX, toDestroyY;
        for (int i = 0; i < _elementsToDestroy.Count; i++)
        {
            toDestroyX = _elementsToDestroy[i].Item1;
            toDestroyY = _elementsToDestroy[i].Item2;

            _grid[toDestroyX, toDestroyY].DestroyElement();
            _grid[toDestroyX, toDestroyY] = null;
        }


        for (int x = _rows - 1; x >= 0; x--)
        {
            for (int y = _columns - 1; y >= 0; y--)
            {
                if (_grid[x, y] == null)
                    FindElementToFall(x, y);
            }
        }

    }

    //Looks for an above element to fall in the given position. If there isn't a valid position, it creates a new element.
    private void FindElementToFall(int posX, int posY)
    {
        for (int x = posX - 1; x >= 0; x--)
        {
            if (_grid[x, posY] != null)
            {
                _grid[posX, posY] = _grid[x, posY];
                _grid[posX, posY].UpdateElementPosition(posY, posX);
                _grid[x, posY] = null;
                
                _elementsToFall++;
                _grid[posX, posY].StartFall();
                return;
            }
        }

        if (_grid[posX, posY] == null)
        {
            _grid[posX, posY] = CreateRandomElement(posY, posX);
            _grid[posX, posY].StartFall();
        }
    }

    //Callback for when an element has ended its fall animation.
    private void OnElementFallEnded()
    {
        _fallsEnded++;

        if(_fallsEnded == _elementsToFall)
        {
            ResetElementsFalling();
            Engine.Input.BlockInput(false);
        }
    }

    private void ResetElementsFalling()
    {
        _elementsToFall = 0;
        _fallsEnded = 0;

        for (int i = 0; i < _elementsFalling.Length; i++)
            _elementsFalling[i] = 0;
    }
}
