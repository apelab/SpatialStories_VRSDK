using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialStories
{
    public class S_Scheduler : MonoBehaviour
    {
        public static long TaskCounter = 0;

        //TODO(apelab): Make sure that this list is clean when an scene is loaded
        private static List<S_SchedulerTask> actionsToPerform = new List<S_SchedulerTask>();
        private static int tasksCount = 0;
        private static List<S_SchedulerTask> actionsToPerformAtNextFrame = new List<S_SchedulerTask>();

        public static S_SchedulerTask AddTask(float _delay, Action _actionToPerform)
        {
            float taskTimeOut = Time.time + _delay;
            S_SchedulerTask task = new S_SchedulerTask(_actionToPerform, S_SchedulerTaskType.NORMAL, taskTimeOut);
            actionsToPerform.Add(task);
            ++tasksCount;
            return task;
        }

        public static S_SchedulerTask AddTaskAtNextFrame(Action _actionToPerform)
        {
            S_SchedulerTask task = new S_SchedulerTask(_actionToPerform, S_SchedulerTaskType.NEXT_FRAME, Time.time);
            actionsToPerformAtNextFrame.Add(task);
            return task;
        }

        public static S_SchedulerTask AddUniqueTask(float _delay, Action _actionToPerform)
        {
            if (CheckIfActionIsUniqueInList(_actionToPerform, actionsToPerform))
            {
                return AddTask(_delay, _actionToPerform);
            }
            return null;
        }

        public static S_SchedulerTask AddUniqueTaskAtNextFrame(Action _actionToPerform)
        {
            if (CheckIfActionIsUniqueInList(_actionToPerform, actionsToPerformAtNextFrame))
            {
                return AddTaskAtNextFrame(_actionToPerform);
            }
            return null;
        }

        public static bool CheckIfActionIsUniqueInList(Action _action, List<S_SchedulerTask> _list)
        {
            int counter = _list.Count;
            for (int i = 0; i < counter; i++)
            {
                Action act = _list[i].ActionToPerform;
                if (act.Target == _action.Target && _action.Method == _action.Method)
                {
                    return false;
                }
            }
            return true;
        }

        private void Update()
        {
            // Execute all the tasks scheduled for this frame
            ExecuteNextFrameRequests();

            // start from the end of the list to be able to delete elements (without null pointer exceptions)
            for (int i = tasksCount - 1; i >= 0; i--)
            {
                // Get the task
                S_SchedulerTask task = actionsToPerform[i];

                if (!task.IsAlive)
                {
                    actionsToPerform.RemoveAt(i);
                    --tasksCount;
                }
                // if the delay for this task is finished, it's time to execute it !
                else if (Time.time >= task.TimeOut)
                {
                    // Excecute the acttion
                    task.ActionToPerform();

                    // Set the task as dead
                    task.IsAlive = false;

                    // remove the last action in the list (the one we executed above)
                    actionsToPerform.RemoveAt(i);

                    // update the remaining number of tasks to execute
                    --tasksCount;
                }
            }
        }

        public void ExecuteNextFrameRequests()
        {
            int counter = actionsToPerformAtNextFrame.Count;
            S_SchedulerTask task;
            for (int i = counter - 1; i >= 0; i--)
            {
                task = actionsToPerformAtNextFrame[i];
                if (task.IsAlive)
                {
                    task.ActionToPerform.Invoke();
                    task.IsAlive = false;
                }
                actionsToPerformAtNextFrame.RemoveAt(i);

            }
        }

    }
}
