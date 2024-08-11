using DataSystem;
using EventSystem;

namespace EnvironmentSystem.Time
{
    public static class TimeManager
    {
        private static DateTime _dateTime;

        private static float _elapsedTime;
        private static bool _isPlaying;

        public static string GetDayString() => "DAY " + _dateTime.Day;
        public static string GetTimeString() => $"{_dateTime.Hour:00}:{_dateTime.Minute:00}";
        public static string GetDataString() => $"{_dateTime.Day}:{_dateTime.Hour}:{_dateTime.Minute}:{_elapsedTime}";

        public static int TotalMinutes() => _dateTime.Hour * Constants.Time.MinutesInAHour + _dateTime.Minute;

        public static void Init()
        {
            _dateTime = new DateTime(0, Constants.Time.StartHour, 0);
            _elapsedTime = 0;
            _isPlaying = true;
        }

        public static void Play()
        {
            _isPlaying = true;
        }

        public static void Pause()
        {
            _isPlaying = false;
        }

        public static void OnUpdate()
        {
            if (!_isPlaying)
            {
                return;
            }

            _elapsedTime += UnityEngine.Time.deltaTime;

            if (_elapsedTime < Constants.Time.SecondsPerMinute)
            {
                return;
            }

            _dateTime.Minute++;
            _elapsedTime -= Constants.Time.SecondsPerMinute;
            EventManager.OnNext(Message.OnEveryMinute);
            if (_dateTime.Minute < Constants.Time.MinutesInAHour)
            {
                return;
            }

            _dateTime.Hour++;
            _dateTime.Minute -= Constants.Time.MinutesInAHour;
            EventManager.OnNext(Message.OnEveryHour);

            if (_dateTime.Hour < Constants.Time.HoursInADay)
            {
                return;
            }
            _dateTime.Day++;
            _dateTime.Hour = 0;
        }

        public static void OnMoveHome()
        {
            _elapsedTime += Constants.FastPathPenaltyHour * Constants.Time.SecondsPerHour;
        }

        public static void SetTime(string[] timeData)
        {
            int.TryParse(timeData[0], out _dateTime.Day);
            int.TryParse(timeData[1], out _dateTime.Hour);
            int.TryParse(timeData[2], out _dateTime.Minute);
            float.TryParse(timeData[3], out _elapsedTime);
        }
    }
}