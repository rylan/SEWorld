﻿﻿/*******************************************************************************
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
using Microsoft.Phone.Net.NetworkInformation;
using System.ComponentModel;

namespace com.iCottrell.SEWorld
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            App.ViewModel.PropertyChanged += new PropertyChangedEventHandler(NotifyPropertyChanged);
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                Loading.Visibility = Visibility.Visible;
                App.ViewModel.LoadData();
            }
        }
        
        private void NotifyPropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            Loading.Visibility = Visibility.Collapsed;
            Searching.Visibility = Visibility.Collapsed;
        }

        private void search_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
               Searching.Visibility = Visibility.Visible;
               SearchButton.Focus();
               runSearch();
            }
        }

        private void runSearch()
        {
            if (this.SearchBox.Text != "" && DeviceNetworkInformation.IsNetworkAvailable)
            {
                String str = this.SearchBox.Text.Trim();
                str = str.Replace(" ", "%20");
                App.ViewModel.callSearch(str);
            }
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.SearchBox.Text != "" && DeviceNetworkInformation.IsNetworkAvailable)
            {
                Searching.Visibility = Visibility.Visible;
                String str = this.SearchBox.Text.Trim();
                str = str.Replace(" ", "%20");
                App.ViewModel.callSearch(str);
            }
        }

        private void textLoaded(object sender, RoutedEventArgs e)
        {
            TextBlock b = (TextBlock)sender;
            Boolean isRead = (Boolean)b.Tag;
            if (isRead)
            {
                b.FontWeight = FontWeights.Normal;
            }
        }

        private void saveForLater(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string url = (string)((StackPanel)sender).Tag;
            App.ViewModel.setLaterByURL(url);
        }

        private void openRSSItem(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string title = "";
            string url = "";
            if (sender is StackPanel)
            {
                url = (string)((StackPanel)sender).Tag;
                
                foreach (Object obj in ((StackPanel)sender).Children)
                {
                    if (obj is TextBlock)
                    {
                        title = ((TextBlock)obj).Text;
                        break;
                    }
                }
            }
            else if (sender is TextBlock)
            {
                url = (string)((TextBlock)sender).Tag;
                title = ((TextBlock)sender).Text;
            }

            if (DeviceNetworkInformation.IsNetworkAvailable)
            {
                this.NavigationService.Navigate(new Uri("/SEWorldPage.xaml?title="+title+"&href="+Uri.EscapeDataString(url), UriKind.Relative));
            }
            else
            {
                this.NavigationService.Navigate(new Uri("/ErrorPage.xaml", UriKind.Relative));
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
           base.OnNavigatedTo(e);
           
        }

        private void removeStarred(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (sender is Rectangle)
            {
                String url = (string)((Rectangle)sender).Tag;
                App.ViewModel.setStarredByURL(url);
            }
        }

        private void OpenAbout(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

        private void OpenSettings(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.Relative));
        }

        private void RefreshEvent(object sender, EventArgs e)
        {
            Loading.Visibility = Visibility.Visible;
            App.ViewModel.refresh();
        }

    }
}