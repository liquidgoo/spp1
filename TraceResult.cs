using System;
using System.Collections.Generic;
using System.Collections.Immutable;
namespace Tracer
{
    public class TraceResult
    {
        private List<ThreadResult> threadsResults = new List<ThreadResult>();

        public void InsertMethodResult(MethodResult methodResult, List<(string, string)> methodsClasses)
        {
            threadsResults[0].InsertMethodResult(methodResult, methodsClasses);
        }

        public void StopTrace(DateTime stopTime, List<(string, string)> methodsClasses)
        {
            threadsResults[0].StopTrace(stopTime, methodsClasses);
        }
        public TraceResult()
        {
            threadsResults.Add(new ThreadResult());
        }
    }
}
