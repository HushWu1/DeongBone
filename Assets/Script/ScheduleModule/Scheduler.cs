using System;


    public class Scheduler : IComparable<Scheduler>
    {
        public delegate void OnTickEventHandler(bool isFirst);
        public delegate void OnReleasedEventHandler();

        int IComparable<Scheduler>.CompareTo(Scheduler other)
        {
            //到期时间近的在前面
            return NextTime.CompareTo(other.NextTime);
        }

        public event OnTickEventHandler OnTick;
        public event OnReleasedEventHandler OnRelease;

        /// <summary>
        /// 最大触发次数
        /// </summary>
        public int MaxTimes { get; private set; }
        /// <summary>
        /// 触发间隔
        /// </summary>
        public float Interval { get; private set; }
        /// <summary>
        /// 下次触发时间
        /// </summary>
        public float NextTime { get; private set; }
        /// <summary>
        /// 触发次数
        /// </summary>
        public int Times { get; private set; }
        /// <summary>
        /// 是否结束
        /// </summary>
        public bool Finished { get; internal set; }
        /// <summary>
        /// 回调全名
        /// </summary>
        public string OnTickMethodFullName
        {
            get
            {
                return OnTick == null ? string.Empty : $"{OnTick.Method.DeclaringType.FullName}.{OnTick.Method.Name}";
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="maxTimes"></param>
        /// <param name="interval"></param>
        /// <param name="nextTime"></param>
        internal void Init(int maxTimes, float interval, float nextTime)
        {
            MaxTimes = maxTimes;
            Interval = interval;
            NextTime = nextTime;
        }

        /// <summary>
        /// 到达触发时间
        /// </summary>
        /// <returns></returns>
        internal bool OnAlarm()
        {
            if (Finished)
                return true;

            NextTime += Interval;
            bool finish = ++Times >= MaxTimes && MaxTimes != -1;
            OnTick?.Invoke(Times == 1);
            return finish;
        }

        public void OnStop()
        {
            Finished = true;
            MaxTimes = 0;
            Interval = 0;
            NextTime = 0;
            Times = 0;
            OnTick = null;
            OnRelease?.Invoke();
            OnRelease = null;
        }
    }
