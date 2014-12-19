using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CNBlogs.DataHelper.DataModel
{
    [XmlRoot("categories")]
    public class CategoryCollection
    {
        [XmlElement("category")]
        public List<Category> Categories { get; set; }
    }

    [XmlRoot("category")]
    public class Category
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("href")]
        public string Href { get; set; }

        [XmlArray(ElementName = "subs")]
        [XmlArrayItem(ElementName = "category")]
        public List<Category> SubCategories { get; set; }
    }
}
