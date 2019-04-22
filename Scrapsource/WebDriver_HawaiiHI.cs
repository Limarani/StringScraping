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
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace ScrapMaricopa.Scrapsource
{
    public class WebDriver_HawaiiHI
    {
        IWebDriver driver;
        DBconnection db = new DBconnection();
        GlobalClass gc = new GlobalClass();
        MySqlParameter[] mParam;
        public string FTP_HawaiiHI(string houseno, string Direction, string sname, string stype, string account, string parcelNumber, string ownername, string searchType, string orderNumber)
        {
            GlobalClass.global_orderNo = orderNumber;
            HttpContext.Current.Session["orderNo"] = orderNumber;
            GlobalClass.global_parcelNo = parcelNumber;
            string StartTime = "", AssessmentTime = "", TaxTime = "", CitytaxTime = "", LastEndTime = "";

            string outparcelno = "", taxparcel = "", mailingaddress = "", tax_district = "", siteaddr = "", legal_desc = "", year_built = "", propuse = "", par1 = "", par2 = "";
            string Building_Value = "", LandValue = "", JustValue = "", cctaxyear = "", Cap = "", AssessedValue = "", Exemption = "", TaxableValue = "";
            string strBill = "-", strBalance = "-", strBillDate = "-", strBillPaid = "-";
            string taxowner = "", tax_addr = "", accno = "", alterkey = "", millagecode = "", milagerate = "", ctax_year = "", cpaiddate = "", cpaidamount = "", creceipt = "", combinedtaxamount = "", ceffdate = "", grosstax = "", ifpaidby = "", pleasepay = "";
            string taxauth1 = "", taxauth2 = "", taxauth3 = "", taxauth4 = "", taxauth5 = "", taxauth6 = "", TaxYear = "", Gis_Map=""; 
            List<string> strissuecertificate = new List<string>();
            List<string> taxhistorylink = new List<string>();
            List<string> downloadlink = new List<string>();
            List<string> taxhistorylinkinst = new List<string>();

            string multi = "", TaxAuthority = "";
            var driverService = PhantomJSDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            using (driver = new PhantomJSDriver())
            {
              //  using (driver = new ChromeDriver())
                    try
                    {
                        StartTime = DateTime.Now.ToString("HH:mm:ss");

                    try
                    {
                        driver.Navigate().GoToUrl("http://www.hawaiipropertytax.com/");
                        Thread.Sleep(1000);
                        taxauth1 = driver.FindElement(By.XPath("//*[@id='xr_xri']/div[6]/span[5]")).Text;
                        taxauth2 = driver.FindElement(By.XPath("//*[@id='xr_xri']/div[6]/span[7]")).Text;
                        taxauth3 = driver.FindElement(By.XPath("//*[@id='xr_xri']/div[6]/span[9]")).Text;
                        taxauth4 = driver.FindElement(By.XPath("//*[@id='xr_xri']/div[6]/span[19]/span")).Text.Trim();
                        taxauth5 = driver.FindElement(By.XPath("//*[@id='xr_xri']/div[6]/span[20]")).Text.Trim();
                        taxauth6 = driver.FindElement(By.XPath("//*[@id='xr_xri']/div[6]/span[23]")).Text.Trim();
                        TaxAuthority = taxauth1 + " " + taxauth2 + " " + taxauth3 + " " + taxauth4 + " " + taxauth5 + " " + taxauth6;
                        gc.CreatePdf_WOP(orderNumber, "Taxing Authority", driver, "HI", "Hawaii");
                    }
                    catch { }

                    driver.Navigate().GoToUrl("http://qpublic9.qpublic.net/hi_hawaii_search.php");
                        Thread.Sleep(1000);

                        string Parcelno = "", Ownername = "", parcellocation = "";
                        if (searchType == "titleflex")
                        {

                            gc.TitleFlexSearch(orderNumber, "", ownername, "", "HI", "Hawaii");
                            //gc.TitleFlexSearch(orderNumber, "", "", address.Trim(), "HI", "Hawaii");
                            if ((HttpContext.Current.Session["TitleFlex_Search"] != null && HttpContext.Current.Session["TitleFlex_Search"].ToString() == "Yes"))
                            {
                                return "MultiParcel";
                            }
                            if ((HttpContext.Current.Session["TitleFlex_Search"] == null))
                            {
                                HttpContext.Current.Session["Zero_Hawaii"] = "Zero";
                                return "No Data Found";
                            }
                            parcelNumber = HttpContext.Current.Session["titleparcel"].ToString();
                            searchType = "parcel";
                            parcelNumber = parcelNumber.Replace("-", "");

                        }
                        if (searchType == "address")
                        {


                            try
                            {
                                driver.FindElement(By.LinkText("Search by Location Address")).Click();
                                Thread.Sleep(1000);


                                driver.FindElement(By.XPath("/html/body/form/table/tbody/tr/td[2]/table/tbody/tr[1]/td[2]/input")).SendKeys(houseno);
                                driver.FindElement(By.Id("streetName")).SendKeys(sname);
                                // driver.FindElement(By.XPath("/html/body/form/table/tbody/tr/td[2]/table/tbody/tr[3]/td[2]/input")).SendKeys(stype);
                                // driver.FindElement(By.XPath("/html/body/form/table/tbody/tr/td[2]/table/tbody/tr[4]/td[2]/input")).SendKeys(account);
                                gc.CreatePdf_WOP(orderNumber, "Address Search", driver, "HI", "Hawaii");
                                driver.FindElement(By.XPath("/html/body/form/table/tbody/tr/td[2]/table/tbody/tr[6]/td[2]/input")).SendKeys(Keys.Enter);
                                Thread.Sleep(1000);
                                //IWebElement multirecord = driver.FindElement(By.XPath("//*[@id='mMessage']"));
                            }
                            catch { }
                            gc.CreatePdf_WOP(orderNumber, "Address Search Result", driver, "HI", "Hawaii");
                            int Max = 0;
                            IWebElement multiaddress = driver.FindElement(By.XPath("/html/body/table/tbody"));
                            IList<IWebElement> TRmultiaddress = multiaddress.FindElements(By.TagName("tr"));
                            IList<IWebElement> THmultiaddress = multiaddress.FindElements(By.TagName("th"));
                            IList<IWebElement> TDmultiaddress;


                            if (TRmultiaddress.Count > 28)
                            {
                                HttpContext.Current.Session["multiParcel_Hawaii_Maximum"] = "Maimum";
                                return "Maximum";
                                Max++;
                            }
                            if (TRmultiaddress.Count == 8)
                            {
                                IWebElement multiclick = driver.FindElement(By.XPath("/html/body/table/tbody/tr[5]/td[1]/a"));
                                multiclick.Click();
                                Max++;
                            }
                            if (TRmultiaddress.Count > 7 && Max != 1)
                            {
                                foreach (IWebElement row in TRmultiaddress)
                                {
                                    TDmultiaddress = row.FindElements(By.TagName("td"));
                                    if (!row.Text.Contains("Parcel Number") && !row.Text.Contains("Search") && !row.Text.Contains("Owner Name") && row.Text.Trim() != "" && TDmultiaddress.Count == 4)
                                    {
                                        try
                                        {
                                            Parcelno = TDmultiaddress[0].Text.Trim();
                                            Ownername = TDmultiaddress[1].Text.Trim();
                                            parcellocation = TDmultiaddress[2].Text.Trim();
                                            Gis_Map = TDmultiaddress[3].Text.Trim();
                                            string Multi = Ownername + "~" + parcellocation + "~" + Gis_Map;
                                            gc.insert_date(orderNumber, Parcelno, 1850, Multi, 1, DateTime.Now);
                                            Max++;
                                        }
                                        catch { }

                                    }



                                }
                                HttpContext.Current.Session["multiParcel_Hawaii"] = "Yes";
                                driver.Quit();
                                return "MultiParcel";
                            }
                            //if (TRmultiaddress.Count <= 4)
                            //{
                            //    //TDmultiaddress[0].Click();
                            //    driver.FindElement(By.XPath("//*[@id='searchResults']/tbody/tr[3]/td[1]/table/tbody/tr/td[2]/font")).Click();
                            //    Thread.Sleep(1000);
                            //    Max++;
                            //}
                            if (Max == 0)
                            {
                                HttpContext.Current.Session["Zero_Hawaii"] = "Zero";
                                driver.Quit();
                                return "No Data Found";
                            }
                        }

                        if (searchType == "parcel")
                        {

                            try
                            {
                                driver.FindElement(By.XPath("/html/body/table[1]/tbody/tr/td/ul/li[2]/b/a")).SendKeys(Keys.Enter);
                                Thread.Sleep(1000);
                            }
                            catch { }
                            string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "", s6 = "";
                            if (Convert.ToInt16(parcelNumber.Replace("-", "").Count()) != 12)
                            {
                                string[] parcelSplit = parcelNumber.Split('-');
                                s1 = parcelSplit[0];
                                s2 = parcelSplit[1];
                                s3 = parcelSplit[2];
                                s4 = parcelSplit[3];
                                s5 = parcelSplit[4];
                                s6 = parcelSplit[5];
                                parcelNumber = s2 + s3 + s4 + s5 + s6;
                            }
                            else
                            {
                                parcelNumber = parcelNumber.Replace("-", "");
                            }
                            driver.FindElement(By.XPath("/html/body/table[1]/tbody/tr/td[1]/form/table/tbody/tr/td/input[2]")).SendKeys(parcelNumber);
                            gc.CreatePdf(orderNumber, parcelNumber, "Parcel Search", driver, "HI", "Hawaii");
                            driver.FindElement(By.XPath("/html/body/table[1]/tbody/tr/td[1]/form/table/tbody/tr/td/input[5]")).SendKeys(Keys.Enter);
                            Thread.Sleep(1000);
                            gc.CreatePdf(orderNumber, parcelNumber, "Parcel Search Result", driver, "HI", "Hawaii");
                            try
                            {
                                IWebElement Iclick = driver.FindElement(By.XPath("/html/body/table/tbody/tr[5]/td[1]/a"));
                                Iclick.Click();
                                Thread.Sleep(2000);
                            }
                            catch { }

                            int Max = 0;
                            try
                            {
                                IWebElement multiaddress = driver.FindElement(By.XPath("/html/body/table/tbody"));
                                IList<IWebElement> TRmultiaddress = multiaddress.FindElements(By.TagName("tr"));
                                IList<IWebElement> THmultiaddress = multiaddress.FindElements(By.TagName("th"));
                                IList<IWebElement> TDmultiaddress;


                                if (TRmultiaddress.Count > 28)
                                {
                                    HttpContext.Current.Session["multiParcel_Hawaii_Maximum"] = "Maimum";
                                    return "Maximum";
                                    Max++;
                                }
                                if (TRmultiaddress.Count == 7)
                                {
                                    IWebElement multiclick = driver.FindElement(By.XPath("/html/body/table/tbody/tr[5]/td[1]/a"));
                                    multiclick.Click();
                                    Max++;
                                }
                                if (TRmultiaddress.Count > 7)
                                {
                                    foreach (IWebElement row in TRmultiaddress)
                                    {
                                        TDmultiaddress = row.FindElements(By.TagName("td"));
                                        if (!row.Text.Contains("Parcel Number") && !row.Text.Contains("Search") && row.Text.Trim() != "" && TDmultiaddress.Count == 4)
                                        {
                                            try
                                            {
                                                Parcelno = TDmultiaddress[0].Text.Trim();
                                                Ownername = TDmultiaddress[1].Text.Trim();
                                                parcellocation = TDmultiaddress[2].Text.Trim();
                                                Gis_Map = TDmultiaddress[3].Text.Trim();
                                                string Multi = Ownername + "~" + parcellocation + "~" + Gis_Map;
                                                gc.insert_date(orderNumber, Parcelno, 1850, Multi, 1, DateTime.Now);
                                                Max++;
                                            }
                                            catch { }

                                        }



                                    }
                                    HttpContext.Current.Session["multiParcel_Hawaii"] = "Yes";
                                    driver.Quit();
                                    return "MultiParcel";
                                }
                                //if (TRmultiaddress.Count <= 4)
                                //{
                                //    //TDmultiaddress[0].Click();
                                //    driver.FindElement(By.XPath("//*[@id='searchResults']/tbody/tr[3]/td[1]/table/tbody/tr/td[2]/font")).Click();
                                //    Thread.Sleep(1000);
                                //    Max++;
                                //}
                                if (Max == 0)
                                {
                                    HttpContext.Current.Session["Zero_Hawaii"] = "Zero";
                                    driver.Quit();
                                    return "No Data Found";
                                }
                            }
                            catch { }
                        }

                        //if (searchType == "ownername")
                        //{

                        //    try
                        //    {
                        //        driver.FindElement(By.LinkText("Owner Name")).SendKeys(Keys.Enter);
                        //        Thread.Sleep(1000);
                        //    }
                        //    catch { }

                        //    driver.FindElement(By.XPath("/html/body/form/table/tbody/tr/td/input[2]")).SendKeys(ownername);
                        //    gc.CreatePdf_WOP(orderNumber, "Ownername Search", driver, "HI", "Hawaii");
                        //    driver.FindElement(By.XPath("/html/body/form/table/tbody/tr/td/input[5]")).SendKeys(Keys.Enter);
                        //    Thread.Sleep(1000);
                        //    gc.CreatePdf_WOP(orderNumber, "Ownername Search Result", driver, "HI", "Hawaii");

                        //    string ParcelNum = "", Owner_Name = "", ParcelLocation = "";
                        //    int Max = 0;
                        //    IWebElement multiaddress = driver.FindElement(By.XPath("/html/body/table/tbody"));
                        //    IList<IWebElement> TRmultiaddress = multiaddress.FindElements(By.TagName("tr"));
                        //    IList<IWebElement> THmultiaddress = multiaddress.FindElements(By.TagName("th"));
                        //    IList<IWebElement> TDmultiaddress;


                        //    if (TRmultiaddress.Count > 28)
                        //    {
                        //        HttpContext.Current.Session["multiParcel_Honolulu_Maximum"] = "Maimum";
                        //        return "Maximum";
                        //        Max++;
                        //    }
                        //    if (TRmultiaddress.Count == 7)
                        //    {
                        //        IWebElement multiclick = driver.FindElement(By.XPath("/html/body/table/tbody/tr[4]/td[1]/a"));
                        //        multiclick.Click();
                        //        Max++;
                        //    }
                        //    if (TRmultiaddress.Count > 7)
                        //    {
                        //        foreach (IWebElement row in TRmultiaddress)
                        //        {
                        //            TDmultiaddress = row.FindElements(By.TagName("td"));
                        //            if (!row.Text.Contains("Parcel Number") && !row.Text.Contains("Search") && row.Text.Trim() != "" && TDmultiaddress.Count >= 7)
                        //            {
                        //                try
                        //                {
                        //                    ParcelNum = TDmultiaddress[0].Text.Trim();
                        //                    Owner_Name = TDmultiaddress[1].Text.Trim();
                        //                    ParcelLocation = TDmultiaddress[2].Text.Trim();
                        //                    string Multi = Owner_Name + "~" + ParcelLocation;
                        //                    gc.insert_date(orderNumber, ParcelNum, 1328, Multi, 1, DateTime.Now);
                        //                    Max++;
                        //                }
                        //                catch { }

                        //            }



                        //        }
                        //        HttpContext.Current.Session["multiParcel_Honolulu"] = "Yes";
                        //        driver.Quit();
                        //        return "Multiparcel";
                        //    }
                        //    //if (TRmultiaddress.Count <= 4)
                        //    //{
                        //    //    //TDmultiaddress[0].Click();
                        //    //    driver.FindElement(By.XPath("//*[@id='searchResults']/tbody/tr[3]/td[1]/table/tbody/tr/td[2]/font")).Click();
                        //    //    Thread.Sleep(1000);
                        //    //    Max++;
                        //    //}
                        //    if (Max == 0)
                        //    {
                        //        HttpContext.Current.Session["Zero_Honolulu"] = "Zero";
                        //        driver.Quit();
                        //        return "Zero";
                        //    }
                        //}

                        // Property Details

                        string propertydata = driver.FindElement(By.XPath("/html/body/center[2]/table[2]/tbody")).Text;
                        string LocationAddress = "", Yearbuilt = "", LandArea = "", MillageRates = "", Propertyclass = "";
                        string OwnerName = "", LandArea_Acres = "", LegalInformation = "", NeighborhoodCode = "";
                        //IWebElement Iparcelno = driver.FindElement(By.XPath("/html/body/center[2]/table[2]/tbody/tr[3]/td[4]"));
                        // parcelNumber = Iparcelno.Text.Trim();
                        parcelNumber = gc.Between(propertydata, "Parcel Number", "Location Address").Trim();
                        OwnerName = gc.Between(propertydata, "Owner Name", "Today's Date").Trim();
                        LocationAddress = gc.Between(propertydata, "Location Address", "Project Name").Trim();
                        Propertyclass = driver.FindElement(By.XPath("/html/body/center[2]/table[2]/tbody/tr[5]/td[2]")).Text.Trim();
                        LandArea = gc.Between(propertydata, "Area (acres)", "Legal Information").Trim();
                        NeighborhoodCode = gc.Between(propertydata, "Neighborhood Code", "Land Area (acres)").Trim();
                        LegalInformation = gc.Between(propertydata, "Legal Information", "Land Area (approximate sq ft)").Trim();
                        try
                        {
                            IWebElement Iyearbuilt = driver.FindElement(By.XPath("/html/body/center[2]/table[5]/tbody/tr[3]/td[2]"));
                            Yearbuilt = Iyearbuilt.Text.Trim();
                        }
                        catch { }
                        if (Yearbuilt.Count() != 4)
                        {
                            try
                            {
                                IWebElement Iyearbuilt = driver.FindElement(By.XPath("/html/body/center[2]/table[6]/tbody/tr[3]/td[2]"));
                                Yearbuilt = Iyearbuilt.Text.Trim();
                            }
                            catch { }
                        }
                        string propertydetails = OwnerName + "~" + LocationAddress + "~" + Propertyclass + "~" + NeighborhoodCode + "~" + LegalInformation + "~" + LandArea + "~" + Yearbuilt;
                        gc.insert_date(orderNumber, parcelNumber, 1661, propertydetails, 1, DateTime.Now);
                        gc.CreatePdf(orderNumber, parcelNumber, "Assessment Details", driver, "HI", "Hawaii");

                        // Assessment Details
                        string valuetype = "", Information = "";
                        try
                        {
                            driver.FindElement(By.LinkText("Show Historical Assessments")).Click();
                            Thread.Sleep(4000);
                            gc.CreatePdf(orderNumber, parcelNumber, "Assessment History Details", driver, "HI", "Hawaii");
                        }
                        catch { }

                        try
                        {
                            IWebElement Assessmentdetails = driver.FindElement(By.XPath("/html/body/center[2]/table[3]/tbody"));
                            IList<IWebElement> TRAssessmentdetails = Assessmentdetails.FindElements(By.TagName("tr"));
                            IList<IWebElement> THAssessmentdetails = Assessmentdetails.FindElements(By.TagName("th"));
                            IList<IWebElement> TDAssessmentdetails;
                            foreach (IWebElement row in TRAssessmentdetails)
                            {
                                TDAssessmentdetails = row.FindElements(By.TagName("td"));
                                if (TDAssessmentdetails.Count != 0 && !row.Text.Contains("Assessment Information") && row.Text.Trim() != "" && !row.Text.Contains("Assessment Year") && !row.Text.Contains("Year") && TDAssessmentdetails.Count == 11)
                                {
                                    string AssessmentDetail = TDAssessmentdetails[0].Text + "~" + TDAssessmentdetails[1].Text + "~" + TDAssessmentdetails[2].Text + "~" + TDAssessmentdetails[3].Text + "~" + TDAssessmentdetails[4].Text + "~" + TDAssessmentdetails[5].Text + "~" + TDAssessmentdetails[6].Text + "~" + TDAssessmentdetails[7].Text + "~" + TDAssessmentdetails[8].Text + "~" + TDAssessmentdetails[9].Text + "~" + TDAssessmentdetails[10].Text;
                                    gc.insert_date(orderNumber, parcelNumber, 1662, AssessmentDetail, 1, DateTime.Now);

                                }
                            }

                        }
                        catch { }



                        // Current tax details
                        string strcurrenttax = "";
                        strcurrenttax = driver.FindElement(By.XPath("/html/body/center[2]/table[11]/tbody/tr[1]/td")).Text;
                        if (!strcurrenttax.Contains("Current Tax Bill Information"))
                        {
                            try
                            {
                                IWebElement CurrentTax = driver.FindElement(By.XPath("/html/body/center[2]/table[10]/tbody"));
                                IList<IWebElement> TRCurrentTax = CurrentTax.FindElements(By.TagName("tr"));
                                IList<IWebElement> THCurrentTax = CurrentTax.FindElements(By.TagName("th"));
                                IList<IWebElement> TDCurrentTax;
                                foreach (IWebElement row in TRCurrentTax)
                                {
                                    TDCurrentTax = row.FindElements(By.TagName("td"));
                                    if (TDCurrentTax.Count != 0 && !row.Text.Contains("Current Tax") && row.Text.Trim() != "" && !row.Text.Contains("Tax Period") && !row.Text.Contains("pay online") && TDCurrentTax.Count == 10)
                                    {
                                        string CurrentTaxDetail1 = TDCurrentTax[0].Text + "~" + TDCurrentTax[1].Text + "~" + TDCurrentTax[2].Text + "~" + TDCurrentTax[3].Text + "~" + TDCurrentTax[4].Text + "~" + TDCurrentTax[5].Text + "~" + TDCurrentTax[6].Text + "~" + TDCurrentTax[7].Text + "~" + TDCurrentTax[8].Text + "~" + TDCurrentTax[9].Text + "~" + TaxAuthority;
                                        gc.insert_date(orderNumber, parcelNumber, 1663, CurrentTaxDetail1, 1, DateTime.Now);

                                    }
                                    if (TDCurrentTax.Count != 0 && !row.Text.Contains("Current Tax") && row.Text.Trim() != "" && !row.Text.Contains("Tax Period") && !row.Text.Contains("pay online") && TDCurrentTax.Count == 2)
                                    {
                                        string CurrentTaxDetail2 = "" + "~" + "" + "~" + "" + "~" + "" + "~" + "" + "~" + "" + "~" + "" + "~" + "" + "~" + "" + "~" + TDCurrentTax[1].Text + "~" + TaxAuthority;
                                        gc.insert_date(orderNumber, parcelNumber, 1663, CurrentTaxDetail2, 1, DateTime.Now);

                                    }
                                }

                            }
                            catch { }
                        }
                        if (strcurrenttax.Contains("Current Tax Bill Information"))
                        {
                            try
                            {
                                IWebElement CurrentTax = driver.FindElement(By.XPath("/html/body/center[2]/table[11]/tbody"));
                                IList<IWebElement> TRCurrentTax = CurrentTax.FindElements(By.TagName("tr"));
                                IList<IWebElement> THCurrentTax = CurrentTax.FindElements(By.TagName("th"));
                                IList<IWebElement> TDCurrentTax;
                                foreach (IWebElement row in TRCurrentTax)
                                {
                                    TDCurrentTax = row.FindElements(By.TagName("td"));
                                    if (TDCurrentTax.Count != 0 && !row.Text.Contains("Current Tax") && row.Text.Trim() != "" && !row.Text.Contains("Tax Period") && !row.Text.Contains("pay online") && TDCurrentTax.Count == 10)
                                    {
                                        string CurrentTaxDetail1 = TDCurrentTax[0].Text + "~" + TDCurrentTax[1].Text + "~" + TDCurrentTax[2].Text + "~" + TDCurrentTax[3].Text + "~" + TDCurrentTax[4].Text + "~" + TDCurrentTax[5].Text + "~" + TDCurrentTax[6].Text + "~" + TDCurrentTax[7].Text + "~" + TDCurrentTax[8].Text + "~" + TDCurrentTax[9].Text + "~" + TaxAuthority;
                                        gc.insert_date(orderNumber, parcelNumber, 1663, CurrentTaxDetail1, 1, DateTime.Now);

                                    }
                                    if (TDCurrentTax.Count != 0 && !row.Text.Contains("Current Tax") && row.Text.Trim() != "" && !row.Text.Contains("Tax Period") && !row.Text.Contains("pay online") && TDCurrentTax.Count == 2)
                                    {
                                        string CurrentTaxDetail2 = "" + "~" + "" + "~" + "" + "~" + "" + "~" + "" + "~" + "" + "~" + "" + "~" + "" + "~" + "" + "~" + TDCurrentTax[1].Text + "~" + TaxAuthority;
                                        gc.insert_date(orderNumber, parcelNumber, 1663, CurrentTaxDetail2, 1, DateTime.Now);

                                    }
                                }

                            }
                            catch { }
                        }
                        //try
                        //{
                        //    IWebElement CurrentTax = driver.FindElement(By.XPath("/html/body/center[2]/table[10]/tbody"));
                        //    IList<IWebElement> TRCurrentTax = CurrentTax.FindElements(By.TagName("tr"));
                        //    IList<IWebElement> THCurrentTax = CurrentTax.FindElements(By.TagName("th"));
                        //    IList<IWebElement> TDCurrentTax;
                        //    foreach (IWebElement row in TRCurrentTax)
                        //    {
                        //        TDCurrentTax = row.FindElements(By.TagName("td"));
                        //        if (TDCurrentTax.Count != 0 && !row.Text.Contains("Current Tax") && row.Text.Trim() != "" && !row.Text.Contains("Tax Period") && !row.Text.Contains("pay online") && TDCurrentTax.Count == 10)
                        //        {
                        //            string CurrentTaxDetail1 = TDCurrentTax[0].Text + "~" + TDCurrentTax[1].Text + "~" + TDCurrentTax[2].Text + "~" + TDCurrentTax[3].Text + "~" + TDCurrentTax[4].Text + "~" + TDCurrentTax[5].Text + "~" + TDCurrentTax[6].Text + "~" + TDCurrentTax[7].Text + "~" + TDCurrentTax[8].Text + "~" + TDCurrentTax[9].Text;
                        //            gc.insert_date(orderNumber, parcelNumber, 1557, CurrentTaxDetail1, 1, DateTime.Now);

                        //        }
                        //        if (TDCurrentTax.Count != 0 && !row.Text.Contains("Current Tax") && row.Text.Trim() != "" && !row.Text.Contains("Tax Period") && !row.Text.Contains("pay online") && TDCurrentTax.Count == 2)
                        //        {
                        //            string CurrentTaxDetail2 = "" + "~" + "" + "~" + "" + "~" + "" + "~" + "" + "~" + "" + "~" + "" + "~" + "" + "~" + "" + "~" + TDCurrentTax[1].Text;
                        //            gc.insert_date(orderNumber, parcelNumber, 1557, CurrentTaxDetail2, 1, DateTime.Now);

                        //        }
                        //    }

                        //}
                        //catch { }

                        // Tax History Taxes
                        int tyear = 0, tmonth=0;
                        try
                        {
                            //IWebElement ITaxyear = driver.FindElement(By.XPath("/html/body/center[2]/table[3]/tbody/tr[3]/td[1]"));
                            //TaxYear = DateTime.Now.Year;
                            //tyear = Convert.ToInt16(TaxYear);
                            tyear = DateTime.Now.Year;
                            tmonth = DateTime.Now.Month;
                            if (tmonth < 9)
                            {
                                tyear = tyear-1;
                            }
                            else
                            {
                                tyear = tyear;
                            }
                        }
                        catch { }
                        //try
                        //{
                        //    driver.FindElement(By.LinkText("Show Historical Taxes")).Click();
                        //    Thread.Sleep(2000);
                        //    gc.CreatePdf(orderNumber, parcelNumber, "Tax Histroy Details", driver, "HI", "Hawaii");
                        //}
                        //catch { }
                        string strHistory = "", strcurrent="";
                        strHistory = driver.FindElement(By.XPath("/html/body/center[2]/table[12]/tbody/tr[1]/td")).Text;
                        strcurrent = driver.FindElement(By.XPath("/html/body/center[2]/table[11]/tbody/tr[1]/td")).Text;
                        if (strcurrent.Contains("Historical Tax Information"))
                        {
                            try
                            {
                                IWebElement TaxHistroy1 = driver.FindElement(By.XPath("/html/body/center[2]/table[11]/tbody"));
                                IList<IWebElement> TRTaxHistroy1 = TaxHistroy1.FindElements(By.TagName("tr"));
                                IList<IWebElement> THTaxHistroy1 = TaxHistroy1.FindElements(By.TagName("th"));
                                IList<IWebElement> TDTaxHistroy1;
                                foreach (IWebElement row in TRTaxHistroy1)
                                {
                                    TDTaxHistroy1 = row.FindElements(By.TagName("td"));
                                    if (TDTaxHistroy1.Count != 0 && !row.Text.Contains("Historical Tax") && row.Text.Trim() != "" && !row.Text.Contains("Penalty") && !row.Text.Contains("Click") && TDTaxHistroy1.Count == 7)
                                    {
                                        string TaxHistoryDetail = TDTaxHistroy1[0].Text + "~" + TDTaxHistroy1[1].Text + "~" + TDTaxHistroy1[2].Text + "~" + TDTaxHistroy1[3].Text + "~" + TDTaxHistroy1[4].Text + "~" + TDTaxHistroy1[5].Text + "~" + TDTaxHistroy1[6].Text;
                                        gc.insert_date(orderNumber, parcelNumber, 1664, TaxHistoryDetail, 1, DateTime.Now);
                                    }

                                }

                            }
                            catch { }
                        }
                        else if (strHistory.Contains("Historical Tax Information"))
                        {

                            try
                            {
                                IWebElement TaxHistroy1 = driver.FindElement(By.XPath("/html/body/center[2]/table[12]/tbody"));
                                IList<IWebElement> TRTaxHistroy1 = TaxHistroy1.FindElements(By.TagName("tr"));
                                IList<IWebElement> THTaxHistroy1 = TaxHistroy1.FindElements(By.TagName("th"));
                                IList<IWebElement> TDTaxHistroy1;
                                foreach (IWebElement row in TRTaxHistroy1)
                                {
                                    TDTaxHistroy1 = row.FindElements(By.TagName("td"));
                                    if (TDTaxHistroy1.Count != 0 && !row.Text.Contains("Historical Tax") && row.Text.Trim() != "" && !row.Text.Contains("Penalty") && !row.Text.Contains("Click") && TDTaxHistroy1.Count == 7)
                                    {
                                        string TaxHistoryDetail = TDTaxHistroy1[0].Text + "~" + TDTaxHistroy1[1].Text + "~" + TDTaxHistroy1[2].Text + "~" + TDTaxHistroy1[3].Text + "~" + TDTaxHistroy1[4].Text + "~" + TDTaxHistroy1[5].Text + "~" + TDTaxHistroy1[6].Text;
                                        gc.insert_date(orderNumber, parcelNumber, 1664, TaxHistoryDetail, 1, DateTime.Now);
                                    }

                                }

                            }
                            catch { }
                        }

                        string current = driver.CurrentWindowHandle;

                        for (int i = 0; i < 3; i++)
                        {
                            if (strcurrent.Contains("Historical Tax Information"))
                            {
                                try
                                {
                                    IWebElement TaxHistroy = driver.FindElement(By.XPath("/html/body/center[2]/table[11]/tbody"));
                                    IList<IWebElement> TRTaxHistroy = TaxHistroy.FindElements(By.TagName("tr"));
                                    IList<IWebElement> THTaxHistroy = TaxHistroy.FindElements(By.TagName("th"));
                                    IList<IWebElement> TDTaxHistroy;
                                    foreach (IWebElement row in TRTaxHistroy)
                                    {
                                        TDTaxHistroy = row.FindElements(By.TagName("td"));
                                        if (TDTaxHistroy.Count != 0 && !row.Text.Contains("Historical Tax") && row.Text.Trim() != "" && !row.Text.Contains("Penalty") && !row.Text.Contains("Click") && TDTaxHistroy[0].Text.Trim() == Convert.ToString(tyear) && TDTaxHistroy.Count == 7)
                                        {
                                            IWebElement IRealEstate = TDTaxHistroy[0].FindElement(By.TagName("a"));
                                            IRealEstate.Click();
                                            Thread.Sleep(4000);
                                            break;
                                        }

                                    }

                                }
                                catch { }

                            }
                            else if (strHistory.Contains("Historical Tax Information"))
                            {
                                try
                                {
                                    IWebElement TaxHistroy = driver.FindElement(By.XPath("/html/body/center[2]/table[12]/tbody"));
                                    IList<IWebElement> TRTaxHistroy = TaxHistroy.FindElements(By.TagName("tr"));
                                    IList<IWebElement> THTaxHistroy = TaxHistroy.FindElements(By.TagName("th"));
                                    IList<IWebElement> TDTaxHistroy;
                                    foreach (IWebElement row in TRTaxHistroy)
                                    {
                                        TDTaxHistroy = row.FindElements(By.TagName("td"));
                                        if (TDTaxHistroy.Count != 0 && !row.Text.Contains("Historical Tax") && row.Text.Trim() != "" && !row.Text.Contains("Penalty") && !row.Text.Contains("Click") && TDTaxHistroy[0].Text.Trim() == Convert.ToString(tyear) && TDTaxHistroy.Count == 7)
                                        {
                                            IWebElement IRealEstate = TDTaxHistroy[0].FindElement(By.TagName("a"));
                                            IRealEstate.Click();
                                            Thread.Sleep(4000);
                                            break;
                                        }

                                    }

                                }
                                catch { }

                            }
                            try
                            {
                                driver.SwitchTo().Window(driver.WindowHandles.Last());
                                Thread.Sleep(2000);
                            }
                            catch { }


                            try
                            {
                                gc.CreatePdf(orderNumber, parcelNumber, "Tax Info Details" + tyear, driver, "HI", "Hawaii");
                                IWebElement TaxPayHistroy = driver.FindElement(By.XPath("/html/body/center[2]/table[4]/tbody"));
                                IList<IWebElement> TRTaxPayHistroy = TaxPayHistroy.FindElements(By.TagName("tr"));
                                IList<IWebElement> THTaxPayHistroy = TaxPayHistroy.FindElements(By.TagName("th"));
                                IList<IWebElement> TDTaxPayHistroy;
                                foreach (IWebElement row in TRTaxPayHistroy)
                                {
                                    TDTaxPayHistroy = row.FindElements(By.TagName("td"));
                                    if (TDTaxPayHistroy.Count != 0 && !row.Text.Contains("Tax Payments") && row.Text.Trim() != "" && !row.Text.Contains("Payment Sequence") && !row.Text.Contains("Totals") && TDTaxPayHistroy.Count == 7)
                                    {
                                        string TaxPayHistoryDetail1 = TDTaxPayHistroy[0].Text + "~" + TDTaxPayHistroy[1].Text + "~" + TDTaxPayHistroy[2].Text + "~" + TDTaxPayHistroy[3].Text + "~" + TDTaxPayHistroy[4].Text + "~" + TDTaxPayHistroy[5].Text + "~" + TDTaxPayHistroy[6].Text;
                                        gc.insert_date(orderNumber, parcelNumber, 1668, TaxPayHistoryDetail1, 1, DateTime.Now);
                                    }
                                    if (TDTaxPayHistroy.Count != 0 && !row.Text.Contains("Tax Payments") && row.Text.Trim() != "" && !row.Text.Contains("Payment Sequence") && row.Text.Contains("Totals") && TDTaxPayHistroy.Count == 5)
                                    {
                                        string TaxPayHistoryDetail1 = "" + "~" + TDTaxPayHistroy[0].Text + "~" + "" + "~" + TDTaxPayHistroy[1].Text + "~" + TDTaxPayHistroy[2].Text + "~" + TDTaxPayHistroy[3].Text + "~" + TDTaxPayHistroy[4].Text;
                                        gc.insert_date(orderNumber, parcelNumber, 1668, TaxPayHistoryDetail1, 1, DateTime.Now);
                                    }
                                    if (TDTaxPayHistroy.Count != 0 && row.Text.Trim() != "" && !row.Text.Contains("Payment Sequence") && row.Text.Contains("No Tax Payments") && TDTaxPayHistroy.Count == 1)
                                    {
                                        string TaxPayHistoryDetail1 = "" + "~" + TDTaxPayHistroy[0].Text + "~" + "" + "~" + "" + "~" + "" + "~" + "" + "~" + "";
                                        gc.insert_date(orderNumber, parcelNumber, 1668, TaxPayHistoryDetail1, 1, DateTime.Now);
                                    }

                                }

                            }
                            catch { }

                            string strOwnername = "", strlocation = "", strPropertyType = "", strNeighborhoodcode = "", strlegalinfo = "", strlandarea = "";
                            try
                            {
                                strOwnername = driver.FindElement(By.XPath("/html/body/center[2]/table[2]/tbody/tr[2]/td[2]")).Text.Trim();
                                strlocation = driver.FindElement(By.XPath("/html/body/center[2]/table[2]/tbody/tr[4]/td[2]")).Text.Trim();
                                strPropertyType = driver.FindElement(By.XPath("/html/body/center[2]/table[2]/tbody/tr[5]/td[2]")).Text.Trim();
                                strNeighborhoodcode = driver.FindElement(By.XPath("/html/body/center[2]/table[2]/tbody/tr[6]/td[2]")).Text.Trim();
                                strlegalinfo = driver.FindElement(By.XPath("/html/body/center[2]/table[2]/tbody/tr[7]/td[2]")).Text.Trim();
                                strlandarea = driver.FindElement(By.XPath("/html/body/center[2]/table[2]/tbody/tr[6]/td[4]")).Text.Trim();
                            }
                            catch { }

                            //  Tax Information Details
                            try
                            {
                                IWebElement TaxDetails = driver.FindElement(By.XPath("/html/body/center[2]/table[3]/tbody"));
                                IList<IWebElement> TRTaxDetails = TaxDetails.FindElements(By.TagName("tr"));
                                IList<IWebElement> THTaxDetails = TaxDetails.FindElements(By.TagName("th"));
                                IList<IWebElement> TDTaxDetails;
                                foreach (IWebElement row in TRTaxDetails)
                                {
                                    TDTaxDetails = row.FindElements(By.TagName("td"));
                                    if (TDTaxDetails.Count != 0 && !row.Text.Contains("Tax Details") && row.Text.Trim() != "" && !row.Text.Contains("Tax Period") && !row.Text.Contains("Totals") && TDTaxDetails.Count == 7)
                                    {
                                        string TaxInfoDetail1 = TDTaxDetails[0].Text + "~" + TDTaxDetails[1].Text + "~" + TDTaxDetails[2].Text + "~" + TDTaxDetails[3].Text + "~" + TDTaxDetails[4].Text + "~" + TDTaxDetails[5].Text + "~" + TDTaxDetails[6].Text;
                                        gc.insert_date(orderNumber, parcelNumber, 1669, TaxInfoDetail1, 1, DateTime.Now);

                                    }
                                    if (TDTaxDetails.Count != 0 && !row.Text.Contains("Tax Details") && row.Text.Trim() != "" && !row.Text.Contains("Tax Period") && row.Text.Contains("Totals") && TDTaxDetails.Count == 6)
                                    {
                                        string TaxInfoDetail2 = "" + "~" + TDTaxDetails[0].Text + "~" + TDTaxDetails[1].Text + "~" + TDTaxDetails[2].Text + "~" + TDTaxDetails[3].Text + "~" + TDTaxDetails[4].Text + "~" + TDTaxDetails[5].Text;
                                        gc.insert_date(orderNumber, parcelNumber, 1669, TaxInfoDetail2, 1, DateTime.Now);

                                    }
                                }


                            }
                            catch { }

                            //  Tax Credit Details

                            //try
                            //{
                            //    IWebElement TaxCredit = driver.FindElement(By.XPath("/html/body/center[2]/table[5]/tbody"));
                            //    IList<IWebElement> TRTaxCredit = TaxCredit.FindElements(By.TagName("tr"));
                            //    IList<IWebElement> THTaxCredit = TaxCredit.FindElements(By.TagName("th"));
                            //    IList<IWebElement> TDTaxCredit;
                            //    foreach (IWebElement row in TRTaxCredit)
                            //    {
                            //        TDTaxCredit = row.FindElements(By.TagName("td"));
                            //        if (TDTaxCredit.Count != 0 && !row.Text.Contains("Tax Credits") && row.Text.Trim() != "" && !row.Text.Contains("Period") && !row.Text.Contains("Total") && TDTaxCredit.Count == 3)
                            //        {
                            //            string TaxCreditDetail1 = TDTaxCredit[0].Text + "~" + TDTaxCredit[1].Text + "~" + TDTaxCredit[2].Text;
                            //            gc.insert_date(orderNumber, parcelNumber, 1584, TaxCreditDetail1, 1, DateTime.Now);

                            //        }
                            //        if (TDTaxCredit.Count != 0 && !row.Text.Contains("Tax Credits") && row.Text.Trim() != "" && !row.Text.Contains("Period") && row.Text.Contains("Total") && TDTaxCredit.Count == 2)
                            //        {
                            //            string TaxCreditDetail2 = "" + "~" + TDTaxCredit[0].Text + "~" + TDTaxCredit[1].Text;
                            //            gc.insert_date(orderNumber, parcelNumber, 1584, TaxCreditDetail2, 1, DateTime.Now);

                            //        }
                            //        if (TDTaxCredit.Count != 0 && row.Text.Trim() != "" && !row.Text.Contains("Period") && !row.Text.Contains("Total") && row.Text.Contains("No Tax Credits applicable") && TDTaxCredit.Count == 1)
                            //        {
                            //            string TaxCreditDetail2 = "" + "~" + TDTaxCredit[0].Text + "~" + "";
                            //            gc.insert_date(orderNumber, parcelNumber, 1584, TaxCreditDetail2, 1, DateTime.Now);

                            //        }
                            //    }


                            //}
                            //catch { }

                            //driver.SwitchTo().Window(current);

                            driver.Navigate().Back();
                            Thread.Sleep(3000);
                            tyear--;


                        }

                        TaxTime = DateTime.Now.ToString("HH:mm:ss");

                        driver.Quit();

                        gc.mergpdf(orderNumber, "HI", "Hawaii");
                        LastEndTime = DateTime.Now.ToString("HH:mm:ss");
                        gc.insert_TakenTime(orderNumber, "HI", "Hawaii", StartTime, AssessmentTime, TaxTime, CitytaxTime, LastEndTime);
                        return "Data Inserted Successfully";

                    }
                    catch (Exception ex)
                    {
                        driver.Quit();
                        GlobalClass.LogError(ex, orderNumber);
                        throw ex;

                    }
            }
        }


    }
}