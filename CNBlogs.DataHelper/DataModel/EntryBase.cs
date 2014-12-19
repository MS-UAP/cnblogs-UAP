using CNBlogs.DataHelper.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CNBlogs.DataHelper.DataModel
{
    [XmlRoot(ElementName = "entry")]
    public class EntryBase : DataModelBase
    {
        [XmlElement("id")]
        public string ID { get; set; }

        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("summary")]
        public string Summary { get; set; }

        [XmlElement("published")]
        public string PublishTimeString { get; set; }

        [XmlElement("link", typeof(Link))]
        public Link Link { get; set; }

        [XmlElement("blogapp")]
        public string BlogApp { get; set; }

        [XmlElement("diggs")]
        public string DiggsCount { get; set; }

        [XmlElement("views")]
        public string ViewsCount { get; set; }

        [XmlElement("comments")]
        public string CommentsCount { get; set; }

        private PostStatus status;
        [XmlIgnore]
        public PostStatus Status{ 
            get
            {
                return this.status;
            }
            set
            {
                this.status = value;
                OnPropertyChanged("Status");
            }
        }

        [XmlIgnore]
        public DateTime PublishTime
        {
            get
            {
                return Functions.ParseDateTime(this.PublishTimeString);
            }
        }
    }
}
