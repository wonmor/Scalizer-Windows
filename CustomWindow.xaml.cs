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
            SelectQuery q = new SelectQuery("SELECT Name, DeviceID, Description FROM Win32_DesktopMonitor");

            List<string> strings = new List<string>();

            using (ManagementObjectSearcher mos = new ManagementObjectSearcher(q))
            {
                foreach (ManagementObject mo in mos.Get())
                {
                    string currentValue = String.Format("{0}, {1}, {2}",
                        mo.Properties["Name"].Value.ToString(),
                        mo.Properties["DeviceID"].Value.ToString(),
                        mo.Properties["Description"].Value.ToString());

                    strings.Add(currentValue);
                }
            }

            return strings;
        }
    }
}
