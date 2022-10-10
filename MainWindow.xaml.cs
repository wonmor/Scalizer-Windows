﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
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
    /// This is the class of the main menu.
    /// </summary>
 
    public partial class MainWindow : Window
    {
        private enum Startup_Type
        {
            Enable,
            Disable,
            Get
        }

        public MainWindow()
        {
            InitializeComponent();

            String msg = Set_Startup(Startup_Type.Get);

            isOpenStartUp.IsChecked = msg.Contains("not");
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
            Change_Click(sender, e);
        }

        private static string Set_Startup(Startup_Type startup_Type)
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
                        bool keyDoesntExist = key!.GetValue(curAssembly.GetName().Name!) == null;

                        if (keyDoesntExist) { return "Key not found!"; } else { return "Key found!";  }

                    default:
                        return "Specify a startup behaviour...";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private void Change_Click(object sender, RoutedEventArgs e)
        {
            CustomWindow customWindow = new CustomWindow();
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
    }
}