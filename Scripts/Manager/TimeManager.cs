using Core;
using DataSystem;
using Event;
using UnityEngine;

namespace Manager
{
    public static class TimeManager
    {
        private static int _day;
        private static int _hour;
        private static int _minute;

        private static float _elapsedTime;
        private static bool _isPlaying;

        public static string GetDayString() => "DAY " + _day;
        public static string GetTimeString() => _hour.ToString("00") + ":" + _minute.ToString("00");

        public static int TotalMinutes() => _hour * Constants.Time.MinutesInAHour + _minute;
        
        public static void Init()
        {
            _day = 0;
            _hour = 0;
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

            _elapsedTime += Time.deltaTime;

            if (_elapsedTime < Constants.Time.SecondsPerMinute)
            {
                return;
            }
            _minute++;
            _elapsedTime -= Constants.Time.SecondsPerMinute;
            EventManager.OnNext(Message.OnEveryMinute);
            
            if (_minute < Constants.Time.MinutesInAHour)
            {
                return;
            }
            _hour++;
            _minute -= Constants.Time.MinutesInAHour;
            EventManager.OnNext(Message.OnEveryHour);

            if (_hour < Constants.Time.HoursInADay)
            {
                return;
            }
            
            _day++;
            _hour = 0;
        }

        public static void OnMoveHome()
        {
            _elapsedTime += Constants.FastPathPenaltyHour * Constants.Time.SecondsPerHour;
        }
    }
}