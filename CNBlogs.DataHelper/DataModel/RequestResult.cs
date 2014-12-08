using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNBlogs.DataHelper.DataModel
{
    public class RequestResult<T>
    {
        public bool IsSuccess { get; set; }
        public bool IsNetworkError { get; set; }
        public int Code { get; set; }
        public T Result { get; set; }
    }
}
