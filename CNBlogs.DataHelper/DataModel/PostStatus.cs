using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNBlogs.DataHelper.DataModel
{
    public enum PostStatus
    {
        None = 1,
        Skip = 2,
        Read = 4,
        Favorite = 8
    }
}
