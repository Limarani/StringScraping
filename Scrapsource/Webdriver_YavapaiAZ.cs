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
using System.Collections.ObjectModel;

namespace ScrapMaricopa.Scrapsource
{
    public class Webdriver_YavapaiAZ
    {
        string outputPath = "";
        IWebDriver driver;
        IWebElement Parcelweb;
        DBconnection db = new DBconnection();
        GlobalClass gc = new GlobalClass();
        MySqlParameter[] mParam;
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["MyConnectionString"].ToString());
        public string FTP_YavapaiAZ(string address, string parcelNumber, string ownername, string searchType, string orderNumber, string directParcel)
        {
            GlobalClass.global_orderNo = orderNumber;
            HttpContext.Current.Session["orderNo"] = orderNumber;
            GlobalClass.global_parcelNo = parcelNumber;
            List<string> taxinformation = new List<string>();
            string StartTime = "", AssessmentTime = "", TaxTime = "", CitytaxTime = "", LastEndTime = "";
            string As_of = "", Total_Due = "", MillLevy = "", Class = "", Built = "";
            List<string> pdflink = new List<string>();
            string Parcel_number = "", Tax_Authority = "", yearbuild = "", AddressCombain = "", Addresshrf = "", TaxesDue = "", Multiaddressadd = "", MailingAddress = "";
            var driverService = PhantomJSDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            //driver = new PhantomJSDriver();
            //driver = new ChromeDriver()
            using (driver = new PhantomJSDriver())
            {
                StartTime = DateTime.Now.ToString("HH:mm:ss");
                try
                {
                    if (searchType == "titleflex")
                    {

                        gc.TitleFlexSearch(orderNumber, "", ownername, address, "CO", "Adams");
                        if ((HttpContext.Current.Session["TitleFlex_Search"] != null && HttpContext.Current.Session["TitleFlex_Search"].ToString() == "Yes"))
                        {
                            return "MultiParcel";
                        }
                        searchType = "parcel";
                        parcelNumber = HttpContext.Current.Session["titleparcel"].ToString().Replace(".", "");
                    }
                    driver.Navigate().GoToUrl("http://gis.yavapai.us/v4/");
                    driver.FindElement(By.XPath("//*[@id='Disclaimer_tab']/input")).Click();
                    Thread.Sleep(2000);
                    if (searchType == "address")
                    {
                        driver.FindElement(By.Id("p_search")).SendKeys(address);
                        gc.CreatePdf_WOP(orderNumber, "Address Search", driver, "AZ", "Yavapai");
                        driver.FindElement(By.Id("search")).Click();
                        Thread.Sleep(2000);
                        gc.CreatePdf_WOP(orderNumber, "Address After", driver, "AZ", "Yavapai");
                    }
                    if (searchType == "parcel")
                    {
                        driver.FindElement(By.XPath("//*[@id='parcSearch_titleBarNode']/div/span[3]")).Click();
                        Thread.Sleep(2000);
                        string[] arryparcel = parcelNumber.Split('-');
                        driver.FindElement(By.Id("par_book")).SendKeys(arryparcel[0]);
                        driver.FindElement(By.Id("par_map")).SendKeys(arryparcel[1]);
                        driver.FindElement(By.Id("par_par")).SendKeys(arryparcel[1]);
                        driver.FindElement(By.Id("par_submit")).Click();
                        Thread.Sleep(2000);
                    }
                    if (searchType == "ownername")
                    {
                        driver.FindElement(By.XPath("//*[@id='ownerSearch_titleBarNode']/div/span[3]")).Click();
                        Thread.Sleep(2000);
                        gc.CreatePdf_WOP(orderNumber, "OwnerName1", driver, "AZ", "Yavapai");
                        string[] Ownersplit = ownername.Split(' ');
                        driver.FindElement(By.Id("lastName")).SendKeys(Ownersplit[1]);
                        driver.FindElement(By.Id("firstName")).SendKeys(Ownersplit[0]);
                        gc.CreatePdf_WOP(orderNumber, "Owner After", driver, "AZ", "Yavapai");
                        driver.FindElement(By.XPath("//*[@id='ownerSearch_pane']/table/tbody/tr[3]/td/input")).Click();
                        Thread.Sleep(2000);
                        try
                        {
                            string Nodata = driver.FindElement(By.Id("alertmsg")).Text;
                            gc.CreatePdf_WOP(orderNumber, "No data", driver, "AZ", "Yavapai");
                            if (Nodata.Contains("Sorry, no records"))
                            {
                                HttpContext.Current.Session["Nodata_Yavapai"] = "Zero";
                                driver.Quit();
                                return "No Data Found";
                            }
                        }
                        catch { }
                    }
                    string parcelno = driver.FindElement(By.XPath("//*[@id='results_Status']/h3/i")).Text;
                    Parcel_number = GlobalClass.After(parcelno, "Information for Parcel").Replace(":", "");
                    string owner = driver.FindElement(By.Id("owner_name")).Text;
                    string proaddress1 = driver.FindElement(By.Id("owner_address")).Text;
                    string proaddress2 = driver.FindElement(By.XPath("//*[@id='resOwner_pane']/table/tbody/tr[4]/td")).Text;
                    string proaddress = proaddress1 + " " + proaddress2;
                    string AssessorAcres = driver.FindElement(By.Id("dorAcres")).Text;
                    string subdivision = driver.FindElement(By.Id("subdiv")).Text;
                    string Maptype = driver.FindElement(By.Id("subdivType")).Text;
                    string countyzoning = driver.FindElement(By.Id("ctyZoneViol")).Text;
                    string sectionTownship = driver.FindElement(By.Id("str")).Text;
                    string Homestead = driver.FindElement(By.Id("HES")).Text;
                    string IncorporatedArea = driver.FindElement(By.Id("inc_area")).Text;
                    string Tracts = driver.FindElement(By.Id("tract")).Text;
                    driver.FindElement(By.XPath("//*[@id='resImps_titleBarNode']/div")).Click();
                    Thread.Sleep(2000);
                    string type1 = driver.FindElement(By.Id("imps")).Text;
                    string type = gc.Between(type1, "Type:", "Floor area:");
                    string Constructed = GlobalClass.After(type1, "Constructed:");
                    string Proresult = owner + "~" + proaddress + "~" + AssessorAcres + "~" + subdivision + "~" + Maptype + "~" + countyzoning + "~" + sectionTownship + "~" + Homestead + "~" + IncorporatedArea + "~" + Tracts + "~" + type + "~" + Constructed;
                    gc.insert_date(orderNumber, Parcel_number.Replace("-", ""), 1803, Proresult, 1, DateTime.Now);
                    driver.FindElement(By.XPath("//*[@id='resAssess_titleBarNode']")).Click();
                    Thread.Sleep(2000);
                    IWebElement Assessmenttable = driver.FindElement(By.XPath("//*[@id='resAssess_pane']/table/tbody"));
                    IList<IWebElement> Assessmentrow = Assessmenttable.FindElements(By.TagName("tr"));
                    IList<IWebElement> assessmentid;
                    foreach (IWebElement assessment in Assessmentrow)
                    {
                        assessmentid = assessment.FindElements(By.TagName("td"));
                        if (assessmentid.Count != 0 && !assessment.Text.Contains("Tax Year") && assessment.Text.Trim() != "")
                        {
                            string Assessmentresult = assessmentid[0].Text + "~" + assessmentid[1].Text + "~" + assessmentid[2].Text;
                            gc.insert_date(orderNumber, Parcel_number.Replace("-", ""), 1804, Assessmentresult, 1, DateTime.Now);
                        }
                        if (assessment.Text.Contains("Tax Year"))
                        {
                            string AssessmentHead = assessmentid[0].Text + "~" + assessmentid[1].Text + "~" + assessmentid[2].Text;
                            db.ExecuteQuery("update data_field_master set Data_Fields_Text='" + AssessmentHead + "' where Id = '" + 1804 + "'");
                        }
                    }
                    driver.FindElement(By.XPath("//*[@id='resTaxes_titleBarNode']")).Click();
                    Thread.Sleep(2000);
                    string TaxHead = "", Taxresult = "";
                    IWebElement Taxtable = driver.FindElement(By.XPath("//*[@id='taxTable']/table/tbody"));
                    IList<IWebElement> Taxtrow = Taxtable.FindElements(By.TagName("tr"));
                    IList<IWebElement> Taxid;
                    foreach (IWebElement Taxweb in Taxtrow)
                    {
                        Taxid = Taxweb.FindElements(By.TagName("td"));
                        if (Taxid.Count > 1 && !Taxweb.Text.Contains("Year"))
                        {
                            TaxHead += Taxid[0].Text + "~";
                            Taxresult += Taxid[1].Text + "~";
                        }
                    }
                    db.ExecuteQuery("update data_field_master set Data_Fields_Text='" + TaxHead.Remove(TaxHead.Length - 1) + "' where Id = '" + 1805 + "'");
                    gc.insert_date(orderNumber, Parcel_number.Replace("-", ""), 1805, Taxresult.Remove(Taxresult.Length - 1), 1, DateTime.Now);
                    try
                    {
                        driver.FindElement(By.LinkText("Tax Authorities:")).Click();
                        Thread.Sleep(2000);
                    }
                    catch { }
                        IWebElement Iviewtax = driver.FindElement(By.Id("taxAuth"));
                        IJavaScriptExecutor js = driver as IJavaScriptExecutor;
                        js.ExecuteScript("arguments[0].click();", Iviewtax);
                 
                    gc.CreatePdf(orderNumber, parcelNumber, "Authorit", driver, "AZ", "Yavapai");
                    IWebElement Authoritytable = driver.FindElement(By.XPath("//*[@id='taxAuth']/table/tbody"));
                    IList<IWebElement> Authorityrow = Authoritytable.FindElements(By.TagName("tr"));
                    IList<IWebElement> Authorityid;
                    foreach (IWebElement Authority in Authorityrow)
                    {
                        Authorityid = Authority.FindElements(By.TagName("td"));
                        if (Authorityid.Count > 1 && !Authority.Text.Contains("Authority"))
                        {
                            string Authorityresult = Authorityid[0].Text + "~" + Authorityid[1].Text;
                            gc.insert_date(orderNumber, Parcel_number.Replace("-", ""), 1806, Authorityresult, 1, DateTime.Now);
                        }
                    }
                    //Tax Site
                    driver.Navigate().GoToUrl("http://taxinquiry.yavapai.us/");
                    driver.FindElement(By.XPath("//*[@id='main']/table/tbody/tr/td[2]/div/div[1]/form/div/div/span/span/input")).SendKeys(Parcel_number);
                    driver.FindElement(By.XPath("//*[@id='main']/table/tbody/tr/td[2]/div/div[1]/form/div/input[1]")).Click();
                    IWebElement TaxInfotable = driver.FindElement(By.XPath("//*[@id='Grid']/table/tbody"));
                    IList<IWebElement> TaxInforow = TaxInfotable.FindElements(By.TagName("tr"));
                    IList<IWebElement> TaxInfoid;
                    foreach (IWebElement TaxInfo in TaxInforow)
                    {
                        TaxInfoid = TaxInfo.FindElements(By.TagName("td"));
                        if (TaxInfoid.Count > 1)
                        {
                            string Taxinforesult = TaxInfoid[1].Text + "~" + TaxInfoid[2].Text + "~" + TaxInfoid[3].Text + "~" + TaxInfoid[4].Text + "~" + TaxInfoid[5].Text + "~" + TaxInfoid[6].Text;
                            gc.insert_date(orderNumber, Parcel_number.Replace("-", ""), 1807, Taxinforesult, 1, DateTime.Now);
                        }
                    }
                    IWebElement Taxyeardue = driver.FindElement(By.XPath("//*[@id='PanelBar']/li[1]/ul/li[3]")).FindElement(By.TagName("a"));
                    string taxyearhref = Taxyeardue.GetAttribute("href");
                    driver.Navigate().GoToUrl(taxyearhref);
                    for (int i = 0; i < 3; i++)
                    {
                        if (i == 1)
                        {
                            IWebElement javaclick = driver.FindElement(By.XPath("//*[@id='taxYear_listbox']/li[2]"));
                            IJavaScriptExecutor js1 = driver as IJavaScriptExecutor;
                            js1.ExecuteScript("arguments[0].click();", javaclick);
                            Thread.Sleep(9000);
                        }
                        if (i == 2)
                        {
                            IWebElement javaclick = driver.FindElement(By.XPath("//*[@id='taxYear_listbox']/li[3]"));
                            IJavaScriptExecutor js1 = driver as IJavaScriptExecutor;
                            js1.ExecuteScript("arguments[0].click();", javaclick);
                            Thread.Sleep(9000);
                        }
                        IWebElement TaxYeartable = driver.FindElement(By.XPath("//*[@id='Grid']/table/tbody"));
                        IList<IWebElement> TaxYearorow = TaxYeartable.FindElements(By.TagName("tr"));
                        IList<IWebElement> TaxYearid;
                        foreach (IWebElement TaxYearweb in TaxYearorow)
                        {
                            TaxYearid = TaxYearweb.FindElements(By.TagName("td"));
                            if (TaxYearid.Count > 1)
                            {
                                string TaxYearresult = TaxYearid[1].Text + "~" + TaxYearid[2].Text + "~" + TaxYearid[3].Text;
                                gc.insert_date(orderNumber, Parcel_number.Replace("-", ""), 1808, TaxYearresult, 1, DateTime.Now);
                            }
                        }
                        IWebElement TotalDuetable = driver.FindElement(By.XPath("//*[@id='Grid']/table/tfoot"));
                        IList<IWebElement> TotalDuerow = TotalDuetable.FindElements(By.TagName("tr"));
                        IList<IWebElement> TotalDueid;
                        foreach (IWebElement TotalDue in TotalDuerow)
                        {
                            TotalDueid = TotalDue.FindElements(By.TagName("td"));
                            if (TotalDueid.Count > 1)
                            {
                                string TaxYearresult = TotalDueid[1].Text + "~" + TotalDueid[2].Text + "~" + TotalDueid[3].Text;
                                gc.insert_date(orderNumber, Parcel_number.Replace("-", ""), 1808, TaxYearresult, 1, DateTime.Now);
                            }
                        }
                        IWebElement Viwebill = driver.FindElement(By.XPath("//*[@id='main']/table/tbody/tr/td[2]/form/table/tbody/tr/td[5]")).FindElement(By.TagName("a"));
                        string Viewhref = Viwebill.GetAttribute("href");
                        pdflink.Add(Viewhref);
                        //gc.downloadfile(Viewhref, orderNumber, Parcel_number, "TaxBill" + i, "AZ", "Yavapai");
                    }
                    int m = 0;
                    foreach (string pdf in pdflink)
                    {
                        driver.Navigate().GoToUrl(pdf);
                        gc.CreatePdf(orderNumber, parcelNumber, "Tax Bill" + m, driver, "AZ", "Yavapai");
                        m++;
                    }
                    TaxTime = DateTime.Now.ToString("HH:mm:ss");
                    LastEndTime = DateTime.Now.ToString("HH:mm:ss");
                    gc.insert_TakenTime(orderNumber, "AZ", "Yavapai", StartTime, AssessmentTime, TaxTime, CitytaxTime, LastEndTime);
                    driver.Quit();
                    //HttpContext.Current.Session["titleparcel"] = null;
                    gc.mergpdf(orderNumber, "AZ", "Yavapai");
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