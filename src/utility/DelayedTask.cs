using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace mathbattle.utility
{

    public class DelayedTask
    {
        public Action Action;
        public int Delay;

        public DelayedTask(Action action, int delay)
        {
            Action = action;
            Delay = delay;
        }
        public static TaskDelayer DelayTask(List<DelayedTask> actions)
        {
            return new TaskDelayer(actions);
        }
    }

    public class TaskDelayer {
        List<DelayedTask> Actions;
        bool Stopped;
        public int CurrentTimer {get { return _currentTimer;}}
        int _currentTimer;
        public TaskDelayer(List<DelayedTask> actions) {
            Actions = actions;
            Loop();
        }

        async void Loop() {
            _currentTimer += 1;
            while (Actions.Count > 0 && !Stopped)
            {
                for (int i = 0; i < Actions.Count; i++)
                {
                    Actions[i].Delay--;

                    if (Actions[i].Delay <= 0)
                    {
                        Actions[i].Action();
                        Actions[i] = null;
                    }
                }

                while (Actions.Contains(null))
                {
                    Actions.Remove(null);
                }

                await Task.Delay(1000);
            }
        }

        public void Stop() {
            Stopped = true;
        }
    }
}