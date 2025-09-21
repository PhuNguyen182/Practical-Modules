using UnityEngine;

namespace GameVisualUpdateByTimeSystem.Core.Interfaces
{
    /// <summary>
    /// Interface for components that update visuals based on time
    /// </summary>
    public interface ITimeBasedVisual
    {
        /// <summary>
        /// Unique identifier for this visual component
        /// </summary>
        string VisualId { get; }
        
        /// <summary>
        /// Priority for update order (lower values update first)
        /// </summary>
        int UpdatePriority { get; }
        
        /// <summary>
        /// Whether this visual component is currently active
        /// </summary>
        bool IsActive { get; set; }
        
        /// <summary>
        /// Update the visual based on current time
        /// </summary>
        /// <param name="timeProvider">Current time provider</param>
        /// <param name="deltaTime">Time since last update</param>
        void UpdateVisual(ITimeProvider timeProvider, float deltaTime);
        
        /// <summary>
        /// Initialize the visual component
        /// </summary>
        void Initialize();
        
        /// <summary>
        /// Cleanup resources when component is destroyed
        /// </summary>
        void Cleanup();
    }
    
    /// <summary>
    /// Interface for visual components that support interpolation between states
    /// </summary>
    /// <typeparam name="T">Type of visual state</typeparam>
    public interface IInterpolatableVisual<T> : ITimeBasedVisual where T : IVisualState
    {
        /// <summary>
        /// Interpolate between two visual states
        /// </summary>
        /// <param name="from">Starting state</param>
        /// <param name="to">Target state</param>
        /// <param name="t">Interpolation factor (0-1)</param>
        void Interpolate(T from, T to, float t);
        
        /// <summary>
        /// Get current visual state
        /// </summary>
        /// <returns>Current visual state</returns>
        T GetCurrentState();
        
        /// <summary>
        /// Set visual state directly
        /// </summary>
        /// <param name="state">State to set</param>
        void SetState(T state);
    }
    
    /// <summary>
    /// Base interface for visual states
    /// </summary>
    public interface IVisualState
    {
        /// <summary>
        /// Unique identifier for this state
        /// </summary>
        string StateId { get; }
        
        /// <summary>
        /// Time when this state should be active
        /// </summary>
        TimeCondition TimeCondition { get; }
    }
    
    /// <summary>
    /// Conditions for when a visual state should be active
    /// </summary>
    [System.Serializable]
    public struct TimeCondition
    {
        public int Hour;
        public int Day;
        public int Month;
        public int Year;
        public TimeType TimeType;
        public bool UseSeason;
        public Season Season;
        
        public TimeCondition(int hour, int day, int month, int year, TimeType timeType = TimeType.Hour, bool useSeason = false, Season season = Season.Spring)
        {
            Hour = hour;
            Day = day;
            Month = month;
            Year = year;
            TimeType = timeType;
            UseSeason = useSeason;
            Season = season;
        }
    }
    
    /// <summary>
    /// Seasons for seasonal visual updates
    /// </summary>
    public enum Season
    {
        Spring,
        Summer,
        Autumn,
        Winter
    }
}
