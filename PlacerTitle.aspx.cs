using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;

namespace ScrapMaricopa
{
    public partial class PlacerTitle : System.Web.UI.Page
    {

        DBconnection dbconn = new DBconnection();
        GlobalClass gc = new GlobalClass();
        MySqlParameter[] mParam;
        DataSet ds = new DataSet();
        MySqlDataReader mdr;
        DateTime dt = new DateTime();
        string strfrmdate = string.Empty;
        string strtodate = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btngetdetails_Click(object sender, EventArgs e)
        {
            //dt = Convert.ToDateTime(txtfrmdate.Text);
            //strfrmdate = String.Format("{0:MM/dd/yyyy}", dt);
            //dt = Convert.ToDateTime(txttodate.Text);
            //strtodate = String.Format("{0:MM/dd/yyyy}", dt);
            ShowDataView(strfrmdate, strtodate);
        }
        public void ShowDataView(string fdate, string tdate)
        {
            try
            {
                ds.Dispose();
                ds.Reset();
                //dt = Convert.ToDateTime(txtfrmdate.Text);
                //strfrmdate = String.Format("{0:MM/dd/yyyy}", dt);
                //dt = Convert.ToDateTime(txttodate.Text);
                //strtodate = String.Format("{0:MM/dd/yyyy}", dt);
                ds =gc.GetOrderCount_placer(strfrmdate, strtodate);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    GridView1.DataSource = ds;
                    GridView1.DataBind();
                    // Panelinformation.Visible = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void btnhome_Click(object sender, EventArgs e)
        {
            Response.Redirect("Stars.aspx");
        }


    }
}