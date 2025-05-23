﻿using hacker_typer_simulator;
using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace hacker_typer_simulator
{
    public partial class MainWindow : Window
    {
        private string simulationText;
        private int currentIndex = 0;
        private readonly DispatcherTimer cursorTimer;
        private string username = "tappat0xE1"; // Default username
        private bool isFullScreen = false;

        public MainWindow()
        {
            InitializeComponent();
            TerminalTextBlock.Text = "";
            //this.AllowsTransparency = false;

            SetDefalutWindowStyle();

            // Remove the window's border and make it non-resizable
            //this.WindowStyle = WindowStyle.None;
            ////this.ResizeMode = ResizeMode.NoResize;
            //this.ResizeMode = ResizeMode.CanResizeWithGrip;
            //this.Background = Brushes.Black;

            // Read command from a file or use default commands
            string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "simulationContent.txt");
            if (File.Exists(filePath))
            {
                simulationText = File.ReadAllText(filePath);
            }
            else
            {
                simulationText =
                    "nmap -A 192.168.1.1\n" +
                    "dig example.com\n" +
                    "whoami\n" +
                    "cat /etc/passwd\n";
            }

            // Initialize and start the cursor blinking Timer, toggling the cursor visibility every 500 milliseconds
            cursorTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            cursorTimer.Tick += CursorTimer_Tick;
            cursorTimer.Start();
        }

        private void SetDefalutWindowStyle()
        {
            this.WindowStyle = WindowStyle.None;
            this.ResizeMode = ResizeMode.CanResize;
            this.Background = Brushes.Black;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow(username);
            if (settingsWindow.ShowDialog() == true)
            {
                username = settingsWindow.Username;
                MessageBox.Show($"UserName has been updated to：{username}", "Settings saved successfully", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // Toggles the visibility of the cursor every 500 milliseconds
        private void CursorTimer_Tick(object? sender, EventArgs e)
        {
            BlinkCursor.Visibility =
                BlinkCursor.Visibility == Visibility.Visible
                ? Visibility.Hidden
                : Visibility.Visible;
        }

        // Captures the KeyDown event to append the next character from the simulation text
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && isFullScreen)
            {
                this.WindowState = WindowState.Normal;
                //this.WindowStyle = WindowStyle.None;
                //this.ResizeMode = ResizeMode.CanResizeWithGrip;
                SetDefalutWindowStyle();
                isFullScreen = false;

                // Make suire that the window is in normal state
                this.Activate();

                Keyboard.Focus(TerminalTextBlock);

                e.Handled = true;
                return;
            }

            AppendNextCharacter();
            e.Handled = true;
        }

        private void UpdateCursorPosition()
        {
            // Calculate the new position of the cursor based on the text length
            FormattedText formattedText = new FormattedText(
            TerminalTextBlock.Text,
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface(TerminalTextBlock.FontFamily, TerminalTextBlock.FontStyle, TerminalTextBlock.FontWeight, TerminalTextBlock.FontStretch),
            TerminalTextBlock.FontSize,
            TerminalTextBlock.Foreground,
            VisualTreeHelper.GetDpi(this).PixelsPerDip);

            // Calculate the new cursor position based on the text length
            double offset = formattedText.Width;
            BlinkCursor.Margin = new Thickness(TerminalTextBlock.Margin.Left + offset,
                                                 TerminalTextBlock.Margin.Top, 0, 0);
        }

        private void SimulateNetworkEffect(string command)
        {
            TerminalTextBlock.Text += "\n[Network transferring...]";
        }

        private void AppendNextCharacter()
        {
            if (currentIndex < simulationText.Length)
            {
                if (currentIndex == 0 || simulationText[currentIndex - 1] == '\n')
                {
                    TerminalTextBlock.Text += $"{username}@linux:~$ ";
                }

                char currentChar = simulationText[currentIndex];
                TerminalTextBlock.Text += currentChar;
                currentIndex++;

                if (currentChar == '\n' || currentIndex == simulationText.Length)
                {
                    string[] lines = TerminalTextBlock.Text.Split('\n');
                    string lastLine = lines[^1].Trim();

                    // Handle special commands
                    if (lastLine.StartsWith("#"))
                    {
                        SimulateNetworkEffect(lastLine.TrimStart('#'));
                    }
                }

                UpdateCursorPosition();
            }
        }

        private void FullScreenButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isFullScreen)
            {
                this.WindowState = WindowState.Maximized;
                //this.WindowStyle = WindowStyle.None;
                //this.ResizeMode = ResizeMode.CanResizeWithGrip;
                SetDefalutWindowStyle();
                isFullScreen = true;
            }
        }

    }
}