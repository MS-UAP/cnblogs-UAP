using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CNBlogs.DataHelper.DataModel
{
    [XmlRoot("root")]
    public class Link
    {
        [XmlAttribute("href")]
        public string Href { get; set; }
    }
}
