using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Tracer
{
    [Serializable]
    public class ThreadResult
    {
        public int Id { get; set; }
        public double Time { get; set; }
        public ImmutableList<MethodResult> MethodsResults { get; }
        public ThreadResult(int threadId, double time, ImmutableList<MethodResult> methodsResults)
        {
            Id = threadId;
            Time = time;
            MethodsResults = methodsResults;
        }
        public ThreadResult() { }

        public override bool Equals(object obj)
        {
            if (typeof(ThreadResult) != obj.GetType()) return false;

            ThreadResult other = (ThreadResult)obj;

            if (Id != other.Id) return false;
            if (Math.Abs(Time - other.Time) > MethodsResults.Count * 5) return false;

            if (MethodsResults.Count != other.MethodsResults.Count) return false;
            
            for (int i = 0; i < MethodsResults.Count; i++)
            {
                if (!MethodsResults[i].Equals(other.MethodsResults[i])) return false;
            }
            return true;
        }
    }
}
