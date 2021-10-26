using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tracer;

namespace Serialization
{
    public class CommonCase
    {
        static void Main(string[] args)
        {
            ITracer tracer = new Tracer.SimpleTracer();
            OuterClass outer = new OuterClass(tracer);

            Task task = new Task(outer.M0);
            task.Start();
            outer.M0();
            task.Wait();


            TraceResult result = outer._tracer.GetTraceResult();
            ISerializer serializer = new XmlSerializer();
            serializer.Serialize(result);

            Writer writer = new Writer(Console.OpenStandardOutput(), serializer);
            writer.Write();

            writer = new Writer(File.OpenWrite("result.xml"), serializer);
            writer.Write();

            serializer = new JsonSerializer();
            serializer.Serialize(result);

            Console.WriteLine("====================================================================================");
            writer = new Writer(Console.OpenStandardOutput(), serializer);
            writer.Write();

            writer = new Writer(File.OpenWrite("result.json"), serializer);
            writer.Write();


            Console.ReadLine();
        }
    }

    class InnerClass
    {
        public ITracer _tracer;

        private void Wait(int milliseconds)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (sw.ElapsedMilliseconds < milliseconds) ;

        }

        public InnerClass(ITracer tracer)
        {
            _tracer = tracer;
        }

        public void M5()
        {
            _tracer.StartTrace();
            Wait(20);
            M8();
            _tracer.StopTrace();
        }

        public void M6()
        {
            _tracer.StartTrace();
            Wait(20);
            _tracer.StopTrace();
        }

        public void M8()
        {
            _tracer.StartTrace();
            _tracer.StopTrace();
        }
    }

    public class OuterClass
    {
        private void Wait(int milliseconds)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (sw.ElapsedMilliseconds < milliseconds) Thread.SpinWait(1000);
            sw.Stop();
        }

        public ITracer _tracer;
        private InnerClass inner;

        public OuterClass(ITracer tracer)
        {
            _tracer = tracer;
            inner = new InnerClass(_tracer);
        }

        public void M0()
        {
            M1();
            M2();
            inner.M5();
            inner.M6();
            inner.M6();
            inner.M5();
            M4();
        }

        private void M1()
        {
            _tracer.StartTrace();
            Wait(100);
            M3();
            _tracer.StopTrace();
        }

        private void M2()
        {
            _tracer.StartTrace();
            Wait(300);
            M3();
            _tracer.StopTrace();
        }

        public void M3()
        {
            _tracer.StartTrace();
            M4();
            Wait(111);
            _tracer.StopTrace();
        }

        public void M4()
        {
            _tracer.StartTrace();
            inner.M5();
            Wait(20);
            inner.M6();
            _tracer.StopTrace();
        }
    }
}

