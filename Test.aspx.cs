using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ScrapMaricopa
{
    public partial class Test : System.Web.UI.Page
    {
        GlobalClass gc = new GlobalClass();
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            List<dynaMic> dyB = new List<dynaMic>();
            dyB.Add(new dynaMic { countyID = "304", catID = "1", orderNumber = "29112018 064255-FL-Fla", headerDetails = "Property Details:" });
            dyB.Add(new dynaMic { countyID = "304", catID = "2", orderNumber = "29112018 064255-FL-Fla", headerDetails = "Assessment Details:" });
            dyB.Add(new dynaMic { countyID = "304", catID = "3", orderNumber = "29112018 064255-FL-Fla", headerDetails = "Tax Distribution Details:" });
            dyB.Add(new dynaMic { countyID = "304", catID = "4", orderNumber = "29112018 064255-FL-Fla", headerDetails = "Tax Due Date Details:" });
            dyB.Add(new dynaMic { countyID = "304", catID = "5", orderNumber = "29112018 064255-FL-Fla", headerDetails = "Tax Information Details:" });
            dyB.Add(new dynaMic { countyID = "304", catID = "13", orderNumber = "29112018 064255-FL-Fla", headerDetails = "Tax History Details" });
            dyB.Add(new dynaMic { countyID = "304", catID = "9", orderNumber = "29112018 064255-FL-Fla", headerDetails = "Tax Deliquent Details" });
            DynamicBind(dyB);

        }
        public DataTable BindGrid(string query)
        {
            DataTable dt = gc.GridDisplay(query);
            return dt;
        }



        public void DynamicBind(List<dynaMic> lstDy)
        {
           
            //DataTable dt = new DataTable();
            //dt.Columns.Add("State");
            //dt.Columns.Add("County");
            //DataRow dRow = dt.NewRow();
            //dRow[0] = "AZ";
            //dRow[1] = "Maricopa";
            //dt.Rows.Add(dRow);
            int i = 0;
            foreach (var objDy in lstDy)
            {
                string qry = "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + objDy.countyID + "' where DFM.Category_Id = '" + objDy.catID + "' and DVM.Order_No = '" + objDy.orderNumber + "' order by 1";
                DataTable dt = BindGrid(qry);

                Label testlbl = new Label();
                testlbl.Text = i.ToString();
                form1.Controls.Add(testlbl);
                GridView test = new GridView();
                test.ID = i.ToString();
                test.CssClass = "table table-striped table-bordered table-hover th";
                test.DataSource = dt;
                test.DataBind();
                testlbl.Text = objDy.headerDetails;
                form1.Controls.Add(test);
                i++;
            }
        }
    }
    public class dynaMic
    {
        public string countyID;
        public string catID;
        public string orderNumber;
        public string headerDetails;
    }
}