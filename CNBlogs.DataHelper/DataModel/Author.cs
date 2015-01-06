using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Data.Xml.Dom;
using CNBlogs.DataHelper.Function;
using System.Xml.Serialization;

namespace CNBlogs.DataHelper.DataModel
{
    [XmlRoot("author")]
    public class Author
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("avatar")]
        public string Avatar { get; set; }

        [XmlElement("uri")]
        public string Uri { get; set; }

        [XmlIgnore]
        public string BlogApp { get; set; }
    }
}
