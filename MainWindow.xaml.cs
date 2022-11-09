using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
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

/*
 * The Scalizer Project for Windows 11.
 * Developed and Designed by John Seong.
 * 
 * UPCOMING MILESTONES
 * 1. DETECT DISPLAY CHANGE THEN SCALE CHANGE OR RUN IT PERIODICALLY, IDEALLY EVERY HOUR... (RUN IN CYCLES)
 * 2. MAKE A COMMUNITY WEBSITE WITH RESTful API FOR ALL CUSTOM DISPLAY SCALING DATA... (A CENTRAL SQL DATABASE TO STORE ALL THOSE INFO.)
 * 
 * BUG FIXES NEED TO BE MADE
 * 1. SAVE DROPDOWN BAR CURRENT PROFILE NAME LOCALLY (ONLY RUN IF THERE'S DETECTED JSON FILE IN THE APP FOLDER)
 */

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

        private List<string> jsonPaths;

        private bool isExecute = false;

        private int? displayNumber, selectedProfileIndex;
        private string? displayScaling;

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
                editButton.Visibility = Visibility.Visible;

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

                try
                {
                    selectedProfile.SelectedIndex = (int)selectedProfileIndex!;

                }
                catch (IndexOutOfRangeException)
                {
                    // Runs if the saved profile selection isn't in "what's available" bounds...
                    selectedProfile.SelectedIndex = 0;

                    Update_Config("selectedProfileIndex", 0);
                }

                Parse_Current_Profile();

            } else
            {
                editButton.Visibility = Visibility.Hidden;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox? comboBox = sender as ComboBox;

            // Save the current selected profile's index on combobox selection change...
            Update_Config("selectedProfileIndex", comboBox!.SelectedIndex);
            Parse_Current_Profile();
        }

        // Parse the JSON file and run a terminal command accordingly...
        private void Parse_Current_Profile()
        {
            // Load from the saved settings...
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);


            Trace.WriteLine(config.AppSettings.Settings["isEnabled"].Value);

            isExecute = bool.Parse(config.AppSettings.Settings["isEnabled"].Value);
            selectedProfileIndex = int.Parse(config.AppSettings.Settings["selectedProfileIndex"].Value);

            // Startup behaviour if and only if it is set to true...
            if (isExecute == true)
            {
                isEnabled.IsChecked = true;

                Scale_Display();
            }
            else
            {
                isEnabled.IsChecked = false;
            }
        }

        // Runs a terminal command that scales the display based upon user settings...
        private void Scale_Display()
        {
            DisplayConfig currentDisplayConfig;

            string? currentProfile = selectedProfile.SelectedValue.ToString();

            for (int i = 0; i < jsonPaths.Count; i++)
            {
                if (jsonPaths[i].Contains(currentProfile!))
                {
                    currentDisplayConfig = Parse_Json_File(jsonPaths[i]);

                    // Set the instance variables to what parsed JSON objects are pointing at...
                    displayNumber = currentDisplayConfig.displayIndex;
                    displayScaling = currentDisplayConfig.dpiSetting;

                    // Execute a Terminal command...
                    TerminalHelper.execute(@".\Assets\SetDpi.exe" + " " + displayNumber + " " + displayScaling);
                }
            }
        }

        private DisplayConfig Parse_Json_File(string path)
        {
            JObject jo = JObject.Parse(File.ReadAllText(path));

            return JsonConvert.DeserializeObject<DisplayConfig>(jo.ToString())!;
        }

        private void LaunchOnStartUp_Checked(object sender, RoutedEventArgs e)
        {
            Tinker_Startup_Settings(Startup_Type.Enable);
        }

        private void LaunchOnStartUp_Unchecked(object sender, RoutedEventArgs e)
        {
            Tinker_Startup_Settings(Startup_Type.Disable);
        }

        private async void Tinker_Startup_Settings(Startup_Type startup_Type)
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
            Update_Config("isEnabled", true);
            Scale_Display();
        }

        private void Deactivate_Selected_Profile(object sender, RoutedEventArgs e)
        {
            Update_Config("isEnabled", false);
        }

        private void Update_Config(string key, object value)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["isEnabled"].Value = value.ToString();
            config.Save(ConfigurationSaveMode.Modified);

            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
