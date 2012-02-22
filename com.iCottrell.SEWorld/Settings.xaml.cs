using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.IO.IsolatedStorage;

namespace com.iCottrell.SEWorld
{
    public partial class Settings : PhoneApplicationPage
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void RecordUnChecked(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (!settings.Contains(MainViewModel.RSS_REMOVE_READ))
            {
                settings.Add(MainViewModel.RSS_REMOVE_READ, false);
            }
            else
            {
                settings[MainViewModel.RSS_REMOVE_READ] = false;
            }
        }

        private void RecordChecked(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (!settings.Contains(MainViewModel.RSS_REMOVE_READ))
            {
                settings.Add(MainViewModel.RSS_REMOVE_READ, true);
            }
            else
            {
                settings[MainViewModel.RSS_REMOVE_READ] = true;
            }
        }
    }
}