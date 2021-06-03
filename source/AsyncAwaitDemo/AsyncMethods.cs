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
    }
}
