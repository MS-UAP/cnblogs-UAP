using CNBlogs.DataHelper.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CNBlogs.DataHelper.DataModel
{

    public class NewsBody
    {
        [XmlElement]
        public string SubmitDateString { get; set; }

        [XmlElement]
        public string Content { get; set; }

        [XmlElement]
        public string ImageUrl { get; set; }

        [XmlElement]
        public string PrevNews { get; set; }

        [XmlElement]
        public string NextNews { get; set; }

        [XmlElement]
        public string CommentCount { get; set; }

        [XmlElement]
        public string Title { get; set; }

        [XmlIgnore]
        public DateTime SubmitDate
        {
            get {
                return Functions.ParseDateTime(this.SubmitDateString);
            }
        }
    }
}
