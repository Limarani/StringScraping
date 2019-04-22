using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace ScrapMaricopa.lereta
{
    public class LeretaData
    {
        scrape sc = new scrape();
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["lereta"].ConnectionString);
        public void InsertData()
        {

            //Read order list from record status table...
            string query = "", sname = "", cname = "", orderno = "", o_id = "", name = "", address = "", parcelno = "";
            DataTable dt = executeqry("select id,orderno,state,county,name,propertyaddress,parcelid from tbl_record_status where scrape=1 and scrape_status=0");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                sname = dt.Rows[i]["state"].ToString();
                cname = dt.Rows[i]["county"].ToString();
                orderno = dt.Rows[i]["orderno"].ToString();
                o_id = dt.Rows[i]["id"].ToString();
                name = dt.Rows[i]["name"].ToString();
                address = dt.Rows[i]["propertyaddress"].ToString();
                parcelno = dt.Rows[i]["parcelid"].ToString().TrimEnd();
                string json = sc.RunOrderList(orderno, cname, sname, name, address, parcelno);



                //Get county id
                query = "SELECT * FROM state_county_master where State_Name = '" + sname + "' and County_Name='" + cname + "'";
                DataTable dtCID = executeqry(query);
                //string countyID = dtCID.Rows[0]["state_county_id"].ToString();
                string countyID = "167";

                string year = "", taxType = "", amount = "", dueDate = "", paid = "";
                //AL Baldwin

                if (json.Contains("Data Inserted Successfully") || json.Contains("Timeout") || json.Contains("Success"))
                {


                    if (countyID == "167")
                    {
                        string CurrentTaxYear = "", taxDue = "", paidAmount = "", taxSale = "", curtStatus1 = "", prevStatus1 = "", curtDueDate1 = "", prevDueDate1 = "", bkparty = "", npThru = "";
                        taxType = "Annual";
                        //year
                        DataTable dtYear = ReadDataAB("select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 9 and DVM.Order_No = '" + orderno + "' order by 1");
                        if (dtYear.Rows.Count > 0)
                        {
                            CurrentTaxYear = dtYear.Rows[0]["tax year"].ToString();
                            taxDue = dtYear.Rows[0]["tax due"].ToString();
                            paidAmount = dtYear.Rows[0]["paid amount"].ToString();
                            taxSale = dtYear.Rows[0]["TAX SALES"].ToString();
                            if (taxSale.Contains("**NO TAX SALES FOUND**"))
                            {
                                bkparty = "YES";
                            }
                            else
                            {
                                bkparty = "NO";
                            }
                        }



                        //Tax installments
                        DataTable dtIns = ReadDataAB("select order_no, parcel_no,DVM.Data_Field_Text_Id,DVM.Data_Field_value from data_value_master DVM join data_field_master DFM on DFM.ID = DVM.Data_Field_Text_Id join state_county_master SCM on SCM.ID = DFM.State_County_ID and SCM.State_County_Id = '" + countyID + "' where DFM.Category_Id = 4 and DVM.Order_No = '" + orderno + "' order by 1");
                        if (dtIns.Rows.Count > 0)
                        {
                            //current
                            paid = dtIns.Rows[0]["Paid(Y/N)"].ToString();
                            if (paid.Contains("Y"))
                            {
                                curtStatus1 = "1";
                                curtDueDate1 = After(paid, " ");
                            }
                            else if (paid.Contains("N"))
                            {
                                curtStatus1 = "0";
                                curtDueDate1 = After(paid, " ");
                            }


                            for (int k = 0; k < dtIns.Rows.Count; k++)
                            {
                                if (dtIns.Rows[k]["Paid(Y/N)"].ToString().Contains("N"))
                                {
                                    k++;
                                    if (k < dtIns.Rows.Count)
                                        npThru = dtIns.Rows[k]["Year"].ToString();
                                }

                            }

                            //previous
                            paid = dtIns.Rows[1]["Paid(Y/N)"].ToString();
                            if (paid.Contains("Y"))
                            {
                                prevStatus1 = "1";
                                prevDueDate1 = After(paid, " ");
                            }
                            else if (paid.Contains("N"))
                            {
                                prevStatus1 = "1";
                                prevDueDate1 = After(paid, " ");
                            }

                            executeqry("insert into tbl_output_scrape (o_id,orderno,currentyear,currenttaxtype,curtstatus1,curtamount1,curtduedate1,previousyear,previoustaxtype,prvtstatus1,prvtamount1,prvtduedate1,bkparty,npthru)values('" + o_id + "','" + orderno + "','" + dtIns.Rows[0]["Year"].ToString() + "','" + taxType + "','" + curtStatus1 + "','" + dtIns.Rows[0]["Total Tax"].ToString() + "','" + curtDueDate1 + "','" + dtIns.Rows[1]["year"].ToString() + "','" + taxType + "','" + prevStatus1 + "','" + dtIns.Rows[1]["Total Tax"].ToString() + "','" + prevDueDate1 + "','" + bkparty + "','" + npThru + "')");

                        }
                    }
                    else if (countyID == "")
                    {

                    }
                }
            }
        }

        public static string ReadDataFROMCloudAB(string qry)
        {
            string apiUrl = "http://173.192.83.98/RestService/api/TSI/FtpAbteam";

            object input = new
            {
                Query = qry
            };

            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(apiUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.PostAsJsonAsync(apiUrl, input).Result;

                var result = response.Content.ReadAsStringAsync().Result;
                var s = Newtonsoft.Json.JsonConvert.DeserializeObject(result);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return "An error has occured";
                }
                else
                {
                    return s.ToString();
                }

            }
            catch
            {
                return "Timeout";
            }
        }

        public DataTable ReturnDtAPIAB(string qry)
        {
            string data = ReadDataFROMCloudAB(qry);
            IList<TestbindAB> UserList = JsonConvert.DeserializeObject<IList<TestbindAB>>(data);
            DataTable dt = Convertdt.ToDataTable(UserList);
            return dt;
        }
        public DataTable ReadDataAB(string Query)
        {
            DataTable dt = ReturnDtAPIAB(Query);
            DataTable dTable = new DataTable();
            if (dt.Rows.Count > 0)
            {
                string data_text_id = dt.Rows[0]["Data_Field_Text_Id"].ToString();
                string order_no = dt.Rows[0]["order_no"].ToString();
                DataTable dtfield = ReturnDtAPIAB("select Data_Fields_Text from data_field_master where id='" + data_text_id + "'");
                string columnName = dtfield.Rows[0]["Data_Fields_Text"].ToString();
                string[] columnArray = columnName.Split('~');

                foreach (string cName in columnArray)
                {
                    dTable.Columns.Add(cName);
                }
                DataRow dr = dTable.NewRow();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dTable.Rows.Add();
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string rowvals = dt.Rows[i]["Data_Field_value"].ToString();
                    string[] rowValue = rowvals.Split('~');
                    for (int k = 0; k < rowValue.Count(); k++)
                    {
                        dTable.Rows[i][k] = rowValue[k];
                    }
                }
            }

            return dTable;

        }

        public DataTable executeqry(string qry)
        {
            MySqlCommand cmd = new MySqlCommand(qry, con);
            con.Open();
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }
        public string Between(string Text, string FirstString, string LastString)
        {

            string STR = Text;
            string STRFirst = FirstString;
            string STRLast = LastString;
            string FinalString;
            int Pos1 = STR.IndexOf(FirstString) + FirstString.Length;
            int Pos2 = STR.IndexOf(LastString);
            FinalString = STR.Substring(Pos1, Pos2 - Pos1);
            FinalString = FinalString.Replace(System.Environment.NewLine, string.Empty);
            return FinalString;

        }
        public static string Before(string value, string a)
        {
            int posA = value.IndexOf(a);
            if (posA == -1)
            {
                return "";
            }
            return value.Substring(0, posA);
        }
        public static string After(string value, string a)
        {
            int posA = value.LastIndexOf(a);
            if (posA == -1)
            {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= value.Length)
            {
                return "";
            }
            return value.Substring(adjustedPosA);
        }


    }

    public static class Convertdt
    {
        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection props =
            TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }
    }
    public class TestbindAB
    {
        public string Order_No { get; set; }
        public string Parcel_no { get; set; }
        public string Data_Field_Text_Id { get; set; }
        public string Data_Field_value { get; set; }
        public string Data_Fields_Text { get; set; }
    }
}
  