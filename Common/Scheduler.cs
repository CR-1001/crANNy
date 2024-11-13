/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.Common
{
    using System;
    using System.Threading;

    public static class Scheduler
    {
        [Serializable] public class TimerAction
        {
            public DateTime QueuedAt { get; set; }
            public Action Action { get; set; }
            public TimeSpan Delay { get; set; }
            public String Name { get; set; }
            public bool IsExecuted { get; set; }
            public bool IsAlertInPast { get; set; }
            public Timer Timer { get; set; }

            public DateTime ExecuteAt
            {
                get { return QueuedAt + Delay; }
            }

            public TimeSpan Remaining
            {
                get { return IsExecuted ? new TimeSpan(0) : ExecuteAt - DateTime.Now; }
            }

            public override String ToString()
            {
                if (!String.IsNullOrWhiteSpace(Name)) return Name;

                return
                    String.Format(
                    "ExecuteAt={0};QueuedAt={1};Delay={2};IsExecuted={3};IsAlertInPast={4};Name={5};",
                    ExecuteAt, QueuedAt, Delay, IsExecuted, IsAlertInPast, Name);
            }
        }

        public static TimerAction RunAtTimeOfDay(this Action action, TimeSpan timeSpanTimeOfDayAlert, bool runImmidealtyIfToLate, String name = null)
        {
            var dateTimeNow = DateTime.Now;
            var timeSpanToWait = timeSpanTimeOfDayAlert - dateTimeNow.TimeOfDay;

            if (timeSpanToWait < TimeSpan.Zero && !runImmidealtyIfToLate)
            {
                return new TimerAction()
                {
                    Action = action,
                    IsAlertInPast = true,
                    Name = name,
                    QueuedAt = dateTimeNow,
                };
            }

            var timerAction = RunDelayed(action, timeSpanToWait, name);

            timerAction.QueuedAt = dateTimeNow;

            return timerAction;
        }

        public static TimerAction RunDelayed(this Action action, TimeSpan timeSpanToWait, String name = null)
        {
            var dateTimeNow = DateTime.Now;

            var timerAction = new TimerAction()
            {
                Action = action,
                Delay = timeSpanToWait,
                IsExecuted = false,
                Name = name,
                QueuedAt = dateTimeNow
            };

            if (timeSpanToWait < TimeSpan.Zero)
            {
                timeSpanToWait = new TimeSpan(0);
                timerAction.IsAlertInPast = true;
            }

            var timer = new Timer(_ =>
            {
                action();
                timerAction.IsExecuted = true;
            }, null, timeSpanToWait, new TimeSpan(0, 0, 0, 0, Timeout.Infinite));

            timerAction.Timer = timer;

            return timerAction;
        }
    }
}
