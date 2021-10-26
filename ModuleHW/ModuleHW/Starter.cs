using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModuleHW
{
    public class Starter
    {
        private readonly List<Task> _taskList;
        private readonly List<Task<int>> _list1;
        private readonly List<Task<int>> _list2;
        private readonly StringBuilder _sb1;
        private readonly StringBuilder _sb2;
        private readonly CancellationTokenSource _cts;
        private readonly CancellationToken _token;

        public Starter()
        {
            _taskList = new List<Task>();
            _list1 = new List<Task<int>>();
            _list2 = new List<Task<int>>();
            _sb1 = new StringBuilder();
            _sb2 = new StringBuilder();
            _cts = new CancellationTokenSource();
            _token = _cts.Token;
        }

        public void Run()
        {
            TasksAsync();
            Log();
            Print();
            Console.ReadKey();
            _cts?.Dispose();
        }

        public async void TasksAsync()
        {
            if (_taskList == default)
            {
                return;
            }

            var f1 = Task.Run(() => FibonacciTask1Async(), _token);
            var f2 = Task.Run(() => FibonacciTask2Async(), _token);
            var wait = Task.Run(() => Wait(10), _token);

            _taskList.Add(f1);
            _taskList.Add(f2);
            _taskList.Add(wait);

            await Task.WhenAll(_taskList);
        }

        public async void FibonacciTask1Async()
        {
            if (_list1 == null)
            {
                return;
            }

            _list1.Add(Task.Run(() => Fibonacci(0)));
            _list1.Add(Task.Run(() => Fibonacci(1)));
            _list1.Add(Task.Run(() => Fibonacci(2)));
            _list1.Add(Task.Run(() => Fibonacci(3)));
            _list1.Add(Task.Run(() => Fibonacci(4)));
            _list1.Add(Task.Run(() => Fibonacci(5)));

            await Task.WhenAll(_list1);
        }

        public async void FibonacciTask2Async()
        {
            Console.WriteLine($"{nameof(FibonacciTask2Async)} started: {DateTime.Now:hh:mm:ss}");

            for (var i = 40; i >= 0; i--)
            {
                if (_list2 == null)
                {
                    return;
                }

                _list2.Add(Task.Run(() => Fibonacci(i)));

                await Task.WhenAll(_list2);
            }

            _cts?.Cancel();

            Console.WriteLine($"{nameof(FibonacciTask2Async)} finished: {DateTime.Now:hh:mm:ss}");
        }

        public int Fibonacci(int n)
        {
            if (n == 0 || n == 1)
            {
                return n;
            }
            else
            {
                return Fibonacci(n - 1) + Fibonacci(n - 2);
            }
        }

        public void Log()
        {
            var c1 = 0;
            var c2 = 0;

            while (true)
            {
                while (_token.IsCancellationRequested)
                {
                    return;
                }

                while (_list1?.Count > c1 && _list1[c1]?.Result != default)
                {
                    _sb1?.AppendLine($"ListTask1[{c1}] = {_list1[c1]?.Result}");
                    c1++;
                }

                while (_list2?.Count > c2 && _list2[c2]?.Result != default)
                {
                    _sb2?.AppendLine($"ListTask2[{c2}] = {_list2[c2]?.Result}");
                    c2++;
                }
            }
        }

        public void Print()
        {
            while (_token.IsCancellationRequested)
            {
                Console.WriteLine(string.Empty);

                if (_sb1?.Length > 0)
                {
                    Console.WriteLine("List 1:");
                    Console.WriteLine(_sb1);
                }

                if (_sb2?.Length > 0)
                {
                    Console.WriteLine("List 2:");
                    Console.WriteLine(_sb2);
                }

                Console.WriteLine("Application has been canceled by CancellationToken!");

                return;
            }
        }

        public void Wait(int s)
        {
            Thread.Sleep(200);
            Console.WriteLine($"Wait started: {DateTime.Now.AddMilliseconds(-200):hh:mm:ss}");
            Thread.Sleep((s * 1000) - 200);

            if (_token.IsCancellationRequested)
            {
                Console.WriteLine($"Wait finished: {DateTime.Now:hh:mm:ss}");
                return;
            }

            _cts?.Cancel();
            Console.WriteLine($"Wait finished: {DateTime.Now:hh:mm:ss}");
        }
    }
}
