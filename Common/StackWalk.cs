/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.Common
{
    using System;
    using System.Linq;
    using System.Diagnostics;

    public static class StackWalk
    {
        [DebuggerStepThrough]
        public static bool IsCalledFromMethod(params String[] methods)
        {
            return IsCalledFromMethod(0, methods);
        }

        [DebuggerStepThrough]
        public static bool IsCalledFromMethod(int methodsInCallstackToSkip, params String[] methods)
        {
            var methodNames
                = (new StackTrace())
                .GetFrames()
                .Skip(methodsInCallstackToSkip)
                .Select(f => f.GetMethod().Name)
                .ToList();

            var isMatch = methods.Any(m => m.IsIn(methodNames));

            return isMatch;
        }

        public static Tuple<String, String> GetCallingMethod(int numberInStackTrace)
        {
            var method
                = (new StackTrace())
                .GetFrames()
                .ElementAt(numberInStackTrace)
                .GetMethod();

            return Tuple.Create(method.DeclaringType.Name, method.ToString());
        }

    }
}
