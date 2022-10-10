using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
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
using System.Xml.Linq;
using WindowsDisplayAPI.DisplayConfig;

namespace Scalizer
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class CustomWindow : Window
    {
        public CustomWindow()
        {
            InitializeComponent();

            List<string> displayInfoList = Retrieve_Display_Info();

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

        public static List<string> Retrieve_Display_Info()
        {
            List<string> strings = new List<string>();

            foreach (PathInfo pi in PathInfo.GetActivePaths())
            {
                if (!pi.TargetsInfo[0].DisplayTarget.IsAvailable) continue;

                string currentValue = String.Format("{0}",
                        string.IsNullOrEmpty(pi.TargetsInfo[0].DisplayTarget.FriendlyName) ? "Generic PnP Monitor" : pi.TargetsInfo[0].DisplayTarget.FriendlyName
                        // , pi.DisplaySource.DisplayName,
                        // pi.TargetsInfo[0].DisplayTarget.DevicePath
                        );

                strings.Add(currentValue);
            }

            return strings;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
