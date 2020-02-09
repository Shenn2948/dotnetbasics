using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable StyleCop.SA1400
namespace UsingContinuousTasks
{
    class Program
    {
        public const string Token = "Thread";

        public static void Main(string[] args)
        {
            string[] urls =
                {
                    "http://microsoft.com",
                    "http://www.learnasync.net/",
                    "http://www.albahari.com/threading/",
                    "http://jonskeet.uk/csharp/threads/",
                    "https://stephencleary.com/book/",
                    "https://docs.microsoft.com/en-us/dotnet/standard/threading/the-managed-thread-pool",
                    "https://mva.microsoft.com/search/SearchResults.aspx#!q=Jeffrey%20Richter%20Threading&lang=1033",
                    "https://channel9.msdn.com/Events/Build/2012/3-011",
                    "http://asd234efwdw23reefsfdsfds.com",
                    "https://codewala.net/2015/07/29/concurrency-vs-multi-threading-vs-asynchronous-programming-explained/",
                    "http://dotnetpattern.com/multi-threading-interview-questions",
                    "https://www.pluralsight.com/courses/skeet-async",
                    "https://rsdn.org/article/dotnet/CSThreading1.xml"
                };

            var tasks = new Task<int>[urls.Length];

            for (int i = 0; i < tasks.Length; i++)
            {
                var index = i;

                // TODO Create a new task and queue it to run.
                Task<byte[]> webClientTask = Task.Run(
                    () =>
                        {
                            Console.WriteLine($"Task {Task.CurrentId} is started. Downloading {urls[index]}");

                            var webClient = new WebClient();
                            var bytes = webClient.DownloadData(urls[index]);

                            Console.WriteLine($"Task {Task.CurrentId} is completed.");

                            // TODO Set task result.
                            return bytes;
                        });

                Task<string> getStringTask = webClientTask.ContinueWith(
                    x =>
                        {
                            // TODO Create a new task as a continuation of the web-client task. This task should run only if the previous task completed successfully.
                            Console.WriteLine($"Task {Task.CurrentId} is converting bytes to string.");

                            // TODO Set the result from the previous task.
                            byte[] bytes = x.Result;
                            string resultString = Encoding.UTF8.GetString(bytes);

                            // TODO Set task result.
                            return resultString;
                        });

                webClientTask.ContinueWith(
                    x =>
                        {
                            if (x.IsFaulted)
                            {
                                // TODO Create a new task as a continuation of the web-client task. This task should run only if the previous task failed.
                                // TODO Set Id of the previous task.
                                int id = x.Id;

                                // TODO Set inner exception message that happen in the previous task.
                                string message = x.Exception?.Message;

                                Console.WriteLine($"Task {id} failed with exception: {message}");
                            }
                        });

                tasks[index] = getStringTask.ContinueWith(
                    x =>
                        {
                            // TODO Create a new task as a continuation of the get-string task. This task should run only if the previous task completed successfully.
                            Console.WriteLine($"Task {Task.CurrentId} is returning string length.");

                            // TODO Set the result from the previous task.
                            string resultString = x.Result;

                            var occurences = IndexesOf(resultString, Token).Length;

                            // TODO Set task result.
                            return occurences;
                        });
            }

            Console.WriteLine("Waiting for tasks to complete.");
            try
            {
                // TODO Use tasks array to wait until all the tasks will complete their work.
                Task.WaitAll(tasks);
            }
            catch
            {
                // NOTE Eat an exception here because we do not care about it here.
            }

            Console.WriteLine($"\nResults for searching '{Token}' word:");
            for (int i = 0; i < urls.Length; i++)
            {
                Console.WriteLine($"{urls[i]} - {(!(tasks[i].IsFaulted || tasks[i].IsCanceled) ? tasks[i].Result.ToString() : tasks[i].Status.ToString())}");
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static int[] IndexesOf(string str, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("value is empty", "value");
            }

            List<int> indexes = new List<int>();
            for (int index = 0;; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                {
                    return indexes.ToArray();
                }

                indexes.Add(index);
            }
        }
    }
}