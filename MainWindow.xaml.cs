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

        public MainWindow()
        {
            InitializeComponent();
            TerminalTextBlock.Text = "";

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

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Open settings", "Settings", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

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
            AppendNextCharacter();
            e.Handled = true;
        }

        private void AppendNextCharacter()
        {
            if (currentIndex < simulationText.Length)
            {
                TerminalTextBlock.Text += simulationText[currentIndex];
                currentIndex++;

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
        }
    }
}