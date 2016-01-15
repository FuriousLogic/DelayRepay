using DelayRepay_BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DelayRepay_Console
{
    class Program
    {
        static bool isRunning = false;

        static void Main(string[] args)
        {
            Console.WriteLine("Main thread: starting a timer");
            Timer t = new Timer(DoSomething, 0, 0, (60 * 1000));
            Console.ReadLine();
            t.Dispose(); // Cancel the timer now
        }

        private static void DoSomething(Object state)
        {
            if (isRunning)
                Console.WriteLine("DoSomething still running from last time {0}", DateTime.Now.ToString("dd/MMM/yy HH:mm:ss"));
            else
                Console.WriteLine("DoSomething {0}", DateTime.Now.ToString("dd/MMM/yy HH:mm:ss"));

            isRunning = true;
            DRHelper helper = new DRHelper();
            helper.DoSomething();
            helper = null;
            isRunning = false;
        }
    }
}
