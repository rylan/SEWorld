using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Net;
using System.Xml.Linq;
using System.IO.IsolatedStorage;
using HtmlAgilityPack;


namespace com.iCottrell.SEWorld
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string RSS_FEED = "http://listserv.acm.org/scripts/wa-acmlpx.exe?RSS&L=seworld&v=ATOM1.0";
        public const string RSS_UPDATED = "com.iCottrell.SEWorld.LastUpdated";
        public const string RSS_LIST_SAVED = "com.iCottrell.SEWorld.RSSListSaved";
        public const string RSS_REMOVE_READ = "com.iCottrell.SEWorld.RSSRemoveRead";

        public MainViewModel()
        {
            this.RSSItems = new ObservableCollection<RSSItem>();
            this.SearchItems = new ObservableCollection<SearchItem>();
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<RSSItem> RSSItems { get; private set; }
        public ObservableCollection<SearchItem> SearchItems { get; private set; }

        private string _sampleProperty = "Sample Runtime Property Value";
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding
        /// </summary>
        /// <returns></returns>
        public string SampleProperty
        {
            get
            {
                return _sampleProperty;
            }
            set
            {
                if (value != _sampleProperty)
                {
                    _sampleProperty = value;
                    NotifyPropertyChanged("SampleProperty");
                }
            }
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            this.RSSItems.Clear();
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            Boolean removeRead = false;
            if (settings.Contains(RSS_REMOVE_READ))
            {
                removeRead = (Boolean)settings[RSS_REMOVE_READ];
            }
            if (settings.Contains(RSS_LIST_SAVED))
            {
                ObservableCollection<RSSItem> list = (ObservableCollection<RSSItem>)settings[RSS_LIST_SAVED];
                foreach (RSSItem ri in list)
                {
                    if(removeRead)
                    {
                        if(!ri.Starred && !ri.Read && !ri.Read)
                        {
                            this.RSSItems.Add(ri); 
                        }

                    }
                    else
                    {
                       this.RSSItems.Add(ri); 
                    }
                }
                NotifyPropertyChanged("RSS_FEED_Loaded");
            }

            

            WebClient wc = new WebClient();
            wc.OpenReadAsync(new Uri(RSS_FEED));
            wc.OpenReadCompleted += new OpenReadCompletedEventHandler(parseRSSFeed);
            this.IsDataLoaded = true;
        }

        void parseRSSFeed(Object sender, OpenReadCompletedEventArgs e)
        {
            XElement resultXml;
            if (e.Error != null)
            {
                NotifyPropertyChanged("RSS_FEED_Errror");
                return;
            }
            else
            {
                IList<RSSItem> tmp = new List<RSSItem>();

                XNamespace web = "http://www.w3.org/2005/Atom";
                try
                {
                    resultXml = XElement.Load(e.Result);
                    DateTime t;
                    
                    IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
                    DateTime lastUpdated = DateTime.MinValue;
                    if(settings.Contains(RSS_UPDATED))
                    {
                        lastUpdated = (DateTime) settings[RSS_UPDATED]; 
                    }
                    DateTime min = DateTime.MinValue;
                    foreach (XElement result in resultXml.Descendants(web + "entry"))
                    {
                        RSSItem item = new RSSItem();
                        item.Title = result.Element(web + "title").Value.Trim();
                        item.Description = result.Element(web + "content").Value.Replace("&lt;br&gt;&lt;br&gt;", ". ").Replace("&lt;br&gt;", " ").Trim();
                        
                        foreach (XAttribute a in result.Element(web + "link").Attributes())
                        {
                            if (a.Name.ToString().ToLower() == "href")
                            {
                                item.URL = a.Value.Trim();
                            }
                        }
                        item.ID = result.Element(web + "id").Value.Trim();
                        item.Read = false;
                        item.Later = false;
                        item.DateAddedToList = DateTime.Now;
                        if(DateTime.TryParse(result.Element(web + "updated").Value.Trim(), out t))
                        {
                            item.Updated = t;
                        }
                        if (item.Updated.CompareTo(lastUpdated) > 0)
                        {
                            if (item.Updated.CompareTo(min) > 0)
                            {
                                min = item.Updated;
                            }
                            tmp.Add(item);
                        }
                        
                    }
                    if (!settings.Contains(RSS_UPDATED))
                    {
                        settings.Add(RSS_UPDATED, min);
                    }
                    else
                    {
                        settings[RSS_UPDATED] = min;
                    }

                    foreach (RSSItem ri in tmp.OrderByDescending(x => x.Updated))
                    {
                        this.RSSItems.Add(ri);
                    }
                    tmp.Clear();

                }
                catch (System.Xml.XmlException ex)
                {

                }
                NotifyPropertyChanged("RSSItemsRetrived");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
           IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
           IList<RSSItem> tmpItems = new List<RSSItem>(RSSItems);
           RSSItems.Clear();
           Boolean removeRead = false;
           if (settings.Contains(RSS_REMOVE_READ))
           {
               removeRead = (Boolean)settings[RSS_REMOVE_READ];
           }
           foreach (RSSItem ri in tmpItems.OrderByDescending(x => x.Updated))
           {
               if (removeRead)
               {
                   if (!ri.Starred && !ri.Read && !ri.Read)
                   {
                       this.RSSItems.Add(ri);
                   }

               }
               else
               {
                   this.RSSItems.Add(ri);
               }
           }
               
           tmpItems.Clear();
            
            
            
            if (!settings.Contains(RSS_LIST_SAVED))
            {
                settings.Add(RSS_LIST_SAVED, this.RSSItems);
            }
            else
            {
                settings[RSS_LIST_SAVED] = this.RSSItems;
            }

           
            _NewsItems = null;
            _ReadLater = null;
            _Starred = null;

            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void callSearch(String query)
        {
            string searchQuery = "http://listserv.acm.org/scripts/wa-acmlpx.exe?REPORT=SEWORLD&K=1&d=No+Match%3BMatch%3BMatches&I=-3&J=on&L=SEWORLD&1=SEWORLD&0=S&z=4&q="
                + Uri.EscapeDataString(query)
                + "&9=A&_charset_=UTF-8";

            HtmlWeb webGet = new HtmlWeb();
            webGet.LoadCompleted += parse_DownloadSearchResultsCompleted;
            webGet.LoadAsync(searchQuery);
        }

        public void parse_DownloadSearchResultsCompleted(object sender, HtmlDocumentLoadCompleted e)
        {
            this.SearchItems.Clear();
            IList<HtmlNode> hnc = e.Document.DocumentNode.DescendantNodes().ToList();
            int table = 0;
            Boolean content_record = false;
            foreach (HtmlNode htmlNode in hnc)
            {
                if (htmlNode.Name.ToLower() == "h2" && htmlNode.InnerText.Contains("LISTSERV.ACM.ORG"))
                {
                    table = 0;
                    content_record = true;
                }
                else if(htmlNode.Name.ToLower() == "table" && content_record)
                {
                    if (table == 1)
                    {
                        foreach (HtmlNode tr in htmlNode.DescendantNodes().ToList())
                        {
                            if (tr.Name.ToLower() == "tr")
                            {
                                SearchItem st = new SearchItem();
                                foreach(HtmlNode td in tr.DescendantNodes().ToList())
                                {
                                    if (td.Name.ToLower() == "a")
                                    {
                                        foreach (HtmlAttribute at in td.Attributes)
                                        {
                                            if (at.Name.ToLower() == "href")
                                            {
                                                st.URL = at.Value;
                                            }
                                        }
                                    }
                                    else if (td.Name.ToLower() == "td" && td.NextSibling != null 
                                        && td.NextSibling.NextSibling != null 
                                        && td.NextSibling.NextSibling.InnerText == "SEWORLD")
                                    {
                                        st.Title = td.InnerText;
                                    }

                                }
                                if (st.Title != null)
                                {
                                    this.SearchItems.Add(st);
                                }
                            }
                           
                        }
                        content_record = false;
                        break;
                    }
                    table++;

                }
            }
            NotifyPropertyChanged("SearchItemsRetrived");
        }

        public RSSItem getItemByID(String id)
        {
            foreach (RSSItem i in this.RSSItems)
            {
                if (i.ID == id)
                {
                    return i;
                }
            }
            return null;
        }

        public void setStarredByURL(String url)
        {
            foreach (RSSItem i in this.RSSItems)
            {
                if (i.URL == url)
                {
                    i.Starred = !i.Starred;
                    break;
                }
            }
            _NewsItems = null;
            _Starred = null;
            _ReadLater = null;

            NewsItems.View.Refresh();
            Starred.View.Refresh();
            ReadLater.View.Refresh();
            NotifyPropertyChanged("StarredUpdated");
        }
        public void setLaterByURL(String url)
        {
            foreach (RSSItem i in this.RSSItems)
            {
                if (i.URL == url)
                {
                    i.Later = !i.Later;
                    break;
                }
            }
            _NewsItems = null;
            _Starred = null;
            _ReadLater = null;

            NewsItems.View.Refresh();
            Starred.View.Refresh();
            ReadLater.View.Refresh();
            NotifyPropertyChanged("LaterUpdated");
        }

        public RSSItem getItemByURL(String url)
        {
            foreach (RSSItem i in this.RSSItems)
            {
                if (i.URL == url)
                {
                    return i;
                }
            }
            return null;
        }

        private CollectionViewSource _NewsItems;
        public CollectionViewSource NewsItems
        {
            get
            {
                if (null == _NewsItems)
                {
                    _NewsItems = new CollectionViewSource { Source = RSSItems };
                    _NewsItems.View.Filter = (x) => FilterItemsByNewsItems(x);
                }
                return _NewsItems;
            }
        }
        private bool FilterItemsByNewsItems(Object item)
        {
            if (item is RSSItem)
            {
                return !((RSSItem)item).Later && !((RSSItem)item).Starred;
            }
            return false;
        }
        private CollectionViewSource _ReadLater;
        public CollectionViewSource ReadLater
        {
            get
            {
                if (null == _ReadLater)
                {
                    _ReadLater = new CollectionViewSource { Source = RSSItems };
                    _ReadLater.View.Filter = (x) => FilterItemsByReadLater(x);
                }
                return _ReadLater;
            }
        }
        private bool FilterItemsByReadLater(Object item)
        {
            if (item is RSSItem)
            {
                return ((RSSItem)item).Later;
                
            }
            return false;
        }

        private CollectionViewSource _Starred;
        public CollectionViewSource Starred
        {
            get
            {
                if (null == _Starred)
                {
                    _Starred = new CollectionViewSource { Source = RSSItems };
                    _Starred.View.Filter = (x) => FilterItemsByStarred(x);
                }
                return _Starred;
            }
        }
        private bool FilterItemsByStarred(Object item)
        {
            if (item is RSSItem)
            {
                return ((RSSItem)item).Starred;

            }
            return false;
        }
    }
}