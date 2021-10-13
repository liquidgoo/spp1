using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Tracer;

namespace Serialization
{
    public class JsonSerializer : ISerializer
    {
        public void Serialize(Stream outputStream, TraceResult result)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            byte[] bytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(result, options);
            outputStream.Write(bytes);
        }
    }
}
