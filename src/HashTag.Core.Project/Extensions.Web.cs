using HashTag.Web;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

namespace HashTag
{
    public static partial class Extensions
    {
        public static NameValueCollection Expand(this HttpRequest request)
        {
            return WebUtils.ToList(request);
        }
    }
}
