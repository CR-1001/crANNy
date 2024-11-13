/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

/*namespace CRAI.Common.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;
    using System.Threading;

    [Serializable] public class LoggingStorage : LoggingMemory
    {
        public IFileAccess FileAccess { get; set; }

        public LoggingStorage(IFileAccess fileAccess, int autoFlushIntervalSeconds)
        {
            FileAccess = fileAccess;

            if (autoFlushIntervalSeconds > 0)
            {
                var timerFlush = new Timer(delegate { Flush(); }, null, 0, 1000 * autoFlushIntervalSeconds);
            }
        }

        public override int Flush()
        {
            lock (_LockProcess)
            {
                var logEntriesNotFlushed = GetLogEntriesNotFlushed().ToList();

                if (logEntriesNotFlushed.Any())
                {
                    var saveInfo = StoragePath.GetPathLogging();

                    var contentExisting = FileAccess.Read(saveInfo);

                    var lines 
                        = contentExisting == null 
                        ? String.Empty 
                        : Encoding.UTF8.GetString(contentExisting);

                    var stringBuilder = new StringBuilder();

                    stringBuilder.AppendLine(lines);

                    foreach (var loggingEventArgsOccured in logEntriesNotFlushed)
                    {
                        var line = loggingEventArgsOccured.ToStringShort();

                        stringBuilder.AppendLine(line);
                    }

                    var content = Encoding.UTF8.GetBytes(stringBuilder.ToString());

                    FileAccess.Write(saveInfo, content);
                }

                return base.Flush();
            }
        }

    }
}
*/