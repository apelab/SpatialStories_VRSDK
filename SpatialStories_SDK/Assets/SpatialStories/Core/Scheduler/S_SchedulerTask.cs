using System;
using UnityEngine;

namespace SpatialStories
{
    public class S_SchedulerTask
    {
        S_SchedulerTaskType taskType;
        public Action ActionToPerform;

        public long taskID;

        public bool IsAlive = true;

        public float TimeOut;

        /// <summary>
        /// If a task gets interrupted it will store how much time had before firing
        /// (Usefull when reescheduling it)
        /// </summary>
        private float remainingTimeOnInterruption = 0;

        public S_SchedulerTask(Action _action, S_SchedulerTaskType _type, float _timeOut)
        {
            taskType = _type;
            ActionToPerform = _action;
            taskID = S_Scheduler.TaskCounter++;
            TimeOut = _timeOut;
        }

        public void Interrupt()
        {
            IsAlive = false;
            remainingTimeOnInterruption = GetRemainingTime();
        }

        public S_SchedulerTask ReSchedule(float _delay = -1)
        {
            if (_delay == -1)
            {
                _delay = remainingTimeOnInterruption;
            }

            if (taskType == S_SchedulerTaskType.NEXT_FRAME)
            {
                return S_Scheduler.AddTaskAtNextFrame(ActionToPerform);
            }
            else
            {
                return S_Scheduler.AddTask(_delay, ActionToPerform);
            }
        }

        private float GetRemainingTime()
        {
            return TimeOut - Time.time;
        }
    }
}
