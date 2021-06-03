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
        private readonly AsyncMethods _asyncMethods;

        public MainWindow()
        {
            _asyncMethods = new AsyncMethods(LockUi, Reset, LightUp);
            InitializeComponent();
        }

        private void LightUp(int light) =>
            (light switch
            {
                0 => Light1,
                1 => Light2,
                2 => Light3,
                _ => throw new ArgumentOutOfRangeException(nameof(light), light, "Allowed values are 0 through 2")
            }).Fill = Brushes.Red;

        private void LockUi(bool isLocked) =>
            MainGrid.IsEnabled = !isLocked;

        private void Reset()
        {
            Light1.Fill = Brushes.Transparent;
            Light2.Fill = Brushes.Transparent;
            Light3.Fill = Brushes.Transparent;
        }

        private void Demo_01_Click(object sender, RoutedEventArgs e) =>
            _asyncMethods.Demo_01();

        private void Demo_02_Click(object sender, RoutedEventArgs e) =>
             _asyncMethods.Demo_02();

        private void Demo_03_Click(object sender, RoutedEventArgs e) =>
            _asyncMethods.Demo_03();

        private void Demo_04_Click(object sender, RoutedEventArgs e) =>
            _asyncMethods.Demo_04();

        private void Demo_05_Click(object sender, RoutedEventArgs e) =>
            _asyncMethods.Demo_05();

        private async void Demo_06_Click(object sender, RoutedEventArgs e) =>
            await _asyncMethods.Demo_06();

        private async void Demo_07_Click(object sender, RoutedEventArgs e) =>
            await _asyncMethods.Demo_07();

        private async void Demo_08_Click(object sender, RoutedEventArgs e) =>
            await _asyncMethods.Demo_08();

        private async void Demo_09_Click(object sender, RoutedEventArgs e) =>
            await _asyncMethods.Demo_09();

        private async void Demo_10_Click(object sender, RoutedEventArgs e) =>
            await _asyncMethods.Demo_10();

        private void Demo_11_Click(object sender, RoutedEventArgs e) =>
            _asyncMethods.Demo_11();

        private void Demo_12_Click(object sender, RoutedEventArgs e) =>
            _asyncMethods.Demo_12();

        private async void Demo_13_Click(object sender, RoutedEventArgs e) =>
            await _asyncMethods.Demo_13();

        private async void Demo_14_Click(object sender, RoutedEventArgs e) =>
            await _asyncMethods.Demo_14();

        private void Demo_15_Click(object sender, RoutedEventArgs e) =>
            _asyncMethods.Demo_15();

        private void Demo_16_Click(object sender, RoutedEventArgs e) =>
            _asyncMethods.Demo_16();

        private async void Demo_17_Click(object sender, RoutedEventArgs e) =>
            await _asyncMethods.Demo_17();

        private void Demo_18_Click(object sender, RoutedEventArgs e) =>
            _asyncMethods.Demo_18();
    }
}
