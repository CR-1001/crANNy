/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.Common.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [Serializable] public class LoggingMemory : ILogging
    {
        protected Object _LockProcess = new Object();

        protected List<LoggingEventArgs> _LoggingEventArgsOccured = new List<LoggingEventArgs>();

        public event LoggingEventHandler Occured = delegate { };

        public void Log(Exception exception, String format, Object[] arguments)
        {
            var loggingEventArgs = new LoggingEventArgs(exception, format, arguments);

            QueueLog(loggingEventArgs);
        }

        public void Log(String format, object[] arguments)
        {
            Log(Level.Hint, format, arguments);
        }

        public void Log(Level loggingLevel, String format, object[] arguments)
        {
            LoggingEventArgs loggingEventArgs;

            if (!arguments.Any())
            {
                loggingEventArgs = new LoggingEventArgs(DateTime.Now, loggingLevel, format);
            }
            else
            {
                try
                {
                    loggingEventArgs = new LoggingEventArgs(loggingLevel, format, arguments);
                }
                catch
                {
                    var message 
                        = format + " " 
                        + arguments
                        .Select(a => String.Empty + a)
                        .Aggregate((a1, a2) => String.Format("{0} {1}", a1, a2));

                    loggingEventArgs = new LoggingEventArgs(DateTime.Now, loggingLevel, message);
                }
            }

            QueueLog(loggingEventArgs);
        }

        private void QueueLog(LoggingEventArgs loggingEventArgs)
        {
            Occured(this, loggingEventArgs);

            lock (_LockProcess)
            {
                _LoggingEventArgsOccured.Add(loggingEventArgs);
            }
        }

        public virtual int Flush()
        {
            lock (_LockProcess)
            {
                var count = _LoggingEventArgsOccured.Count;

                _LoggingEventArgsOccured.Clear();

                return count;
            }
        }

        public IEnumerable<LoggingEventArgs> GetLogEntriesNotFlushed()
        {
            List<LoggingEventArgs> loggingEventArgsOccured;

            lock (_LockProcess)
            {
                loggingEventArgsOccured = _LoggingEventArgsOccured.ToList();
            }

            return _LoggingEventArgsOccured;
        }
    }
}
