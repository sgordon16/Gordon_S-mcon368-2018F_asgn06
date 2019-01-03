using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoldbachConjecture
{
    class Program
    {
        static Thread[] threads;
        private static int counter = 0;
        private const int min = 2;
        public static int max;
        public static Object lockObject = new Object();
        private static List<int> primes = new List<int>();
        public static ConcurrentDictionary<int, Tuple<int, int>> dict = new ConcurrentDictionary<int, Tuple<int, int>>();
        private static void GeneratePrimes(int start, int range)
        {
            var isPrime = true;
            var end = start + range;
            for (var i = start; i < end; i++)
            {
                for (var j = min; j < Math.Sqrt(end); j++)
                {
                    if (i != j && i % j == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }
                if (isPrime)
                {
                    primes.Add(i);
                }
                isPrime = true;
            }
        }
        public static void GenerateGoldbachConjecture(int num)
        {
            int prime1 = 0;
            int prime2 = 0;
            int i = 0;
            int j = 0;
            
            while(prime1 + prime2 != num && i < primes.Count)
            {
                prime1 = primes.ElementAt(i);
                j = i;
                while (prime1 + prime2 != num && j < primes.Count)
                {
                    prime2 = primes.ElementAt(j);
                    if (prime1 + prime2 == num)
                    {
                        dict.TryAdd(num, new Tuple<int, int>(prime1, prime2));
                    }
                    j++;
                }
                i++;
            }
        }
        public static void loopGoldbachConjecture(int max)
        {
            while (counter < max)
            {
                lock (lockObject)
                {
                    if (counter == 0)
                        counter = 4;
                    else
                        counter += 2;
                }
                GenerateGoldbachConjecture(counter);
            }
        }
        public static void Cancel()
        {
            if (Console.ReadKey().Key == ConsoleKey.Enter)
            {
                foreach (Thread t in threads)
                    t.Abort();
            }
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Enter number of threads");
            var threadCount = Convert.ToInt32(Console.ReadLine());
            threads = new Thread[threadCount];
            Console.WriteLine("Enter number");
            max = Convert.ToInt32(Console.ReadLine());
            new Thread(() => Cancel()).Start();
            GeneratePrimes(2, max);
            for (var i = 0; i < threadCount; i++)
            { 
                threads[i] = new Thread(() => loopGoldbachConjecture(max));
                threads[i].Start();
            }
            for (var i = 0; i < threadCount; i++)
               threads[i].Join();
            foreach (KeyValuePair<int, Tuple<int, int>> entry in dict.OrderBy(e => e.Key))
            {
                Console.WriteLine("{0}) {1} + {2} = {0}", entry.Key, entry.Value.Item1, entry.Value.Item2);
            }
            Console.ReadKey();
        }  
    }
}
