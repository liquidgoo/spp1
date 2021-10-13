using System;
using System.Collections.Generic;
using System.Collections.Immutable;
namespace Tracer
{
    [Serializable]
    public class TraceResult
    {
        public List<ThreadResult> ThreadsResults { get; }

        public void InsertMethodResult(MethodResult methodResult, List<(string, string)> methodsClasses, int threadId)
        {
            ThreadResult currentThread = null;
            foreach (ThreadResult threadResult in ThreadsResults)
            {
                if (threadId == threadResult.Id)
                {
                    currentThread = threadResult;
                    break;
                }
            }
            if (currentThread == null)
            {
                currentThread = new ThreadResult(threadId);
                ThreadsResults.Add(currentThread);
            }
            currentThread.InsertMethodResult(methodResult, methodsClasses);
        }

        public void StopTrace(DateTime stopTime, List<(string, string)> methodsClasses, int threadId)
        {
            ThreadResult currentThread = null;
            foreach (ThreadResult threadResult in ThreadsResults)
            {
                if (threadId == threadResult.Id)
                {
                    currentThread = threadResult;
                    break;
                }
            }
            currentThread.StopTrace(stopTime, methodsClasses);
        }
        public TraceResult()
        {
            ThreadsResults = new List<ThreadResult>();
        }
    }
}
