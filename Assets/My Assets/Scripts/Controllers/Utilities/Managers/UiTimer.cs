﻿using System;
using TMPro;
using UnityEngine;

public class UiTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text _timerText;
    public Action OnTimesUp;

    private float _currentTime; // Tracks time in seconds
    private bool _isRunning;    // Controls whether timer updates
    private bool _isCountingDown; // True for countdown, false for count-up

    private void Start()
    {
        StartTimer(152626);
    }
    public void StartTimer(int CountDownTime=0) // Time in seconds
    {
        _currentTime = CountDownTime;
        _isCountingDown = CountDownTime > 0; // Count down if positive, up if 0 or negative
        _isRunning = true;

        // Initial display
        UpdateTimerDisplay();
    }

    private void Update()
    {
        if (!_isRunning) return;

        // Update time based on direction
        if (_isCountingDown)
        {
            _currentTime -= Time.deltaTime;
            if (_currentTime <= 0)
            {
                _currentTime = 0; // Clamp to zero
                _isRunning = false;
                OnTimesUp?.Invoke(); // Trigger event if assigned
            }
        }
        else
        {
            _currentTime += Time.deltaTime; // Count up indefinitely
        }

        // Update UI
        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        if (_timerText == null)
        {
            Debug.LogWarning("Timer text is missing!");
            return;
        }

        // Convert to minutes and seconds
        int totalSeconds = Mathf.Abs(Mathf.FloorToInt(_currentTime)); // Absolute for display
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        // Format: Minutes can expand, seconds always 2 digits
        _timerText.text = $"{minutes:D2}:{seconds:D2}"; // D2 ensures at least 2 digits
    }

    public float GetTime()
    {
        return _currentTime; // Positive if counting up, negative or decreasing if down
    }
}