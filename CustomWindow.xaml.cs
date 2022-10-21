using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

using WindowsDisplayAPI.DisplayConfig;
using System.Configuration;
using Newtonsoft.Json;

namespace Scalizer
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    
    internal class DisplayConfig
    {
        public int? displayIndex { get; set; }
        public String? displayName { get; set; }
        public String? dpiSetting { get; set; }
    }

    static class Extensions
    {
        public static List<T> Clone<T>(this List<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }
    }

    public partial class CustomWindow : Window
    {
        private List<string>? displayInfoList;

        public CustomWindow()
        {
            InitializeComponent();

            Retrieve_Display_Info();

            monitorName.ItemsSource = displayInfoList;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button? b = sender as Button;

            if (b!.Name == "backButton") Change_Window(sender, e);
        }
        private void Change_Window(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            Visibility = Visibility.Hidden;

            mainWindow.Show();
        }

        public void Retrieve_Display_Info()
        {
            List<string> displays = new List<string>();

            foreach (PathInfo pi in PathInfo.GetActivePaths())
            {
                if (!pi.TargetsInfo[0].DisplayTarget.IsAvailable) continue;

                string currentValue = String.Format("{0}",
                        string.IsNullOrEmpty(pi.TargetsInfo[0].DisplayTarget.FriendlyName) ? "Generic PnP Monitor" : pi.TargetsInfo[0].DisplayTarget.FriendlyName);

                displays.Add(currentValue);
            }
            
            displayInfoList = displays.Clone();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox? textBox = sender as TextBox;

            DisplayConfig displayConfig = new DisplayConfig
            {
                displayIndex = displayInfoList?.IndexOf(monitorName.Text.Trim()) + 1,
                displayName = monitorName.Text.Trim(),
                dpiSetting = textBox?.Text.Trim()
            };

            if (profileName.Text.Trim() == "") return;

            File.WriteAllText(String.Format(@"{0}@{1}.json", profileName.Text.Trim().Replace(" ", "_"), monitorName.Text), JsonConvert.SerializeObject(displayConfig));
        }

        // Only allows numbers to be entered in the DPI textbox...
        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if ((e.Text) == null || !(e.Text).All(char.IsDigit))
            {
                e.Handled = true;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }


    }
}
