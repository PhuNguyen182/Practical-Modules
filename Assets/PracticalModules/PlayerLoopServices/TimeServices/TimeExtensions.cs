using System;

namespace PracticalModules.PlayerLoopServices.TimeServices
{
    public static class TimeExtensions
    {
        public const int TotalSecondsInDay = 86400;
        public static DateTime CurrentTime => DateTime.Now;
        
        public static DateTime CurrentUtcTime => DateTime.UtcNow;
        
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
        
        public static long DatetimeToBinary(DateTime dateTime) => dateTime.ToBinary();

        public static DateTime BinaryToDatetime(long binary) => DateTime.FromBinary(binary);
        
        public static long GetCurrentTimestamp() => DateTimeToUnix(DateTime.Now);

        public static void ToShortReadableString(TimeSpan timeSpan, out string timeDisplayerOutput)
        {
            if (timeSpan.TotalDays < 1)
                timeDisplayerOutput = $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
            
            else if (timeSpan.TotalDays < 30)
            {
                int days = timeSpan.Days;
                int hours = timeSpan.Hours;
                timeDisplayerOutput = $"{days}d{(hours > 0 ? $"{hours}h" : "")}";
            }
            
            else if (timeSpan.TotalDays < 365)
            {
                int months = timeSpan.Days / 30;
                int days = timeSpan.Days % 30;
                timeDisplayerOutput = $"{months}m{(days > 0 ? $"{days}d" : "")}";
            }
            
            else
            {
                int years = timeSpan.Days / 365;
                int monthsLeft = (timeSpan.Days % 365) / 30;
                int daysLeft = (timeSpan.Days % 365) % 30;

                timeDisplayerOutput = $"{years}y";
                if (monthsLeft > 0)
                    timeDisplayerOutput = $"{years}y{monthsLeft}m";
                if (daysLeft > 0)
                    timeDisplayerOutput = $"{years}y{monthsLeft}m{daysLeft}d";
            }
        }

        public static long DateTimeToUnix(DateTime dateTime) => new DateTimeOffset(dateTime).ToUnixTimeSeconds();

        public static long DateTimeToUnixMilliseconds(DateTime dateTime) =>
            new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
        
        public static DateTime UnixToDateTime(long timestamp)
        {
            DateTime epoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(timestamp).ToLocalTime();
        }

        public static DateTime UnixMillisecondsToDateTime(long timestamp)
        {
            DateTime epoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(timestamp).ToLocalTime();
        }

        public static bool IsNewDay(DateTime dateTime)
        {
            if (dateTime.Year > DateTime.Now.Year)
                return true;

            if (dateTime.Month > DateTime.Now.Month)
                return true;

            return dateTime.Day > DateTime.Now.Day;
        }
    }
}
