﻿using System;
using System.Threading;

// ReSharper disable StyleCop.SA1400
// ReSharper disable StyleCop.SA1202
// ReSharper disable StyleCop.SA1201

// This lab trains the following skills:
// 1. Creating a new thread with specified thread start.
// 2. Running a thread.
// 3. Passing parameters when running a new thread.
// 4. Getting a thread state.
// 5. Sending a notification to another thread using shared memory.
// 6. Waiting for another thread to complete.
namespace ManagingThreads
{
    // TODO The code in this application contains an issue with timeouts. That leads to a huge amount of records in console.
    public class Program
    {
        private const int ThreadsCount = 10;

        private const int WorkTimeout = 2500;

        private static bool _isRunning = true;

        class WorkerThreadParameters
        {
            public int ThreadNumber { get; set; }

            public int Timeout { get; set; }
        }

        // NOTE Use this documentation page to complete this task:
        // https://msdn.microsoft.com/en-us/library/system.threading.thread(v=vs.110).aspx
        public static void Main(string[] args)
        {
            // TODO Modify the next line to create a new thread using StarterThreadStart delegate as a thread start and save it into starterThread variable.
            Thread starterThread = new Thread(StarterThreadStart);

            Console.WriteLine("Press any key to start.");
            Console.ReadKey();

            // TODO Run a starter thread.
            starterThread.Start();

            // TODO Modify the next line to display state of the current thread.
            Console.WriteLine($"Main thread: starter thread state is {starterThread.ThreadState}.");

            Console.WriteLine("Press any key to stop all worker threads.");
            Console.ReadLine();
            
            // TODO Send a signal to all worker threads that they should stop doing their work.
            // TODO Wait until starter thread will finish its work. Use an instance method of the Thread class.
            _isRunning = false;
            starterThread.Join();

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static void StarterThreadStart()
        {
            Console.WriteLine("--- Starter thread {0}: Started. ---", Thread.CurrentThread.ManagedThreadId);

            var threads = new Thread[ThreadsCount];

            for (int i = 1; i < ThreadsCount; i++)
            {
                var threadNumber = i;
                var timeout = (WorkTimeout / ThreadsCount) * i;
                Thread.Sleep(timeout);

                // TODO Modify the next line to create a new worker thread using WorkerThreadStart delegate as a thread start and save it into threads array.
                threads[i] = new Thread(WorkerThreadStart);

                // TODO Run recently created worker thread and pass threadNumber and WorkTimeout as a parameter using WorkerThreadParameters class.
                threads[i].Start(new WorkerThreadParameters() { ThreadNumber = threadNumber, Timeout = timeout });
            }

            for (int i = 1; i < threads.Length; i++)
            {
                // TODO Wait until worker thread with thread number == i will finish its work. Use an instance method of the Thread class.
                var threadNumber = i;
                var timeout = (WorkTimeout / ThreadsCount) * i;

                threads[threadNumber].Join(timeout);
            }

            Console.WriteLine("--- Starter thread {0}: Completed. ---", Thread.CurrentThread.ManagedThreadId);
        }

        private static void WorkerThreadStart(object parameter)
        {
            // TODO Set threadNumber and timeout in relevant values using a thread parameter.
            WorkerThreadParameters parameters = (WorkerThreadParameters)parameter;
            int threadNumber = parameters.ThreadNumber;
            int timeout = parameters.Timeout;

            Console.WriteLine("Worker thread {0}-{1:00}: Started.", threadNumber, Thread.CurrentThread.ManagedThreadId);

            do
            {
                Console.WriteLine("Worker thread {0}-{1:00}: Working.", threadNumber,
                    Thread.CurrentThread.ManagedThreadId);
                DoSomeWork(timeout);
            }
            while (_isRunning);

            Console.WriteLine("Worker thread {0}-{1}: Completed.", threadNumber, Thread.CurrentThread.ManagedThreadId);
        }

        private static void DoSomeWork(int timeout)
        {
            // NOTE Sleep() is used here instead of work task.
            Thread.Sleep(timeout);
        }
    }
}