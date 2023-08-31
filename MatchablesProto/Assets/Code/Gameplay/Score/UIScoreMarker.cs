using System.Collections;
using TMPro;
using UnityEngine;

public class UIScoreMarker : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _scoreValue;
    [SerializeField] float _timeToUpdateScore = 0.8f;

    private int _score = default;
    private int _lastScore = default;

    private Coroutine _updateScoreRoutine = null;

    public void InitScore()
    {
        Engine.Gameplay.SetScore(default);
        SetScore(default);
    }

    private void OnDestroy()
    {
        StopRoutine();
    }

    private void SetScore(int value)
    {
        _scoreValue.SetText(value.ToString());
        _score = value;
    }

    public void AddScore(int scoreToAdd)
    {
        _lastScore = _score;
        _score += scoreToAdd;

        StopRoutine();
        _updateScoreRoutine = StartCoroutine(UpdateScore());
    }

    IEnumerator UpdateScore()
    {
        _scoreValue.SetText(_lastScore.ToString());

        float currentScore = default;

        float currentTime = default;

        while(currentTime < _timeToUpdateScore)
        {
            currentScore = Mathf.Lerp(_lastScore, _score, currentTime / _timeToUpdateScore);
            _scoreValue.SetText(((int)currentScore).ToString());

            currentTime += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        _scoreValue.SetText(_score.ToString());
    }

    private void StopRoutine()
    {
        if (_updateScoreRoutine != null)
        {
            StopCoroutine(_updateScoreRoutine);
            _updateScoreRoutine = null;
        }
    }
}
