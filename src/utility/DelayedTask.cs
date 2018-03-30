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
        public async static void DelayTask(List<DelayedTask> actions)
        {
            var Actions = actions;

            while (Actions.Count > 0)
            {
                for (int i = 0; i < Actions.Count; i++)
                {
                    Actions[i].Delay--;

                    if (Actions[i].Delay == 0)
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

        public async static void DelayTask(DelayedTask action)
        {
            while (action.Delay > 0)
            {
                action.Delay--;
                await Task.Delay(1000);
            }

            action.Action();
        }
    }
}