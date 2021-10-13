using System;
using System.Collections.Generic;
using System.Collections.Immutable;
namespace Tracer
{
    public class TraceResult
    {
        private List<ThreadResult> threadsResults = new List<ThreadResult>();

        public void InsertMethodResult(MethodResult methodResult, List<(string, string)> methodsClasses, int threadId)
        {
            ThreadResult currentThread = null;
            foreach (ThreadResult threadResult in threadsResults)
            {
                if (threadId == threadResult.id)
                {
                    currentThread = threadResult;
                    break;
                }
            }
            if (currentThread == null)
            {
                currentThread = new ThreadResult(threadId);
                threadsResults.Add(currentThread);
            }
            currentThread.InsertMethodResult(methodResult, methodsClasses);
        }

        public void StopTrace(DateTime stopTime, List<(string, string)> methodsClasses, int threadId)
        {
            ThreadResult currentThread = null;
            foreach (ThreadResult threadResult in threadsResults)
            {
                if (threadId == threadResult.id)
                {
                    currentThread = threadResult;
                    break;
                }
            }
            currentThread.StopTrace(stopTime, methodsClasses);
        }
        public TraceResult()
        {
        }
    }
}
