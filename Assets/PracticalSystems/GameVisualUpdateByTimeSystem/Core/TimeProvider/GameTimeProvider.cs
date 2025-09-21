using System;
using UnityEngine;
using GameVisualUpdateByTimeSystem.Core.Interfaces;

namespace GameVisualUpdateByTimeSystem.Core.TimeProvider
{
    /// <summary>
    /// Implementation of ITimeProvider for game time progression
    /// Supports configurable time scales and progression modes
    /// </summary>
    public class GameTimeProvider : ITimeProvider
    {
        private float _currentTime;
        private float _timeSpeed = 1.0f;
        private bool _isPaused = false;
        
        // Current time components
        private int _currentHour;
        private int _currentMinute;
        private int _currentDay = 1;
        private int _currentMonth = 1;
        private int _currentYear = 2024;
        
        // Time tracking for change detection
        private int _lastHour = -1;
        private int _lastDay = -1;
        private int _lastMonth = -1;
        private int _lastYear = -1;
        private int _lastMinute = -1;
        
        public float CurrentTime => _currentTime;
        public int CurrentHour => _currentHour;
        public int CurrentMinute => _currentMinute;
        public int CurrentDay => _currentDay;
        public int CurrentMonth => _currentMonth;
        public int CurrentYear => _currentYear;
        
        public float TimeSpeed
        {
            get => _timeSpeed;
            set => _timeSpeed = Mathf.Max(0f, value);
        }
        
        public bool IsPaused
        {
            get => _isPaused;
            set => _isPaused = value;
        }
        
        public event Action<TimeChangeType> OnTimeChanged;
        
        public GameTimeProvider()
        {
            InitializeTime();
        }
        
        public GameTimeProvider(int hour, int minute, int day, int month, int year)
        {
            SetTime(hour, minute, day, month, year);
        }
        
        private void InitializeTime()
        {
            // Start at 6 AM on day 1
            SetTime(6, 0, 1, 1, 2024);
        }
        
        public void UpdateTime(float deltaTime)
        {
            if (_isPaused) return;
            
            // Advance time based on speed
            float timeAdvancement = deltaTime * _timeSpeed;
            _currentTime += timeAdvancement;
            
            // Update time components
            UpdateTimeComponents();
            
            // Check for time changes and fire events
            CheckForTimeChanges();
        }
        
        private void UpdateTimeComponents()
        {
            // Convert seconds to time components
            int totalMinutes = Mathf.FloorToInt(_currentTime / 60f);
            int totalHours = totalMinutes / 60;
            int totalDays = totalHours / 24;
            
            _currentMinute = totalMinutes % 60;
            _currentHour = totalHours % 24;
            _currentDay = (totalDays % 30) + 1; // Simplified: 30 days per month
            _currentMonth = ((totalDays / 30) % 12) + 1;
            _currentYear = 2024 + (totalDays / (30 * 12));
        }
        
        private void CheckForTimeChanges()
        {
            if (_currentMinute != _lastMinute)
            {
                _lastMinute = _currentMinute;
                OnTimeChanged?.Invoke(TimeChangeType.Minute);
            }
            
            if (_currentHour != _lastHour)
            {
                _lastHour = _currentHour;
                OnTimeChanged?.Invoke(TimeChangeType.Hour);
            }
            
            if (_currentDay != _lastDay)
            {
                _lastDay = _currentDay;
                OnTimeChanged?.Invoke(TimeChangeType.Day);
            }
            
            if (_currentMonth != _lastMonth)
            {
                _lastMonth = _currentMonth;
                OnTimeChanged?.Invoke(TimeChangeType.Month);
            }
            
            if (_currentYear != _lastYear)
            {
                _lastYear = _currentYear;
                OnTimeChanged?.Invoke(TimeChangeType.Year);
            }
        }
        
        public float GetNormalizedTime(TimeType timeType)
        {
            return timeType switch
            {
                TimeType.Hour => _currentHour / 24f,
                TimeType.Day => (_currentDay - 1) / 30f, // Simplified: 30 days per month
                TimeType.Month => (_currentMonth - 1) / 12f,
                TimeType.Year => (_currentYear - 2024) / 100f, // Normalize over 100 years
                TimeType.Seasonal => GetSeasonalNormalizedTime(),
                _ => 0f
            };
        }
        
        private float GetSeasonalNormalizedTime()
        {
            // Map months to seasons (Spring: 3-5, Summer: 6-8, Autumn: 9-11, Winter: 12-2)
            int seasonMonth = ((_currentMonth - 3 + 12) % 12);
            return seasonMonth / 12f;
        }
        
        public void SetTime(int hour, int minute, int day, int month, int year)
        {
            _currentHour = Mathf.Clamp(hour, 0, 23);
            _currentMinute = Mathf.Clamp(minute, 0, 59);
            _currentDay = Mathf.Clamp(day, 1, 31);
            _currentMonth = Mathf.Clamp(month, 1, 12);
            _currentYear = Mathf.Max(year, 1);
            
            // Update internal time representation
            _currentTime = CalculateTimeInSeconds();
            
            // Reset change tracking
            _lastHour = _currentHour;
            _lastMinute = _currentMinute;
            _lastDay = _currentDay;
            _lastMonth = _currentMonth;
            _lastYear = _currentYear;
        }
        
        public void AdvanceTime(float seconds)
        {
            _currentTime += seconds;
            UpdateTimeComponents();
            CheckForTimeChanges();
        }
        
        private float CalculateTimeInSeconds()
        {
            float totalMinutes = _currentMinute + (_currentHour * 60f);
            float totalHours = totalMinutes / 60f;
            float totalDays = (_currentDay - 1) + (totalHours / 24f);
            float totalMonths = (_currentMonth - 1) * 30f + totalDays; // Simplified: 30 days per month
            float totalYears = (_currentYear - 2024) * (12f * 30f) + totalMonths;
            
            return totalYears * (365f * 24f * 60f * 60f); // Convert to seconds
        }
        
        public Season GetCurrentSeason()
        {
            return _currentMonth switch
            {
                3 or 4 or 5 => Season.Spring,
                6 or 7 or 8 => Season.Summer,
                9 or 10 or 11 => Season.Autumn,
                _ => Season.Winter
            };
        }
        
        public float GetDayProgress()
        {
            return (_currentHour + _currentMinute / 60f) / 24f;
        }
        
        public float GetMonthProgress()
        {
            return (_currentDay - 1) / 30f; // Simplified: 30 days per month
        }
        
        public float GetYearProgress()
        {
            return ((_currentMonth - 1) * 30f + (_currentDay - 1)) / (12f * 30f);
        }
    }
}
