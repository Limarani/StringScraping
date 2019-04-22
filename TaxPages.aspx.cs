using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ScrapMaricopa
{
    public partial class ScrapedPDF : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["orderno"] != null)
            {
                string orderNo = Session["orderno"].ToString();
                imagedisplay(orderNo);
            }
        }
        public void imagedisplay(string ordernumber)
        {
            try
            {
                Response.Redirect("http://173.192.83.98/Stars/MergePDF/" + ordernumber + ".pdf");
            }
            catch
            {

            }


        }

    }
}