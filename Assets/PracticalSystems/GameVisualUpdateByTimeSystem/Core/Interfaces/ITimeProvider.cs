using System;
using UnityEngine;

namespace GameVisualUpdateByTimeSystem.Core.Interfaces
{
    /// <summary>
    /// Provides time information for the visual update system.
    /// Supports different time scales and progression modes.
    /// </summary>
    public interface ITimeProvider
    {
        /// <summary>
        /// Current game time in seconds since epoch
        /// </summary>
        float CurrentTime { get; }
        
        /// <summary>
        /// Current hour of the day (0-23)
        /// </summary>
        int CurrentHour { get; }
        
        /// <summary>
        /// Current minute of the hour (0-59)
        /// </summary>
        int CurrentMinute { get; }
        
        /// <summary>
        /// Current day of the month (1-31)
        /// </summary>
        int CurrentDay { get; }
        
        /// <summary>
        /// Current month (1-12)
        /// </summary>
        int CurrentMonth { get; }
        
        /// <summary>
        /// Current year
        /// </summary>
        int CurrentYear { get; }
        
        /// <summary>
        /// Time progression speed multiplier (1.0 = real time)
        /// </summary>
        float TimeSpeed { get; set; }
        
        /// <summary>
        /// Whether time progression is paused
        /// </summary>
        bool IsPaused { get; set; }
        
        /// <summary>
        /// Event fired when time changes significantly (hour, day, month, year)
        /// </summary>
        event Action<TimeChangeType> OnTimeChanged;
        
        /// <summary>
        /// Get normalized time value for interpolation (0-1)
        /// </summary>
        /// <param name="timeType">Type of time normalization</param>
        /// <returns>Normalized time value (0-1)</returns>
        float GetNormalizedTime(TimeType timeType);
        
        /// <summary>
        /// Set specific time values
        /// </summary>
        /// <param name="hour">Hour (0-23)</param>
        /// <param name="minute">Minute (0-59)</param>
        /// <param name="day">Day (1-31)</param>
        /// <param name="month">Month (1-12)</param>
        /// <param name="year">Year</param>
        void SetTime(int hour, int minute, int day, int month, int year);
        
        /// <summary>
        /// Advance time by specified amount
        /// </summary>
        /// <param name="seconds">Seconds to advance</param>
        void AdvanceTime(float seconds);
    }
    
    /// <summary>
    /// Types of time changes that can occur
    /// </summary>
    public enum TimeChangeType
    {
        Hour,
        Day,
        Month,
        Year,
        Minute
    }
    
    /// <summary>
    /// Types of time normalization for interpolation
    /// </summary>
    public enum TimeType
    {
        Hour,
        Day,
        Month,
        Year,
        Seasonal
    }
}
