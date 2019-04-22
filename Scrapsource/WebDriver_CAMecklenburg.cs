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
using HtmlAgilityPack;
using iTextSharp.text;
using System.Text.RegularExpressions;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Support.Extensions;
using System.Net;

namespace ScrapMaricopa.Scrapsource
{
    public class WebDriver_CAMecklenburg
    {
        string outputPath = "";
        IWebDriver driver;
        DBconnection db = new DBconnection();

        GlobalClass gc = new GlobalClass();
        MySqlParameter[] mParam;
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["MyConnectionString"].ToString());
        public string FTP_Mecklenburg(string address, string ownername, string parcelNumber, string searchType, string orderNumber, string directParcel)
        {
            string StartTime = "", AssessmentTime = "", TaxTime = "", CitytaxTime = "", LastEndTime = "", AssessTakenTime = "", TaxTakentime = "", CityTaxtakentime = "";
            string TotaltakenTime = "";
            GlobalClass.global_orderNo = orderNumber;
            HttpContext.Current.Session["orderNo"] = orderNumber;
            GlobalClass.global_parcelNo = parcelNumber;
            string Parcel_No = "-", Account_No = "-", Location_Address = "-", Owner_Name = "-", Mailing_Address = "-", Land_Use_Code = "-", Land_Use_Desc = "-", Legal_Description = "-", Year_Built = "-";
            string Land_Value = "-", Building_Value = "-", Extra_Features = "-", Total_Appraised_Value = "-", Exemption_Deferment = "-";
            string owner_name = "-", Propertytax_Bill = "-", Bill_Status = "-", Bill_Flag = "-", Due_Date = "-", Interest_Begins = "-", Total_Billed = "-", Interest = "-", Paid_Date = "-", Type = "-", Paid_By = "-", Receipt_No = "-", Paid_Amount = "-", current_due = "-", Good_through_date = "-";

            var driverService = PhantomJSDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            //driver = new ChromeDriver();
            // driver = new PhantomJSDriver();
            using (driver = new PhantomJSDriver()) //ChromeDriver
            {
                try
                {
                    StartTime = DateTime.Now.ToString("HH:mm:ss");

                    if (searchType == "titleflex")
                    {

                        gc.TitleFlexSearch(orderNumber, "", "", address, "NC", "Mecklenburg");
                        if ((HttpContext.Current.Session["TitleFlex_Search"] != null && HttpContext.Current.Session["TitleFlex_Search"].ToString() == "Yes"))
                        {
                            return "MultiParcel";
                        }
                        searchType = "parcel";
                        parcelNumber = HttpContext.Current.Session["titleparcel"].ToString().Replace(".", "").Replace("-", "");
                    }
                    if (searchType == "address")
                    {
                        driver.Navigate().GoToUrl("https://property.spatialest.com/nc/mecklenburg/#/");
                        driver.FindElement(By.Id("primary_search")).SendKeys(address);
                        gc.CreatePdf_WOP(orderNumber, "Address search", driver, "NC", "Mecklenburg");

                        driver.FindElement(By.XPath("//*[@id='main-app']/div/div[1]/div[1]/div/div/div/div/div[1]/div/div[1]/div/button")).SendKeys(Keys.Enter);
                        Thread.Sleep(2000);
                        gc.CreatePdf_WOP(orderNumber, "Address search Result", driver, "NC", "Mecklenburg");
                        Thread.Sleep(3000);

                        //multi parcel

                    }

                    if (searchType == "parcel")
                    {
                        if ((HttpContext.Current.Session["TitleFlex_Search"] != null && HttpContext.Current.Session["TitleFlex_Search"].ToString() == "Yes"))
                        {
                            parcelNumber = HttpContext.Current.Session["titleparcel"].ToString();
                        }
                        driver.Navigate().GoToUrl("https://property.spatialest.com/nc/mecklenburg/#/");
                        driver.FindElement(By.Id("primary_search")).SendKeys(parcelNumber);
                        gc.CreatePdf(orderNumber, parcelNumber, "Parcel Search", driver, "NC", "Mecklenburg");
                        driver.FindElement(By.XPath("//*[@id='main-app']/div/div[1]/div[1]/div/div/div/div/div[1]/div/div[1]/div/button")).SendKeys(Keys.Enter);
                        Thread.Sleep(2000);
                        gc.CreatePdf(orderNumber, parcelNumber, "Parcel Search Result", driver, "NC", "Mecklenburg");
                    }

                    if (searchType == "ownername")
                    {
                        driver.Navigate().GoToUrl("https://property.spatialest.com/nc/mecklenburg/#/");
                        driver.FindElement(By.Id("primary_search")).SendKeys(ownername);
                        gc.CreatePdf_WOP(orderNumber, "Owner search", driver, "NC", "Mecklenburg");
                        driver.FindElement(By.XPath("//*[@id='main-app']/div/div[1]/div[1]/div/div/div/div/div[1]/div/div[1]/div/button")).SendKeys(Keys.Enter);
                        Thread.Sleep(3000);
                        gc.CreatePdf_WOP(orderNumber, "Owner search Result", driver, "NC", "Mecklenburg");
                        ////Multiparcel
                        try
                        {
                            //multi parcel
                            string multi = driver.FindElement(By.XPath("//*[@id='main-app']/div/div[1]/div[2]/div/div/div/div[1]/div/div[2]")).Text;
                            string multicount = GlobalClass.Before(multi, "of ").Trim().Replace("1 to", "");
                            int imulticount = Convert.ToInt16(multicount);
                            int p = 0;

                            if (Convert.ToInt16(multicount) > 21)
                            {
                                HttpContext.Current.Session["multiParcel_mecklenberg_count"] = "Maximum";
                                driver.Quit();
                                return "Maximum";
                            }
                            IWebElement Multiaddresstable = driver.FindElement(By.XPath("//*[@id='main-app']/div/div[1]/div[2]/div/div/div/div[2]"));
                            IList<IWebElement> multiaddressrow = Multiaddresstable.FindElements(By.TagName("div"));
                            IList<IWebElement> Multiaddressid;
                            foreach (IWebElement Multiaddress in multiaddressrow)
                            {

                                Multiaddressid = Multiaddress.FindElements(By.TagName("a"));
                                if (Multiaddressid.Count != 0 && Multiaddressid.Count == 3)
                                {
                                    for (int s = 0; s < Multiaddressid.Count; s++)
                                    {
                                        string[] Multiparcelnumbersplit = Multiaddressid[s].Text.Split('\r');
                                        string Multiparcelnumber = Multiparcelnumbersplit[3].Replace("Parcel:", "").Trim();
                                        string OWnername = Multiparcelnumbersplit[4].Replace("Owners:", "").Trim();
                                        string Address1 = Multiparcelnumbersplit[0].Trim();

                                        string multiaddressresult = OWnername + "~" + Address1;
                                        gc.insert_date(orderNumber, Multiparcelnumber, 57, multiaddressresult, 1, DateTime.Now);
                                    }

                                    p = p + 3;
                                }
                            }
                            if (p < 4)
                            {
                                driver.FindElement(By.XPath("//*[@id='ctl00_BodyContentPlaceHolder_PropertySearchUpdatePanel']/div[3]/table/tbody/tr[1]/td[5]/table/tbody/tr/td/table/tbody/tr[1]/th[1]/a")).Click();
                                Thread.Sleep(2000);
                            }
                            if (Convert.ToInt16(multicount) <= 21)
                            {
                                HttpContext.Current.Session["multiParcel_mecklenberg"] = "Yes";
                                driver.Quit();
                                return "MultiParcel";
                            }
                        }
                        catch { }
                        try
                        {
                            //No Data Found
                            string nodata = driver.FindElement(By.Id("ctl00_BodyContentPlaceHolder_ErrorLabel")).Text;
                            if (nodata.Contains("Returned 0 records."))
                            {
                                HttpContext.Current.Session["Nodata_LeeFL"] = "Yes";
                                driver.Quit();
                                return "No Data Found";
                            }
                        }
                        catch { }
                    }
                }
                catch
                {
                }
                Thread.Sleep(2000);
                try
                {
                    driver.FindElement(By.XPath("//*[@id='main-app']/div/div[1]/div[1]/div/div/div/div/div[1]/div/div[1]/div/button")).SendKeys(Keys.Enter);
                    Thread.Sleep(10000);
                }
                catch { }

                //Property details
                //Parcel_No = "-", Account_No = "-", Location_Address = "-", Owner_Name = "-", Mailing_Address = "-", Land_Use_Code = "-", Land_Use_Desc = "-", Legal_Description = "-", Year_Built = "-";
                Parcel_No = driver.FindElement(By.XPath("//*[@id='prccontent']/div/section/div/div[1]/div[2]/header/div/div/div[1]/div[1]/span")).Text.Replace("PARCEL ID:", "");
                string ownerelement = driver.FindElement(By.XPath("//*[@id='prccontent']/div/section/div/div[1]/div[2]/header/div/div/div[2]/div/div")).Text;
                string[] ownerelementsplit = ownerelement.Split('\r');
                Owner_Name = ownerelementsplit[0].Trim();
                Location_Address = driver.FindElement(By.XPath("//*[@id='prccontent']/div/section/div/div[1]/div[2]/header/div/div/div[1]/div[2]")).Text;
                Mailing_Address = driver.FindElement(By.XPath("//*[@id='prccontent']/div/section/div/div[1]/div[2]/header/div/div/div[2]/div/div")).Text;
                Legal_Description = driver.FindElement(By.XPath("//*[@id='keyinformation']/div[1]/div[2]/div/ul/li[6]/p/span[2]")).Text;
                Land_Use_Code = driver.FindElement(By.XPath("//*[@id='keyinformation']/div[1]/div[2]/div/ul/li[1]/p[1]/span[2]")).Text;
                Land_Use_Desc = driver.FindElement(By.XPath("//*[@id='keyinformation']/div[1]/div[2]/div/ul/li[2]/p[1]/span[2]")).Text;
                Year_Built = driver.FindElement(By.XPath("//*[@id='BuildingSection_residential_0']/div/div[1]/div/div/ul/li[2]/p/span[2]")).Text;

                string prop_details = Owner_Name + "~" + Location_Address + "~" + Mailing_Address + "~" + Legal_Description + "~" + Land_Use_Code + "~" + Land_Use_Desc + "~" + Year_Built;
                gc.insert_date(orderNumber, Parcel_No, 58, prop_details, 1, DateTime.Now);

                //Assessment details
                Land_Value = driver.FindElement(By.XPath("//*[@id='keyinformation']/div[2]/div/div/ul/li[2]/p/span[2]")).Text;
                Building_Value = driver.FindElement(By.XPath("//*[@id='keyinformation']/div[2]/div/div/ul/li[3]/p/span[2]")).Text;
                Extra_Features = driver.FindElement(By.XPath("//*[@id='keyinformation']/div[2]/div/div/ul/li[4]/p/span[2]")).Text;
                Total_Appraised_Value = driver.FindElement(By.XPath("//*[@id='keyinformation']/div[2]/div/div/ul/li[5]/p/span[2]")).Text;
                Exemption_Deferment = driver.FindElement(By.XPath("//*[@id='keyinformation']/div[1]/div[2]/div/ul/li[3]/p[1]/span[2]")).Text;
                gc.CreatePdf(orderNumber, parcelNumber, "Assessment and property details", driver, "NC", "Mecklenburg");
                //Tax Authority
                string Taxauthority = "";
                try
                {
                    driver.Navigate().GoToUrl("https://www.mecknc.gov/TaxCollections/Pages/Home.aspx");
                    Taxauthority = driver.FindElement(By.Id("ctl00_PlaceHolderMain_ctl02_MailingAddressContainer")).Text.Replace("Mailing Address:", "").Trim();
                    gc.CreatePdf(orderNumber, parcelNumber, "Tax Authority details", driver, "NC", "Mecklenburg");
                }
                catch { }
                string ass_details = Land_Value + "~" + Building_Value + "~" + Extra_Features + "~" + Total_Appraised_Value + "~" + Exemption_Deferment + "~" + Taxauthority;
                gc.insert_date(orderNumber, Parcel_No, 59, ass_details, 1, DateTime.Now);

                //gc.CreatePdf(orderNumber, parcelNumber, "Assessment and property details", driver, "NC", "Mecklenburg");
                AssessmentTime = DateTime.Now.ToString("HH:mm:ss");
                //Tax Details
                driver.Navigate().GoToUrl("https://taxbill.co.mecklenburg.nc.us/publicwebaccess/");
                //select parcel number from drop down
                var SearchBy = driver.FindElement(By.Id("lookupCriterion"));
                var selectElement = new SelectElement(SearchBy);
                selectElement.SelectByText("Parcel Number");

                //select tax type from drop down
                var SerachTax = driver.FindElement(By.Id("taxYear"));
                var selectElement1 = new SelectElement(SerachTax);
                selectElement1.SelectByText("ALL");

                driver.FindElement(By.Id("txtSearchString")).SendKeys(Parcel_No);
                gc.CreatePdf(orderNumber, parcelNumber, "Tax Details Result", driver, "NC", "Mecklenburg");
                driver.FindElement(By.Id("btnGo")).SendKeys(Keys.Enter);
                gc.CreatePdf(orderNumber, parcelNumber, "Tax information", driver, "NC", "Mecklenburg");
                //Tax History
                List<string> billinfo = new List<string>();
                string Bill = "-", Old_Bill = "-", Parcel = "-", Name = "-", location = "-", Bill_Flags = "-", Current_Due = "-", Tax_Year = "";
                IWebElement TBTax = driver.FindElement(By.XPath("//*[@id='G_dgResults']/tbody"));
                IList<IWebElement> TRTax = TBTax.FindElements(By.TagName("tr"));
                IList<IWebElement> TDTax;
                int count = TRTax.Count() - 1;
                int j = 1;
                foreach (IWebElement row1 in TRTax)
                {

                    if (j <= count)
                    {
                        TDTax = row1.FindElements(By.TagName("td"));
                        Bill = TDTax[0].Text;
                        Old_Bill = TDTax[1].Text;
                        if (Old_Bill == " ")
                        {
                            Old_Bill = "-";
                        }
                        Parcel = TDTax[2].Text;
                        Name = TDTax[3].Text;
                        location = TDTax[4].Text;
                        Bill_Flags = TDTax[5].Text;
                        if (Bill_Flags == " ")
                        {
                            Bill_Flags = "-";
                        }
                        Current_Due = TDTax[6].Text;
                        string Tax_History = Bill + "~ " + Old_Bill + "~" + Name + "~" + location + "~" + Bill_Flags + "~" + Current_Due;
                        gc.insert_date(orderNumber, Parcel_No, 60, Tax_History, 1, DateTime.Now);

                        if (billinfo.Count < 3 && TDTax.Count != 0 && TDTax[0].Text.Trim() != "" && !row1.Text.Contains("Account"))
                        {
                            //gc.CreatePdf(orderNumber, parcelNumber, "Test search result2", driver, "CA", "Solano");
                            IWebElement value1 = TDTax[0].FindElement(By.TagName("a"));
                            string addview = value1.GetAttribute("href");
                            billinfo.Add(addview);

                        }
                    }

                    if (j > count)
                    {

                        string amount = WebDriverTest.After(row1.Text, "Total:");
                        string Tax_History = "-" + "~" + "-" + "~" + "-" + "~" + "-" + "~" + "Total:" + "~" + amount;
                        gc.insert_date(orderNumber, Parcel_No, 60, Tax_History, 1, DateTime.Now);
                    }
                    j++;
                }

                ////Tax iformation table
                int i = 0;
                foreach (string assessmentclick in billinfo)
                {
                    driver.Navigate().GoToUrl(assessmentclick);
                    //driver.FindElement(By.XPath("//*[@id='dgResults_r_0']/td[1]/a")).SendKeys(Keys.Enter);
                    gc.CreatePdf(orderNumber, parcelNumber, "Tax iformation Result" + i, driver, "NC", "Mecklenburg");
                    owner_name = driver.FindElement(By.XPath("//*[@id='txtName']")).Text;
                    Propertytax_Bill = driver.FindElement(By.XPath("//*[@id='lblBill']")).Text;
                    string[] Tax_Yearsplit = Propertytax_Bill.Split('-');
                    Tax_Year = Tax_Yearsplit[1];
                    Bill_Status = driver.FindElement(By.XPath("//*[@id='lblBillStatus']")).Text;
                    Bill_Flag = driver.FindElement(By.XPath("//*[@id='lblBillFlag']")).Text;
                    if (Bill_Flag == "")
                    {
                        Bill_Flag = "-";
                    }
                    Due_Date = driver.FindElement(By.XPath("//*[@id='lblDueDate']")).Text;
                    Interest_Begins = driver.FindElement(By.XPath("//*[@id='lblInterest']")).Text;

                    IWebElement good_date = driver.FindElement(By.XPath("//*[@id='interestCalDate_input']"));
                    Good_through_date = good_date.GetAttribute("value");
                    if (Good_through_date.Contains("Select A Date"))
                    {
                        Good_through_date = "-";
                        Total_Billed = driver.FindElement(By.XPath("//*[@id='lblTotalAmountDue']")).Text;
                        Interest = driver.FindElement(By.XPath("//*[@id='lblInterestAmt']")).Text;
                        current_due = driver.FindElement(By.XPath("//*[@id='lblCurrentDue']")).Text;
                    }
                    else
                    {
                        if (Bill_Flag == "DELINQUENT" || Bill_Flag.Contains("DELINQUENT"))
                        {

                            DateTime G_Date = Convert.ToDateTime(Good_through_date);
                            string dateChecking = DateTime.Now.ToString("MM") + "/15/" + DateTime.Now.ToString("yyyy");

                            if (G_Date < Convert.ToDateTime(dateChecking))
                            {
                                //end of the month
                                Good_through_date = new DateTime(Convert.ToInt16(DateTime.Now.ToString("yyyy")), Convert.ToInt16(Convert.ToInt16(DateTime.Now.ToString("MM"))), DateTime.DaysInMonth(Convert.ToInt16(DateTime.Now.ToString("yyyy")), Convert.ToInt16(DateTime.Now.ToString("MM")))).ToString("MM/dd/yyyy");

                            }

                            else if (G_Date > Convert.ToDateTime(dateChecking))
                            {
                                // nextEndOfMonth 
                                if ((Convert.ToInt16(Convert.ToInt16(DateTime.Now.ToString("MM"))) < 12))
                                {
                                    Good_through_date = new DateTime(Convert.ToInt16(DateTime.Now.ToString("yyyy")), Convert.ToInt16(Convert.ToInt16(DateTime.Now.ToString("MM")) + 1), DateTime.DaysInMonth(Convert.ToInt16(DateTime.Now.ToString("yyyy")), Convert.ToInt16(DateTime.Now.ToString("MM")) + 1)).ToString("MM/dd/yyyy");
                                }
                                else
                                {
                                    int nxtYr = Convert.ToInt16(DateTime.Now.ToString("yyyy")) + 1;
                                    Good_through_date = new DateTime(nxtYr, 1, DateTime.DaysInMonth(Convert.ToInt16(DateTime.Now.ToString("yyyy")), 1)).ToString("MM/dd/yyyy");

                                }
                            }
                            //recalculate interest
                            good_date.Clear();
                            good_date.SendKeys(Good_through_date);
                            driver.FindElement(By.Id("btnRecalInterest")).SendKeys(Keys.Enter);
                            Thread.Sleep(2000);
                            Total_Billed = driver.FindElement(By.XPath("//*[@id='lblTotalAmountDue']")).Text;
                            Interest = driver.FindElement(By.XPath("//*[@id='lblInterestAmt']")).Text;
                            current_due = driver.FindElement(By.XPath("//*[@id='lblCurrentDue']")).Text;
                            gc.CreatePdf(orderNumber, parcelNumber, "After Good Through Date Calculate" + Tax_Year, driver, "NC", "Mecklenburg");

                        }
                    }
                    //transcation history
                    IWebElement tb_trans = driver.FindElement(By.XPath("//*[@id='dgShowResultHistory']/tbody"));
                    IList<IWebElement> TR_trans = tb_trans.FindElements(By.TagName("tr"));
                    IList<IWebElement> TD_trans;

                    foreach (IWebElement row2 in TR_trans)
                    {
                        if (!row2.Text.Contains("Date"))
                        {
                            TD_trans = row2.FindElements(By.TagName("td"));
                            Paid_Date = TD_trans[0].Text;
                            Type = TD_trans[1].Text;
                            Paid_By = TD_trans[2].Text;
                            Receipt_No = TD_trans[3].Text;
                            Paid_Amount = TD_trans[4].Text;
                            //Split Title
                            string tax1 = Tax_Year.Replace("\r\n", "") + "~" + Paid_Date.Replace("\r\n", "") + "~" + Type.Replace("\r\n", "") + "~" + Paid_By.Replace("\r\n", "") + "~" + Receipt_No.Replace("\r\n", "") + "~" + Paid_Amount.Replace("\r\n", "");
                            gc.insert_date(orderNumber, Parcel_No, 1459, tax1, 1, DateTime.Now);
                        }
                    }
                    string tax2 = Tax_Year.Replace("\r\n", "") + "~" + owner_name.Replace("\r\n", "") + "~" + Propertytax_Bill.Replace("\r\n", "") + "~" + Bill_Status.Replace("\r\n", "") + "~" + Bill_Flag.Replace("\r\n", "") + "~" + Due_Date.Replace("\r\n", "") + "~" + Interest_Begins.Replace("\r\n", "") + "~" + Total_Billed.Replace("\r\n", "") + "~" + Interest.Replace("\r\n", "") + "~" + current_due.Replace("\r\n", "") + "~" + Good_through_date.Replace("\r\n", "");
                    gc.insert_date(orderNumber, Parcel_No, 61, tax2, 1, DateTime.Now);
                    // Tax / Fee Distribution Table: 
                    string Rate = "-", Tax_Districts = "-", Description = "-", Amount = "-";
                    IWebElement TBTaxFee = driver.FindElement(By.XPath("//*[@id='dgShowResultRate']/tbody"));
                    IList<IWebElement> TRTaxFee = TBTaxFee.FindElements(By.TagName("tr"));
                    IList<IWebElement> TDTaxFee;

                    foreach (IWebElement row2 in TRTaxFee)
                    {
                        if (!row2.Text.Contains("Rate"))
                        {
                            TDTaxFee = row2.FindElements(By.TagName("td"));
                            Rate = TDTaxFee[0].Text;
                            if (Rate == "")
                            {
                                Rate = "-";
                            }
                            Tax_Districts = TDTaxFee[1].Text;
                            Description = TDTaxFee[2].Text;
                            Amount = TDTaxFee[3].Text;
                            string taxfee = Tax_Year + "~" + Rate + "~ " + Tax_Districts + "~ " + Description + "~ " + Amount;
                            gc.insert_date(orderNumber, Parcel_No, 64, taxfee, 1, DateTime.Now);
                        }
                    }
                    i++;
                }
                TaxTime = DateTime.Now.ToString("HH:mm:ss");
                LastEndTime = DateTime.Now.ToString("HH:mm:ss");
                gc.insert_TakenTime(orderNumber, "NC", "Mecklenburg", StartTime, AssessmentTime, TaxTime, CitytaxTime, LastEndTime);

                driver.Quit();
                gc.mergpdf(orderNumber, "NC", "Mecklenburg");
                return "Data Inserted Successfully";
            }
        }
    }
}