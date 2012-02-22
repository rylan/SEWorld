﻿using System;
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
using Microsoft.Phone.Tasks;

namespace com.iCottrell.SEWorld
{
    public partial class About : PhoneApplicationPage
    {
        public About()
        {
            InitializeComponent();
        }

         protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
         {
            base.OnNavigatedTo(e);
            if (e.NavigationMode != System.Windows.Navigation.NavigationMode.Back)
            {
                string href = "";

                if (NavigationContext.QueryString.TryGetValue("href", out href))
                {
                    WebBrowserTask task = new WebBrowserTask();
                    task.Uri = new Uri(href);
                    task.Show();
                    NavigationService.RemoveBackEntry();
                }
            }
        }

        private void EmailDev_Tap(object sender, EventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            emailComposeTask.To = "dev@icottrell.com";
            emailComposeTask.Body = "";
            emailComposeTask.Subject = "Feedback - SEWorld";
            emailComposeTask.Show();
        }
    
    }
}