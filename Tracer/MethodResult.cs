using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Tracer
{
    public class MethodResult
    {
        private DateTime _startTime;
        private DateTime _stopTime;
        private TimeSpan _time;
        public readonly string methodName;
        public readonly string className;
        private ImmutableList<MethodResult> methodsResults = ImmutableList<MethodResult>.Empty;
        private (MethodResult, int) FindCallingMethod(List<(string, string)> methodsClasses, int lastStackDepth)
        {
            for (int i = lastStackDepth; i >= 0; i--)
            {
                foreach (MethodResult method in methodsResults)
                {
                    if (methodsClasses[i].Item1.Equals(method.className) &&
                        methodsClasses[i].Item2.Equals(method.methodName))
                    {
                        return (method, i);
                    }
                }
            }
            return (null, -1);
        }
        public void InsertMethodResult(MethodResult methodResult, List<(string, string)> methodsClasses, int lastStackDepth)
        {
            (MethodResult, int) callingMethod = FindCallingMethod(methodsClasses, lastStackDepth);
            if (callingMethod.Item1 != null)
                callingMethod.Item1.InsertMethodResult(methodResult, methodsClasses, callingMethod.Item2 + 1);
            else
                methodsResults.Add(methodResult);
        }
        public TimeSpan StopTrace(DateTime stopTime, List<(string, string)> methodsClasses, int lastStackDepth)
        {
            (MethodResult, int) callingMethod = FindCallingMethod(methodsClasses, lastStackDepth);
            if (callingMethod.Item1 != null)
                return callingMethod.Item1.StopTrace(stopTime, methodsClasses, callingMethod.Item2);
            else
            {
                _stopTime = stopTime;
                _time = _stopTime - _startTime;
                return _time;
            }
        }
        public MethodResult(DateTime startTime, string className, string methodName)
        {
            _startTime = startTime;
            this.methodName = methodName;
            this.className = className;
        }
    }
}
