/* This file is part of the Async/Await Demo Project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/AsyncAwaitDemo/blob/master/LICENSE.md
 */
using System;

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
    }
}
