using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Scalizer
{
    /// <summary>
    /// Loads the profile and automatically sets up the startup behaviour,
    /// which is to match the current display name with the DPI value recorded on the saved JSON objects.
    /// Then, the program automatically scales the desktop UI to improve Windows 10/11 user experience on high res. displays,
    /// similar to those found on macOS.
    /// 
    /// SCALIZER: ONE AND ONLY CUSTOM SCALING SOFTWARE FOR WINDOWS 10 AND 11
    /// An Open-source Project by John Seong. Served under the Apache License.
    /// </summary>

    public partial class MainWindow : Window
    {
        private enum Startup_Type
        {
            Enable,
            Disable,
            Get
        }

        private List<string> profileNames = new List<string>();

        private List<DisplayConfig> displayProfileList = new List<DisplayConfig>();

        private List<string> jsonPaths;

        public MainWindow()
        {
            InitializeComponent();

            String msg = Set_Startup(Startup_Type.Get);

            isOpenStartUp.IsChecked = msg.Contains("not");

            // Retrieve all the JSON files present in the root EXE file directory...
            jsonPaths = Directory.EnumerateFiles(@".\", "*", SearchOption.AllDirectories)
               .Where(s => s.EndsWith(".json") && s.Count(c => c == '.') == 2)
               .ToList();

            // Only if jsonPaths list is NOT empty...
            if (jsonPaths.Any())
            {
                for (int i = 0; i < jsonPaths.Count; i++)
                {
                    string[] parsedFileName = jsonPaths[i].Replace(@".\", @"").Split("@");
                    string currentProfileName = parsedFileName[0];

                    // If the config. with same profile doesn't exist in the profileNames array...
                    if (!profileNames.Any(x => x == currentProfileName))
                    {
                        profileNames.Add(currentProfileName);
                    }
                }

                selectedProfile.ItemsSource = profileNames;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            tinkerStartUpSettings(Startup_Type.Enable);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            tinkerStartUpSettings(Startup_Type.Disable);
        }

        private async void tinkerStartUpSettings(Startup_Type startup_Type)
        {
            statusText.Content = Set_Startup(startup_Type);

            Dynamically_Set_Label_Dimensions();

            await Task.Delay(TimeSpan.FromSeconds(3));

            statusText.Content = "";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button? b = sender as Button;

            Change_Click(sender, e, b!.Name);
        }
         
        // Adds the automatic launch on Windows startup command on the registry...
        private string Set_Startup(Startup_Type startup_Type)
        {
            try
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true)!;

                Assembly curAssembly = Assembly.GetExecutingAssembly();

                switch (startup_Type) {
                    case Startup_Type.Enable:
                        key!.SetValue(curAssembly.GetName().Name, curAssembly.Location);

                        return "Successfuly enabled!";

                    case Startup_Type.Disable:
                        key!.DeleteValue(curAssembly.GetName().Name!);

                        return "Successfully disabled!";

                    case Startup_Type.Get:
                        bool keyExists = key.GetValue(curAssembly.GetName().Name) == null;

                        if (keyExists) { return "Key found!"; } else { return "Key not found!"; }

                    default:
                        return "Specify a startup behaviour...";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private void Change_Click(object sender, RoutedEventArgs e, string buttonBehaviour)
        {
            CustomWindow customWindow = new CustomWindow(buttonBehaviour);

            if (buttonBehaviour == "editButton")
            {
                customWindow.setSelectedProfile(profileNames[selectedProfile.SelectedIndex]);
                customWindow.setJsonPaths(jsonPaths);
            }

            Visibility = Visibility.Hidden;

            customWindow.Show();
        }

        private void Dynamically_Set_Label_Dimensions()
        {
            double[] dimensions = Measure_String(statusText.ContentStringFormat);

            statusText.Width = dimensions[0];
            statusText.Height = dimensions[1];
        }

        private double[] Measure_String(string candidate)
        {
            if (candidate == null) return new double[] { 130, 50 };

            var formattedText = new FormattedText(
                candidate,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(this.statusText.FontFamily, this.statusText.FontStyle, this.statusText.FontWeight, this.statusText.FontStretch),
                this.statusText.FontSize,
                Brushes.Black,
                new NumberSubstitution(),
                1);

            return new double[] { formattedText.Width, formattedText.Height };
        }

        private void Activate_Selected_Profile(object sender, RoutedEventArgs e)
        {

        }
    }
}
