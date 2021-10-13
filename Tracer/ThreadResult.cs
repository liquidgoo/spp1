using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Tracer
{
    public class ThreadResult
    {
        private int id;
        private TimeSpan time = new TimeSpan(0);
        private ImmutableList<MethodResult> methodsResults = ImmutableList<MethodResult>.Empty;
        
        private (MethodResult, int) FindCallingMethod(List<(string,string)> methodsClasses)
        {
            for (int i = methodsClasses.Count - 1; i >= 0; i--)
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
            (MethodResult, int) callingMethod = FindCallingMethod(methodsClasses);
            if (callingMethod.Item1 != null)
                callingMethod.Item1.InsertMethodResult(methodResult, methodsClasses, callingMethod.Item2 + 1);
            else
                methodsResults.Add(methodResult);
        }
        public void StopTrace(DateTime stopTime, List<(string, string)> methodsClasses)
        {
            (MethodResult, int) callingMethod = FindCallingMethod(methodsClasses);

            time += callingMethod.Item1.StopTrace(stopTime, methodsClasses, callingMethod.Item2);
        }
    }
}
