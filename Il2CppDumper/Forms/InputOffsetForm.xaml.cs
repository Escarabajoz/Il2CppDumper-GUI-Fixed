using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Il2CppDumper.Forms
{
    /// <summary>
    /// Interaction logic for InputOffsetWindow.xaml
    /// </summary>
    public partial class InputOffsetForm : Window
    {
        public string ReturnedOffset { get; set; }

        public InputOffsetForm()
        {
            InitializeComponent();
        }

        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string offsetText = dumpAdrTxtBox.Text?.Trim();
                if (string.IsNullOrEmpty(offsetText))
                {
                    System.Windows.MessageBox.Show("Please enter a valid hexadecimal address.", "Invalid Input", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validate hexadecimal format
                if (!IsHexadecimal(offsetText))
                {
                    System.Windows.MessageBox.Show("Please enter a valid hexadecimal address (0-9, A-F).", "Invalid Input", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Test conversion to ensure it's valid
                try
                {
                    Convert.ToUInt64(offsetText, 16);
                }
                catch (OverflowException)
                {
                    System.Windows.MessageBox.Show("Address value is too large.", "Invalid Input", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                catch (FormatException)
                {
                    System.Windows.MessageBox.Show("Invalid hexadecimal format.", "Invalid Input", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                ReturnedOffset = offsetText;
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error processing input: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static readonly Regex HexadecimalRegex = new Regex("^[0-9A-Fa-f]+$");

        private void HexTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Validate the input character
            e.Handled = !IsHexadecimal(e.Text);
        }

        private void HexTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                var text = (string)e.DataObject.GetData(typeof(string));
                if (!IsHexadecimal(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private static bool IsHexadecimal(string input)
        {
            // Check if the input string is a valid hexadecimal
            return HexadecimalRegex.IsMatch(input);
        }
    }
}
