using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Drawing.Imaging;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Drawing;
using System.Data;
using System.Net;
using System.Xml;
using System.Text.RegularExpressions;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Support.Extensions;
using System.Net;
using OpenQA.Selenium.Firefox;
using System.Diagnostics;
using OpenQA.Selenium.Remote;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;
using System.Text;
using System.Collections;
using System.Security;

namespace ScrapMaricopa
{
    public partial class TaxDetails : System.Web.UI.Page
    {
        GlobalClass gl = new GlobalClass();
        DataSet ds = new DataSet();
        DBbulk dbConn = new DBbulk();
        DBconnection newcon = new DBconnection();
        GlobalClass gc = new GlobalClass();
        string state = "", county = "", status = "", statecountyid = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string orderno = Session["ono"].ToString();
                state = Session["state"].ToString();
                county = Session["County"].ToString().Trim();
                status = Session["status"].ToString().Trim();
                if (status == "2")
                {
                    get_orderdetails(orderno);
                }
                else
                {
                    pnlnodata.Visible = true;
                }
            }
        }
        public void SelectAddressSearchType()
        {
            string searchType = ""; GlobalClass.sname = ""; GlobalClass.cname = "";
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString);
            string query = "SELECT Address_Type,State_County_Id,state_name,county_name,CountyTakenTime FROM state_county_master where State_Name = '" + state + "' and County_Name='" + county + "'";
            con.Open();
            MySqlCommand cmd = new MySqlCommand(query, con);
            MySqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                searchType = dr["Address_Type"].ToString();
                statecountyid = dr["State_County_Id"].ToString();
                GlobalClass.sname = dr["state_name"].ToString();
                GlobalClass.cname = dr["county_name"].ToString();
                HttpContext.Current.Session["CountyTakentime"] = dr["CountyTakenTime"].ToString();
            }
            dr.Close();
            con.Close();

            string Time = HttpContext.Current.Session["CountyTakentime"].ToString();
            // Cotime.Value = Time;


        }

        public void get_orderdetails(string orderno)
        {
            pnldet.Visible = true;
            ds = gl.GetCountyId(state, county);
            string addresstype = ds.Tables[0].Rows[0]["Address_Type"].ToString();
            string countyID = ds.Tables[0].Rows[0]["State_County_Id"].ToString();
            statecountyid = countyID;
            if (statecountyid == "4")
            {

                Label7.Text = "Property Details :";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details :";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Valorem Tax Details :";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Current Tax Details :";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Bill History Details :";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Due Details :";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Unpaid Total Details :";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                // MessageBox("Data Inserted Successfully....");
            }

            else if (statecountyid == "5")
            {
                Label7.Text = "Property Details :";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Appraisals Value Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Assessed and Taxable Values Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Taxes Value Details:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Bill Details:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Tax Payment History Details";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Installment Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Tax & Assessment Details ";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label11.Text = "Tax Payment Details:";
                BindGrid(GridView11, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 15 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "1")
            {


                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Assessment Details";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Property Characteristics and Values Details";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Past and Current Charges, Next Insatallment, Total Amount Due Details";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Payment History Details";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Tax Summary Details";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 13 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Tax Detail of Due Amount Details";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 10 and DVM.Order_No = '" + orderno + "' order by 1");
                Label11.Text = "Tax Payment Posted Details";
                BindGrid(GridView11, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 11 and DVM.Order_No = '" + orderno + "' order by 1");
                Label12.Text = "Special Assessment Details";
                BindGrid(GridView12, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label13.Text = "Special Assessment Current Tax Details";
                BindGrid(GridView13, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 12  and DVM.Order_No = '" + orderno + "' order by 1");
                Label14.Text = "Pay Off Breakdown Details";
                BindGrid(GridView14, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "2")
            {

                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Distribution Details:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Tax Information Details:";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Tax History Details:";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label11.Text = "Due Date Details:";
                BindGrid(GridView11, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "6")
            {
                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Information Details";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Ad Valorem Details";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax History Details";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Combine Taxes and Assessments Details";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 15 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Tax Certificate Details";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");

            }


            else if (statecountyid == "9")
            {
                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no ,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGrid(GridView2, "select order_no, parcel_no ,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Current Tax Details";
                BindGrid(GridView4, "select order_no, parcel_no ,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax History Details";
                BindGrid(GridView5, "select order_no, parcel_no ,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");


            }

            else if (statecountyid == "72")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax information Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Due Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "86")
            {

                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id where DFM.Category_Id = 16 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Tax Information Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Distribution Details";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Delinquent Details";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "36")
            {


                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Information Details";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "63")
            {
                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Assessment Details";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Real Estate Tax and Special Assessment Insatllment Details";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Real Estate,Special Assessment and Future Payment History Details";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Real Estate Tax Distribution Details";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Special Assessment Tax Insatllment Details";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Future Special Assessment Details";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8  and DVM.Order_No = '" + orderno + "' order by 1");
                Label11.Text = "Special Assesssment Tax Distribution and PrelimsDeficiencies Details";
                BindGrid(GridView11, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 10 and DVM.Order_No = '" + orderno + "' order by 1");
            }

            else if (statecountyid == "99")
            {


                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Information Details";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Distribution Details";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Bill History Details";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Sale Details";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Tax Due Date Details";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 15 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "153")
            {

                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Information Details";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax History Details";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "41")
            {

                Label6.Text = "Mobile Tax Details:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 10 and DVM.Order_No = '" + orderno + "' order by 1");
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Current Tax Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Payment History Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "73")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Payment Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Authority Details:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Balance Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Detailed Statement Details:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "74")
            {

                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Assessment Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax History Details:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Transaction Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "City Tax Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Payment Details:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Payment Details:";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "12")
            {


            }

            else if (statecountyid == "19")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Prior Assessment Details";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Installment Details";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "20")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Bill Details:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax BreakDown Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
            }

            else if (statecountyid == "22")
            {
                Label2.Text = "Assessment Details";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label7.Text = "Property Details";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Bill Details";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Bill Details";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Break down Details";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "40")
            {
                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Current Year tax information";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Payment History Details";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax summary tax information";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "23")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Installment Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Payment Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Break Down Details:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Amg Details:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Tax Bill Details:";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "32")
            {
                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Real Property Tax Details";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Real Property Payment Details";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "33")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Information Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax History Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
            }
            else if (statecountyid == "108")
            {
                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Information Details";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Value History Details";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Installment Details";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Due Details";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Tax Payment History Details";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
            }
            else if (statecountyid == "98")
            {

                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no,parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no,parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Special Assessments Details:";
                BindGrid(GridView4, "select order_no,parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Details:";
                BindGrid(GridView6, "select order_no,parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Payment Details:";
                BindGrid(GridView8, "select order_no,parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Tax Summary Details1:";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Tax Summary Details2:";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label11.Text = "2017 - 2018 PROPERTY TAXES Payment Option Details:";
                BindGrid(GridView11, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 12 and DVM.Order_No = '" + orderno + "' order by 1");
                Label12.Text = "Payment Options Details:";
                BindGrid(GridView12, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 14 and DVM.Order_No = '" + orderno + "' order by 1");
                Label13.Text = "Tax Statement1 Details:";
                BindGrid(GridView13, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 13 and DVM.Order_No = '" + orderno + "' order by 1");
                Label14.Text = "Tax Statement2 Details:";
                BindGrid(GridView14, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 15 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "29")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no,parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no,parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax History Details:";
                BindGrid(GridView4, "select order_no,parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Current Year Tax Details:";
                BindGrid(GridView5, "select order_no,parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Delinquent Taxes/Tax Sale Details:";
                BindGrid(GridView6, "select order_no,parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "30")
            {

                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no,parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no,parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax history Details:";
                BindGrid(GridView4, "select order_no,parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Taxinfo Details:";
                BindGrid(GridView5, "select order_no,parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Taxfee Details:";
                BindGrid(GridView6, "select order_no,parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");


            }


            else if (statecountyid == "34")
            {

                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no,parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no,parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                //Label7.Text = "Property Details";
                //BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id where DFM.Category_Id = 16 and DVM.Order_No = '"  +  orderno + "' order by 1");
                Label4.Text = "Tax Information Details:";
                BindGrid(GridView4, "select order_no,parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Distribution Details:";
                BindGrid(GridView5, "select order_no,parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "31")
            {

                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Assessment Details";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Account Details";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Payment History Details";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Fee Distribution Details";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Special Assessment Details";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");



            }
            else if (statecountyid == "35")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Payment History Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax information Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");

            }


            else if (statecountyid == "37")
            {
                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id where DFM.Category_Id = 16 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Tax Assessment Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Payment Details";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Installment Details";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Delinquent Tax Details";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 13 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Delinquent Tax Summary Details";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 14 and DVM.Order_No = '" + orderno + "' order by 1");


            }

            else if (statecountyid == "54")
            {

                Label7.Text = "Property Details :";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Current Tax Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Rate Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label1.Text = "Other Tax Due Details :";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "55")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax bill Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Current Tax Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Payment History Details:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Remaining Installment Balance Details:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "57")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax information Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label1.Text = "Tax History Details:";
                BindGrid(GridView1, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
            }


            else if (statecountyid == "78")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Distribution Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax History Details:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Information Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "60")
            {
                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Information Details";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Distribution Details";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Tax Flag Details";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax History Details";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Payment Details";
                BindGrid(GridView6, "select DVM.Order_No, DVM.Parcel_no,DVM.Data_Field_value,DVM.Data_Field_Text_Id from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id  join state_county_master SCM on SCM.ID = DFM.State_County_ID and DFM.State_County_ID = 60  where DFM.Category_Id = 3  and DVM.order_no='" + orderno + "' order by 1");
            }
            else if (statecountyid == "58")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no, DVM.Data_Field_value,DVM.Data_Field_Text_Id from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = 58 where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_value,DVM.Data_Field_Text_Id from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = 58 where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax History Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_value ,DVM.Data_Field_Text_Id from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = 58 where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_value ,DVM.Data_Field_Text_Id from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = 58 where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Delinquent Tax Details:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_value ,DVM.Data_Field_Text_Id from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = 58 where DFM.Category_Id = 13 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "59")
            {

                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Tax/Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax and Sewer History information:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label1.Text = "Current Tax Details:";
                BindGrid(GridView1, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Payment History Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 15 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Current Year Property Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Assessment Information Details:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Tax Information Details:";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 13 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Tax and Sewer History information:";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");


            }
            else if (statecountyid == "61")
            {

                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label1.Text = "Tax History Details:";
                BindGrid(GridView1, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax information Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Payment History Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Certificate Details:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");


            }
            else if (statecountyid == "103")
            {


                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Distribution Details";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Tax Information Details:";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Tax History Details(City Tax)";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label11.Text = "Tax information Details(City Tax)";
                BindGrid(GridView11, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label1.Text = "Tax History Details(City Tax Holly Springs)";
                BindGrid(GridView1, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax information Details(City Tax Holly Springs)";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");


            }

            else if (statecountyid == "82")
            {
                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Assessment Details";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Distribution Details";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Tax Payment History Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Tax Information Details";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "76")
            {


                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Value Information Details";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 2");
                Label8.Text = "Tax Information Details";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 2");
                Label9.Text = "Tax Payment History Details";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 2");

            }

            else if (statecountyid == "49")
            {

                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Current Tax Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label1.Text = "Default Tax Details:";
                BindGrid(GridView1, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "93")
            {

                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Value Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Current Tax Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Notices and Payments Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Property Millage Details:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "100")
            {


                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Amounts Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Payment History Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Installments Payable Details:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Delinquent Tax Information  Details:";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");


            }
            else if (statecountyid == "101")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment History Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Billing Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Payment Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Special Assessments Details:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Tax Information Details:";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "8")
            {

                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Information Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Additional Information Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "95")
            {


                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Current Tax Details";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Payment Details";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");


            }
            else if (statecountyid == "83")
            {



                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Tax information Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Distribution Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = " Due Date Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax History Details:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label1.Text = "Assessment Details:";
                BindGrid(GridView1, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");


            }

            else if (statecountyid == "80")
            {
                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Assessment Details";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Information Details";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Tax Distribution Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax History Details";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Tax Due Date Details";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 15 and DVM.Order_No = '" + orderno + "' order by 1");
                try
                {
                    BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                    Label10.Text = "Tax Sale Details";
                }
                catch { }
            }

            else if (statecountyid == "96")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                //Label5.Text = "Valorem Tax Details:";
                //BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '"  +  orderno + "' order by 1");
                Label1.Text = "Current Tax Details:";
                BindGrid(GridView1, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Payment Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Certificate Details:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Prior Deliquent Details:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 13 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            //CA_Santabarbara
            else if (statecountyid == "88")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Jurisdiction Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Current Tax Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Payment Receipts Details:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = " Taxes and Values Details:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            //hamilton
            else if (statecountyid == "94")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Assessment History Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Information Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Distribution Details:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Payment Details:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "122")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax History Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax information Details:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Distribution Details:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "13")
            {
                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Summary Details";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Assessment Details";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Activites Details";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Due Details";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Special Tax Details";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Detailed Tax Statement Details";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 10 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "62")
            {
                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Information Details";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Jurisdictions Details";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Flags Details";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax History Details";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "129")
            {
                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Value Summary Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Ad Valorem Taxes,Assessment and Value History Details";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Taxes Details";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Account Details";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Non-Ad Valorem Assessments and Taxes Details";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Tax Status Details";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Payment History Details";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
            }

            else if (statecountyid == "3")
            {

                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Property Assessment Values Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Exemption and Taxable Values Details";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Real Estate Account Details";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Ad Valorem Details";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Combine Taxes and Assessment Details";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Tax Non-Ad Valorem Details";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Tax History Details";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");


            }
            else if (statecountyid == "7")
            {
                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Historical Value Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Taxes Benefits Details";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Taxable Values, Certified Taxes and Non Ad Valorem Assessment Details";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Payment History Details";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Bill Details";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Unpaid Real Estate Details";


            }

            //IN_Marion
            else if (statecountyid == "97")
            {


                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Current Tax Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax History Details";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "10")
            {
                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessor's Value Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Bill Summary Details";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Secure and Supplimental Annual Tax Bill Details";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 13 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Direct Levy Tax Bill Details";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Payment History Details";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
            }
            else if (statecountyid == "112")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax History Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Valorem Tax Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Current Tax Details:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Issue Certificate Details:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Payment Details:";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            //san luis obispo
            else if (statecountyid == "125")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Sale Comments Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
            }

            else if (statecountyid == "119")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Account Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Ad Valorem Taxes Details:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Status Details:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Payment History Details:";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "132")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label1.Text = "Tax Information Details:";
                BindGrid(GridView1, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "115")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Information Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Current Year Tax Details:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Distribution Details:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "159")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Bill Details";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Tax Payment Details";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Tax History";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
            }

            else if (statecountyid == "116")
            {
                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Information Details";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Payment Receipt Details";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "114")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label1.Text = "Tax History Details:";
                BindGrid(GridView1, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Information Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Distribution Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Special Assessments Details";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Utility Payment History Details:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Delinquent Details:";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Tax Information(City Tax-Havre de Grace)";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 10 and DVM.Order_No = '" + orderno + "' order by 1");
                Label11.Text = "City Tax";
                BindGrid(GridView11, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 11 and DVM.Order_No = '" + orderno + "' order by 1");
                Label12.Text = "Tax Information(City Tax-Aberdeen )";
                BindGrid(GridView12, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 12 and DVM.Order_No = '" + orderno + "' order by 1");
                Label13.Text = "Tax History(City Tax-Aberdeen )";
                BindGrid(GridView13, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 13 and DVM.Order_No = '" + orderno + "' order by 1");
                Label14.Text = "City Tax";
                BindGrid(GridView14, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 14 and DVM.Order_No = '" + orderno + "' order by 1");
            }


            else if (statecountyid == "128")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label1.Text = "Tax Information Details:";
                BindGrid(GridView1, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
            }

            else if (statecountyid == "138")
            {
                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Certified Tax Roll Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Account Details";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Values/Reductions/Exemptions Details";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Pay Terms Details";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Ad Valorem Details";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Non-Ad Valorem Details";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Payment Installment Details";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
            }

            else if (statecountyid == "139")
            {
                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Information Details";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Special Assessment Details";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "141")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label1.Text = "Tax Payment Details ";
                BindGrid(GridView1, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Bill Details";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Payoff Details ";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Current Tax Statement Details:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "149")
            {
                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Special Assessment Details";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax History Details";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Information Details";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Tax Distribution Details";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Tax Payment History Details";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
            }



            else if (statecountyid == "151")
            {

                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax History Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Valorem Tax Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Payment  Details:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Current Tax  Details:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Certificate  Details:";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "140")
            {


                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Tax Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Account Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Pay Tax Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Payment History Details";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");


            }
            else if (statecountyid == "152")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label1.Text = "Tax information Details:";
                BindGrid(GridView1, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Distribution Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Delinquent Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "158")
            {

                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label1.Text = "Tax History Details:";
                BindGrid(GridView1, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax information Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Delinquent Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
            }
            else if (statecountyid == "18")
            {

                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Information";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Receipt";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Distribution Details";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Authority Details";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");


            }

            else if (statecountyid == "15")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Supplemental Tax Bill Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                try
                {
                    Label5.Text = "Supplemental Tax Bill Details:";
                    BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                    BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                }
                catch
                { }
                Label8.Text = "Tax Payment History Details:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");


            }
            else if (statecountyid == "17")
            {


                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Payment History Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Secured(Current Tax) Bills Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Unsecured Bills Details:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");


            }
            else if (statecountyid == "173")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Payment Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");


            }
            else if (statecountyid == "160")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label1.Text = "Tax Sale History Details:";
                BindGrid(GridView1, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax History Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Information Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Receipt Details:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "16")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label1.Text = "Tax information Details:";
                BindGrid(GridView1, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Deliquent:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "174")
            {
                Label7.Text = ("Property Details Table");
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = ("Assessment Details Table");
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = ("Tax Payment History Details Table");
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = ("Tax Information Details Table");
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = ("Tax Information Details");
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = ("Tax Information Details");
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "185")
            {
                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Information";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Distribution Details";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax History Details";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");


            }
            else if (statecountyid == "111")
            {
                Label7.Text = ("Property Details Table:");
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = ("Assessment Details Table:");
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = ("Tax Payment Receipt Details:");
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = ("Tax Information Details:");
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = ("Penalty Dates Details:");
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = ("Tax Authority Details:");
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");


            }

            else if (statecountyid == "164")
            {
                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Information Details";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Receipt Details";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Distribution Details";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Distribution Details";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");


            }

            else if (statecountyid == "165")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Payment Details Table: ";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Tax Bill Details Table:";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            //Medina

            else if (statecountyid == "186")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Bill";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Tax Distribution Details Table";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Special Assessment Details";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label11.Text = "Tax Information ";
                BindGrid(GridView11, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label1.Text = "Tax History Details Table:";
                BindGrid(GridView1, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "192")
            {
                Label3.Text = "Property Details:";
                BindGrid(GridView3, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id where DFM.Category_Id = 16 and DVM.Order_No = '" + orderno + "' order by 1");

                Label7.Text = "Property Details1:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label1.Text = "Tax information Details:";
                BindGrid(GridView1, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Distribution Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Delinquent Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "190")
            {
                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Bill Details Table";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Due Amounts Details Table";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Payment History Details Table:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Taxing Bodies Details Table";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "166")
            {

                Label7.Text = ("Property Details:");
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = ("Assessment Details:");
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = ("Tax Payment Details:");
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = ("Tax Bill Details:");
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = ("Tax Breakdown Details:");
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = ("Tax Authority Details:");
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = ("Deliquent Tax Details:");
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "189")
            {
                Label7.Text = ("Property Details Table");
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = ("Assessment Details Table");
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = ("Tax Information Details");
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = ("Delinquent Tax Information Details Table");
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 13 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            // stafford

            else if (statecountyid == "197")
            {

                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Details:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Tax Payment History Details:";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "176")
            {
                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment History Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Value Summary Details";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Information Details";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Billed - Property code Details";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Due - Trasaction Details";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Tax Due Details";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Payment Details";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "178")
            {


                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Value Summary Details ";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label13.Text = " Certified Tax Roll Values Details Table";
                BindGrid(GridView13, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Distribution Details:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Tax Information Details:";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Tax History Details:";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label11.Text = "Due Date Details:";
                BindGrid(GridView11, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");


            }
            else if (statecountyid == "193")
            {
                Label7.Text = ("Property Details:");
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = ("Current Values Details:");
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = ("History of Assessed Values Details:");
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = ("Tax Payment History Details:");
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = ("Tax Information Details:");
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = ("Tax Receipt Details:");
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = ("Ad Valorem/Non-Ad Valorem Taxes Details:");
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = ("Taxing Authority Details:");
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 15 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "181")
            {

                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Parish Millage Details Table";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "City Tax";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Due Amounts Details Table";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Homestead Details Table";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Payment History Details Table";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");



            }
            else if (statecountyid == "38")
            {

                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label1.Text = "Entity Details Table:";
                BindGrid(GridView1, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Assessment History Table:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Distribution Table:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Information Table:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Payment History Details Table:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");


            }
            else if (statecountyid == "196")
            {


                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Bill";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Tax History";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "City Tax Information";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label11.Text = "City Tax History ";
                BindGrid(GridView11, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");




            }
            else if (statecountyid == "204")
            {


                Label7.Text = ("Property Details:");
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = ("Assessment Details:");
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = ("Tax Payment Details:");
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = ("Tax Bill Details:");
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = ("Tax Breakdown Details:");
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = ("Tax Authority Details:");
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = ("Deliquent Tax Details:");
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = ("City Tax Payment History Details:");
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label11.Text = ("City Tax Information Details:");
                BindGrid(GridView11, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 10 and DVM.Order_No = '" + orderno + "' order by 1");
                Label12.Text = ("City Assessment Details:");
                BindGrid(GridView12, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 11 and DVM.Order_No = '" + orderno + "' order by 1");
                Label13.Text = ("City Entity Tax Details:");
                BindGrid(GridView13, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 12 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "175")
            {


                Label7.Text = ("Property Details Table");
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = ("Current Year Assessment Details Table");
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = ("Assessment Details Table");
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = ("Tax Information Details Table");
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = ("Tax History Details");
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = ("Delinquent Tax Information Details Table");
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 13 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = ("Tax Distribution Details Table");
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "200")
            {

                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Assessment History Table:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax information Table:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Current Year Tax Table:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Prior Year Tax Table:";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 13 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Tax Distribution Details Table:";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label11.Text = "Tax History Table:";
                BindGrid(GridView11, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");


            }
            else if (statecountyid == "208")
            {

                Label7.Text = ("Property Details:");
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = ("Assessment Details:");
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = ("County Tax History Details:");
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = ("County Tax information Details:");
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = ("City Tax History Details:");
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = ("City Tax Details:");
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = ("City Tax information Details:");
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
            }
            else if (statecountyid == "85")
            {


                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label1.Text = "Jurisdiction Details Table(Assessment)";
                BindGrid(GridView1, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = " Values Details Table:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Due (Payoff Calculation) Details Table:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 12 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Jurisdiction Details Table(Tax)";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Taxes Due Detail by Year Details Table: ";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Tax Payment Details Table: ";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Tax Bill Details Table: ";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label11.Text = "City of McAllen Tax Details Table: ";
                BindGrid(GridView11, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 10 and DVM.Order_No = '" + orderno + "' order by 1");
                Label12.Text = "Payment History(City Tax) ";
                BindGrid(GridView12, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 11 and DVM.Order_No = '" + orderno + "' order by 1");


            }
            else if (statecountyid == "154")
            {

                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label1.Text = "Entity Details Table:";
                BindGrid(GridView1, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Assessment History Table:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Distribution Table:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Information Table:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Payment History Details Table:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");



            }
            else if (statecountyid == "211")
            {


                Label7.Text = ("Property Details:");
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = ("Assessment Details:");
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = ("Tax Payment Details:");
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = ("Tax Bill Details:");
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = ("Tax Status Details1:");
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = ("Tax Status Details2:");
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = ("Tax Authority Details:");
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "142")
            {


                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment History table";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label1.Text = "Entity Details Table:";
                BindGrid(GridView1, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = " Assessment Details Table:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Due Details Table:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id =5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Information Table:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Distribution Details Table:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "201")
            {
                Label7.Text = ("Property Details Table");
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = ("Assessment Details Table");
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = ("Appraised Value Details");
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = ("Tax Bill Details Table");
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = ("Payment Details Table:");
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = ("Tax And Assessment Details");
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "206")
            {



                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax information Table:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Payment History Table";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Delinquent Details Table ";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");


            }
            else if (statecountyid == "205")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Account";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Account Summary";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Current Tax Details Table";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Tax Account Transaction Details";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Tax Distribution Details Table:";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "137")
            {

                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label1.Text = "Jurisdiction Details Table(Assessment)";
                BindGrid(GridView1, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = " Values Details Table:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Due Details Table:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Missing Entity";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "212")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Bill";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");


            }
            else if (statecountyid == "161")
            {

                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Values Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = ("Taxing Jurisdiction Details:");
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = ("Roll Value History Details:");
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = ("Tax Bill Details:");
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = ("Jurisdiction Information Details:");
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = ("Tax Payment Details:");
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Taxes Due Detail by Year Details Table:";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 13 and DVM.Order_No = '" + orderno + "' order by 1");
                Label11.Text = "Tax Authority Table:";
                BindGrid(GridView11, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 15 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "207")
            {


                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Current Tax Details Table";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Account";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Account Summary";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Tax Account Property";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Tax Account Transaction Details";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label11.Text = "Tax Distribution Details Table:";
                BindGrid(GridView11, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label12.Text = "Tax Payment Receipts Details";
                BindGrid(GridView12, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 10 and DVM.Order_No = '" + orderno + "' order by 1");


            }
            else if (statecountyid == "214")
            {

                Label7.Text = ("Property Details Table");
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = ("Assessment Details Table");
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = ("Tax Information Details 1");
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = ("Tax Information Details 2");
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = ("Payment History Details");
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");


            }

            else if (statecountyid == "180")
            {


                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Values Details Table:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label1.Text = "Jurisdiction Details Table(Assessment)";
                BindGrid(GridView1, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Roll Value History Details Table:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Due Details Table:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Missing Entity";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 10 and DVM.Order_No = '" + orderno + "' order by 1");

                Label9.Text = ("Tax Payment Details:");
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Tax Breakdown Details Table:";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label11.Text = "Tax Bill Details Table:";
                BindGrid(GridView11, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");



            }
            else if (statecountyid == "79")
            {
                Label7.Text = ("Property Details Table");
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = ("Assessment Details Table");
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = ("Previous Appraisals Details");
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = ("Tax Information Details");
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = ("Payment History Details");
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = ("Parcel Details");
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "48")
            {

                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Installment Table";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Amg Table:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Bill Table:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "191")
            {
                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment History Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Value Summary Details";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Information Details";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Billed - Property code Details";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Due - Trasaction Details";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Tax Due Details";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Payment Details";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
            }
            else if (statecountyid == "195")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax information Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Due Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "146")
            {
                Label7.Text = ("Property Details Table");
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = ("Assessment Details Table");
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = ("Tax Payment Details");
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = ("Tax Delinquent Details ");
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = ("Tax Info Details");
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");

                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            if (statecountyid == "43")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax information Table:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "199")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Bill";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Payment Information Details";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Charges Details";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Assessment value Details";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Assessment value Details1";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Assessment History Details";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label11.Text = "Tax Payment History Table";
                BindGrid(GridView11, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 10 and DVM.Order_No = '" + orderno + "' order by 1");
                Label12.Text = "City Tax Information ";
                BindGrid(GridView12, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 11 and DVM.Order_No = '" + orderno + "' order by 1");
                Label13.Text = "City Tax Payment Information Details ";
                BindGrid(GridView13, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 12 and DVM.Order_No = '" + orderno + "' order by 1");


            }
            else if (statecountyid == "143")
            {

                Label7.Text = ("Property Details:");
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = ("Tax, Levies & Assessments1 Details:");
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = ("Tax, Levies & Assessments2 Details:");
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = ("Value & Tax History  Details:");
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = ("Tax Payment History Details:");
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = ("Tax Distribution Details:");
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = ("Tax Due Status Second Half Details:");
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = ("Tax Due Status First Half Details:");
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 11 and DVM.Order_No = '" + orderno + "' order by 1");
                Label11.Text = ("Tax Authority Details:");
                BindGrid(GridView11, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 12 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "64")
            {

                Label7.Text = ("Property Details Table");
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = ("Assessment Details Table");
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = ("Levy Details Table");
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = ("Tax Information Details");
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = ("Tax Payment History Details");
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = ("Property Tax Detail information Alert Message");
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "14")
            {
                Label7.Text = ("Assessment Details:");
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = ("Secure / Supplemental Details:");
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = ("Values & Exemptions Details:");
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = ("Taxing Authority Details:");
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "179")
            {

                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax information Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Current tax information";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Delinquent Details Table:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Additional Tax Details Table:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Tax History Details Table:";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Special Assessments Table:";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 12 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "198")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Information Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Payment Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Sale Information:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "135")
            {
                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Homestead Details";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Current Tax Details";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Status Details";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Payment History Details";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Tax Bill Details";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");

            }


            else if (statecountyid == "120")
            {

                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details1:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = ("Assessment Details2:");
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = ("Tax Distribution Details:");
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = ("Tax information Details1:");
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = ("Tax information Details2:");
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = ("Tax information Details3:");
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Payment Options Details:";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 13 and DVM.Order_No = '" + orderno + "' order by 1");
                Label11.Text = "Tax Authority Details:";
                BindGrid(GridView11, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 15 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "213")
            {
                Label7.Text = ("Property Details Table");
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = ("Tax Assessment Details Table");
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = ("Tax Details Table");
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = ("Tax Information Details");
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = ("Property Tax Billing History Details Table");
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = ("Delinquent Tax Details Table");
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = ("Property Tax Detail information Alert Message");
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 10 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            //FL Escambia

            else if (statecountyid == "202")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details Table:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label1.Text = "Tax History";
                BindGrid(GridView1, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Distribution Table:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Due Date Details Table:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax information Table";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = ("Tax Payment Details:");
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "188")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Values Details Table:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label1.Text = "Jurisdiction Details Table(Assessment)";
                BindGrid(GridView1, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = " Roll Value History Details Table:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Taxes Due Detail by Year Details Table:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Payment Details Table: ";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Information ";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Alert Table ";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "155")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Values Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = ("Taxing Jurisdiction Details:");
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = ("Roll Value History Details:");
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = ("Tax Bill Details:");
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = ("Tax Breakdown Details:");
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = ("Tax Payment Details:");
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label11.Text = "Tax Authority Details:";
                BindGrid(GridView11, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 15 and DVM.Order_No = '" + orderno + "' order by 1");

            }


            else if (statecountyid == "163")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax information Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Code Details:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Default Tax";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");


            }

            else if (statecountyid == "162")
            {
                Label7.Text = ("Property Details:");
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = ("Assessment Details:");
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = ("Entity Details:");
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = ("Assessment History Details:");
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = ("Tax Distribution Details:");
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = ("Tax Information Details:");
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = ("Tax Payment History Details:");
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "157")
            {
                Label7.Text = ("Property Details Table");
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = ("Assessment History Table");
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = ("Tax Payment Bill History Details Table");
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = ("Bill Value Assessment Details Table");
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = ("Bill Value Transanction Assessment Details Table");
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = ("Owner Information Details Table");
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            // Yamhill

            else if (statecountyid == "156")
            {
                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment History Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Information Table";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Payment History Table";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Due Details Table";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax History Table";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "226")
            {
                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Tax Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Assessment Details";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Secured Tax Assessment Details";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Secured Supplemental Tax AssessmentDetails";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Payment History Details";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Defaulted Tax Details";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Defaulted Tax Amount Due Details";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
            }

            else if (statecountyid == "216")
            {
                Label7.Text = "Tax Bill Details Table:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Tax Exemptions Details Table";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Delinquent Taxes (Current)";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Commends";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Sold Taxes Details Table";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = " Delinquent Tax Search Status";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "92")
            {

                Label7.Text = ("Property Details Table");
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = ("Assessment Details");
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = ("Current Tax Infomation Details Table");
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = ("Tax History Details");
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = ("Tax  Distribution Details Table");
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = ("Tax Information Details With Taxauthority Table");
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = ("Tax  Distribution Installments Details ");
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "77")
            {


                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Values Details Table:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label1.Text = "Jurisdiction Details Table(Assessment)";
                BindGrid(GridView1, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = " Roll Value History Details Table:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Taxes Due Detail by Year Details Table:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Payment Details Table: ";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Information ";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Alert Table ";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "MUD Property Details";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 10 and DVM.Order_No = '" + orderno + "' order by 1");
                Label11.Text = "MUD Valuation Summary Details";
                BindGrid(GridView11, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 11 and DVM.Order_No = '" + orderno + "' order by 1");
                Label12.Text = "MUD Payment Details";
                BindGrid(GridView12, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 12 and DVM.Order_No = '" + orderno + "' order by 1");
                Label13.Text = "MUD Tax Due Details";
                BindGrid(GridView13, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 13 and DVM.Order_No = '" + orderno + "' order by 1");
                Label14.Text = "MUD Tax Statement Details";
                BindGrid(GridView14, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 14 and DVM.Order_No = '" + orderno + "' order by 1");
                BindGrid(GridView15, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 15 and DVM.Order_No = '" + orderno + "' order by 1");
                Label16.Text = "Utility Property Details";
                BindGrid(GridView16, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 16 and DVM.Order_No = '" + orderno + "' order by 1");
                Label17.Text = "Utility Taxing Details";
                BindGrid(GridView17, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 17 and DVM.Order_No = '" + orderno + "' order by 1");
                Label18.Text = "Utility Postmarked Details";
                BindGrid(GridView18, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 18 and DVM.Order_No = '" + orderno + "' order by 1");
                Label19.Text = "Utility Payment Details";
                BindGrid(GridView19, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 19 and DVM.Order_No = '" + orderno + "' order by 1");
                Label20.Text = "Alert Message Details";
                BindGrid(GridView20, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 20 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "184")
            {
                Label7.Text = ("Property Details:");
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = ("Values Details:");
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = ("Taxing Jurisdiction Details:");
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = ("Roll Value History Details:");
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = ("Tax Bill Details:");
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = ("Taxing Unit Details:");
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = ("Tax Payment Details:");
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label11.Text = "Previous Year(s) Taxes Due By Year Details:";
                BindGrid(GridView11, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 15 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "124")
            {

                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Information Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Payment Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Fee Breakdown";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "81")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Values Details Table:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label1.Text = "Jurisdiction Details Table(Assessment)";
                BindGrid(GridView1, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = " Roll Value History Details Table:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Taxes Due Detail by Year Details Table:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Payment Details Table: ";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Information ";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Creek ISD Property Details";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Creek ISD Due Details";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 10 and DVM.Order_No = '" + orderno + "' order by 1");
                Label11.Text = "Creek ISD Tax Details";
                BindGrid(GridView11, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 11 and DVM.Order_No = '" + orderno + "' order by 1");
                Label12.Text = "Creek ISD Payment Details";
                BindGrid(GridView12, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 12 and DVM.Order_No = '" + orderno + "' order by 1");
                Label13.Text = "Texas City Property Details";
                BindGrid(GridView13, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 13 and DVM.Order_No = '" + orderno + "' order by 1");
                Label14.Text = "Texas City Jurisdiction Information Details";
                BindGrid(GridView14, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 14 and DVM.Order_No = '" + orderno + "' order by 1");
                Label15.Text = "Texas City ISD Tax Due Details";
                BindGrid(GridView15, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 15 and DVM.Order_No = '" + orderno + "' order by 1");
                Label16.Text = "Texas City ISD Tax Reciept Details";
                BindGrid(GridView16, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 16 and DVM.Order_No = '" + orderno + "' order by 1");
                Label17.Text = "Friendswood ISD Tax Property Details";
                BindGrid(GridView17, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 17 and DVM.Order_No = '" + orderno + "' order by 1");
                Label18.Text = "Friendswood ISD Tax Exemption and Valuation Details";
                BindGrid(GridView18, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 18 and DVM.Order_No = '" + orderno + "' order by 1");
                Label19.Text = "Friendswood ISD Tax Payment Details";
                BindGrid(GridView19, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 19 and DVM.Order_No = '" + orderno + "' order by 1");
                Label20.Text = "Friendswood ISD Tax Due Details";
                BindGrid(GridView20, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 20 and DVM.Order_No = '" + orderno + "' order by 1");
                Label21.Text = "Friendswood ISD Tax Statement Details";
                BindGrid(GridView21, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 21 and DVM.Order_No = '" + orderno + "' order by 1");
                Label22.Text = "Friendswood ISD Tax Reciept Details";
                BindGrid(GridView22, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 22 and DVM.Order_No = '" + orderno + "' order by 1");
                Label23.Text = "MUD Tax Details";
                BindGrid(GridView23, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 23 and DVM.Order_No = '" + orderno + "' order by 1");
                Label24.Text = "MUD Tax Due Details";
                BindGrid(GridView24, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 24 and DVM.Order_No = '" + orderno + "' order by 1");
                Label25.Text = "PID Statement Details";
                BindGrid(GridView25, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 25 and DVM.Order_No = '" + orderno + "' order by 1");
                Label26.Text = "Current PID Details";
                BindGrid(GridView26, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 26 and DVM.Order_No = '" + orderno + "' order by 1");
                Label27.Text = "PID Payment Details";
                BindGrid(GridView27, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 27 and DVM.Order_No = '" + orderno + "' order by 1");
                Label28.Text = "Assessments of The Southwest Inc.Tax Details";
                BindGrid(GridView28, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 29 and DVM.Order_No = '" + orderno + "' order by 1");
                Label29.Text = "Assessments of The Southwest Inc. Valuation Details";
                BindGrid(GridView29, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 30 and DVM.Order_No = '" + orderno + "' order by 1");
                Label30.Text = "Assessments of The Southwest Inc Payment Details";
                BindGrid(GridView30, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 31 and DVM.Order_No = '" + orderno + "' order by 1");
                Label31.Text = "Assessments of The Southwest Inc Tax Due Details";
                BindGrid(GridView31, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 32 and DVM.Order_No = '" + orderno + "' order by 1");
                Label32.Text = "Assessments of The Southwest Inc Tax Statements Details";
                BindGrid(GridView32, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 33 and DVM.Order_No = '" + orderno + "' order by 1");
                Label33.Text = "Assessments of The Southwest Inc Tax Reciept Details";
                BindGrid(GridView33, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 34 and DVM.Order_No = '" + orderno + "' order by 1");
                Label34.Text = "Utility Property Details";
                BindGrid(GridView34, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 35 and DVM.Order_No = '" + orderno + "' order by 1");
                Label35.Text = "Utility Taxing Details";
                BindGrid(GridView35, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 36 and DVM.Order_No = '" + orderno + "' order by 1");
                Label36.Text = "Utility Postmarked Details";
                BindGrid(GridView36, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 37 and DVM.Order_No = '" + orderno + "' order by 1");
                Label37.Text = "Utility Payment Details";
                BindGrid(GridView37, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 38 and DVM.Order_No = '" + orderno + "' order by 1");
                Label38.Text = "Alert Message Details";
                BindGrid(GridView38, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 28 and DVM.Order_No = '" + orderno + "' order by 1");


            }
            else if (statecountyid == "52")
            {

                Label7.Text = "Property Details Table";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details Table:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Entity Details Table:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Taxes Due Detail by Year";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Payment History Table:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Information Table:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Tax Distribution Details Table:";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "127")
            {

                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment History Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Rollback Summary";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Summary Details";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Payment History";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Distribution Details";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Special Assessments";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Tax Without Payments";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label11.Text = "Tax Information";
                BindGrid(GridView11, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 10 and DVM.Order_No = '" + orderno + "' order by 1");
                Label12.Text = "Alert Message";
                BindGrid(GridView12, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 11 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "106")
            {
                Label7.Text = "Property Details Table";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Appraisals Details Table:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Assessments Details Table:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Special Assessments Details Table:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Billings Details Table:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Authorities Details Table:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Transaction History Details Table:";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "182")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment details:";
                BindGrid(GridView2, "select order_no, parcel_no,dvm.data_field_text_id,dvm.data_field_value from data_value_master dvm join data_field_master dfm on dfm.id = dvm.data_field_text_id join state_county_master scm on scm.id = dfm.state_county_id and scm.state_county_id = '" + statecountyid + "' where dfm.category_id = 2 and dvm.order_no = '" + orderno + "' order by 1");
                Label1.Text = "Entity Details Table:";
                BindGrid(GridView1, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Assessment History Table:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Taxes Due Detail by Year Details Table:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Payment Details Table:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Information Details:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Mud Details Table:";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Mud Taxing Details Table:";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 11 and DVM.Order_No = '" + orderno + "' order by 1");
                Label11.Text = "Post Marked Details Table:";
                BindGrid(GridView11, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 12 and DVM.Order_No = '" + orderno + "' order by 1");
                Label12.Text = "Mud Payment Details Table:";
                BindGrid(GridView12, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 13 and DVM.Order_No = '" + orderno + "' order by 1");
                Label13.Text = "Fort Bend CO MUD Tax Details Table:";
                BindGrid(GridView13, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 17 and DVM.Order_No = '" + orderno + "' order by 1");
                Label14.Text = "Current and Other Year(s) Due Details Table:";
                BindGrid(GridView14, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 18 and DVM.Order_No = '" + orderno + "' order by 1");
                Label15.Text = "Blueridge West MUD Tax Details Table:";
                BindGrid(GridView15, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 10 and DVM.Order_No = '" + orderno + "' order by 1");
                Label16.Text = "MUD Tax Summary Details Table:";
                BindGrid(GridView16, "select order_no, parcel_no,dvm.data_field_text_id,dvm.data_field_value from data_value_master dvm join data_field_master dfm on dfm.id = dvm.data_field_text_id join state_county_master scm on scm.id = dfm.state_county_id and scm.state_county_id = '" + statecountyid + "' where dfm.category_id = 14 and dvm.order_no = '" + orderno + "' order by 1");
                Label17.Text = "Penalties for Paying Late Details Table:";
                BindGrid(GridView17, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 15 and DVM.Order_No = '" + orderno + "' order by 1");
                Label18.Text = "Assessments of The Southwest Inc Tax Due Details Table:";
                BindGrid(GridView18, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 16 and DVM.Order_No = '" + orderno + "' order by 1");
                Label19.Text = "Assessments of The Southwest Inc.Tax Details Table";
                BindGrid(GridView19, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 19 and DVM.Order_No = '" + orderno + "' order by 1");
                Label20.Text = "Assessments of The Southwest Inc. Valuation Details Table:";
                BindGrid(GridView20, "select order_no, parcel_no,dvm.data_field_text_id,dvm.data_field_value from data_value_master dvm join data_field_master dfm on dfm.id = dvm.data_field_text_id join state_county_master scm on scm.id = dfm.state_county_id and scm.state_county_id = '" + statecountyid + "' where dfm.category_id = 20 and dvm.order_no = '" + orderno + "' order by 1");
                Label21.Text = "Assessments of The Southwest Inc Payment Details Table:";
                BindGrid(GridView21, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 21 and DVM.Order_No = '" + orderno + "' order by 1");
                Label22.Text = "Assessments of The Southwest Inc Tax Due Details Table:";
                BindGrid(GridView22, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 22 and DVM.Order_No = '" + orderno + "' order by 1");
                Label23.Text = "Assessments of The Southwest Inc Tax Statements Details Table:";
                BindGrid(GridView23, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 23 and DVM.Order_No = '" + orderno + "' order by 1");
                Label24.Text = "Assessments of The Southwest Inc Tax Statements Details Table:";
                BindGrid(GridView24, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 24 and DVM.Order_No = '" + orderno + "' order by 1");
                Label25.Text = "Assessments of The Southwest Inc Tax Due Details Table:";
                BindGrid(GridView25, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 25 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "109")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Payment Details Table";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Current Year Tax Table:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Delinquent Details Table:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "177")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Information Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Payment Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                if (HttpContext.Current.Session["multiparcel_MontgomeryAL_TaxSale"] != null && HttpContext.Current.Session["multiparcel_MontgomeryAL_TaxSale"].ToString() == "Yes")
                {
                    HttpContext.Current.Session["multiparcel_MontgomeryAL_TaxSale"] = null;
                    Label6.Text = "Tax Sale Details";
                    BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                }

            }
            // Mohave_AZ

            else if (statecountyid == "117")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Account Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Assessment Value Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Area Details:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Property Code Details:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Tax Summary Details:";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Transaction Details Table:";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");



            }
            else if (statecountyid == "381")
            {
                Label4.Text = "Tax information Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Code Details:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Default Tax";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "400")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label1.Text = "Tax information Details:";
                BindGrid(GridView1, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Distribution Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Delinquent Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "113")
            {
                Label7.Text = ("Property Details Table");
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = ("Assessment Details Table");
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = ("Tax Values Assessment Details Table:");
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = ("Secured Property Tax Bill Details Table:");
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = ("Tax  Summary Details Table");
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = ("Secured Property Tax Charges Details Table:");
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = ("Tax Payment History Details ");
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label11.Text = ("Delinquent Details Alert Comments");
                BindGrid(GridView11, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "133")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Values Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Current Taxes Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Special Assessments Taxes Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax History Details:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Payments Details:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Alert Comment:";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "422")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Information Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Payment Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Sale Information:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
            }

            else if (statecountyid == "302")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment History Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax History Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Valorem Tax Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Payment  Details:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Certificate  Details:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Current Tax  Details:";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
            }

            else if (statecountyid == "144")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label1.Text = "Tax information Details:";
                BindGrid(GridView1, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Special Assessment Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Distribution Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
            }
            else if (statecountyid == "294")
            {
                BindGridDisplay(GridView7, Label7, statecountyid, "1", orderno, "Property Details");
                BindGridDisplay(GridView2, Label2, statecountyid, "2", orderno, "Assessment Details");
                BindGridDisplay(GridView4, Label4, statecountyid, "3", orderno, "Current Tax Details");
                BindGridDisplay(GridView5, Label5, statecountyid, "4", orderno, "Tax Charges Details Table:");
                BindGridDisplay(GridView6, Label6, statecountyid, "5", orderno, "Tax History Details Table:");
            }
            else if (statecountyid == "11")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Current Tax Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax History Details";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
            }
         


            else if (statecountyid == "65")
            {

                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Real Estate Account Details";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Ad Valorem Details";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Combine Taxes and Assessment Details";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Tax Non-Ad Valorem Details";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Tax History Details";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
            }

            //Ab Team
            else if (statecountyid == "105")
            {
                Label7.Text = "Property Details";
                BindGridAB(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGridAB(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Payment Details";
                BindGridAB(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Information Details";
                BindGridAB(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
            }
            else if (statecountyid == "69")
            {
                Label7.Text = "Property Details";
                BindGridAB(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGridAB(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Information Details";
                BindGridAB(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Account Value Details";
                BindGridAB(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Transaction & Summary Details";
                BindGridAB(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
            }
            else if (countyID == "44")
            {
                Label7.Text = "Property Details";
                BindGridAB(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Current Tax and Payment Details";
                BindGridAB(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
            }

            else if (countyID == "167")
            {
                Label7.Text = "Property Details";
                BindGridAB(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Tax Information Details";
                BindGridAB(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Assessment Details";
                BindGridAB(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax History Details";
                BindGridAB(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Sale Details";
                BindGridAB(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 13 and DVM.Order_No = '" + orderno + "' order by 1");
            }

            //beaufort
            else if (countyID == "148")
            {
                Label7.Text = "Property Details";
                BindGridAB(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGridAB(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Information Details";
                BindGridAB(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax History Details";
                BindGridAB(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Distribution Details";
                BindGridAB(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Delinquent Details";
                BindGridAB(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
            }
            //Benton AR
            else if (countyID == "84")
            {
                Label7.Text = "Property Details";
                BindGridAB(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGridAB(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Information Details";
                BindGridAB(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Current Tax Details";
                BindGridAB(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Payment Reciept Details";
                BindGridAB(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
            }
           
            else if (countyID == "27")
            {
                Label7.Text = "Property Details";
                BindGridAB(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGridAB(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "City Tax Information Details";
                BindGridAB(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Payment History Details";
                BindGridAB(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
            }
            //Cuyahoga
            else if (countyID == "26")
            {
                BindGridAB(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                BindGridAB(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                BindGridAB(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                BindGridAB(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            //Douglas
            else if (countyID == "145")
            {
                Label7.Text = "Property Details";
                BindGridAB(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGridAB(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Payment Details";
                BindGridAB(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Bill Details";
                BindGridAB(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
            }

            //Elpasco CO
            else if (countyID == "66")
            {
                Label7.Text = "Property Details";
                BindGridAB(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGridAB(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Current Year Payment Due Details";
                BindGridAB(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Property Details";
                BindGridAB(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Current Year Payment Due Details";
                BindGridAB(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
            }
            else if (countyID == "126")
            {

                Label7.Text = "Property Details";
                BindGridAB(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGridAB(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Account Details";
                BindGridAB(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Current Tax Statement Details";
                BindGridAB(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Payment Details";
                BindGridAB(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
            }

            else if (countyID == "107")
            {
                Label7.Text = "Property Details";
                BindGridAB(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Current Tax Information Details";
                BindGridAB(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Distribution Details";
                BindGridAB(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Payment Details";
                BindGridAB(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax History Details";
                BindGridAB(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
            }
            else if (countyID == "104")
            {
                Label7.Text = "Property Details";
                BindGridAB(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGridAB(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Information Details";
                BindGridAB(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Current Year Tax Information Details";
                BindGridAB(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Payment Details";
                BindGridAB(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax History Details";
                BindGridAB(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Tax Payment History Details";
                BindGridAB(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Tax History Details";
                BindGridAB(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label11.Text = "Tax Payment Details";
                BindGridAB(GridView11, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 13 and DVM.Order_No = '" + orderno + "' order by 1");
            }
            else if (countyID == "87")
            {
                BindGridAB(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                BindGridAB(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                BindGridAB(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                BindGridAB(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                BindGridAB(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
            }
            else if (countyID == "47")
            {
                Label7.Text = "Property Details";
                BindGridAB(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGridAB(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Current Year Tax Information Details";
                BindGridAB(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Information Details";
                BindGridAB(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Payment Details";
                BindGridAB(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
            }
            else if (countyID == "70")
            {
                Label7.Text = "Property Details";
                BindGridAB(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGridAB(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Current Tax Information Details";
                BindGridAB(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Payment History Details";
                BindGridAB(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Parcel Details";
                BindGridAB(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
            }
            //Jackson 
            else if (countyID == "45")
            {
                Label7.Text = "Property Details";
                BindGridAB(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGridAB(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Current Tax Information Details";
                BindGridAB(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Payment Details";
                BindGridAB(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Distribution Of Current Taxes Details";
                BindGridAB(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
            }
            else if (countyID == "168")
            {
                Label7.Text = "Property Details";
                BindGridAB(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGridAB(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Payment Details";
                BindGridAB(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Bill Details";
                BindGridAB(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Bodies Details";
                BindGridAB(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Exemptions Details";
                BindGridAB(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
            }
            //LakeIL
            else if (countyID == "169")
            {
                BindGridAB(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                BindGridAB(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                BindGridAB(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                BindGridAB(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                BindGridAB(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
            }
            else if (countyID == "136")
            {
                Label7.Text = "Property Details";
                BindGridAB(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGridAB(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Current Tax Installment Details";
                BindGridAB(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Information Details";
                BindGridAB(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Distribution Details";
                BindGridAB(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
            }
            //Lake OH
            else if (countyID == "118")
            {
                Label7.Text = "Property Details";
                BindGridAB(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGridAB(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Information Details";
                BindGridAB(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Payment Details";
                BindGridAB(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Special Assessment Details";
                BindGridAB(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
            }
            else if (countyID == "46")
            {
                Label7.Text = "Property Details";
                BindGridAB(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGridAB(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Information Details";
                BindGridAB(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Ad Valorem Details";
                BindGridAB(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Non-Ad Valorem Details";
                BindGridAB(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax History Details";
                BindGridAB(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
            }
            else if (countyID == "134")
            {
                Label7.Text = "Property Details";
                BindGridAB(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGridAB(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = " Details";
                BindGridAB(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (countyID == "131")
            {
                BindGridAB(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                BindGridAB(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                BindGridAB(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                BindGridAB(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                BindGridAB(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                BindGridAB(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
            }
            else if (countyID == "147")
            {
                Label7.Text = "Property Details";
                BindGridAB(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessmnet Details";
                BindGridAB(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Payment Details";
                BindGridAB(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Information Details";
                BindGridAB(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax History Details";
                BindGridAB(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (countyID == "172")
            {
                Label7.Text = "Property Details";
                BindGridAB(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGridAB(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Bodies Details";
                BindGridAB(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Information Details";
                BindGridAB(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Bill Details";
                BindGridAB(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Exemptions Details";
                BindGridAB(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
            }
            else if (countyID == "105")
            {
                Label7.Text = "Property Details";
                BindGridAB(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGridAB(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Payment Details";
                BindGridAB(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Information Details";
                BindGridAB(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
            }
            else if (countyID == "25")
            {
                BindGridDisplay(GridView7, Label7, statecountyid, "1", orderno, "Property Details");
                BindGridDisplay(GridView2, Label2, statecountyid, "2", orderno, "Assessment Details");
                BindGridDisplay(GridView4, Label4, statecountyid, "3", orderno, "Tax Information Details");
                BindGridDisplay(GridView5, Label5, statecountyid, "4", orderno, "Tax History Details:");
                BindGridDisplay(GridView6, Label6, statecountyid, "5", orderno, "Tax Due Details:");
            }
            else if (countyID == "50")
            {
                BindGridAB(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                BindGridAB(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                BindGridAB(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                BindGridAB(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                BindGridAB(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
            }
            else if (countyID == "90")
            {
                BindGridAB(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                BindGridAB(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                BindGridAB(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                BindGridAB(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                BindGridAB(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
            }
            else if (countyID == "110")
            {
                Label7.Text = "Property Details";
                BindGridAB(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGridAB(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Due Details";
                BindGridAB(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Information Details";
                BindGridAB(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "UnPaid Delinquent Details";
                BindGridAB(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Paid Delinquent Details";
                BindGridAB(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (countyID == "28")
            {
                BindGridDisplay(GridView7, Label7, statecountyid, "1", orderno, "Property Details:");
                BindGridDisplay(GridView2, Label2, statecountyid, "2", orderno, "Assessment Details:");
                BindGridDisplay(GridView1, Label1, statecountyid, "5", orderno, "Tax Installment Current and Prior Year Details:");
                BindGridDisplay(GridView4, Label4, statecountyid, "4", orderno, "Tax Distribution Details:");
                BindGridDisplay(GridView5, Label5, statecountyid, "3", orderno, "Tax Payment History Details:");
                BindGridDisplay(GridView6, Label6, statecountyid, "6", orderno, "Tax Delinquent Details:");
            }
            else if (countyID == "91")
            {
                BindGridAB(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                BindGridAB(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                BindGridAB(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                BindGridAB(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                BindGridAB(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                BindGridAB(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
            }

            else if (countyID == "170")
            {
                Label7.Text = "Property Details";
                BindGridAB(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Installment Details";
                BindGridAB(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Payment History Details";
                BindGridAB(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Details";
                BindGridAB(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Bill Total Details";
                BindGridAB(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGridAB(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 15 and DVM.Order_No = '" + orderno + "' order by 1");
            }
            else if (countyID == "89")
            {
                Label7.Text = "Property Details";
                BindGridAB(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGridAB(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Information Details";
                BindGridAB(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Authority Details";
                BindGridAB(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax History Details";
                BindGridAB(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Due Date Details";
                BindGridAB(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
            }
            else if (countyID == "71")
            {
                Label7.Text = "Property Details";
                BindGridAB(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGridAB(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax History Details";
                BindGridAB(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
            }

            else if (statecountyid == "304")
            {

                Label7.Text = ("Property Details:");
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = ("Assessment Details:");
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label1.Text = "Tax Distribution Details:";
                BindGrid(GridView1, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Due Date Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Information Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax History Details";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 13 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Tax Deliquent Details";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Tax Sale Details";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 10 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "314")
            {


                Label7.Text = ("Property Details:");
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = ("Assessment Details:");
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label1.Text = "Tax Distribution Details:";
                BindGrid(GridView1, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Due Date Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Information Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax History Details";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 13 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Tax Deliquent Details";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "393")
            {



                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment History Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax History Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Valorem Tax Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Payment  Details:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Current Tax  Details:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Certificate  Details:";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");

            }
            else if (statecountyid == "287")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details Table:";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label1.Text = "Special Assessment Table";
                BindGrid(GridView1, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax Distribution Table:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Due Date Details Table:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax information Table";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = ("Tax Payment Details:");
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = ("Tax Payment Details:");
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");


            }


            else if (statecountyid == "294")
            {
                BindGridDisplay(GridView7, Label7, statecountyid, "1", orderno, "Property Details");
                BindGridDisplay(GridView2, Label2, statecountyid, "2", orderno, "Assessment Details");
                BindGridDisplay(GridView4, Label4, statecountyid, "3", orderno, "Current Tax Details");
                BindGridDisplay(GridView5, Label5, statecountyid, "4", orderno, "Tax Charges Details Table:");
                BindGridDisplay(GridView6, Label6, statecountyid, "5", orderno, "Tax History Details Table:");


            }
            else if (statecountyid == "341")
            {
                BindGridDisplay(GridView7, Label7, statecountyid, "1", orderno, "Property Details");
                BindGridDisplay(GridView2, Label2, statecountyid, "2", orderno, "Assessment Details");
                BindGridDisplay(GridView4, Label4, statecountyid, "3", orderno, "Tax History Details");
                BindGridDisplay(GridView5, Label5, statecountyid, "4", orderno, "Tax Distribution Table:");
                BindGridDisplay(GridView6, Label6, statecountyid, "5", orderno, "Due Date Details Table:");
                BindGridDisplay(GridView8, Label8, statecountyid, "6", orderno, "Tax information Table");
                BindGridDisplay(GridView9, Label9, statecountyid, "8", orderno, "Tax information Table 1");
            }

            else if (statecountyid == "24")
            {

                BindGridDisplay(GridView7, Label7, statecountyid, "1", orderno, "Property Details:");
                BindGridDisplay(GridView2, Label2, statecountyid, "2", orderno, "Assessment Details:");
                BindGridDisplay(GridView1, Label1, statecountyid, "3", orderno, "Tax information Details:");
                BindGridDisplay(GridView4, Label4, statecountyid, "4", orderno, "Tax Installment Details:");
                BindGridDisplay(GridView5, Label5, statecountyid, "5", orderno, "Tax History Details:");
                BindGridDisplay(GridView6, Label6, statecountyid, "6", orderno, "Tax Summary Details:");
                BindGridDisplay(GridView8, Label8, statecountyid, "8", orderno, "Tax Statement Details:");
                BindGridDisplay(GridView9, Label9, statecountyid, "9", orderno, "City Tax information Details:");
                BindGridDisplay(GridView10, Label10, statecountyid, "10", orderno, "City Transactions Details:");
                BindGridDisplay(GridView11, Label11, statecountyid, "11", orderno, "City Repayment Details:");

            }

            else if (statecountyid == "28")
            {

                BindGridDisplay(GridView7, Label7, statecountyid, "1", orderno, "Property Details:");
                BindGridDisplay(GridView2, Label2, statecountyid, "2", orderno, "Assessment Details:");
                BindGridDisplay(GridView1, Label1, statecountyid, "5", orderno, "Tax Installment Current and Prior Year Details:");
                BindGridDisplay(GridView4, Label4, statecountyid, "4", orderno, "Tax Distribution Details:");
                BindGridDisplay(GridView5, Label5, statecountyid, "3", orderno, "Tax Payment History Details:");


            }

            else if (statecountyid == "25")
            {
                BindGridDisplay(GridView7, Label7, statecountyid, "1", orderno, "Property Details");
                BindGridDisplay(GridView2, Label2, statecountyid, "2", orderno, "Assessment Details");
                BindGridDisplay(GridView4, Label4, statecountyid, "3", orderno, "Tax Information Details");
                BindGridDisplay(GridView5, Label5, statecountyid, "4", orderno, "Tax History Details:");

            }

            else if (statecountyid == "45")
            {
                BindGridDisplay(GridView7, Label7, statecountyid, "1", orderno, "Property Details");
                BindGridDisplay(GridView2, Label2, statecountyid, "2", orderno, "Assessment Details");
                BindGridDisplay(GridView4, Label4, statecountyid, "3", orderno, "Current Tax Details");
                BindGridDisplay(GridView5, Label5, statecountyid, "4", orderno, "Tax Payment History Details");
                BindGridDisplay(GridView6, Label6, statecountyid, "5", orderno, "Delinquent Tax Details");

            }

            else if (statecountyid == "351")
            {

                BindGridDisplay(GridView7, Label7, statecountyid, "1", orderno, "Property Details");
                BindGridDisplay(GridView2, Label2, statecountyid, "2", orderno, "Assessment Details");
                BindGridDisplay(GridView4, Label4, statecountyid, "3", orderno, "Tax Information Details");
                BindGridDisplay(GridView5, Label5, statecountyid, "4", orderno, "Special Assessment Details:");
                BindGridDisplay(GridView6, Label6, statecountyid, "5", orderno, "Tax Payment History Details:");

            }
            else if (statecountyid == "150")
            {
                Label7.Text = ("Property Details Table");
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = ("Assessment Details Table");
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = ("Tax Payment History Details Table");
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = ("Tax Information Details Table");
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = ("Tax Information Details");
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = ("Tax Information Details");
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");

            }

            else if (statecountyid == "323")
            {

                BindGridDisplay(GridView7, Label7, statecountyid, "1", orderno, "Property Details");
                BindGridDisplay(GridView2, Label2, statecountyid, "2", orderno, "Assessment Details");
                BindGridDisplay(GridView4, Label4, statecountyid, "3", orderno, "Tax Bill Details");
                BindGridDisplay(GridView5, Label5, statecountyid, "4", orderno, "Tax Breakdown Details");
                BindGridDisplay(GridView6, Label6, statecountyid, "5", orderno, "Tax Payment History Details");
                BindGridDisplay(GridView8, Label8, statecountyid, "6", orderno, "Tax Information Details");
                BindGridDisplay(GridView9, Label9, statecountyid, "8", orderno, "Tax Payment status Details");



            }
            else if (statecountyid == "356")
            {

                BindGridDisplay(GridView7, Label7, statecountyid, "1", orderno, "Property Details");
                BindGridDisplay(GridView2, Label2, statecountyid, "2", orderno, "Assessment Details");
                BindGridDisplay(GridView4, Label4, statecountyid, "3", orderno, "Tax Information Details");
                BindGridDisplay(GridView5, Label5, statecountyid, "4", orderno, "Ad Valorem And Non Ad Valorem Taxes Details");
                BindGridDisplay(GridView6, Label6, statecountyid, "5", orderno, " Fees And Interest Details");
                BindGridDisplay(GridView8, Label8, statecountyid, "6", orderno, "Payments Details");
                BindGridDisplay(GridView9, Label9, statecountyid, "8", orderno, "Payments History Details");
                BindGridDisplay(GridView10, Label10, statecountyid, "9", orderno, "Parcel History Details");
                BindGridDisplay(GridView11, Label11, statecountyid, "10", orderno, "Bankrupt Alert Message Details");



            }
            else if (statecountyid == "369")
            {
                Label7.Text = "Property Details";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label5.Text = "Tax Info Details:";
                BindGrid(GridView5, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Payments/Adjustments Details:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Charge Details:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Tax History Details:";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
                Label12.Text = "Tax Lien view Payment";
                BindGrid(GridView12, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 11 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Tax Lien Detail";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Tax Lien Payments/Adjustments Detail";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label11.Text = "Tax Assessment";
                BindGrid(GridView11, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 10 and DVM.Order_No = '" + orderno + "' order by 1");
            }
            else if (statecountyid == "240")
            {
                Label7.Text = "Property Details:";
                BindGrid(GridView7, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 1 and DVM.Order_No = '" + orderno + "' order by 1");
                Label2.Text = "Assessment Details";
                BindGrid(GridView2, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 2 and DVM.Order_No = '" + orderno + "' order by 1");
                Label4.Text = "Secured And Supplemental Details";
                BindGrid(GridView4, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 12 and DVM.Order_No = '" + orderno + "' order by 1");
                Label6.Text = "Tax Installment Details:";
                BindGrid(GridView6, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                Label8.Text = "Tax Charge Details:";
                BindGrid(GridView8, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 5 and DVM.Order_No = '" + orderno + "' order by 1");
                Label9.Text = "Payment Details:";
                BindGrid(GridView9, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 6 and DVM.Order_No = '" + orderno + "' order by 1");
                Label10.Text = "Tax Bill Delinquent Details:";
                BindGrid(GridView10, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 8 and DVM.Order_No = '" + orderno + "' order by 1");
                Label11.Text = " Prior Year Delinquent Tax Details";
                BindGrid(GridView11, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                Label12.Text = "Delinquent Tax Year Details";
                BindGrid(GridView12, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 10 and DVM.Order_No = '" + orderno + "' order by 1");
                Label13.Text = "Delinquent Tax Comments";
                BindGrid(GridView13, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 11 and DVM.Order_No = '" + orderno + "' order by 1");
                Label14.Text = "Tax Bill Details:";
                BindGrid(GridView14, "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + statecountyid + "' where DFM.Category_Id = 3 and DVM.Order_No = '" + orderno + "' order by 1");
            }
            else if (statecountyid == "294")
            {
                BindGridDisplay(GridView7, Label7, statecountyid, "1", orderno, "Property Details");
                BindGridDisplay(GridView2, Label2, statecountyid, "2", orderno, "Assessment Details");
                BindGridDisplay(GridView4, Label4, statecountyid, "3", orderno, "Current Tax Details");
                BindGridDisplay(GridView5, Label5, statecountyid, "4", orderno, "Tax Payment Detail Table:");
                BindGridDisplay(GridView6, Label6, statecountyid, "5", orderno, "Tax Charges Details Table:");
                BindGridDisplay(GridView8, Label8, statecountyid, "6", orderno, "Tax History Details Table:");
                BindGridDisplay(GridView9, Label9, statecountyid, "8", orderno, "Lien View Details Table:");
                BindGridDisplay(GridView10, Label10, statecountyid, "9", orderno, "Tax Assessment Details Table:");
            }

            bloc2.Visible = true;

        }

        //public void BindGrid(GridView grdname, string query)
        //{
        //    DataTable dt = gl.readdatafromcloud(query);
        //    grdname.DataSource = dt;
        //    grdname.DataBind();
        //}
        public void BindGridDisplay(GridView grdname, Label lbl, string stateCountyID, string categoryID, string orderNumber, string gridHeader)
        {
            string query = "select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + stateCountyID + "' where DFM.Category_Id = '" + categoryID + "' and DVM.Order_No = '" + orderNumber + "' order by 1";
            DataTable dt = gc.GridDisplay(query);
            if (dt.Rows.Count > 0)
            {
                dt.Columns.Remove("order_no");
            }
            grdname.DataSource = dt;
            grdname.DataBind();
            lbl.Text = gridHeader;
        }
        public void BindGridAB(GridView grdname, string query)
        {
            DataTable dt = gl.readdatafromcloudAB(query);
            grdname.DataSource = dt;
            grdname.DataBind();
        }
        public void BindGrid(GridView grdname, string query)
        {
            DataTable dt = gc.GridDisplay(query);
            if (dt.Rows.Count > 0)
            {
                dt.Columns.Remove("order_no");
            }
            grdname.DataSource = dt;
            grdname.DataBind();
        }
        public DataTable GridDisplay(string Query)
        {

            DataSet ds = newcon.ExecuteQuery(Query);
            DataTable dTable = new DataTable();
            if (ds.Tables[0].Rows.Count > 0)
            {
                string data_text_id = ds.Tables[0].Rows[0]["Data_Field_Text_Id"].ToString();
                string order_no = ds.Tables[0].Rows[0]["order_no"].ToString();

                DataSet dsField = newcon.ExecuteQuery("select Data_Fields_Text from data_field_master where id='" + data_text_id + "'");
                string columnName = "order_no" + "~" + "Parcel_No" + "~" + dsField.Tables[0].Rows[0]["Data_Fields_Text"].ToString();
                string[] columnArray = columnName.Split('~');


                foreach (string cName in columnArray)
                {
                    dTable.Columns.Add(cName);
                }
                DataRow dr = dTable.NewRow();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    dTable.Rows.Add();

                }

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    string rowvals = order_no + "~" + ds.Tables[0].Rows[i]["parcel_no"].ToString() + "~" + ds.Tables[0].Rows[i]["Data_Field_value"].ToString();
                    string[] rowValue = rowvals.Split('~');
                    for (int k = 0; k < rowValue.Count(); k++)
                    {
                        dTable.Rows[i][k] = rowValue[k];

                    }
                }
            }

            return dTable;

        }
        public DataSet GetCountyId(string state, string county)
        {

            string query = "SELECT Address_Type,State_County_Id,state_name,county_name ,service_url FROM state_county_master where State_Name = '" + state + "' and County_Name='" + county + "'";
            DataSet ds = new DataSet();
            ds = dbConn.ExecuteQuery(query);
            return ds;
        }
    }
}

