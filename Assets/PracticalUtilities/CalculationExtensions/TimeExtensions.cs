using System;

namespace PracticalUtilities.CalculationExtensions
{
    public static class TimeExtensions
    {
        #region Utility Values
        
        private const int TotalSecondsInDay = 86400;
        private static readonly DateTime LocalEpoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
        private static readonly DateTime UtcEpoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime CurrentTime => DateTime.Now;
        public static DateTime CurrentUtcTime => DateTime.UtcNow;

        #endregion

        #region Time In-Day Calculations

        public static double GetPassedSecondsInCurrentDay()
        {
            double passedSeconds = CurrentTime.Second + CurrentTime.Minute * 60 + CurrentTime.Hour * 3600;
            return passedSeconds;
        }

        public static double GetRemainingSecondsInDay()
        {
            double passedSeconds = GetPassedSecondsInCurrentDay();
            return TotalSecondsInDay - passedSeconds;
        }

        #endregion

        #region Current Time Calculations

        public static long GetCurrentLocalTimestampInSeconds() => DateTimeToUnixSeconds(CurrentTime);

        public static long GetCurrentUtcTimestampInSeconds() => DateTimeToUnixSeconds(DateTime.UtcNow);

        public static long GetCurrentLocalTimestampInMilliseconds() => DateTimeToUnixMilliseconds(DateTime.Now);

        public static long GetCurrentUtcTimestampInMilliseconds() => DateTimeToUnixMilliseconds(CurrentUtcTime);

        #endregion

        #region Time Converters

        public static long DatetimeToBinary(DateTime dateTime) => dateTime.ToBinary();

        public static DateTime BinaryToDatetime(long binary) => DateTime.FromBinary(binary);

        public static long DateTimeToUnixSeconds(DateTime dateTime) =>
            new DateTimeOffset(dateTime).ToUnixTimeSeconds();

        public static long DateTimeToUnixMilliseconds(DateTime dateTime) =>
            new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();

        public static DateTime UnixToLocalDateTime(long timestamp)
            => LocalEpoch.AddSeconds(timestamp).ToLocalTime();

        public static DateTime UnixMillisecondsToLocalDateTime(long timestamp)
            => LocalEpoch.AddMilliseconds(timestamp).ToLocalTime();

        public static DateTime UnixToUtcDateTime(long timestamp)
            => UtcEpoch.AddSeconds(timestamp).ToUniversalTime();

        public static DateTime UnixMillisecondsToUtcDateTime(long timestamp)
            => UtcEpoch.AddMilliseconds(timestamp).ToUniversalTime();

        #endregion

        #region Time Offset Checking

        public static bool IsNewDay(DateTime targetTime, bool useUtc = false)
        {
            DateTime currentTime = useUtc ? CurrentUtcTime : CurrentTime;
            int diffDayCount = GetDiffDayCount(currentTime, targetTime);
            return diffDayCount > 0;
        }

        public static int GetDiffDayCount(DateTime start, DateTime end)
        {
            TimeSpan offset = end.Subtract(start);
            return offset.Days;
        }

        public static TimeSpan GetTimeOffset(DateTime start, DateTime end)
        {
            TimeSpan offset = end.Subtract(start);
            return offset;
        }

        #endregion

        #region Time Formatting
        
        public static string ToShortReadableString(TimeSpan timeSpan, bool withCharacters = false)
        {
            if (timeSpan.TotalDays < 1)
                return ToHhMmSs(timeSpan, withCharacters);

            if (timeSpan.TotalDays < 30)
                return ToDaysAndHours(timeSpan, withCharacters);

            if (timeSpan.TotalDays < 365)
                return ToMonthAndDays(timeSpan, withCharacters);

            return ToYearAndMonths(timeSpan, withCharacters);
        }
        
        public static string ToHhMm(TimeSpan timeSpan, bool withCharacters = false)
            => withCharacters
                ? $"{timeSpan.Hours:D2}h:{timeSpan.Minutes:D2}m"
                : $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}";

        public static string ToHhMmSs(TimeSpan timeSpan, bool withCharacters = false)
            => withCharacters
                ? $"{timeSpan.Hours:D2}h:{timeSpan.Minutes:D2}m:{timeSpan.Seconds:D2}s"
                : $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";

        public static string ToDaysAndHours(TimeSpan timeSpan, bool withCharacters = false)
        {
            int days = timeSpan.Days;
            int hours = timeSpan.Hours;
            return hours > 0 ? $"{days}d{hours}h" : ToHhMm(timeSpan, withCharacters);
        }

        public static string ToMonthAndDays(TimeSpan timeSpan, bool withCharacters = false)
        {
            int months = timeSpan.Days / 30;
            int days = timeSpan.Days % 30;
            return days > 0 ? $"{months}m{days}d" : ToDaysAndHours(timeSpan, withCharacters);
        }
        
        public static string ToYearAndMonths(TimeSpan timeSpan, bool withCharacters = false)
        {
            int years = timeSpan.Days / 365;
            int monthsLeft = (timeSpan.Days % 365) / 30;
            int daysLeft = (timeSpan.Days % 365) % 30;

            if (daysLeft > 0)
                return $"{years}y{monthsLeft}m{daysLeft}d";
            
            if (monthsLeft > 0)
                return $"{years}y{monthsLeft}m";
            
            return $"{years}y";
        }

        #endregion
    }
}
