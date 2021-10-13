using System;
using System.Collections.Generic;

namespace Tracer
{
    [Serializable]
    public class MethodResult
    {
        public string MethodName { get; set; }
        public string ClassName { get; set; }
        private DateTime _startTime;
        private DateTime _stopTime;
        public double Time { get; set; }

        public List<MethodResult> methodsResults { get; }
        private (MethodResult, int) FindCallingMethod(List<(string, string)> methodsClasses, int lastStackDepth, int minDepth = 1)
        {
            for (int i = lastStackDepth; i >= minDepth; i--)
            {
                foreach (MethodResult method in methodsResults)
                {
                    if (methodsClasses[i].Item1.Equals(method.ClassName) &&
                        methodsClasses[i].Item2.Equals(method.MethodName))
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
                callingMethod.Item1.InsertMethodResult(methodResult, methodsClasses, callingMethod.Item2 - 1);
            else
                methodsResults.Add(methodResult);
        }
        public double StopTrace(DateTime stopTime, List<(string, string)> methodsClasses, int lastStackDepth)
        {
            (MethodResult, int) callingMethod = FindCallingMethod(methodsClasses, lastStackDepth, 0);
            if (callingMethod.Item1 != null)
                return callingMethod.Item1.StopTrace(stopTime, methodsClasses, callingMethod.Item2);
            else
            {
                _stopTime = stopTime;
                Time = (_stopTime - _startTime).TotalMilliseconds;
                return Time;
            }
        }
        public MethodResult(DateTime startTime, string className, string methodName)
        {
            _startTime = startTime;
            this.MethodName = methodName;
            this.ClassName = className;
            methodsResults = new List<MethodResult>();
        }
        public MethodResult() { }
    }
}
