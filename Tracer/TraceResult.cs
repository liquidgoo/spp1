using System;
using System.Collections.Generic;
using System.Collections.Immutable;
namespace Tracer
{
    [Serializable]
    public class TraceResult
    {
        public ImmutableList<ThreadResult> ThreadsResults { get; }

        public TraceResult(ImmutableList<ThreadResult> threadsResults)
        {
            ThreadsResults = threadsResults;
        }
        public TraceResult() { }

        public override bool Equals(object obj)
        {
            if (typeof(TraceResult) != obj.GetType()) return false;

            TraceResult other = (TraceResult)obj;

            if (ThreadsResults.Count != other.ThreadsResults.Count) return false;

            for (int i = 0;  i < ThreadsResults.Count; i++)
            {
                if (!ThreadsResults[i].Equals(other.ThreadsResults[i])) return false;
            }

            return true;
        }
    }
}
