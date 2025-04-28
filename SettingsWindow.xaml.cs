using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace hacker_typer_simulator
{
    public partial class SettingsWindow : Window
    {
        public string Username { get; private set; } = string.Empty;

        public SettingsWindow(string currentUsername)
        {
            InitializeComponent();
            UsernameTextBox.Text = currentUsername;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Username = UsernameTextBox.Text;
            DialogResult = true;
            Close();
        }
    }
}
