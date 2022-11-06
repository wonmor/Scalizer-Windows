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
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Scalizer
{
    /// <summary>
    /// This is a business logic that retrieves the display information,
    /// parses it, and stores them in the JSON objects.
    /// 
    /// SCALIZER: ONE AND ONLY CUSTOM SCALING SOFTWARE FOR WINDOWS 10 AND 11
    /// An Open-source Project by John Seong. Served under the Apache License.
    /// </summary>
    
    internal class DisplayConfig
    {
        public string? profileName { get; set; }
        public int? displayIndex { get; set; }
        public string? displayName { get; set; }
        public string? dpiSetting { get; set; }
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
        private List<string>? displayInfoList, jsonPaths;

        private List<string> filePaths = new List<string>();

        private List<string> relevantJsonPaths = new List<string>();
        private List<string> relevantDisplayNames = new List<string>();

        private string? buttonBehaviour, selectedProfileName;

        public CustomWindow(string buttonBehaviour)
        {
            InitializeComponent();

            Retrieve_Display_Info();

            profileName.IsReadOnly = false;
            monitorName.ItemsSource = displayInfoList;

            this.buttonBehaviour = buttonBehaviour;

            if (buttonBehaviour == "editButton")
            {
                backButton.Content = "Back";

            } else if (buttonBehaviour == "customButton")
            {
                backButton.Content = "Delete";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button? b = sender as Button;

            if (b!.Name == "backButton") {
                if (buttonBehaviour == "customButton")
                {
                    Save_Display_Config();
                    Delete_Display_Config();
                }

                Change_Window(sender, e);

            } else if (b!.Name == "saveButton") {
                // If the entered dpiValue is divisible by 5 and is between 80 and 300...
                int dpi = int.Parse(dpiValue.Text);

                if ((dpi % 5 == 0) && (dpi >= 80) && (dpi <= 300))
                {
                    Save_Display_Config();
                    Change_Window(sender, e);
                }
            }
        }
        private void Change_Window(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            Visibility = Visibility.Hidden;

            mainWindow.Show();
        }

        public void setSelectedProfile(string selectedProfileName)
        {
            this.selectedProfileName = selectedProfileName;

            profileName.Text = selectedProfileName;
            profileName.IsReadOnly = true;
        }

        public void setJsonPaths(List<string> jsonPaths)
        {
            this.jsonPaths = jsonPaths;

            if (selectedProfileName == null) return;

            foreach (string path in jsonPaths)
            {
                if (path.Contains(selectedProfileName))
                {
                    relevantJsonPaths.Add(path);

                    string[] parsedFileName = path.Replace(@".\", @"").Split("@");

                    relevantDisplayNames.Add(parsedFileName[1].Replace(".json", ""));
                }
            }

            monitorName.ItemsSource = relevantDisplayNames;

            // Default behaviour...
            Parse_Json_File(relevantJsonPaths[0]);
        }

        private void ComboBox_DropDownOpen(object sender, EventArgs e)
        {
            Save_Display_Config();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox? cmb = sender as ComboBox;
            Handle();
        }

        private void Handle()
        {
            if (buttonBehaviour == "editButton")
                foreach (string path in relevantJsonPaths)
                {
                    if (path.Contains(monitorName.SelectedItem.ToString()!))
                    {
                        Parse_Json_File(path);
                    }
                }
        }

        private void Parse_Json_File(string path)
        {
            JObject jo = JObject.Parse(File.ReadAllText(path));

            DisplayConfig currentDisplayConfig = JsonConvert.DeserializeObject<DisplayConfig>(jo.ToString())!;

            dpiValue.Text = currentDisplayConfig.dpiSetting;
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
            if (buttonBehaviour == "customButton")
                Save_Display_Config();
        }

        private void Save_Display_Config()
        {
            // A guard clause that makes sure that the profile name has been entered...
            if (profileName.Text.Trim() == "") return;
            if (monitorName.Text.Trim() == "") return;

            string cleanedProfileName = profileName.Text.Trim().Replace(" ", "_");

            DisplayConfig displayConfig = new DisplayConfig
            {
                profileName = cleanedProfileName,
                displayIndex = displayInfoList?.IndexOf(monitorName.Text.Trim()) + 1,
                displayName = monitorName.Text.Trim(),
                dpiSetting = dpiValue.Text.Trim()
            };

            string path = String.Format(@"{0}@{1}.json", cleanedProfileName, monitorName.Text);

            filePaths.Add(path);

            File.WriteAllText(path, JsonConvert.SerializeObject(displayConfig));
        }

        private void Delete_Display_Config()
        {
            // A Null-checking Guard Clause...
            if (filePaths == null) return;

            foreach (string path in filePaths)
            {
                File.Delete(path);
            }
        }

        // Only allows numbers to be entered in the DPI textbox...
        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if ((e.Text) == null || !(e.Text).All(char.IsDigit))
            {
                e.Handled = true;
            }
        }
    }
}
