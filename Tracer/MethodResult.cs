using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Tracer
{
    [Serializable]
    public class MethodResult
    {
        public string MethodName { get; set; }
        public string ClassName { get; set; }
        public double ElapsedTime { get; set; }
        public ImmutableList<MethodResult> InnerMethodsResults { get; }
        public MethodResult(double time, string className, string methodName, ImmutableList<MethodResult> methodsResults)
        {
            ElapsedTime = time;
            this.MethodName = methodName;
            this.ClassName = className;
            InnerMethodsResults = methodsResults;
        }
        public MethodResult() { }

        public override bool Equals(object obj)
        {
            if (typeof(MethodResult) != obj.GetType()) return false;

            MethodResult other = (MethodResult)obj;

            if (MethodName != other.MethodName) return false;
            if (ClassName != other.ClassName) return false;
            if (Math.Abs(ElapsedTime - other.ElapsedTime) > 5) return false;

            if (InnerMethodsResults.Count != other.InnerMethodsResults.Count) return false;

            for (int i = 0; i < InnerMethodsResults.Count; i++)
            {
                if (!InnerMethodsResults[i].Equals(other.InnerMethodsResults[i])) return false;
            }
            return true;
        }
    }
}
