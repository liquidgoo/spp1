using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace Tracer
{
    public class Tracer : ITracer
    {
        private TraceResult result = new TraceResult();
        private object lockObject = new object();

        private List<(string, string)> GetMethodsClasses()
        {
            StackFrame[] stackFrames = new StackTrace(false).GetFrames();
            List<(string, string)> methodsClasses = new List<(string, string)>();
            for(int i = 2; i < stackFrames.Length; i++)
            {
                MethodBase method = stackFrames[i].GetMethod();
                methodsClasses.Add((method.DeclaringType.FullName, method.Name));
            }
            return methodsClasses;
        }
        public void StartTrace()
        {
            lock (lockObject)
            {
                List<(string, string)> methodsClasses = GetMethodsClasses();
                (string, string) methodClass = methodsClasses[0];
                int threadId = Thread.CurrentThread.ManagedThreadId;

                MethodResult methodResult = new MethodResult(DateTime.Now, methodClass.Item1, methodClass.Item2);
                result.InsertMethodResult(methodResult, methodsClasses, threadId);
            }
        }

        public void StopTrace()
        {
            lock (lockObject)
            {
                List<(string, string)> methodsClasses = GetMethodsClasses();
                int threadId = Thread.CurrentThread.ManagedThreadId;

                result.StopTrace(DateTime.Now, methodsClasses, threadId);
            }
        }

        public TraceResult GetTraceResult()
        {
            return result;
        }
    }
}
