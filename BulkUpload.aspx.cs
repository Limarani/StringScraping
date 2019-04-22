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
    public partial class BulkUpload : System.Web.UI.Page
    {
            DBbulk dbConn = new DBbulk();
            MySqlParameter[] mParam;
            DataSet ds = new DataSet();
            MySqlDataReader mdr;
            DateTime dt = new DateTime();
            string strfrmdate = string.Empty;
            string strtodate = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void BtnSubmit_Click(object sender, EventArgs e)
        {
            GetBulk();
            transmitgridbind();
           
            //string batchNo="", orderNo = "", county = "", state = "", address = "", pdate = DateTime.Now.ToString("MM/dd/yyyy");
            //string txt = txtordertype.Text;
            //string[] splitOrderDetails = txt.Split('\n');

            //int count = 0;

            //foreach (string orderDetails in splitOrderDetails)
            //{
            //    if (orderDetails != "")
            //    {
            //        string[] splitOrder = orderDetails.Split('\t');
            //        if (splitOrder.Count() == 5)
            //        {
            //            batchNo = splitOrder[0];
            //            orderNo = splitOrder[1];
            //            address = splitOrder[2];
            //            state = splitOrder[3];
            //            county = splitOrder[4];

            //            //order exists
            //            string exists = dbConn.ExecuteScalar("select count(order_no) from bulk_record_status where order_no='" + orderNo + "'");
            //            if (exists == "0")
            //            {
            //                count++;
            //                dbConn.ExecuteQuery("insert into bulk_record_status(Batch_No,order_no,pdate,state,county,address,scrape,scrape_status) values('" + batchNo + "','" + orderNo + "','" + pdate + "','" + state + "','" + county + "','" + address + "','1','0')");
            //            }
            //        }

            //    }

            //}
            //lblstatus.Text = count.ToString() + " order(s) uploaded successfully...";
            //txtordertype.Text = "";
        }

        public void InsertOrders(GridView MyGridView, Label txtduplicate, Label lblassigncount, Label lblduplicatecount)
        {
            int acount = 0, dcount = 0;
            string duplicateorders = string.Empty;
            foreach (GridViewRow gvr in MyGridView.Rows)
            {
                int result = 0;
                string batchno = gvr.Cells[0].Text.Trim();
                string orderno = gvr.Cells[1].Text.Trim();
                string pdate = gvr.Cells[2].Text.Trim();
                string address = gvr.Cells[3].Text.Trim();
                string state = gvr.Cells[4].Text.Trim();
                string county = gvr.Cells[5].Text.Trim();
                
                mParam = new MySqlParameter[6];
                mParam[0] = new MySqlParameter("?$batchNo", batchno);
                mParam[0].MySqlDbType = MySqlDbType.VarChar;

                mParam[1] = new MySqlParameter("?$orderNo", orderno);
                mParam[1].MySqlDbType = MySqlDbType.VarChar;

                mParam[2] = new MySqlParameter("?$pdate", pdate);
                mParam[2].MySqlDbType = MySqlDbType.VarChar;
                mParam[2].IsNullable = false;

                mParam[3] = new MySqlParameter("?$state", state);
                mParam[3].MySqlDbType = MySqlDbType.VarChar;

                mParam[4] = new MySqlParameter("?$county", county);
                mParam[4].MySqlDbType = MySqlDbType.VarChar;

                mParam[5] = new MySqlParameter("?$address", address);
                mParam[5].MySqlDbType = MySqlDbType.VarChar;
                

                result = dbConn.ExecuteSPNonQuery("sp_insert_order", true, mParam);

                if (result > 0) acount += 1;
                else
                {
                    dcount += 1;
                    if (dcount == 0) duplicateorders += orderno;
                    else duplicateorders += orderno + ", ";
                }
                txtduplicate.Text = duplicateorders;
                lblassigncount.Text = acount.ToString();
                lblduplicatecount.Text = dcount.ToString();
            }
        }
        private bool assigngrid_Validation()
        {
            if (gridtransmit.Rows.Count == 0) return false;
            return true;
        }
        private void transmitgridbind()
        {
            DataSet dExcel = new DataSet();
            string[] row;
            string[] col;
            DataTable dt = new DataTable();
            DataRow dr;
            dt.Columns.Add("Batch No");
            dt.Columns.Add("Order No");
            dt.Columns.Add("Pdate");
            dt.Columns.Add("Address");
            dt.Columns.Add("State");
            dt.Columns.Add("County");
            string todaydate = String.Format("{0:MM/dd/yyyy}", DateTime.Now);
            txtordertype.Text = txtordertype.Text.Trim('\r', '\n');
            row = txtordertype.Text.Split('\n');
            foreach (string rowdata in row)
            {
                col = rowdata.Split('\t', '\r');
                dr = dt.NewRow();
                dr[0] = col[0].ToString();
                dr[1] = col[1].ToString(); ;
                dr[2] = todaydate;
                dr[3] = col[2].ToString();
                dr[4] = col[3].ToString();
                dr[5] = col[4].ToString();
                dt.Rows.Add(dr);
                gridtransmit.Visible = true;
                gridtransmit.DataSource = dt;
                gridtransmit.DataBind();
                txtordertype.Text = "";

            }
        }
        public DataSet Getorderdetails()
        {
            DataSet ds = new DataSet();
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["BulkMyConnection"].ConnectionString);
            con.Open();
            string query = "SELECT bacth_no,order_no,county,state,borrowername ,address, scrape_status FROM bulk_record_status";
            MySqlCommand cmd = new MySqlCommand(query, con);
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(ds);
            con.Close();
            return ds;
        }

        public DataSet GetBulk()
        {
            DataSet ds = new DataSet();
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["BulkMyConnection"].ConnectionString);
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("sp_bulk", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                MySqlDataAdapter da = new MySqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(ds);
            }
            return ds;
        }

        public void ShowDataView(string fdate, string tdate)
        {
            try
            {
                ds.Dispose();
                ds.Reset();
                dt = Convert.ToDateTime(txtfrmdate.Text);
                strfrmdate = String.Format("{0:MM/dd/yyyy}", dt);
                dt = Convert.ToDateTime(txttodate.Text);
                strtodate = String.Format("{0:MM/dd/yyyy}", dt);
                ds = GetOrderCount_scrape(strfrmdate, strtodate);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    grdorderbatch.DataSource = ds;
                    grdorderbatch.DataBind();
                   // Panelinformation.Visible = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataSet GetOrderCount_scrape(string fdate, string tdate)
        {
            mParam = new MySqlParameter[2];
            mParam[0] = new MySqlParameter("?$fdate", fdate);
            mParam[0].MySqlDbType = MySqlDbType.VarChar;

            mParam[1] = new MySqlParameter("?$tdate", tdate);
            mParam[1].MySqlDbType = MySqlDbType.VarChar;
            return dbConn.Executedataset("Sp_getordercount_stars", true, mParam);

        }
        //protected void ftlnkbtnscrapcomp_Click(object sender, EventArgs e)
        //{
        //    Session["ViewParcel"] = "";
        //    LoadDaywiseCount("ScrapingCompleted");
        //}
        //protected void ftlnkbtnyts_Click(object sender, EventArgs e)
        //{
        //    LoadDaywiseCount("YTS");
        //}
        //protected void ftlnkbtnerror_Click(object sender, EventArgs e)
        //{
        //    Session["ViewParcel"] = "";
        //    LoadDaywiseCount("SCRAPINGERROR");
        //}
        //protected void ftlnkbtnmulti_Click(object sender, EventArgs e)
        //{
        //    Session["ViewParcel"] = "Yes";
        //    LoadDaywiseCount("MULTIPARCEL");
        //}

        //private void LoadDaywiseCount(string status)
        //{
        //    grdorderdet.Visible = false;
        //    try
        //    {
        //        dt = Convert.ToDateTime(txtfrmdate.Text);
        //        strfrmdate = String.Format("{0:MM/dd/yyyy}", dt);
        //        dt = Convert.ToDateTime(txttodate.Text);
        //        strtodate = String.Format("{0:MM/dd/yyyy}", dt);
        //        ds.Dispose();
        //        ds.Reset();
        //        string strquery = "sp_getbatchorder_stars";
        //        mdr = Tracking(strquery,);
        //        ShowGrid(mdr, strfrmdate, strtodate);

        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        private void batchorderdetails(string batchno)
        {
            grdorderdet.Visible = false;
            try
            {
                dt = Convert.ToDateTime(txtfrmdate.Text);
                strfrmdate = String.Format("{0:MM/dd/yyyy}", dt);
                dt = Convert.ToDateTime(txttodate.Text);
                strtodate = String.Format("{0:MM/dd/yyyy}", dt);
                ds.Dispose();
                ds.Reset();
                string strquery = "sp_getbatchorder_stars";
                mdr = Tracking(strquery, batchno);
                ShowGrid(mdr, strfrmdate, strtodate);

            }
            catch
            {

            }
        }
        public void ShowGrid(MySqlDataReader mdr, string fdate, string tdate)
        {
            try
            {
                DataView dataview = ConvertDataReaderToDataView(mdr);
                DataTable dt = dataview.ToTable();

                if (dt.Rows.Count > 0)
                {
                    grdorderdet.DataSource = dt;
                    grdorderdet.DataBind();
                }
                else
                {
                    dt.Rows.Add(dt.NewRow());
                    grdorderdet.DataSource = dt;
                    grdorderdet.DataBind();
                    int Totalcolumns = grdorderdet.Rows[0].Cells.Count;
                    grdorderdet.Rows[0].Cells.Clear();
                    grdorderdet.Rows[0].Cells.Add(new TableCell());
                    grdorderdet.Rows[0].Cells[0].ColumnSpan = Totalcolumns;
                    grdorderdet.Rows[0].Cells[0].Text = "No Records Found";
                    grdorderdet.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Left;
                    grdorderdet.Rows[0].Cells[0].VerticalAlign = VerticalAlign.Middle;
                }

                grdorderdet.Visible = true;
            }
            catch (Exception ex)
            {
                //  errorlabel.Text = ex.ToString();
            }
        }
        public DataView ConvertDataReaderToDataView(MySqlDataReader reader)
        {
            DataView dview;
            DataTable schemaTable = new DataTable();
            schemaTable = reader.GetSchemaTable();
            DataTable DtTable = new DataTable();
            DataColumn Dtcolumn;

            Dtcolumn = new DataColumn();
            Dtcolumn.DataType = System.Type.GetType("System.Int32");
            Dtcolumn.ColumnName = "SlNo";
            Dtcolumn.Caption = "SlNo";
            Dtcolumn.ReadOnly = true;
            Dtcolumn.Unique = true;
            DtTable.Columns.Add(Dtcolumn);

            Dtcolumn = new DataColumn();
            Dtcolumn.DataType = System.Type.GetType("System.String");
            Dtcolumn.ColumnName = "Batch No";
            Dtcolumn.Caption = "Batch No";
            DtTable.Columns.Add(Dtcolumn);

            Dtcolumn = new DataColumn();
            Dtcolumn.DataType = System.Type.GetType("System.String");
            Dtcolumn.ColumnName = "Order No";
            Dtcolumn.Caption = "Order No";
            DtTable.Columns.Add(Dtcolumn);

            Dtcolumn = new DataColumn();
            Dtcolumn.DataType = System.Type.GetType("System.String");
            Dtcolumn.ColumnName = "Date";
            Dtcolumn.Caption = "Date";
            Dtcolumn.ReadOnly = true;
            DtTable.Columns.Add(Dtcolumn);

            Dtcolumn = new DataColumn();
            Dtcolumn.DataType = System.Type.GetType("System.String");
            Dtcolumn.ColumnName = "State";
            Dtcolumn.Caption = "State";
            DtTable.Columns.Add(Dtcolumn);

            Dtcolumn = new DataColumn();
            Dtcolumn.DataType = System.Type.GetType("System.String");
            Dtcolumn.ColumnName = "County";
            Dtcolumn.Caption = "County";
            DtTable.Columns.Add(Dtcolumn);

            Dtcolumn = new DataColumn();
            Dtcolumn.DataType = System.Type.GetType("System.String");
            Dtcolumn.ColumnName = "Address";
            Dtcolumn.Caption = "Address";
            DtTable.Columns.Add(Dtcolumn);

            Dtcolumn = new DataColumn();
            Dtcolumn.DataType = System.Type.GetType("System.String");
            Dtcolumn.ColumnName = "Scrape Status";
            Dtcolumn.Caption = "Scrape Status";
            DtTable.Columns.Add(Dtcolumn);


            Dtcolumn = new DataColumn();
            Dtcolumn.DataType = System.Type.GetType("System.String");
            Dtcolumn.ColumnName = "id";
            Dtcolumn.Caption = "id";
            DtTable.Columns.Add(Dtcolumn);


            Int16 i = 1;

            string scrape_status = "";
            if (reader.HasRows == false)
            {
                //DataRow emptyRow = DtTable.NewRow();
                //DtTable.Rows.Add(emptyRow);
                //dview = new DataView(DtTable);
                //return dview;
            }
            while (reader.Read())
            {
                DataRow dtRow = DtTable.NewRow();
                dtRow[0] = i;
                dtRow[1] = reader["Batch"];
                dtRow[2] = reader["Order_no"];
                dtRow[3] = reader["pdate"];
                //if (dtRow[2] != "")
                //{
                //    DateTime pDt = Convert.ToDateTime(dtRow[2].ToString());
                //    dtRow[2] = String.Format("{0:dd-MMM-yy}", pDt);
                //}
                //dtRow[3] = reader["Downloadtime"];
                ////New
                //dtRow[4] = reader["AssignedDate"];

                //dtRow[5] = reader["serpro"];

                //New
                //dtRow[6] = reader["prior"];
                //dtRow[7] = reader["Time_Zone"];
                dtRow[4] = reader["State"];
                dtRow[5] = reader["County"];
                dtRow[6] = reader["Address"];
                //dtRow[10] = reader["Township"];
                //dtRow[11] = reader["webphone"];
                //dtRow[12] = reader["OrderType"];

                scrape_status = reader["Scrape Status"].ToString();

                if (scrape_status == "2")
                {
                    dtRow[7] = "Scraping Completed";
                }
                else if (scrape_status == "4")
                {
                    dtRow[7] = "Multiparcel";
                }
                else if (scrape_status == "3")
                {
                    dtRow[7] = "Scraping Error";
                }
                else if (scrape_status == "0")
                {
                    dtRow[7] = "Scraping Not Yet Started";
                }
                else if (scrape_status == "5")
                {
                    dtRow[7] = "Manual";
                }
                dtRow[8] = reader["id"];
                //if (loc == "1")
                //{ dtRow[13] = "Locked"; }
                //else if (k1 == "0" && qc == "0")
                //{ dtRow[13] = "YTS"; }
                //else if (k1 == "1" && qc == "0")
                //{ dtRow[13] = "Key Started"; }
                //else if (k1 == "2" && qc == "0" && key_status != "Others")
                //{ dtRow[13] = "Key Completed"; }
                //else if (k1 == "2" && qc == "0" && key_status == "Others")
                //{ dtRow[13] = "Others"; }
                //else if (k1 == "2" && qc == "1")
                //{ dtRow[13] = "QC Started"; }
                //else if (k1 == "5" && qc == "5" && stat == "5")
                //{ dtRow[13] = "Delivered"; }
                //else if (k1 == "4" && qc == "4" && stat == "4")
                //{ dtRow[13] = "On Hold"; }
                //else if (k1 == "7" && qc == "7" && stat == "7")
                //{ dtRow[13] = "Rejected"; }
                //else if (k1 == "9" && qc == "9" && stat == "9" && review == "9")
                //{ dtRow[13] = "Order Missing"; }

                //if (pend == "3")
                //{ dtRow[13] = "In Process"; }
                //else if (pend == "1")
                //{ dtRow[13] = "In Process Started"; }

                //else if (pend == "1" && k1 == "1")
                //{ dtRow[10] = "In Process Key Start"; }
                //else if (pend == "3" && qc == "2")
                //{ dtRow[10] = "In Process"; }
                //else if (pend == "1")
                //{ dtRow[10] = "In Process"; }
                //if (tax == "3")
                //{ dtRow[9] = "Mail Away"; }
                //if (parcel == "3")
                //{ dtRow[10] = "ParcelID"; }

                //if (tax == "3")
                //{ dtRow[13] = "Mail Away"; }
                //else if (tax == "1")
                //{ dtRow[13] = "Mail Away Started"; }

                //if (parcel == "3")
                //{ dtRow[13] = "ParcelID"; }
                //else if (parcel == "1")
                //{ dtRow[13] = "ParcelID Started"; }

                //dtRow[14] = reader["k1_op"];
                //dtRow[15] = reader["Lastcomment"];
                //dtRow[16] = reader["k1_st"];
                //dtRow[17] = reader["k1_et"];

                //if (dtRow[16].ToString() != "" && dtRow[17].ToString() != "")
                //{
                //    DateTime StTime = DateTime.Parse(dtRow[16].ToString());
                //    DateTime EnTime = DateTime.Parse(dtRow[17].ToString());
                //    TimeSpan TimeDiff = EnTime.Subtract(StTime);
                //    dtRow[18] = TimeDiff;

                //}
                //dtRow[19] = reader["qc_op"];
                //dtRow[20] = reader["qc_st"];
                //dtRow[21] = reader["qc_et"];

                //if (dtRow[20].ToString() != "" && dtRow[21].ToString() != "")
                //{
                //    DateTime StTime = DateTime.Parse(dtRow[20].ToString());
                //    DateTime EnTime = DateTime.Parse(dtRow[21].ToString());
                //    TimeSpan TimeDiff = EnTime.Subtract(StTime);
                //    dtRow[22] = TimeDiff;
                //}


                //dtRow[23] = reader["uploadtime"];

                //string strtat = reader["TAT_Rep"].ToString();
                //if (strtat == "0") dtRow[24] = "No";
                //else if (strtat == "1") dtRow[24] = "Yes";
                //dtRow[25] = reader["Delivered"];
                //dtRow[26] = reader["id"];

                //if (k1 == "5" && qc == "5" && stat == "5" && review == "5") dtRow[27] = "Yes";
                //else dtRow[27] = "No";


                i += 1;
                DtTable.Rows.Add(dtRow);
            }
            dview = new DataView(DtTable);
            return dview;
        }
        public MySqlDataReader Tracking(string procedure, string batchno)
        {
            mParam = new MySqlParameter[1];

            mParam[0] = new MySqlParameter("?$batchno", batchno);
            mParam[0].MySqlDbType = MySqlDbType.VarChar;           

            MySqlDataReader myDr = dbConn.ExecuteSPReader(procedure, true, mParam);
            return myDr;
        }
        protected void btnsubmit_Click(object sender, EventArgs e)
        {
            dt = Convert.ToDateTime(txtfrmdate.Text);
            strfrmdate = String.Format("{0:MM/dd/yyyy}", dt);
            dt = Convert.ToDateTime(txttodate.Text);
            strtodate = String.Format("{0:MM/dd/yyyy}", dt);
            ShowDataView(strfrmdate, strtodate);
            Panelinformation.Visible = true;
        }
        
            protected void btnplacer_Click(object sender, EventArgs e)
        {
            Response.Redirect("PlacerTitle.aspx");
        }
        protected void btnhome_Click(object sender, EventArgs e)
        {
            Response.Redirect("Stars.aspx");
        }
        protected void grdorderdet_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            if (e.CommandName == "Process")
            {
                GridViewRow gvr = (GridViewRow)((LinkButton)e.CommandSource).NamingContainer;
                Session["id"] = grdorderdet.DataKeys[gvr.RowIndex].Values[0].ToString();
                string id = grdorderdet.DataKeys[gvr.RowIndex].Values[0].ToString();
                string query = "select order_no, scrape,county,state, scrape_status from bulk_record_status where id ='" + Session["id"].ToString() + "'";
                ds = dbConn.ExecuteQuery(query);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    Session["ono"] = ds.Tables[0].Rows[0]["order_no"].ToString();
                    Session["state"] = ds.Tables[0].Rows[0]["state"].ToString();
                    Session["county"] = ds.Tables[0].Rows[0]["county"].ToString();
                    Session["status"] = ds.Tables[0].Rows[0]["scrape_status"].ToString();
                    Response.Redirect("TaxDetails.aspx?id=" + Session["ono"].ToString());
                }
                else
                {
                    // Session["TimePro"] = DateTime.Now;

                }

            }


        }
        protected void grdorderbatch_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            if (e.CommandName == "batchwise")
            {
                GridViewRow gvr = (GridViewRow)((LinkButton)e.CommandSource).NamingContainer;
                Session["id"] = grdorderbatch.DataKeys[gvr.RowIndex].Values[0].ToString();
                string id = grdorderbatch.DataKeys[gvr.RowIndex].Values[0].ToString();
                string query = "select batch_no,order_no, scrape,county,state, scrape_status from bulk_record_status where batch_no  ='" + Session["id"].ToString() + "'";
                ds = dbConn.ExecuteQuery(query);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    Session["ono"] = ds.Tables[0].Rows[0]["order_no"].ToString();
                    Session["state"] = ds.Tables[0].Rows[0]["state"].ToString();
                    Session["county"] = ds.Tables[0].Rows[0]["county"].ToString();
                    //string strquery = "sp_getbatchorder_stars";
                    batchorderdetails(id);
                    //Response.Redirect("TaxDetails.aspx?id=" + id);
                }
                else
                {
                    // Session["TimePro"] = DateTime.Now;

                }

            }


        }
        protected void btnupload_Click(object sender, EventArgs e)
        {
            string pp = string.Empty;
            try
            {
                if (assigngrid_Validation())
                {
                    if (gridtransmit.Rows.Count == 0) return;
                    LblDuplicate.Text = string.Empty; lblordersassigned.Text = string.Empty; lblorderduplicates.Text = string.Empty;
                    InsertOrders(gridtransmit, LblDuplicate, lblordersassigned, lblorderduplicates);
                    gridtransmit.DataSource = null;
                }
                gridtransmit.Visible = false;
                Panelinformation.Visible = true;
            }
            catch (Exception ex) { }
        }
        protected void btngetdetails_Click(object sender, EventArgs e)
        {
            dt = Convert.ToDateTime(txtfrmdate.Text);
            strfrmdate = String.Format("{0:MM/dd/yyyy}", dt);
            dt = Convert.ToDateTime(txttodate.Text);
            strtodate = String.Format("{0:MM/dd/yyyy}", dt);
            ShowDataView(strfrmdate, strtodate);
        }
    }
}