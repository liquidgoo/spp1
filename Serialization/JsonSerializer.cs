using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Tracer;

namespace Serialization
{
    public class JsonSerializer : ISerializer
    {
        private string _serializedResult;

        public string GetSerializedResult()
        {
            return _serializedResult;
        }

        public void Serialize( TraceResult result)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            _serializedResult = System.Text.Json.JsonSerializer.Serialize(result, options);
        }
    }
}
