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
        private readonly List<int> _list2;
        private readonly StringBuilder _sb1;
        private readonly StringBuilder _sb2;
        private readonly CancellationTokenSource _cts;
        private readonly CancellationToken _token;
        private bool _isCompleted;
        private bool _isCanceled;

        public Starter()
        {
            _taskList = new List<Task>();
            _list1 = new List<Task<int>>();
            _list2 = new List<int>();
            _sb1 = new StringBuilder();
            _sb2 = new StringBuilder();
            _cts = new CancellationTokenSource();
            _token = _cts.Token;
            _isCompleted = false;
            _isCanceled = false;
        }

        public void Run()
        {
            try
            {
                TasksAsync(38, 70);
                WaitAsync(10);
                Log();
                Print();
                Console.ReadKey();
                _cts?.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{nameof(Run)}: {ex?.GetType().Name}: {ex?.Message}");
            }
        }

        /// <summary>
        /// TasksAsync.
        /// </summary>
        /// <param name="f">Start number to calculate Fibonacci from to 0.</param>
        /// <param name="t">The number of cycles.</param>
        public async void TasksAsync(int f, int t)
        {
            if (_taskList == null)
            {
                return;
            }

            _taskList.Add(Task.Run(() => FibonacciTask1Async(), _token));
            _taskList.Add(Task.Run(() => FibonacciTask2Async(f, t), _token));

            await Task.WhenAll(_taskList);
        }

        public async void FibonacciTask1Async()
        {
            if (_list1 == null)
            {
                return;
            }

            try
            {
                _list1.Add(Task.Run(() => Fibonacci(0), _token));
                _list1.Add(Task.Run(() => Fibonacci(1), _token));
                _list1.Add(Task.Run(() => Fibonacci(2), _token));
                _list1.Add(Task.Run(() => Fibonacci(3), _token));
                _list1.Add(Task.Run(() => Fibonacci(4), _token));
                _list1.Add(Task.Run(() => Fibonacci(5), _token));
                _list1.Add(Task.Run(() => Fibonacci(15), _token));

                await Task.WhenAll(_list1);
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine($"{nameof(FibonacciTask1Async)}: {ex?.GetType().Name}: {ex?.Message}");
                _isCanceled = true;
            }
        }

        /// <summary>
        /// FibonacciTask2Async.
        /// </summary>
        /// <param name="f">Start number to calculate Fibonacci from to 0.</param>
        /// <param name="t">The number of cycles.</param>
        public async void FibonacciTask2Async(int f, int t)
        {
            try
            {
                Console.WriteLine($"{nameof(FibonacciTask2Async)} started: {DateTime.Now:hh:mm:ss}");

                for (int j = 0; j < t; j++)
                {
                    for (var i = f; i >= 0; i--)
                    {
                        if (_list2 == null)
                        {
                            return;
                        }

                        _list2.Add(await Task.Run(() => Fibonacci(i), _token));
                    }
                }

                _isCompleted = true;
                _cts.Cancel();
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine($"{nameof(FibonacciTask2Async)}: {ex?.GetType().Name}: {ex?.Message}");
                _isCanceled = true;
            }
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
                if (_list1?.Count > c1 && _list1[c1]?.Result != default)
                {
                    _sb1?.AppendLine($"ListTask1[{c1}] = {_list1[c1]?.Result}");
                    c1++;
                }

                if (_list2?.Count > c2)
                {
                    _sb2?.AppendLine($"ListTask2[{c2}] = {_list2[c2]}");
                    c2++;
                }

                if ((_list1?.Count == c1 && _list2?.Count == c2 && _isCompleted) || _isCanceled)
                {
                    break;
                }
            }

            return;
        }

        public void Print()
        {
            while (true)
            {
                if (_token.IsCancellationRequested)
                {
                    Console.WriteLine($"{nameof(FibonacciTask2Async)} has been canceled by CancellationToken!");
                }

                if (_isCompleted || _isCanceled)
                {
                    Console.WriteLine($"{nameof(FibonacciTask2Async)} finished: {DateTime.Now:hh:mm:ss}");
                    PrintInternal();
                    break;
                }
            }

            return;
        }

        public void PrintInternal()
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
        }

        public async void WaitAsync(int s)
        {
            try
            {
                _cts?.CancelAfter(s * 1000);
                Console.WriteLine($"Wait started: {DateTime.Now:hh:mm:ss}");
                await Task.Delay(s * 1000, _token);
                Console.WriteLine($"Wait finished: {DateTime.Now:hh:mm:ss}");
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine($"{nameof(WaitAsync)}: {ex?.GetType().Name}: {ex?.Message}");
            }
        }
    }
}
