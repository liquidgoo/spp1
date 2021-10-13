using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Tracer
{
    class Tracer : ITracer
    {
        private TraceResult result = new TraceResult();

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
            List<(string, string)> methodsClasses = GetMethodsClasses();
            (string, string) methodClass = methodsClasses[0];

            MethodResult methodResult = new MethodResult(DateTime.Now, methodClass.Item1, methodClass.Item2);
            result.InsertMethodResult(methodResult, methodsClasses);
        }

        public void StopTrace()
        {
            List<(string, string)> methodsClasses = GetMethodsClasses();

            result.StopTrace(DateTime.Now, methodsClasses);
        }

        public TraceResult GetTraceResult()
        {
            throw new NotImplementedException();
        }
    }
}
