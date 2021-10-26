using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace Tracer
{
    public class SimpleTracer : ITracer
    {
        private Dictionary<int, List<MethodInfo>> _methodsInfo = new Dictionary<int, List<MethodInfo>>();
        private object lockObject = new object();

        private List<(string, string, string, int)> GetStackInfo()
        {
            StackFrame[] stackFrames = new StackTrace(true).GetFrames();
            List<(string, string,string, int)> stackInfo = new List<(string, string, string, int)>();
            MethodBase method;
            for (int i = 2; i < stackFrames.Length - 1; i++)
            {
                method = stackFrames[i].GetMethod();
                stackInfo.Add((method.DeclaringType.FullName, method.Name, stackFrames[i + 1].GetFileName(), stackFrames[i + 1].GetFileLineNumber()));
            }
            method = stackFrames[stackFrames.Length - 1].GetMethod();
            stackInfo.Add((method.DeclaringType.FullName, method.Name, stackFrames[stackFrames.Length - 1].GetFileName(), 0));
            return stackInfo;
        }
        public void StartTrace()
        {
            List<(string, string, string, int)> stackInfo = GetStackInfo();
            (string, string, string, int) stackMethodInfo = stackInfo[0];
            int threadId = Thread.CurrentThread.ManagedThreadId;

            List<MethodInfo> currentThreadMethods;
            lock (lockObject)
            {
                if (!_methodsInfo.TryGetValue(threadId,out currentThreadMethods))
                {
                    currentThreadMethods = new List<MethodInfo>();
                    _methodsInfo.Add(threadId, currentThreadMethods);
                }

                MethodInfo methodInfo = new MethodInfo(stackMethodInfo.Item1, stackMethodInfo.Item2, stackMethodInfo.Item3, stackMethodInfo.Item4);

                _insertMethodInfo(methodInfo, currentThreadMethods, stackInfo, stackInfo.Count - 1);
            }
        }

        private void _insertMethodInfo(MethodInfo methodInfo, List<MethodInfo> calledMethods, List<(string, string, string, int)> stackInfo, int lastStackDepth)
        {
            (MethodInfo, int) callingMethod = FindCallingMethod(calledMethods, stackInfo, lastStackDepth);
            if (callingMethod.Item1 != null)
                _insertMethodInfo(methodInfo, callingMethod.Item1.calledMethods, stackInfo, callingMethod.Item2 - 1);
            else
            {
                calledMethods.Add(methodInfo);
                methodInfo.StartTime = DateTime.Now;
            }
        }

        public void StopTrace()
        {
            DateTime stopTime = DateTime.Now;
            List<(string, string, string, int)> stackInfo = GetStackInfo();
            int threadId = Thread.CurrentThread.ManagedThreadId;

            lock (lockObject)
            {
                List<MethodInfo> currentThreadMethods = _methodsInfo[threadId];
                (MethodInfo, int) outerMethod = FindCallingMethod(currentThreadMethods, stackInfo, stackInfo.Count - 1, 0);
                _insertStopTime(stopTime, outerMethod.Item1, stackInfo, outerMethod.Item2 - 1);
            }
        }

        private void _insertStopTime(DateTime stopTime, MethodInfo callingMethod, List<(string, string, string, int)> stackInfo, int lastStackDepth)
        {
            (MethodInfo, int) newCallingMethod = FindCallingMethod(callingMethod.calledMethods, stackInfo, lastStackDepth, 0);
            if (newCallingMethod.Item1 != null)
            {
                _insertStopTime(stopTime, newCallingMethod.Item1, stackInfo, newCallingMethod.Item2 - 1);
            }
            else
            {
                callingMethod.StopTime = stopTime;
            }
        }

        private (MethodInfo, int) FindCallingMethod(List<MethodInfo> calledMethods, List<(string, string, string, int)> stackInfo, int lastStackDepth, int minDepth = 1)
        {
            for (int i = lastStackDepth; i >= minDepth; i--)
            {
                foreach (MethodInfo method in calledMethods)
                {
                    if (method.FileName.Equals(stackInfo[i].Item3) &&
                        method.Line.Equals(stackInfo[i].Item4))
                    {
                        return (method, i);
                    }
                }
            }
            return (null, -1);
        }


        public TraceResult GetTraceResult()
        {
            List<ThreadResult> threadsResults = new List<ThreadResult>();
            foreach ((int threadId, List<MethodInfo> calledMethodsInfo) in _methodsInfo)
            {
                threadsResults.Add(_getThreadResult(threadId, calledMethodsInfo));
            }
            return new TraceResult(ImmutableList<ThreadResult>.Empty.AddRange(threadsResults));
        }

        private ThreadResult _getThreadResult(int threadId, List<MethodInfo> calledMethodsInfo)
        {
            List<MethodResult> calledMethodsResults = new List<MethodResult>();
            double totalThreadTime = 0 ;
            foreach (MethodInfo methodInfo in calledMethodsInfo)
            {
                MethodResult methodResult = _getMethodResult(methodInfo);
                totalThreadTime += methodResult.ElapsedTime; //_getTotalTime(methodResult);
                calledMethodsResults.Add(methodResult);
            }
            return new ThreadResult(threadId, totalThreadTime, ImmutableList<MethodResult>.Empty.AddRange(calledMethodsResults));
        }
        /*
        private double _getTotalTime(MethodResult methodResult)
        {
            double totalTime = methodResult.ElapsedTime;
            foreach(MethodResult calledMethodResult in methodResult.InnerMethodsResults)
            {
                totalTime += _getTotalTime(calledMethodResult);
            }
            return totalTime;
        }
        */

        private MethodResult _getMethodResult(MethodInfo methodInfo)
        {
            double elapsedTime = (methodInfo.StopTime - methodInfo.StartTime).TotalMilliseconds;

            List<MethodResult> calledMethodsResults = new List<MethodResult>();
            foreach(MethodInfo calledMethodInfo in methodInfo.calledMethods)
            {
                MethodResult methodResult = _getMethodResult(calledMethodInfo);
                //elapsedTime -= methodResult.ElapsedTime;
                calledMethodsResults.Add(methodResult);
            }
            return new MethodResult(elapsedTime, methodInfo.ClassName, methodInfo.MethodName,
                                    ImmutableList<MethodResult>.Empty.AddRange(calledMethodsResults));
        }
    }
}
