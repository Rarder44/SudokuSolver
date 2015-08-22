using System;

namespace SudokuSolver.Extensions
{
    public class TimeSpanPlus
    {
        private TimeSpan _time;
        public TimeSpan Time
        {
            get
            {
                return _time;
            }
            set
            {
                _time = value;
            }
        }
        public TimeSpanPlus(long ticks)
        {
            _time = new TimeSpan(ticks);
        }
        public TimeSpanPlus(int milliseconds = 0, int seconds = 0, int minutes = 0, int hours = 0, int days = 0)
        {
            _time = new TimeSpan(days, hours, minutes, seconds, milliseconds);
        }


        public void AddSeconds( int seconds)
        {
            _time = _time.Add(new TimeSpan(0, 0, seconds));
        }
        public void AddMinuts( int minutes)
        {
            _time = _time.Add(new TimeSpan(0, minutes, 0));
        }
        public void AddHours(int hours)
        {
            _time = _time.Add(new TimeSpan(hours, 0, 0));
        }
        public void AddTime(int sec,int min, int hou)
        {
            _time = _time.Add(new TimeSpan(hou, min, sec));
        }
        override public string ToString()
        {
            return _time.ToString();
        }
        public string ToString(string format)
        {
            return _time.ToString(format);
        }

        public void SetTime(int milliseconds = 0, int seconds = 0, int minutes = 0, int hours = 0, int days = 0)
        {
            _time = new TimeSpan(days, hours, minutes, seconds, milliseconds);
        }
    }
}
