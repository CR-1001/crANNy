/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;    

    public delegate void LoggingEventHandler(Object sender, LoggingEventArgs loggingEventArgs);


    public interface ILogging
    {
        event LoggingEventHandler Occured;

        void Log(Exception exception, String format, params Object[] arguments);

        void Log(Level loggingLevel, String format, params Object[] arguments);

        void Log(String format, params Object[] arguments);

        int Flush();

        IEnumerable<LoggingEventArgs> GetLogEntriesNotFlushed();
    }
}
