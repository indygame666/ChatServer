using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DedicatedServer
{
    class Program
    {
        private static bool isRunning = false;
        static void Main(string[] args)
        {
            isRunning = true;

            Thread mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();

            Server.StartServer(777);
            Console.ReadKey();
        }

        private static void MainThread()
        {
            Console.WriteLine($"Thread started.");
            DateTime loopTime = DateTime.Now;

            while (isRunning == true)
            {
                while (loopTime < DateTime.Now)
                {
                    GameLogic.Update();
                    loopTime = loopTime.AddMilliseconds(Config.MS_PER_TICK);
                }
            }
        }
    }
}
