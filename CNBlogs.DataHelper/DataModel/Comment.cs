using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Data.Xml.Dom;
using CNBlogs.DataHelper.Helper;
using System.Xml.Serialization;

namespace CNBlogs.DataHelper.DataModel
{
    [XmlRoot("entry")]
    public class Comment
    {
        [XmlElement("id")]
        public string ID { get; set; }

        [XmlElement("published")]
        public string PublishTimeString { get; set; }

        [XmlElement("author", typeof(Author))]
        public Author Author { get; set; }

        [XmlElement("content")]
        public string Content { get; set; }

        [XmlIgnore]
        public DateTime PublishTime
        {

            get { return Functions.ParseDateTime(this.PublishTimeString); }
        }
    }
}
