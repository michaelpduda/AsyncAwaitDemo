/* This file is part of the Async/Await Demo Project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/AsyncAwaitDemo/blob/master/LICENSE.md
 */
using System;
using System.Windows;
using System.Windows.Media;

namespace AsyncAwaitDemo
{
    public partial class MainWindow : Window
    {
        public MainWindow() =>
            InitializeComponent();

        private void LightUp(int light) =>
            (light switch
            {
                0 => Light1,
                1 => Light2,
                2 => Light3,
                _ => throw new ArgumentOutOfRangeException(nameof(light), light, "Allowed values are 0 through 2")
            }).Fill = Brushes.Red;

        private void Reset()
        {
            Light1.Fill = Brushes.Transparent;
            Light2.Fill = Brushes.Transparent;
            Light3.Fill = Brushes.Transparent;
        }
    }
}
