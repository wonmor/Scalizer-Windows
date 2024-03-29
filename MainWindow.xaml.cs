﻿using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowsDisplayAPI.DisplayConfig;

/*
 * The Scalizer Project for Windows 11.
 * Developed and Designed by John Seong.
 * Served under the Apache 2.0 License.
 * 
 * UPCOMING MILESTONES
 * 1. DETECT DISPLAY CHANGE THEN SCALE CHANGE OR RUN IT PERIODICALLY, IDEALLY EVERY HOUR... (RUN IN CYCLES)
 * 2. MAKE A COMMUNITY WEBSITE WITH RESTful API FOR ALL CUSTOM DISPLAY SCALING DATA... (A CENTRAL SQL DATABASE TO STORE ALL THOSE INFO.)
 * 3. FIGURE OUT HOW TO PACKAGE THE APP & FIX THE "LAUNCH ON STARTUP" PROBLEM...
 * 4. UPON START, CHECK IF WINDOWS SETTINGS -> CUSTOM SCALING IS ON, AND IF NOT, OPEN A NEW WINDOWS THAT BASICALLY ASKS THE USER TO TURN IT ON...
 * 
 * URGENT CHANGES NEED TO BE MADE
 * 1. REMOVE THE TEMP. LINE -> REPLACE IT WITH A METHOD THAT ONLY SHUTS DOWN THE DISPLAY NOTIFICATION SYSTEM WHEN THE APP IS COMPLETELY CLOSED, NOT JUST WHEN MERELY THE WINDOW IS CLOSED...
 * 2. BUG FIX NEED TO BE MADE -> FIX THE CYCLE ERROR WHEN YOU MAKE A NEW PROFILE ON 2-DISPLAY SETUP WITH ONLY ONE DISPLAY PROFILE ACTIVE... (DUE TO ONLY DISPLAY ON CERTAIN MONITOR OPTION ON WINDOWS SETTINGS)
 * 3. FIX THE ISSUE WHEN AFTER CREATING NEW PROFILE, THE COMBOBOX SELECTED ITEM DOES NOT AUTOMATICALLY SELECT THE NEW ITEM...
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

        private List<string> jsonPaths = new List<string>();

        private bool isExecute = false;
        private bool isAlwaysReactive = true;

        private int? displayNumber, selectedProfileIndex;
        private string? displayScaling;

        private ConfigManager? config = new ConfigManager();

        // Get current directory...
        private readonly DirectoryInfo di = new DirectoryInfo(Directory.GetCurrentDirectory());

        public MainWindow()
        {
            InitializeComponent();

            String msg = Set_Startup(Startup_Type.Get);

            isOpenStartUp.IsChecked = msg.Contains("not");

            // Retrieve all the JSON files present in the root EXE file directory...
            jsonPaths = Directory.EnumerateFiles(di.FullName, "*", SearchOption.AllDirectories)
              .Where(s => s.EndsWith(".json") && s.Count(c => c == '.') == 2)
              .ToList();

            // Only if jsonPaths list is NOT empty...
            if (jsonPaths!.Any())
            {
                editButton.Visibility = Visibility.Visible;

                for (int i = 0; i < jsonPaths!.Count; i++)
                {
                    string[] parsedFileName;

                    // Clean the profile name...
                    parsedFileName = jsonPaths[i].Replace(di.FullName + @"\", @"").Replace(@"resources\assets\sources\Scalizer-Alpha\net6.0-windows\", "").Split("@");

                    string currentProfileName = parsedFileName[0];

                    // If the config. with same profile doesn't exist in the profileNames array...
                    if (!profileNames.Any(x => x == currentProfileName))
                    {
                        profileNames.Add(currentProfileName);
                    }
                }

                selectedProfile.ItemsSource = profileNames;

                Parse_Current_Profile();

                try
                {
                    selectedProfile.SelectedIndex = (int) selectedProfileIndex!;

                }
                catch (Exception)
                {
                    // Runs if the saved profile selection isn't in "what's available" bounds...
                    selectedProfile.SelectedIndex = 0;

                    config!.UpdateProperty("selectedProfileIndex", "0");
                }

            } else
            {
                editButton.Visibility = Visibility.Hidden;
            }

            // When display resolution or orientation setting has changed...
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged!;

            // Temporary fix for closing the display new connection observation process...
            this.Closed += MainWindow_Closed!;

            if (isEnabled.IsChecked == true)
            {
                Pause_SystemEvents_Observer();
                Scale_Display();
            }
        }

        private bool handle = false;

        private void ComboBox_DropDownOpen(object sender, EventArgs e)
        {
            ComboBox? cmb = sender as ComboBox;

            if (handle) Handle(cmb!);
            handle = true;

            Parse_Current_Profile();
            Set_Up_Startup_Behaviour();

            cmb!.SelectedIndex = (int)selectedProfileIndex!;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox? cmb = sender as ComboBox;
            handle = !cmb!.IsDropDownOpen;

            Handle(cmb!);
        }

        private void Handle(ComboBox comboBox)
        {
            // Save the current selected profile's index on combobox selection change...
            config!.UpdateProperty("selectedProfileIndex", comboBox!.SelectedIndex.ToString());

            Parse_Current_Profile();
            Set_Up_Startup_Behaviour();
        }

        // Parse the JSON file and run a terminal command accordingly...
        private void Parse_Current_Profile()
        {
            // Load from the saved settings...
            NameValueCollection? configDict = config!.ReadAppSettingsProperty();

            if (configDict != null)
            {
                isExecute = bool.Parse(configDict["isExecute"]!);
                selectedProfileIndex = int.Parse(configDict["selectedProfileIndex"]!);
            }
        }

        private void Set_Up_Startup_Behaviour()
        {
            // Startup behaviour if and only if it is set to true...
            if (isExecute == true)
            {
                isEnabled.IsChecked = true;

                Pause_SystemEvents_Observer();
                Scale_Display();
            }
            else
            {
                isEnabled.IsChecked = false;
            }
        }

        // Runs a terminal command that scales the display based upon user settings...
        private void Scale_Display([CallerMemberName] string callerName = "")
        {
            DisplayConfig currentDisplayConfig;
            string? currentProfile;

            try
            {
                currentProfile = selectedProfile.SelectedValue.ToString();
            }

            catch (NullReferenceException)
            {
                currentProfile = string.Empty;
            }

            // List<string> displays = Retrieve_Display_Info();
            Trace.WriteLine(callerName + " calling the Scale_Display() function.");

            for (int i = 0; i < jsonPaths.Count; i++)
            {
                if (jsonPaths[i].Contains(currentProfile!))
                {
                    currentDisplayConfig = Parse_Json_File(jsonPaths[i]);

                    // Set the instance variables to what parsed JSON objects are pointing at...
                    displayNumber = currentDisplayConfig.displayIndex;
                    displayScaling = currentDisplayConfig.dpiSetting;

                    // Execute a Terminal command...
                    TerminalHelper.execute(@"cd " + di.FullName + " && " + @".\Assets\SetDpi.exe" + " " + displayNumber + " " + displayScaling);
                }

                // if (displays.Count == 1) break;
            }
        }

        protected static List<string> Retrieve_Display_Info()
        {
            List<string> displays = new List<string>();

            foreach (PathInfo pi in PathInfo.GetActivePaths())
            {
                if (!pi.TargetsInfo[0].DisplayTarget.IsAvailable) continue;

                string currentValue = String.Format("{0}",
                        string.IsNullOrEmpty(pi.TargetsInfo[0].DisplayTarget.FriendlyName) ? "Generic PnP Monitor" : pi.TargetsInfo[0].DisplayTarget.FriendlyName);

                displays.Add(currentValue);
            }

            return displays;
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
            Set_Startup(startup_Type);

            await Task.Delay(TimeSpan.FromSeconds(3));
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

        private void Activate_Selected_Profile(object sender, RoutedEventArgs e)
        {
            config!.UpdateProperty("isExecute", "true");

            Pause_SystemEvents_Observer();
            Scale_Display();
        }

        private void Deactivate_Selected_Profile(object sender, RoutedEventArgs e)
        {
            config!.UpdateProperty("isExecute", "false");
        }

        private static void MainWindow_Closed(object sender, EventArgs e)
        {
            DeviceNotification.UnRegisterUsbDeviceNotification();
            DeviceNotification.UnRegisterMonitorDeviceNotification();
        }


        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            if (!(PresentationSource.FromVisual(this) is HwndSource source))
                return;

            source.AddHook(this.WndProc);

            DeviceNotification.RegisterUsbDeviceNotification(source.Handle);
            DeviceNotification.RegisterMonitorDeviceNotification(source.Handle);
        }

        private IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg != DeviceNotification.WmDeviceChange)
                return IntPtr.Zero;

            Debug.WriteLine("WM_DEVICE_CHANGE TRIGGERED");

            switch ((int)wParam)
            {
                case DeviceNotification.DbtDeviceRemoveComplete:
                case DeviceNotification.DbtDeviceArrival:
                    {
                        if (DeviceNotification.IsMonitor(lParam))
                        {
                            Debug.WriteLine($"Monitor {((int)wParam == DeviceNotification.DbtDeviceArrival ? "arrived" : "removed")}");

                            if (isEnabled.IsChecked == true)
                            {
                                Pause_SystemEvents_Observer();
                                Scale_Display();
                            }
                        }

                        if (DeviceNotification.IsUsbDevice(lParam))
                            Debug.WriteLine($"Usb device {((int)wParam == DeviceNotification.DbtDeviceArrival ? "arrived" : "removed")}");
                    }
                    break;
            }

            return IntPtr.Zero;
        }

        private void Pause_SystemEvents_Observer()
        {
            isAlwaysReactive = false;

            // Asyncronously set the isAlwaysReactive variable back to true after ten seconds...
            Task.Delay(new TimeSpan(0, 0, 10)).ContinueWith(o => { isAlwaysReactive = true; });
        }

        // TO DO: The code below causes infinite loop to occur. Fix it!
        public void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            // if (isAlwaysReactive)
            // {
            //     Scale_Display();
            // }

        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            App.Current.Shutdown();
        }
    }
}
