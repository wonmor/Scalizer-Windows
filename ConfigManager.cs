using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scalizer
{
    internal class ConfigManager
    {
        public void UpdateProperty(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            KeyValueConfigurationCollection appSettings = config.AppSettings.Settings;


            // update SaveBeforeExit
            config.AppSettings.Settings[key].Value = value;
            Console.Write("...Configuration updated: key " + key + ", value: " + value + "...");

            //save the file
            config.Save(ConfigurationSaveMode.Modified);
            Console.Write("...saved Configuration...");
            //relaod the section you modified
            ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
            Console.Write("...Configuration Section refreshed...");
        }

        public NameValueCollection? ReadAppSettingsProperty()
        {
            try
            {
                var section = ConfigurationManager.GetSection("applicationSettings");

                // Get the AppSettings section.
                NameValueCollection? appSettings = ConfigurationManager.AppSettings;

                return appSettings;
            }
            catch (ConfigurationErrorsException e)
            {
                return null;
            }

        }


        public void updateAppSettingProperty(string key, string value)
        {
            // Get the application configuration file.
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            string sectionName = "appSettings";


            config.AppSettings.Settings.Remove(key);
            config.AppSettings.Settings.Add(key, value);

            SaveConfigFile(config);
        }

        public void insertAppSettingProperty(string key, string value)
        {
            // Get the application configuration file.
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            string sectionName = "appSettings";

            config.AppSettings.Settings.Add(key, value);

            SaveConfigFile(config);
        }

        public void deleteAppSettingProperty(string key)
        {
            // Get the application configuration file.
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove(key);

            SaveConfigFile(config);
        }

        private static void SaveConfigFile(System.Configuration.Configuration config)
        {
            string sectionName = "appSettings";

            // Save the configuration file.
            config.Save(ConfigurationSaveMode.Modified);

            // Force a reload of the changed section. This  
            // makes the new values available for reading.
            ConfigurationManager.RefreshSection(sectionName);

            // Get the AppSettings section.
            AppSettingsSection appSettingSection =
              (AppSettingsSection)config.GetSection(sectionName);

            Console.WriteLine();
            Console.WriteLine("Using GetSection(string).");
            Console.WriteLine("AppSettings section:");
            Console.WriteLine(appSettingSection.SectionInformation.GetRawXml());
        }
    }
}
