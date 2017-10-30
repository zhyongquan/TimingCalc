using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimingCalc
{
   internal class Milestone
    {
        private int _no, _interval;

        public int Interval
        {
            get { return _interval; }
            set { _interval = value; }
        }

        public int No
        {
            get { return _no; }
            set { _no = value; }
        }
        private string _description, _baseTime, _workDay, _validTime;

        public string ValidTime
        {
            get { return _time.ToShortDateString(); }
            set { _validTime = value; }
        }

        public string WorkDay
        {
            get { return _workDay; }
            set 
            {
                _isWorkDay = value == "Y" ? true : false;
                _workDay = value; 
            }
        }

        public string BaseTime
        {
            get { return _baseTime; }
            set 
            {
                string str = value.ToString().Trim();
                string[] strArr = str.Split(',');
                _startTime = new int[strArr.Length];
                for (int i = 0; i < strArr.Length; i++)
                {
                    _startTime[i] = int.Parse(strArr[i]);
                }
                _baseTime = value; 
            }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        private int[] _startTime;

        public int[] StartTime
        {
            get { return _startTime; }
            set { _startTime = value; }
        }
        private bool _isWorkDay;

        public bool IsWorkDay
        {
            get { return _isWorkDay; }
            set { _isWorkDay = value; }
        }
        private DateTime _time;

        public DateTime Time
        {
            get { return _time; }
            set { _time = value; }
        }
        public void Calc(DateTime dt) 
        {
            int i = 0;
            while (i < _interval) 
            {
                dt=dt.AddDays(1);
                if (_isWorkDay && (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday))
                {
                    //无效计时
                }
                else 
                {
                    i++;
                }
            }
            _time = dt;
        }
    }
}
