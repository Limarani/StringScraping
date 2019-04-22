using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ScrapMaricopa.Pages
{
    public partial class Upload : System.Web.UI.Page
    {
        DBbulk dbConn = new DBbulk();
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void BtnSubmit_Click(object sender, EventArgs e)
        {
            string orderNo = "", county = "", state = "", address = "", pdate = DateTime.Now.ToString("MM/dd/yyyy");
            string txt = txtordertype.Text;
            string[] splitOrderDetails = txt.Split('\n');

            int count = 0;

            foreach (string orderDetails in splitOrderDetails)
            {
                if (orderDetails != "")
                {
                    string[] splitOrder = orderDetails.Split('\t');
                    if (splitOrder.Count() == 4)
                    {
                        orderNo = splitOrder[0];
                        address = splitOrder[1];
                        state = splitOrder[2];
                        county = splitOrder[3];

                        //order exists
                        string exists = dbConn.ExecuteScalar("select count(order_no) from record_status where order_no='" + orderNo + "'");
                        if (exists == "0")
                        {
                            count++;
                            dbConn.ExecuteQuery("insert into record_status(order_no,pdate,state,county,address,scrape,scrape_status) values('" + orderNo + "','" + pdate + "','" + state + "','" + county + "','" + address + "','1','0')");
                        }
                    }

                }

            }
            lblstatus.Text = count.ToString() + " order(s) uploaded successfully...";
        }
    }
}