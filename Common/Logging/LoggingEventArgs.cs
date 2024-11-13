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

    public enum Level
    {
        Error,
        Hint,
    }

    public sealed class LoggingEventArgs
    {
        public DateTime LoggedAt { get; private set; }
        public Level Level { get; private set; }
        public String Message { get; private set; }
        public Exception Exception { get; private set; }

        public LoggingEventArgs(Exception exception, String format, Object[] arguments)
            : this(DateTime.Now, exception, format, arguments)
        {
        }

        public LoggingEventArgs(DateTime loggedAt, Exception exception, String format, Object[] arguments)
        {
            LoggedAt = loggedAt;
            Level = Level.Error;
            Message = String.Format(format, arguments);
            Exception = exception;
        }

        public LoggingEventArgs(Level level, String format, Object[] arguments)
            : this(DateTime.Now, level, format, arguments)
        {
        }

        public LoggingEventArgs(DateTime loggedAt, Level level, String format, Object[] arguments)
            : this(loggedAt, level, String.Format(format, arguments))
        {
        }

        public LoggingEventArgs(DateTime loggedAt, Level level, String message)
        {
            LoggedAt = loggedAt;
            Level = level;
            Message = message;
        }

        public String ToStringShort()
        {
            var formatted
                = String.Format(
                    "Level={1}{0}LoggedAt={2}{0}Message={3}{0}Exception={4}{0}{5}",
                    Environment.NewLine,
                    Level,
                    LoggedAt,
                    Message,
                    Exception,
                    new String('-', 20));

            return formatted;
        }
    }
}
