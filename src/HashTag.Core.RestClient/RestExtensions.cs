using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HashTag.Web.Http
{
    public static class RestExtensions
    {
        public static Task ReadContentAsync(this HttpContent content)
        {
            return default(Task);
        }
    }
}
