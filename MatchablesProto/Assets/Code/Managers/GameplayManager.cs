using System;
using UnityEngine;

//I'm using manager interfaces to limit the amount of functionality from a Manager that the rest of the project can use.
public interface IGameplayManager
{
    public int MinGameRowsAndCols {get;}
    public int MaxGameRowsAndCols {get;}
    public int MinGameElements { get;}
    public int MaxGameElements { get;}

    public int GameRows { get; }
    public int GameColumns { get; }
    public int GameElements { get; }

    public int CurrentScore { get; }
    public int ScorePerElement { get; }

    public void SetScore(int score);

    public void SetGameRows(int value);

    public void SetGameColumns(int value);

    public void SetGameElements(int value);
}

//Gameplay manager is used to store gameplay settings. It will be consulted from the gameplay scene to set the parameters set in the mainmenu.
public class GameplayManager : Manager, IGameplayManager
{
    public int _minGameRowsAndCols = 5;
    public int _maxGameRowsAndCols = 20;

    public int _minGameElements = 3;
    public int _maxGameElements = 6;

    public int MinGameRowsAndCols => _minGameRowsAndCols;
    public int MaxGameRowsAndCols => _maxGameRowsAndCols;
    public int MinGameElements => _minGameElements;
    public int MaxGameElements => _maxGameElements;
    
    private int _gameRows;
    private int _gameColumns;
    private int _gameElements;

    public int GameRows => _gameRows;
    public int GameColumns => _gameColumns;
    public int GameElements => _gameElements;


    private int _currentScore = default;
    public int CurrentScore => _currentScore;

    [SerializeField] int _scorePerElement = 300;
    public int ScorePerElement => _scorePerElement;

    public override void InitManager(Action onComplete)
    {
        _gameRows = _minGameRowsAndCols;
        _gameColumns = _minGameRowsAndCols;
        _gameElements = _minGameElements;

        onComplete?.Invoke();
    }

    public override void DeinitManager()
    {

    }

    public void SetGameRows(int value)
    {
        _gameRows = Math.Clamp(value, _minGameRowsAndCols, _maxGameRowsAndCols);
    }

    public void SetGameColumns(int value)
    {
        _gameColumns = Math.Clamp(value, _minGameRowsAndCols, _maxGameRowsAndCols);
    }

    public void SetGameElements(int value)
    {
        _gameElements = Math.Clamp(value, _minGameElements, _maxGameElements);
    }

    public void SetScore(int score)
    {
        _currentScore = score;
    }
}
