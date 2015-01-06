using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CNBlogs.DataHelper.DataModel
{
    [XmlRoot("Columns")]
    public class ColumnCollection
    {
        [XmlElement("Column")]
        public List<Column> Columns { get; set; }
    }

    [XmlRoot("Column")]
    public class Column
    {
        [XmlElement("Icon")]
        public string Icon { get; set; }
        
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Desc")]
        public string Desc { get; set; }

        [XmlElement("Page")]
        public string Page { get; set; }
    }
}
