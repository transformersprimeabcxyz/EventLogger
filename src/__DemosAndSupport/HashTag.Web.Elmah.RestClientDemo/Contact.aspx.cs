using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HashTag.Web.Elmah.RestClientDemo
{
    public partial class Contact : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var x = 100;
            while(true)
            {
                var y = x / x--;
            }
        }
    }
}