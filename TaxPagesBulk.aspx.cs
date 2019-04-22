using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ScrapMaricopa
{
    public partial class TaxPagesBulk : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ono"] != null)
            {
                string orderNo = Session["ono"].ToString();
                imagedisplay(orderNo);
            }
        }
        public void imagedisplay(string ordernumber)
        {
            try
            {
                //Response.Redirect("http://173.192.83.98/Stars/MergePDF/" + ordernumber + ".pdf");
                Response.Redirect("http://173.192.83.98/restservice/pdf/" + ordernumber + ".pdf");
            }
            catch
            {

            }


        }
    }
}