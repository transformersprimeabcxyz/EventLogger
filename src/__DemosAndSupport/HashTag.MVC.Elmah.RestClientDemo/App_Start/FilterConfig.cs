using System.Web;
using System.Web.Mvc;

namespace HashTag.MVC.Elmah.RestClientDemo
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
