using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CNBlogs.DataHelper.DataModel
{
    [XmlRoot("feed", Namespace = "http://www.w3.org/2005/Atom")]
    public class BloggerFeed : Feed<Post>
    {
        [XmlElement("logo")]
        public string Logo { get; set; }

        [XmlElement("author", typeof(Author))]
        public Author Author { get; set; }

        [XmlElement("postcount")]
        public int PostCount { get; set; }
    }
}
