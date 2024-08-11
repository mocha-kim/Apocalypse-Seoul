namespace EnvironmentSystem.Time
{
    public struct DateTime
    {
        public int Day;
        public int Hour;
        public int Minute;

        public DateTime(int day, int hour, int minute)
        {
            Day = day;
            Hour = hour;
            Minute = minute;
        }
    }
}