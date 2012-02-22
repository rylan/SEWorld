using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.ComponentModel;

namespace com.iCottrell.SEWorld
{
    public class RSSItem : INotifyPropertyChanged
    {
        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (value != _title)
                {
                    _title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        private string _description;
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (value != _description)
                {
                    _description = value;
                    NotifyPropertyChanged("Description");
                }
            }
        }

        private string _url;
        public string URL
        {
            get
            {
                return _url;
            }
            set
            {
                if (value != _url)
                {
                    _url = value;
                    NotifyPropertyChanged("URL");
                }
            }
        }
        private string _id;
        public string ID
        {
            get
            {
                return _id;
            }
            set
            {
                if (value != _id)
                {
                    _id = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }
        private DateTime _dateAdded;
        public DateTime DateAddedToList
        {
            get
            {
                return _dateAdded;
            }
            set
            {
                if (value != _dateAdded)
                {
                    _dateAdded = value;
                    NotifyPropertyChanged("DateAdded");
                }
            }
        }
        private DateTime _updated;
        public DateTime Updated
        {
            get
            {
                return _updated;
            }
            set
            {
                if (value != _updated)
                {
                    _updated = value;
                    NotifyPropertyChanged("Updated");
                }
            }
        }

        private Boolean _read;
        public Boolean Read
        {
            get
            {
                return _read;
            }
            set
            {
                if (value != _read)
                {
                    _read = value;
                    NotifyPropertyChanged("Read");
                }
            }
        }

        private Boolean _later;
        public Boolean Later
        {
            get
            {
                return _later;
            }
            set
            {
                if (value != _later)
                {
                    _later = value;
                    NotifyPropertyChanged("Later");
                }
            }
        }

        private Boolean _starred;
        public Boolean Starred
        {
            get
            {
                return _starred;
            }
            set
            {
                if (value != _starred)
                {
                    _starred = value;
                    NotifyPropertyChanged("Starred");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
