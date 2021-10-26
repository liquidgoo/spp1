using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracer;

namespace Serialization
{
    public interface ISerializer
    {
        public void Serialize(TraceResult result);

        public string GetSerializedResult();
       
    }
}
