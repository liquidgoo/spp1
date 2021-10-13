using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tracer
{
    static class Class1
    {
        static Tracer tracer = new Tracer();

        public static void Main()
        {
            A1();
        }

        public static void A1()
        {
            tracer.StartTrace();
            A2();
            A3();
            tracer.StopTrace();
        }

        public static void A2()
        {
            tracer.StartTrace();
            Thread.Sleep(2000);
            tracer.StopTrace();
        }

        public static void A3()
        {
            tracer.StartTrace();
            Thread.Sleep(4000);
            tracer.StopTrace();
        }
    }
}
