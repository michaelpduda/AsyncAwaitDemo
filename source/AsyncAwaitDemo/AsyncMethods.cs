/* This file is part of the Async/Await Demo Project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/AsyncAwaitDemo/blob/master/LICENSE.md
 */
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwaitDemo
{
    public class AsyncMethods
    {
        private readonly Action<int> _lightUp;
        private readonly Action<bool> _lockUi;
        private readonly Action _reset;

        public AsyncMethods(Action<bool> lockUi,  Action reset, Action<int> lightUp)
        {
            _lightUp = lightUp;
            _lockUi = lockUi;
            _reset = reset;
        }

        /// <summary>
        /// Not async. Will block the UI thread and won't even reset lights or lock/unlock the UI.
        /// </summary>
        public void Demo_01()
        {
            _lockUi(true); // No *real* effect
            _reset(); // No effect
            for (var i = 0; i < 3; i++)
            {
                Thread.Sleep(TimeSpan.FromSeconds(1)); // Will block the UI thread
                _lightUp(i); // All will light up at the same time
            }
            _lockUi(false);
        }

        /// <summary>
        /// Not async. Doesn't block the UI thread, but crashes. Will reset lights, but won't lock/unlock UI.
        /// </summary>
        public void Demo_02()
        {
            _lockUi(true); // No effect
            _reset(); // Will work
            ThreadPool.QueueUserWorkItem(_ =>
            {
                for (var i = 0; i < 3; i++)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1)); // Will block the UI thread
                    _lightUp(i); // Will crash on first try, since coming from non-UI thread
                }
            });
            _lockUi(false);
        }

        /// <summary>
        /// Not async. Doesn't block the UI thread, but crashes another way. Will reset lights, but won't lock/unlock the UI.
        /// </summary>
        public void Demo_03()
        {
            _lockUi(true); // No effect
            _reset(); // Will work
            var synchronizationContext = SynchronizationContext.Current;
            ThreadPool.QueueUserWorkItem(_ =>
            {
                for (var i = 0; i < 3; i++)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1)); // Will block the UI thread
                    synchronizationContext?.Post(_ => _lightUp(i), null); // Will NOT light up correctly, and will crash
                }
            });
            _lockUi(false);
        }

        /// <summary>
        /// Not async. Doesn't block the UI thread and doesn't crash. Will reset lights, but won't lock/unlock the UI.
        /// </summary>
        public void Demo_04()
        {
            _lockUi(true); // No effect
            _reset(); // Will work
            var synchronizationContext = SynchronizationContext.Current;
            ThreadPool.QueueUserWorkItem(_ =>
            {
                for (var i = 0; i < 3; i++)
                {
                    var j = i; // Have to watch out for multi-thread access to 'i'
                    Thread.Sleep(TimeSpan.FromSeconds(1)); // Will block the UI thread
                    synchronizationContext?.Post(_ => _lightUp(j), null); // Will light up correctly
                }
            });
            _lockUi(false);
        }

        /// <summary>
        /// Not async. Doesn't block the UI thread and doesn't crash. Will reset lights and will lock/unlock the UI.
        /// </summary>
        public void Demo_05()
        {
            _lockUi(true); // Will work
            _reset(); // Will work
            var synchronizationContext = SynchronizationContext.Current;
            ThreadPool.QueueUserWorkItem(_ =>
            {
                for (var i = 0; i < 3; i++)
                {
                    var j = i; // Have to watch out for multi-thread access to 'i'
                    Thread.Sleep(TimeSpan.FromSeconds(1)); // Will block the UI thread
                    synchronizationContext?.Post(_ => _lightUp(j), null); // Will light up correctly
                }
                synchronizationContext?.Post(_ => _lockUi(false), null);
            });
        }

        /// <summary>
        /// Async implementation. Everything works as expected.
        /// </summary>
        public async Task Demo_06()
        {
            _lockUi(true); // Will work
            _reset(); // Will work
            for (var i = 0; i < 3; i++)
            {
                await Task.Delay(TimeSpan.FromSeconds(1)); // Will NOT block the UI thread
                _lightUp(i); // Will light up correctly
            }
            _lockUi(false);
        }

        /// <summary>
        /// Async implementation. Randomized light timers.
        /// </summary>
        public async Task Demo_07()
        {
            _lockUi(true);
            _reset();
            var tasks = new List<Task>();
            var random = new Random(Guid.NewGuid().GetHashCode());
            for (var i = 0; i < 3; i++)
                tasks.Add(Task.Delay(TimeSpan.FromSeconds(random.NextDouble() * 3.0)));
            var j = 0;
            while (tasks.Count > 0)
            {
                var completed = await Task.WhenAny(tasks);
                tasks.Remove(completed);
                _lightUp(j++);
            }
            _lockUi(false);
        }

        /// <summary>
        /// Async implementation. Randomized light timers again. This will crash for same reason as Demo 03.
        /// </summary>
        public async Task Demo_08()
        {
            _lockUi(true);
            _reset();
            var tasks = new List<Task<int>>();
            var random = new Random(Guid.NewGuid().GetHashCode());
            for (var i = 0; i < 3; i++)
                tasks.Add(Task.Delay(TimeSpan.FromSeconds(random.NextDouble() * 3.0))
                    .ContinueWith(t => i));
            while (tasks.Count > 0)
            {
                var completed = await Task.WhenAny(tasks);
                tasks.Remove(completed);
                _lightUp(completed.Result);
            }
            _lockUi(false);
        }

        /// <summary>
        /// Async implementation. Randomized light timers again. This won't crash.
        /// </summary>
        public async Task Demo_09()
        {
            _lockUi(true);
            _reset();
            var tasks = new List<Task<int>>();
            var random = new Random(Guid.NewGuid().GetHashCode());
            for (var i = 0; i < 3; i++)
            {
                var j = i;
                tasks.Add(
                    Task.Delay(TimeSpan.FromSeconds(random.NextDouble() * 3.0))
                        .ContinueWith(t => j));
            }
            while (tasks.Count > 0)
            {
                var completed = await Task.WhenAny(tasks);
                tasks.Remove(completed);
                _lightUp(completed.Result);
            }
            _lockUi(false);
        }

        /// <summary>
        /// Async implementation. Randomized light timers again. This won't crash.
        /// </summary>
        public async Task Demo_10()
        {
            async Task<int> AnotherAyncMethod(double delay, int i)
            {
                await Task.Delay(TimeSpan.FromSeconds(delay));
                return i;
            }

            _lockUi(true);
            _reset();
            var tasks = new List<Task<int>>();
            var random = new Random(Guid.NewGuid().GetHashCode());
            for (var i = 0; i < 3; i++)
                tasks.Add(AnotherAyncMethod(random.NextDouble() * 3.0, i));
            while (tasks.Count > 0)
            {
                var completed = await Task.WhenAny(tasks);
                tasks.Remove(completed);
                _lightUp(completed.Result);
            }
            _lockUi(false);
        }

        /// <summary>
        /// Semi-async implementation. Will cause a deadlock.
        /// </summary>
        public void Demo_11()
        {
            async Task LibraryAsyncMethod()
            {
                await Task.Delay(TimeSpan.FromSeconds(1.5));
            }

            _lockUi(true);
            _reset();
            LibraryAsyncMethod().GetAwaiter().GetResult();
            for (var i = 0; i < 3; i++)
                _lightUp(i);
            _lockUi(false);
        }

        /// <summary>
        /// Semi-async implementation. Won't cause a deadlock, but still not preferred because it blocks the UI thread and breaks UI locking.
        /// </summary>
        public void Demo_12()
        {
            async Task LibraryAsyncMethod()
            {
                await Task.Delay(TimeSpan.FromSeconds(1.5)).ConfigureAwait(false);
            }

            _lockUi(true);
            _reset();
            LibraryAsyncMethod().GetAwaiter().GetResult();
            for (var i = 0; i < 3; i++)
                _lightUp(i);
            _lockUi(false);
        }

        /// <summary>
        /// Async implementation with background work in threadpool.
        /// </summary>
        public async Task Demo_13()
        {
            _lockUi(true);
            _reset();
            for (var i = 0; i < 3; i++)
            {
                await Task.Run(() => Thread.Sleep(TimeSpan.FromSeconds(1)));
                _lightUp(i);
            }
            _lockUi(false);
        }

        /// <summary>
        /// Async implementation with async background work in threadpool.
        /// </summary>
        public async Task Demo_14()
        {
            _lockUi(true);
            _reset();
            for (var i = 0; i < 3; i++)
            {
                await Task.Run(async () => await Task.Delay(TimeSpan.FromSeconds(1)));
                _lightUp(i);
            }
            _lockUi(false);
        }

        /// <summary>
        /// Async void exception, causes crash.
        /// </summary>
        public void Demo_15()
        {
            async void LibraryAsyncMethod()
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                throw new Exception("Goodbye");
            }

            _lockUi(true);
            _reset();
            Thread.Sleep(TimeSpan.FromSeconds(1));
            try
            {
                LibraryAsyncMethod();
            }
            catch (Exception)
            {
                _lightUp(2);
            }
            for (var i = 0; i < 2; i++)
                _lightUp(i);
            _lockUi(false);
        }

        /// <summary>
        /// Async exception, doesn't cause crash.
        /// </summary>
        public void Demo_16()
        {
            async Task LibraryAsyncMethod()
            {
                await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
                throw new Exception("Goodbye");
            }

            _lockUi(true);
            _reset();
            Thread.Sleep(TimeSpan.FromSeconds(1));
            try
            {
                LibraryAsyncMethod().GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                _lightUp(2);
            }
            for (var i = 0; i < 2; i++)
                _lightUp(i);
            _lockUi(false);
        }

        /// <summary>
        /// Async exception, doesn't cause crash.
        /// </summary>
        public async Task Demo_17()
        {
            async Task LibraryAsyncMethod()
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                throw new Exception("Goodbye");
            }

            _lockUi(true);
            _reset();
            await Task.Delay(TimeSpan.FromSeconds(1));
            try
            {
                await LibraryAsyncMethod();
            }
            catch (Exception)
            { }
            for (var i = 0; i < 3; i++)
                _lightUp(i);
            _lockUi(false);
        }

        /// <summary>
        /// Async void exception, caught within, won't crash..
        /// </summary>
        public void Demo_18()
        {
            async void LibraryAsyncMethod()
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    throw new Exception("Goodbye");
                }
                catch (Exception)
                {
                    _lightUp(2);
                }
            }

            _lockUi(true);
            _reset();
            Thread.Sleep(TimeSpan.FromSeconds(1));
            try
            {
                LibraryAsyncMethod();
            }
            catch (Exception)
            {
                _lightUp(1);
            }
            _lightUp(0);
            _lockUi(false);
        }
    }
}
