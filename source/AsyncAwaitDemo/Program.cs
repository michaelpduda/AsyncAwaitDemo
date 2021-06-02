/* This file is part of the Async/Await Demo Project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/AsyncAwaitDemo/blob/master/LICENSE.md
 */
using System;
using System.Windows;

namespace AsyncAwaitDemo
{
    public class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            var app = new Application();
            app.Startup += (sender, e) => new MainWindow().Show();
            app.Run();
        }
    }
}
