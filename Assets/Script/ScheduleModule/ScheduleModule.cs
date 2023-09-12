using System.Collections.Generic;
using UnityEngine;


	public partial class ScheduleModule : BaseGameModule
    {
        public List<Scheduler> Schedulers { get; private set; }

        protected internal override void OnModuleInit()
        {
            base.OnModuleInit();
            Schedulers = new List<Scheduler>();
        }

        protected internal override void OnModuleUpdate(float deltaTime)
        {
            base.OnModuleUpdate(deltaTime);
            Scheduler scheduler;
            while (Schedulers.Count > 0 && (scheduler = Schedulers[0]).NextTime <= Time.time)
            {
                //到达时间
                Schedulers.RemoveAt(0);
                bool finished = scheduler.OnAlarm();
                if (finished)
                {
                    scheduler.OnStop();
                    scheduler = null;
                    continue;
                }

                //Schedulers.OrderedInsert(scheduler);
            }
        }

        /// <summary>
        /// 启动一个新的计时器,delay为0
        /// </summary>
        /// <param name="maxTimes"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public Scheduler StartNew(int maxTimes, float interval, Scheduler.OnTickEventHandler tick)
        {
            return StartNew(0, maxTimes, interval, tick);
        }

        /// <summary>
        /// 启动一个新的计时器,delay为0,times为-1
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public Scheduler StartNew(float interval, Scheduler.OnTickEventHandler tick)
        {
            return StartNew(0, -1, interval, tick);
        }

        /// <summary>
        /// 启动一个新的计时器
        /// </summary>
        /// <param name="delay">延迟</param>
        /// <param name="maxTimes">执行次数,-1为无限次</param>
        /// <param name="interval">间隔</param>
        /// <returns></returns>
        public Scheduler StartNew(float delay, int maxTimes, float interval, Scheduler.OnTickEventHandler tick)
        {
            interval = Mathf.Max(0, interval);
            if (maxTimes != 1 && interval == 0)
            {
                Debug.LogError($"You can't set maxTimes as {maxTimes} and interval as {interval}, because this may lead to an endless loop");
                return null;
            }
            Scheduler scheduler = new Scheduler();
            scheduler.Init(maxTimes, interval, Time.time + Mathf.Max(0, delay));
            scheduler.OnTick += tick;
            //Schedulers.OrderedInsert(scheduler);
            return scheduler;
        }

        /// <summary>
        /// 停止计时器
        /// </summary>
        /// <param name="scheduler"></param>
        public void Stop(Scheduler scheduler)
        {
            if (scheduler == null)
                return;

            scheduler.Finished = true;
        }
    }
