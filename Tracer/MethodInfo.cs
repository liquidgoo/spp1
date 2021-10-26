using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracer
{
    public class MethodInfo
    {
        public string MethodName { get; }
        public string ClassName { get; }
        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }
        public string FileName { get; }
        public int Line { get; }
        public List<MethodInfo> calledMethods;

        public MethodInfo(string className, string methodName, string fileName, int line)
        {
            this.MethodName = methodName;
            this.ClassName = className;
            this.FileName = fileName;
            this.Line = line;
            calledMethods = new List<MethodInfo>();
        }
    }

}
