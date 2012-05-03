﻿/*******************************************************************************
 * Copyright (c) 2012 Rylan Cottrell. iCottrell.com.
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution, and is available at
 * http://www.eclipse.org/legal/epl-v10.html
 *
 * Contributors:
 *    Rylan Cottrell - initial API and implementation and/or initial documentation
 *******************************************************************************/
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
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains(MainViewModel.RSS_REMOVE_READ))
            {
               Removal.IsChecked = (Boolean)settings[MainViewModel.RSS_REMOVE_READ];
            }
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
            settings.Save();
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
            settings.Save();
        }
    }
}