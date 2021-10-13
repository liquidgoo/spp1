using System;
using System.Collections.Generic;

namespace Tracer
{
    [Serializable]
    public class ThreadResult
    {
        public int Id { get; set; }
        public double Time { get; set; }
        public List<MethodResult> MethodsResults { get; private set; }

        private (MethodResult, int) FindMethodInStack(List<(string,string)> methodsClasses, int minDepth = 1)
        {
            for (int i = methodsClasses.Count - 1; i >= minDepth; i--)
            {
                foreach (MethodResult method in MethodsResults)
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
        public void InsertMethodResult(MethodResult methodResult, List<(string, string)> methodsClasses)
        {
            (MethodResult, int) callingMethod = FindMethodInStack(methodsClasses);
            if (callingMethod.Item1 != null)
                callingMethod.Item1.InsertMethodResult(methodResult, methodsClasses, callingMethod.Item2 - 1);
            else
                MethodsResults.Add(methodResult);
        }
        public void StopTrace(DateTime stopTime, List<(string, string)> methodsClasses)
        {
            (MethodResult, int) callingMethod = FindMethodInStack(methodsClasses, 0);

            Time += callingMethod.Item1.StopTrace(stopTime, methodsClasses, callingMethod.Item2 - 1);
        }

        public ThreadResult(int threadId)
        {
            Id = threadId;
            MethodsResults = new List<MethodResult>();
        }
        public ThreadResult() { }
    }
}
