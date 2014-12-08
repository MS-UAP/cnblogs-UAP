using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CNBlogs.DataHelper.Helper;
using System.Xml.Serialization;

namespace CNBlogs.DataHelper.DataModel
{
    /// <summary>
    /// TODO: We should use other way to parse
    /// </summary>
    public class News : EntryBase
    {
        [XmlElement("topic")]
        public string Topic { get; set; }

        [XmlElement("topicIcon")]
        public string TopicIcon { get; set; }

        [XmlElement("sourceName")]
        public string SourceName { get; set; }
    }
}
