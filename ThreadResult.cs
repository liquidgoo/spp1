using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Tracer
{
    public class ThreadResult
    {
        private int id;
        private TimeSpan _time = new TimeSpan(0);
        private List<MethodResult> methodsResults = new List<MethodResult>();

        private (MethodResult, int) FindMethodInStack(List<(string,string)> methodsClasses, int minDepth = 1)
        {
            for (int i = methodsClasses.Count - 1; i >= minDepth; i--)
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
        public void InsertMethodResult(MethodResult methodResult, List<(string, string)> methodsClasses)
        {
            (MethodResult, int) callingMethod = FindMethodInStack(methodsClasses);
            if (callingMethod.Item1 != null)
                callingMethod.Item1.InsertMethodResult(methodResult, methodsClasses, callingMethod.Item2 - 1);
            else
                methodsResults.Add(methodResult);
        }
        public void StopTrace(DateTime stopTime, List<(string, string)> methodsClasses)
        {
            (MethodResult, int) callingMethod = FindMethodInStack(methodsClasses, 0);

            _time += callingMethod.Item1.StopTrace(stopTime, methodsClasses, callingMethod.Item2 - 1);
        }
    }
}
