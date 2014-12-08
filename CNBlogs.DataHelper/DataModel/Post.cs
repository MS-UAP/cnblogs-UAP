using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.IO;


namespace CNBlogs.DataHelper.DataModel
{
    [XmlRoot(ElementName = "entry")]
    public class Post : EntryBase
    {
        [XmlElement("author", typeof(Author))]
        public Author Author { get; set; }
    }
}
