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
using HtmlAgilityPack;
using System.Text;
using Microsoft.Phone.Net.NetworkInformation;
using System.Text.RegularExpressions;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Shell;

namespace com.iCottrell.SEWorld
{
    public partial class SEWorldPage : PhoneApplicationPage
    {
        private String CurrentPage { get; set; }
        private RSSItem CurrentItem { get; set; }

        public SEWorldPage()
        {
            InitializeComponent();
        }

        public void loadPage(String url)
        {
            Loading.Visibility = Visibility.Visible;
            HtmlWeb webGet = new HtmlWeb();
            webGet.LoadCompleted += parse_DownloadSEPageCompleted;
            webGet.LoadAsync(url, Encoding.UTF8);
        }

        public void parse_DownloadSEPageCompleted(Object sender, HtmlDocumentLoadCompleted e)
        {
            if (e != null && e.Document != null && e.Document.DocumentNode != null)
            {
                IList<Block> pageBody = new List<Block>();
                IList<HtmlNode> hnc = e.Document.DocumentNode.DescendantNodes().ToList();

                Paragraph paragraph = new Paragraph();

                foreach (HtmlNode htmlNode in hnc)
                {
                    if (htmlNode.Name.ToLower() == "pre")
                    {
                        /*webBrowser1.NavigateToString("<html><head><meta name='viewport' content='width=480, user-scalable=yes' />"
                            + "<style>pre {font-size: 13px; white-space: pre-wrap; white-space: -moz-pre-wrap !important; white-space: -pre-wrap; white-space: -o-pre-wrap; word-wrap: break-word; _white-space: normal;} .tableframe {border-right: #999999 1px solid; border-top: #999999 1px solid; border-left: #999999 1px solid; border-bottom: #999999 1px solid } .tableframeinv {border-right: #F1F4FA 1px solid; border-top: #F1F4FA 1px solid; border-left: #F1F4FA 1px solid; border-bottom: #F1F4FA 1px solid }</style>"
                            +"</head><body><table width='100%' cellpadding='2' cellspacing='0' border='0'><tr><td id='awesomepre'>"
                            +htmlNode.InnerHtml+"</td></tr></table></body></html>");
                        */
                        foreach (HtmlNode node in htmlNode.DescendantNodes().ToList())
                        {
                            if (paragraph.Inlines.Count > 0)
                            {
                                pageBody.Add(paragraph);
                                paragraph = new Paragraph();
                            }

                            if (node.Name.ToLower() == "#text" && node.ParentNode.Name.ToLower() != "a")
                            {
                                String str = ConvertWhitespacesToSingleSpaces(node.InnerText);
                                if (str != " ")
                                {
                                        string[] str1 = str.Split(new Char[]{'\n'});
                                        foreach(string s in str1)
                                        {
                                            if (s != " " && s != "")
                                            {

                                                Run run = new Run();
                                                run.Text = s;
                                                paragraph.Inlines.Add(run);
                                                paragraph.Inlines.Add(new LineBreak());
                                                pageBody.Add(paragraph);
                                                paragraph = new Paragraph();
                                            }
                                            else
                                            {
                                               // paragraph.Inlines.Add(new LineBreak());
                                            }
                                        }
                                }
                            }
                            else if (node.Name.ToLower() == "a" && !node.InnerText.Contains("[log in to unmask]"))
                            {
                                Hyperlink hl = new Hyperlink();
                                hl.Inlines.Add(node.InnerText);
                                foreach (HtmlAttribute att1 in node.Attributes)
                                {
                                    if (att1.Name.ToLower() == "href")
                                    {
                                        try
                                        {
                                            hl.NavigateUri = new Uri("/SEWorldPage.xaml?external=true&href="+Uri.EscapeUriString(att1.Value), UriKind.Relative);
                                        }
                                        catch (Exception err)
                                        {
                                        }
                                    }
                                }
                                paragraph.Inlines.Add(hl);
                                paragraph.Inlines.Add(new LineBreak());
                            }
                        }
                        break;
                    }
                }
                
                if (paragraph != null && paragraph.Inlines.Count > 0)
                {
                    pageBody.Add(paragraph);
                }
                foreach (Block b in pageBody)
                {
                    RichTextBox rtb = new RichTextBox();
                    rtb.IsReadOnly = true;
                    rtb.VerticalAlignment = VerticalAlignment.Top;
                    rtb.Blocks.Add(b);
                    PageBody.Children.Add(rtb);

                }
                PageBody.InvalidateArrange();
                PageBody.InvalidateMeasure();
                scrollViewer1.InvalidateArrange();
                scrollViewer1.InvalidateMeasure();
                scrollViewer1.InvalidateScrollInfo();
                  
            }
            Loading.Visibility = Visibility.Collapsed;
        }

        public static string ConvertWhitespacesToSingleSpaces(string value)
        {
            if (value != null)
            {
                value = value.Replace("&nbsp;", "");
                value = value.Replace("&quot;", "\"");
                value = value.Replace("&amp;", "&");
                value = value.Replace("\r\n \r\n \r\n\r\n \r\n \r\n", "X+X+X+X+X+X");
                value = value.Replace("\r\n     \r\n", "X+X+X+X+X+X");
                value = value.Replace("\r\n \r\n", "X+X+X+X+X+X");
                value = value.Replace("\r\n\r\n", "Y-Y-Y-Y-Y-Y");
                value = value.Replace("\t", "");
                value = Regex.Replace(value, @"\s+", " ");
                value = value.Replace("Y-Y-Y-Y-Y-Y", "\n");
                value = value.Replace("X+X+X+X+X+X", "\n");
                value = value.Replace("~~~~~~~~~~~~~~~~~~~~~~~", "\n");
                value = value.Replace("============================================================", "==============================\n");
                value = value.Replace("=======================================", "==============================\n");
                value = value.Replace("======================================", "==============================\n");
                value = value.Replace("------------------------------------------", "\n------------------------------\n");
                value = value.Replace("�", "'");
                value = value.Replace(" o ", "\no ");
                value = value.Replace(" O ", "\nO ");
                
                return value;
            }
            else
            {
                return " ";
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            if (DeviceNetworkInformation.IsNetworkAvailable)
            {
                string external = "";
                if (e.NavigationMode == System.Windows.Navigation.NavigationMode.Back && NavigationContext.QueryString.TryGetValue("external", out external))
                {
                    NavigationService.GoBack();
                }
                else
                {
                   
                    if (NavigationContext.QueryString.TryGetValue("external", out external))
                    {
                        string url = "";
                        if (NavigationContext.QueryString.TryGetValue("href", out url))
                        {

                            WebBrowserTask task = new WebBrowserTask();
                            task.Uri = new Uri(url);
                            task.Show();
                            //NavigationService.RemoveBackEntry();
                        }

                    }
                    else
                    {

                        string url = "";
                        if (NavigationContext.QueryString.TryGetValue("href", out url))
                        {
                            CurrentPage = url;
                            CurrentItem = App.ViewModel.getItemByURL(url);
                            App.ViewModel.setReadByURL(url);
                            if (CurrentItem.Later)
                            {
                                ApplicationBarIconButton btn = (ApplicationBarIconButton)ApplicationBar.Buttons[2];
                                btn.IconUri = new Uri("/img/readlater48.png", UriKind.Relative);
                            }
                            if (CurrentItem.Starred)
                            {
                                ApplicationBarIconButton btn = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
                                btn.IconUri = new Uri("/img/appbar.favs.rest.png", UriKind.Relative);
                            }


                            loadPage(url);
                        }
                        string title = "";
                        if (NavigationContext.QueryString.TryGetValue("title", out title))
                        {
                            PageTitle.Text = title;
                        }
                    }
                }
            }
            else
            {
                this.NavigationService.Navigate(new Uri("/ErrorPage.xaml", UriKind.Relative));
            }
        }

        private void OpenInBrowser(object sender, EventArgs e)
        {
            if (CurrentPage != null)
            {
                WebBrowserTask task = new WebBrowserTask();
                task.Uri = new Uri(CurrentPage);
                task.Show();
            }
        }

        private void ShareEvent(object sender, EventArgs e)
        {
            ShareLinkTask slt = new ShareLinkTask();
            slt.LinkUri = new Uri(CurrentPage);
            slt.Title = "SEWorld News";
            slt.Message = "Checkout " + PageTitle.Text;
            slt.Show();
        }

        private void EmailDev_Tap(object sender, EventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            emailComposeTask.To = "dev@icottrell.com";
            emailComposeTask.Body = "";
            emailComposeTask.Subject = "Feedback - SEWorld";
            emailComposeTask.Show();
        }

        private void ReadLaterEvent(object sender, EventArgs e)
        {
            App.ViewModel.setLaterByURL(CurrentPage);
            if (CurrentItem.Later)
            {
                ApplicationBarIconButton btn = (ApplicationBarIconButton)ApplicationBar.Buttons[2];
                btn.IconUri = new Uri("/img/readlater48.png", UriKind.Relative);
            }
            else
            {
                ApplicationBarIconButton btn = (ApplicationBarIconButton)ApplicationBar.Buttons[2];
                btn.IconUri = new Uri("/img/readlateradd48.png", UriKind.Relative);
            }
        }

        private void StarredEvent(object sender, EventArgs e)
        {
            App.ViewModel.setStarredByURL(CurrentPage);
            if (CurrentItem.Starred)
            {
                ApplicationBarIconButton btn = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
                btn.IconUri = new Uri("/img/appbar.favs.rest.png", UriKind.Relative);
            }
            else
            {
                ApplicationBarIconButton btn = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
                btn.IconUri = new Uri("/img/appbar.favs.addto.rest.png", UriKind.Relative);
            }
        }

        private void OpenAbout(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

    }
}