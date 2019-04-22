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
using System.ComponentModel;
using System.Text;
using HtmlAgilityPack;
using iTextSharp.text;
using System.Text.RegularExpressions;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Support.Extensions;
using System.Net;
using OpenQA.Selenium.Firefox;
using System.Diagnostics;
using OpenQA.Selenium.Remote;
using Org.BouncyCastle.Utilities;

namespace ScrapMaricopa.Scrapsource
{
    public class WebDriver_BrevardFL
    {
        IWebDriver driver;
        IList<IWebElement> taxPaymentdetails, taxPaymentAmountdetails, Itaxtd;
        List<string> strSecured = new List<string>();
        List<string> strCombinedTax = new List<string>();
        List<string> strTaxRealestate = new List<string>();
        List<string> downloadbill = new List<string>();
        List<string> strTaxRealCount = new List<string>();
        List<string> IssuedURL = new List<string>();
        DBconnection db = new DBconnection();
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["MyConnectionString"].ToString());
        GlobalClass gc = new GlobalClass();
        string strMulti = "-", strParcel = "-", strMultiDetails = "-", strParcelNumber = "-", strOwner = "-", strAddress = "-", strProeprtyAddress = "-", strOwnerName = "-",
              strMailingAddress = "-", strLegalDiscription = "-", strYear = "-", strLand = "-", strBuilding = "-", strMarket = "-", strAssessed = "-", strTax = "-",
              strType = "-", strTaxType = "-", strAlternetKey = "-", strEscrowCode = "-", strMillageCode = "-", strTaxAuthority = "-", strMillege = "-", strAssess = "-",
              strExemption = "-", strTaxable = "-", strTaxAmount = "-", strParcelNo = "-", strTaxCom = "-", strLInk = "-", strDate = "-", strAmount = "-", strFace = "-",
              strBid = "-", strBidder = "-", strCertificate = "-", strTYear = "-", strPaid = "-", strBill = "-", strBalance = "-", strBillDate = "-", strBillPaid = "-",
              strFBill = "-", strFBalance = "-", strFBillDate = "-", strFBillPaid = "-", strbillTaxAuthority = "-", strbillMillege = "-", strbillAssess = "-",
              strbillExemption = "-", strbillTaxable = "-", strbillTaxAmount = "-", strValAuthority = "-", strValRate = "-", strValAmount = "-", strValoremAuthority = "-",
              strValoremRate = "-", strValoremAmount = "-", strAssessTitle = "-", strCounty = "-", strSchool = "-", strMunicipal = "-", strIndependent = "-", strTaxBill = "",
              strMillageRate = "-", strIssue = "", strIssuePaid = "", strissueTaxType = "", strissueAlternetKey = "", strissueMillageCode = "", strissueMillageRate = "", strissueParcelNo = "-",
              strVTaxType = "-", strNVTaxType = "-", strcombine = "-", strLTaxAuthority = "-", strLMillege = "-", strLAssess = "-", strLExemption = "-", strLTaxable = "-", strLTaxAmount = "-",
               strTaxFeed = "", strBillEff = "", strRate = "", yearbuilt = "", strBillPayReceipt = "", parcelid = "", accno = "", latestfile = "", latestfile1 = "";
        int MultiCount, AnualBillCount, TaxCount = 0, ParcelCount = 0;
        public string FTP_FLBrevard(string houseno, string direction, string sname, string sttype, string unitnumber, string parcelNumber, string ownername, string searchType, string orderNumber, string directParcel)
        {
            GlobalClass.global_orderNo = orderNumber;
            HttpContext.Current.Session["orderNo"] = orderNumber;
            GlobalClass.global_parcelNo = parcelNumber;
            string StartTime = "", AssessmentTime = "", TaxTime = "", CitytaxTime = "", LastEndTime = "";
            var driverService = PhantomJSDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            // driver = new PhantomJSDriver(); //ChromeDriver();
            // driver = new ChromeDriver();
            using (driver = new PhantomJSDriver())
            {
                try
                {
                    StartTime = DateTime.Now.ToString("HH:mm:ss");
                    driver.Navigate().GoToUrl("https://www.bcpao.us/PropertySearch/#/nav/Search");
                    Thread.Sleep(3000);
                    string address = "";

                    if (houseno != "" && unitnumber != "")
                    {
                        if (direction != "")
                        {
                            address = houseno + " " + direction + " " + sname + " " + sttype + " " + unitnumber;
                        }
                        else
                        {
                            address = houseno + " " + sname + " " + sttype + " " + unitnumber;
                        }
                    }

                    if (direction != "")
                    {
                        address = houseno + " " + direction + " " + sname + " " + sttype;
                    }

                    else
                    {
                        address = houseno + " " + sname + " " + sttype;
                    }

                    //if (searchType == "titleflex")
                    //{
                    //    string address = houseno + " " + strDirection + " " + sname + " " + strStreetType + unitnumber;
                    //    gc.TitleFlexSearch(orderNumber, parcelNumber, "", address, "FL", "Brevard");

                    //    if (HttpContext.Current.Session["TitleFlex_Search"] != null && HttpContext.Current.Session["TitleFlex_Search"].ToString() == "Yes")
                    //    {
                    //        return "MultiParcel";
                    //    }
                    //    parcelNumber = HttpContext.Current.Session["titleparcel"].ToString();
                    //    parcelNumber = HttpContext.Current.Session["titleparcel"].ToString().Replace(".", "").Replace("-", "");
                    //    searchType = "parcel";
                    //}

                    if (searchType == "address")
                    {
                        driver.FindElement(By.Id("txtPropertySearch_Address")).SendKeys(address);
                        gc.CreatePdf_WOP(orderNumber, "Address Search", driver, "FL", "Brevard");
                        driver.FindElement(By.Id("btnPropertySearch_RealProperty_Go")).Click();
                        Thread.Sleep(3000);
                        gc.CreatePdf_WOP(orderNumber, "Address Search Result", driver, "FL", "Brevard");

                        try
                        {
                            string nodata = driver.FindElement(By.Id("divNoResultsFound")).Text;
                            if (nodata.Contains("NO RESULTS FOUND"))
                            {
                                HttpContext.Current.Session["Nodata_BrevardFL"] = "Zero";
                                driver.Quit();
                                return "No Data Found";
                            }

                        }
                        catch { }
                        try
                        {
                            strMulti = driver.FindElement(By.Id("Results_Count")).Text;
                            string strMultiCount = gc.Between(strMulti, "of ", " records");
                            if (Convert.ToInt32(strMultiCount) > 25)
                            {
                                HttpContext.Current.Session["multiparcel_FLBrevard_Maximum"] = "Yes";
                                driver.Quit();
                                return "Maximum";
                            }
                            IWebElement IMulti = driver.FindElement(By.XPath("//*[@id='tblSearchResults']/tbody"));
                            IList<IWebElement> IMultirow = IMulti.FindElements(By.XPath("tr"));
                            IList<IWebElement> IMultitd;
                            foreach (IWebElement row in IMultirow)
                            {
                                //GlobalClass.multiPArcel_FLBroward = "Yes";
                                HttpContext.Current.Session["multiPArcel_FLBrevard"] = "Yes";
                                IMultitd = row.FindElements(By.XPath("td"));
                                if (IMultitd.Count != 0 && MultiCount <= 25)
                                {
                                    try
                                    {
                                        MultiCount++;
                                        strParcel = IMultitd[2].Text;
                                        strAddress = IMultitd[3].Text;
                                        strOwner = IMultitd[4].Text;
                                        strMultiDetails = strAddress.Trim() + "~" + strOwner.Trim();
                                        gc.insert_date(orderNumber, strParcel, 1513, strMultiDetails, 1, DateTime.Now);
                                    }
                                    catch { }
                                }
                            }
                            driver.Quit();
                            return "MultiParcel";
                        }
                        catch { }

                    }
                    else if (searchType == "parcel")
                    {
                        driver.FindElement(By.Id("txtPropertySearch_Pid")).SendKeys(parcelNumber);
                        gc.CreatePdf(orderNumber, parcelNumber, "Parcel Search", driver, "FL", "Brevard");
                        driver.FindElement(By.Id("btnPropertySearch_RealProperty_Go")).Click();
                        Thread.Sleep(3000);
                        gc.CreatePdf(orderNumber, parcelNumber, "Parcel Search Result", driver, "FL", "Brevard");
                    }
                    else if (searchType == "account")
                    {
                        driver.FindElement(By.Id("txtPropertySearch_Account")).SendKeys(unitnumber);
                        gc.CreatePdf(orderNumber, unitnumber, "Account Search", driver, "FL", "Brevard");
                        driver.FindElement(By.Id("btnPropertySearch_RealProperty_Go")).Click();
                        Thread.Sleep(3000);
                        gc.CreatePdf(orderNumber, unitnumber, "Account Search Result", driver, "FL", "Brevard");

                    }
                    else if (searchType == "ownername")
                    {
                        driver.FindElement(By.Id("txtPropertySearch_Owner")).SendKeys(ownername);
                        gc.CreatePdf_WOP(orderNumber, "Owner Search", driver, "FL", "Brevard");
                        driver.FindElement(By.Id("btnPropertySearch_RealProperty_Go")).Click();
                        Thread.Sleep(3000);
                        gc.CreatePdf_WOP(orderNumber, "Owner Search Result", driver, "FL", "Brevard");
                        try
                        {
                            strMulti = driver.FindElement(By.Id("Results_Count")).Text;
                            string strMultiCount = gc.Between(strMulti, "of ", " records");
                            if (Convert.ToInt32(strMultiCount) > 25)
                            {
                                HttpContext.Current.Session["multiparcel_FLBroward_Maximum"] = "Yes";
                                driver.Quit();
                                return "Maximum";
                            }
                            IWebElement IMulti = driver.FindElement(By.XPath("//*[@id='tblSearchResults']/tbody"));
                            IList<IWebElement> IMultirow = IMulti.FindElements(By.XPath("tr"));
                            IList<IWebElement> IMultitd;
                            foreach (IWebElement row in IMultirow)
                            {
                                HttpContext.Current.Session["multiPArcel_FLBrevard"] = "Yes";
                                // GlobalClass.multiPArcel_FLBroward = "Yes";
                                IMultitd = row.FindElements(By.XPath("td"));
                                if (IMultitd.Count != 0 && MultiCount <= 25)
                                {
                                    try
                                    {
                                        MultiCount++;
                                        strParcel = IMultitd[2].Text;
                                        strAddress = IMultitd[3].Text;
                                        strOwner = IMultitd[4].Text;
                                        strMultiDetails = strAddress.Trim() + "~" + strOwner.Trim();
                                        gc.insert_date(orderNumber, strParcel, 1513, strMultiDetails, 1, DateTime.Now);
                                    }
                                    catch { }
                                }
                            }
                            driver.Quit();
                            return "MultiParcel";
                        }
                        catch { }
                    }

                    //Property Details 
                    string owner = "", mailaddr = "", siteaddr = "", taxingdist = "", exemp = "", prop_use = "", subdivname = "", landdesc = "";
                    accno = driver.FindElement(By.Id("divPropertySearch_Details_Account")).Text;
                    accno = GlobalClass.After(accno, "Account:").Trim();//*[@id="cssDetails_Top_Outer"]/div[2]/div
                    string bulkproeprty = driver.FindElement(By.XPath("//*[@id='cssDetails_Top_Outer']/div[2]")).Text;
                    owner = gc.Between(bulkproeprty, "Owners:", "Mail Address:");
                    mailaddr = gc.Between(bulkproeprty, "Mail Address:", "Site Address:");
                    siteaddr = gc.Between(bulkproeprty, "Site Address:", "Parcel ID:");
                    parcelid = gc.Between(bulkproeprty, "Parcel ID:", "Taxing District:");
                    taxingdist = gc.Between(bulkproeprty, "Taxing District:", "Exemptions:");
                    exemp = gc.Between(bulkproeprty, "Exemptions:", "Property Use:");
                    prop_use = gc.Between(bulkproeprty, "Property Use:", "Total Acres:");
                    subdivname = gc.Between(bulkproeprty, "Subdivision Name:", "Land Description:");
                    landdesc = GlobalClass.After(bulkproeprty, "Land Description:");
                    try { 
                    string yearbuilttext = driver.FindElement(By.XPath("//*[@id='divBldg_Details']/table/tbody")).Text;
                 yearbuilt = gc.Between(yearbuilttext, "Year Built:", "Story Height:").Trim();
                    }
                    catch { }
                    //string strPropertyDetails = accno + "~" + owner + "~" + mailaddr + "~" + siteaddr + "~" + taxingdist + "~" + exemp + "~" + prop_use + "~" + subdivname + "~" + landdesc + "~" + yearbuilt;
                    //gc.insert_date(orderNumber, parcelid, 1507, strPropertyDetails, 1, DateTime.Now);
                    gc.CreatePdf(orderNumber, accno, "Property Search Result", driver, "FL", "Brevard");

                    //Year~Market Value~Agricultural Land Value~Assessed Value Non School~Assessed Value School~Homestead Exemption~Additional Homestead~Other Exemptions~Taxable Value Non School~Taxable Value School
                    IWebElement valuetableElement = driver.FindElement(By.XPath("//*[@id='divValues_Description']/div/table"));
                    IList<IWebElement> valuetableRow = valuetableElement.FindElements(By.TagName("tr"));
                    IList<IWebElement> valuetableheader = valuetableElement.FindElements(By.TagName("th"));

                    IList<IWebElement> valuerowTD;
                    IList<IWebElement> valuerowTH;
                    List<string> year = new List<string>();
                    List<string> MarketValue = new List<string>();
                    List<string> AgriculturalLandValue = new List<string>();
                    List<string> asvnonschool = new List<string>();
                    List<string> asvschool = new List<string>();
                    List<string> homeexem = new List<string>();
                    List<string> addhome = new List<string>();
                    List<string> otherexem = new List<string>();
                    List<string> taxvaluenonschool = new List<string>();
                    List<string> taxvalueschool = new List<string>();
                    int i = 0;
                    foreach (IWebElement row in valuetableRow)
                    {
                        valuerowTD = row.FindElements(By.TagName("td"));
                        valuerowTH = row.FindElements(By.TagName("th"));

                        if (valuerowTH.Count != 0)
                        {
                            year.Add(valuerowTH[1].Text);
                            year.Add(valuerowTH[2].Text);
                            year.Add(valuerowTH[3].Text);
                        }
                        if (valuerowTD.Count != 0)
                        {
                            if (i == 0)
                            {

                                MarketValue.Add(valuerowTD[1].Text.Trim());
                                MarketValue.Add(valuerowTD[2].Text.Trim());
                                MarketValue.Add(valuerowTD[3].Text.Trim());
                            }
                            else if (i == 1)
                            {
                                AgriculturalLandValue.Add(valuerowTD[1].Text);
                                AgriculturalLandValue.Add(valuerowTD[2].Text);
                                AgriculturalLandValue.Add(valuerowTD[3].Text);
                            }
                            else if (i == 2)
                            {
                                asvnonschool.Add(valuerowTD[1].Text);
                                asvnonschool.Add(valuerowTD[2].Text);
                                asvnonschool.Add(valuerowTD[3].Text);
                            }
                            else if (i == 3)
                            {
                                asvschool.Add(valuerowTD[1].Text);
                                asvschool.Add(valuerowTD[2].Text);
                                asvschool.Add(valuerowTD[3].Text);
                            }
                            else if (i == 4)
                            {
                                homeexem.Add(valuerowTD[1].Text);
                                homeexem.Add(valuerowTD[2].Text);
                                homeexem.Add(valuerowTD[3].Text);
                            }
                            else if (i == 5)
                            {
                                addhome.Add(valuerowTD[1].Text);
                                addhome.Add(valuerowTD[2].Text);
                                addhome.Add(valuerowTD[3].Text);
                            }
                            else if (i == 6)
                            {
                                otherexem.Add(valuerowTD[1].Text);
                                otherexem.Add(valuerowTD[2].Text);
                                otherexem.Add(valuerowTD[3].Text);
                            }
                            else if (i == 7)
                            {
                                taxvaluenonschool.Add(valuerowTD[1].Text);
                                taxvaluenonschool.Add(valuerowTD[2].Text);
                                taxvaluenonschool.Add(valuerowTD[3].Text);
                            }
                            else if (i == 8)
                            {
                                taxvalueschool.Add(valuerowTD[1].Text);
                                taxvalueschool.Add(valuerowTD[2].Text);
                                taxvalueschool.Add(valuerowTD[3].Text);
                            }
                            i++;
                        }

                    }
                    string assessment1 = year[0] + "~" + MarketValue[0] + "~" + AgriculturalLandValue[0] + "~" + asvnonschool[0] + "~" + asvschool[0] + "~" + homeexem[0] + "~" + addhome[0] + "~" + otherexem[0] + "~" + taxvaluenonschool[0] + "~" + taxvalueschool[0];
                    string assessment2 = year[1] + "~" + MarketValue[1] + "~" + AgriculturalLandValue[1] + "~" + asvnonschool[1] + "~" + asvschool[1] + "~" + homeexem[1] + "~" + addhome[1] + "~" + otherexem[1] + "~" + taxvaluenonschool[1] + "~" + taxvalueschool[1];
                    string assessment3 = year[2] + "~" + MarketValue[2] + "~" + AgriculturalLandValue[2] + "~" + asvnonschool[2] + "~" + asvschool[2] + "~" + homeexem[2] + "~" + addhome[2] + "~" + otherexem[2] + "~" + taxvaluenonschool[2] + "~" + taxvalueschool[2];

                    gc.insert_date(orderNumber, parcelid, 1508, assessment1, 1, DateTime.Now);
                    gc.insert_date(orderNumber, parcelid, 1508, assessment2, 1, DateTime.Now);
                    gc.insert_date(orderNumber, parcelid, 1508, assessment3, 1, DateTime.Now);

                    AssessmentTime = DateTime.Now.ToString("HH:mm:ss");
                    //Tax Authority
                    string Taxauthority = "", Taxauthority1 = "";
                    try
                    {
                        driver.Navigate().GoToUrl("https://www.brevardtaxcollector.com/contact/");
                        Taxauthority = driver.FindElement(By.XPath("//*[@id='cs-content']/div[2]/div/div/div[2]/p[3]")).Text.Trim();
                        gc.CreatePdf(orderNumber, accno, "Tax Authority", driver, "FL", "Brevard");
                    }
                    catch { }
                    string strPropertyDetails = accno + "~" + owner + "~" + mailaddr + "~" + siteaddr + "~" + taxingdist + "~" + exemp + "~" + prop_use + "~" + subdivname + "~" + landdesc + "~" + yearbuilt + "~" + Taxauthority;
                    gc.insert_date(orderNumber, parcelid, 1507, strPropertyDetails, 1, DateTime.Now);

                    //Tax Details 
                    // accno = "2531000";
                    driver.Navigate().GoToUrl("https://brevard.county-taxes.com/public");
                    driver.FindElement(By.Name("search_query")).SendKeys(accno);
                    // gc.CreatePdf(orderNumber, parcelid, "Tax Search", driver, "FL", "Brevard");
                    IWebElement element = driver.FindElement(By.XPath("//*[@id='search-controls']/div/form[1]/div[1]/div/span/button"));
                    IJavaScriptExecutor js = driver as IJavaScriptExecutor;
                    js.ExecuteScript("arguments[0].click();", element);
                    Thread.Sleep(4000);
                    gc.CreatePdf(orderNumber, accno, "Tax Search Result", driver, "FL", "Brevard");
                    try
                    {
                        IWebElement ITaxSearch = driver.FindElement(By.LinkText("Full bill history"));
                        string strITaxSearch = ITaxSearch.GetAttribute("href");
                        driver.Navigate().GoToUrl(strITaxSearch);
                        Thread.Sleep(3000);
                        gc.CreatePdf(orderNumber, accno, "Tax Detailed Result", driver, "FL", "Brevard");
                    }
                    catch { }

                    try
                    {
                        IWebElement ITaxReal = driver.FindElement(By.XPath("//*[@id='content']/div[1]/table"));
                        IList<IWebElement> ITaxRealRow = ITaxReal.FindElements(By.TagName("tr"));
                        IList<IWebElement> ITaxRealTd;
                        foreach (IWebElement ItaxReal in ITaxRealRow)
                        {
                            ITaxRealTd = ItaxReal.FindElements(By.TagName("td"));
                            if (ITaxRealTd.Count != 0 && ItaxReal.Text != "Bill")
                            {
                                if (ItaxReal.Text.Contains("Annual Bill"))
                                {
                                    AnualBillCount++;
                                }
                                if ((ItaxReal.Text.Contains("Annual Bill") || ItaxReal.Text.Contains("Pay this bill:")) && !ItaxReal.Text.Contains("Redeemed certificate") && AnualBillCount <= 3)
                                {
                                    IWebElement ITaxBillCount = ITaxRealTd[0].FindElement(By.TagName("a"));
                                    string strTaxReal = ITaxBillCount.GetAttribute("href");
                                    strTaxRealestate.Add(strTaxReal);
                                }
                            }
                        }
                        //Tax History Details
                        taxHistory(orderNumber, parcelid);
                    }
                    catch { }


                    //download 

                    //load chrome driver...
                    //download 

                    //load chrome driver...
                    //IWebDriver chDriver = new ChromeDriver();

                    //var chromeOptions = new ChromeOptions();
                    //var downloadDirectory = "D:\\AutoPdf\\";

                    //chromeOptions.AddUserProfilePreference("download.default_directory", downloadDirectory);
                    //chromeOptions.AddUserProfilePreference("download.prompt_for_download", false);
                    //chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");

                    //var chDriver = new ChromeDriver(chromeOptions);

                    //Array.ForEach(Directory.GetFiles(@downloadDirectory), File.Delete); //Delete all the files in auto pdf
                    //try
                    //{
                    //    foreach (string real in downloadbill)
                    //    {
                    //        chDriver.Navigate().GoToUrl(real);
                    //        Thread.Sleep(4000);
                    //        string dowloadedfile = "";
                    //        latestfile = gc.getfiles(dowloadedfile);
                    //        //  string fileName = "EDC_TaxBill_Copy_for_Secured_APN_" + parcelNumber + "_Year_" + yr + ".pdf";
                    //        gc.AutoDownloadFile(orderNumber, parcelNumber, "Brevard", "FL", latestfile);
                    //    }
                    //    chDriver.Quit();
                    //}

                    //catch (Exception ex)
                    //{
                    //    chDriver.Quit();
                    //    GlobalClass.LogError(ex, orderNumber);
                    //}


                    TaxTime = DateTime.Now.ToString("HH:mm:ss");

                    LastEndTime = DateTime.Now.ToString("HH:mm:ss");
                    gc.insert_TakenTime(orderNumber, "FL", "Brevard", StartTime, AssessmentTime, TaxTime, CitytaxTime, LastEndTime);


                    driver.Quit();
                    gc.mergpdf(orderNumber, "FL", "Brevard");
                    return "Data Inserted Successfully";
                }
                catch (Exception ex)
                {
                    driver.Quit();
                    throw ex;
                }
            }
        }
        public void taxHistory(string orderNumber, string parcelid)
        {

            IWebElement IBillHistorytable = driver.FindElement(By.XPath("//*[@id='content']/div[1]/table"));
            IList<IWebElement> IBillHistoryRow = IBillHistorytable.FindElements(By.TagName("tr"));
            IList<IWebElement> IBillHistoryTD;
            foreach (IWebElement bill in IBillHistoryRow)
            {
                IBillHistoryTD = bill.FindElements(By.TagName("td"));
                if (IBillHistoryTD.Count != 0 && IBillHistoryTD.Count != 1 && (!bill.Text.Contains("Certificate issued") && !bill.Text.Contains("Advertisement file created") && !(bill.Text.Contains("Certificate redeemed"))))
                {
                    try
                    {
                        strBill = IBillHistoryTD[0].Text.Replace("\r\n", " ").TrimStart().TrimEnd();
                        strBalance = IBillHistoryTD[1].Text.Replace("\r\n", " ").TrimStart().TrimEnd();
                        strBillDate = IBillHistoryTD[2].Text.Replace("\r\n", " ").TrimStart().TrimEnd();
                        if (strBillDate.Contains("Effective"))
                        {
                            strBillDate = GlobalClass.Before(IBillHistoryTD[2].Text, "Effective");
                            strBillEff = GlobalClass.After(IBillHistoryTD[2].Text, "Effective");
                        }
                        //strBillPaid = IBillHistoryTD[3].Text.Replace("\r\n", " ").TrimStart().TrimEnd();
                        string billpaid = IBillHistoryTD[3].Text.Replace("\r\n", " ").TrimStart().TrimEnd();
                        if (billpaid.Contains("Paid") || billpaid.Contains("Receipt"))
                        {
                            strBillPaid = gc.Between(billpaid, "Paid ", " Receipt");
                            strBillPayReceipt = GlobalClass.After(billpaid, "Receipt");
                        }

                    }
                    catch
                    {
                        strBillDate = "";
                        strBillPaid = "";
                    }
                    if (strBillPaid.Contains("Print (PDF)"))
                    {
                        strBillPaid = "";
                    }

                    if (strBillPaid.Contains("Deed applied"))
                    {
                        strBillPayReceipt = strBillPaid;
                        strBillPaid = "";
                    }
                    string strTaxHistory = strBill + "~" + "" + "~" + "" + "~" + strBalance + "~" + strBillDate + "~" + strBillEff + "~" + strBillPaid + "~" + strBillPayReceipt;
                    gc.insert_date(orderNumber, parcelid, 1514, strTaxHistory, 1, DateTime.Now);
                }
                if (IBillHistoryTD.Count != 0 && IBillHistoryTD.Count != 1 && ((bill.Text.Contains("Certificate issued") || bill.Text.Contains("Advertisement file created")) || (bill.Text.Contains("Certificate redeemed"))))
                {
                    try
                    {
                        strBill = IBillHistoryTD[0].Text.Replace("\r\n", " ").TrimStart().TrimEnd();
                        if (!strBill.Contains("Redeemed certificate") && !strBill.Contains("Issued certificate") && !strBill.Contains("Certificate issued") && !strBill.Contains("Advertisement file created") && !strBill.Contains("Certificate redeemed"))
                        {
                            strBill = "";
                        }
                        string strBillFaceRate = IBillHistoryTD[1].Text.Replace("\r\n", " ").TrimStart().TrimEnd();
                        if (strBillFaceRate.Contains("Face") || strBillFaceRate.Contains("Rate"))
                        {
                            strFace = gc.Between(IBillHistoryTD[1].Text, "Face ", "\r\nRate");
                            strRate = GlobalClass.After(IBillHistoryTD[1].Text, "Rate ");
                            strBillDate = IBillHistoryTD[2].Text.Replace("\r\n", " ").TrimStart().TrimEnd();
                            if (strBillDate.Contains("Effective"))
                            {
                                strBillDate = GlobalClass.Before(IBillHistoryTD[2].Text, "Effective");
                                strBillEff = GlobalClass.After(IBillHistoryTD[2].Text, "Effective");
                            }
                            strBillPayReceipt = IBillHistoryTD[3].Text.Replace("\r\n", " ").TrimStart().TrimEnd();
                        }
                        if (!strBillFaceRate.Contains("Face") || !strBillFaceRate.Contains("Rate") && IBillHistoryTD.Count == 2)
                        {
                            strBillDate = IBillHistoryTD[0].Text.Replace("\r\n", " ").TrimStart().TrimEnd();
                            if (strBillDate.Contains("Effective"))
                            {
                                strBillDate = GlobalClass.Before(IBillHistoryTD[2].Text, "Effective");
                                strBillEff = GlobalClass.After(IBillHistoryTD[2].Text, "Effective");
                            }
                            strBillPayReceipt = IBillHistoryTD[1].Text.Replace("\r\n", " ").TrimStart().TrimEnd();
                        }

                    }
                    catch
                    {
                        strBillDate = "";
                        strBillPayReceipt = "";
                    }
                    if (strBillPayReceipt.Contains("Print (PDF)"))
                    {
                        strBillPayReceipt = "";
                    }
                    string strTaxHistory = strBill + "~" + strFace + "~" + strRate + "~" + strBalance + "~" + strBillDate + "~" + strBillEff + "~" + strBillPaid + "~" + strBillPayReceipt;
                    gc.insert_date(orderNumber, parcelid, 1514, strTaxHistory, 1, DateTime.Now);
                }
                strBill = ""; strFace = ""; strRate = ""; strBalance = ""; strBillDate = ""; strBillEff = ""; strBillPaid = ""; strBillPayReceipt = "";
            }

            IWebElement IBillHistoryfoottable = driver.FindElement(By.XPath("//*[@id='content']/div[1]/table/tfoot"));
            IList<IWebElement> IBillHistoryfootRow = IBillHistoryfoottable.FindElements(By.TagName("tr"));
            IList<IWebElement> IBillHistoryfootTD;
            foreach (IWebElement bill in IBillHistoryRow)
            {
                IBillHistoryfootTD = bill.FindElements(By.TagName("th"));
                if (IBillHistoryfootTD.Count != 0 && bill.Text.Contains("Total"))
                {
                    try
                    {
                        strFBill = IBillHistoryfootTD[0].Text.Replace("\r\n", " ").TrimStart().TrimEnd();
                        strFBalance = IBillHistoryfootTD[1].Text.Replace("\r\n", " ").TrimStart().TrimEnd();
                        strFBillDate = IBillHistoryfootTD[2].Text.Replace("\r\n", " ").TrimStart().TrimEnd();
                        strFBillPaid = IBillHistoryfootTD[3].Text.Replace("\r\n", " ").TrimStart().TrimEnd();
                    }
                    catch
                    {
                        strFBillDate = "";
                        strFBillPaid = "";
                    }
                    if (strBillPaid.Contains("Print (PDF)"))
                    {
                        strBillPaid = "";
                    }
                    string strTaxHistory = strFBill + "~" + "" + "~" + "" + "~" + strFBalance + "~" + strFBillDate + "~" + "" + "~" + strFBillPaid + "~" + "";
                    gc.insert_date(orderNumber, parcelid, 1514, strTaxHistory, 1, DateTime.Now);
                }
            }

            foreach (string real in strTaxRealestate)
            {
                try
                {
                    driver.Navigate().GoToUrl(real);
                    Thread.Sleep(4000);
                    strPaid = "";
                    try
                    {
                        string strPaidDeatil = driver.FindElement(By.XPath("//*[@id='content']/div[1]/div[8]/div/div[6]/div/form/button")).Text;
                        strPaid = GlobalClass.Before(strPaidDeatil, ":");
                    }
                    catch { }
                    try
                    {
                        string strPaidDeatil = driver.FindElement(By.XPath("/html/body/div[1]/div/div[4]/div[1]/div[7]/div/div[3]/form/button")).Text;
                        strPaid = GlobalClass.Before(strPaidDeatil, ":");
                    }
                    catch { }
                    try
                    {
                        strType = driver.FindElement(By.XPath("//*[@id='content']/div[1]/div[8]/div/div[1]/div[2]")).Text;
                        strTaxType = GlobalClass.Before(strType, "\r\nPrint this bill (PDF)");
                    }
                    catch { }

                    try
                    {
                        strIssuePaid = driver.FindElement(By.XPath("//*[@id='content']/div[1]/div[8]/div/div[4]/div[1]/div[3]/div[1]/form/div")).Text;
                    }
                    catch { }
                    try
                    {
                        strIssue = driver.FindElement(By.XPath("//*[@id='content']/div[1]/div[8]/div/div[4]/div[1]/div[3]/div[1]/form/button")).Text;
                    }
                    catch { }
                    try
                    {
                        strEscrowCode = driver.FindElement(By.XPath("//*[@id='content']/div[1]/div[8]/div/table[1]/tbody/tr/td[3]")).Text;
                    }
                    catch { }
                    try
                    {
                        strTaxBill = driver.FindElement(By.XPath("//*[@id='content']/div[1]/div[8]/div/div[3]/form/div/div/div/span")).Text;
                    }
                    catch { }

                    try
                    {
                        strTaxFeed = driver.FindElement(By.XPath("//*[@id='content']/div[1]/div[7]/div/div[3]/div[1]/div/div")).Text;
                    }
                    catch { }
                    //Latest Bill
                    if (strPaid.Contains("Pay this bill") || strTaxBill.Contains("PAID") || strTaxFeed.Contains("Tax Deed") || strTaxBill.Contains("Cannot be paid online"))
                    {
                        try
                        {
                            //IWebElement ITaxSearch = driver.FindElement(By.LinkText("Latest bill"));
                            //string strITaxSearch = ITaxSearch.GetAttribute("href");
                            //driver.Navigate().GoToUrl(strITaxSearch);
                            gc.CreatePdf(orderNumber, accno, "Tax Detailed Result Latest Bill" + strTaxType, driver, "FL", "Brevard");
                            try
                            {//*[@id="content"]/div[1]/div[8]/div/div[1]/div[2]/form
                                IWebElement Iurl = driver.FindElement(By.XPath("//*[@id='content']/div[1]/div[8]/div/div[1]/div[2]/form"));
                                string strURL = Iurl.GetAttribute("action");
                                downloadbill.Add(strURL);
                                //  gc.downloadfile(strURL, orderNumber, parcelid, "Paid Billl Reciept" + strTaxType + "", "FL", "Brevard");
                            }
                            catch (Exception e)
                            {
                            }
                        }
                        catch { }
                        if (TaxCount < 1)
                        {
                            TaxCount++;
                            //Latest Bill Details
                            try
                            {
                                strVTaxType = driver.FindElement(By.XPath("//*[@id='content']/div[1]/div[8]/div/table[2]/caption")).Text;
                            }
                            catch { }//*[@id="content"]/div[1]/div[8]/div/table[2]/tbody
                            IWebElement ITax = driver.FindElement(By.XPath("//*[@id='content']/div[1]/div[8]/div/table[2]/tbody"));
                            IList<IWebElement> ITaxRow = ITax.FindElements(By.TagName("tr"));
                            IList<IWebElement> ITaxTD;
                            foreach (IWebElement row in ITaxRow)
                            {
                                ITaxTD = row.FindElements(By.TagName("td"));
                                if (ITaxTD.Count != 0 && ITaxTD.Count == 6)
                                {
                                    try
                                    {
                                        strLTaxAuthority = ITaxTD[0].Text;
                                        strLMillege = ITaxTD[1].Text;
                                        strLAssess = ITaxTD[2].Text;
                                        strLExemption = ITaxTD[3].Text;
                                        strLTaxable = ITaxTD[4].Text;
                                        strLTaxAmount = ITaxTD[5].Text;
                                    }
                                    catch
                                    { }

                                    string strTaxDetails = strVTaxType + "~" + strTaxType + "~" + strLTaxAuthority + "~" + strLMillege + "~" + strLAssess + "~" + strLExemption + "~" + strLTaxable + "~" + strLTaxAmount;
                                    gc.insert_date(orderNumber, parcelid, 1510, strTaxDetails, 1, DateTime.Now);

                                }
                                if (ITaxTD.Count != 0 && ITaxTD.Count < 6)
                                {
                                    try
                                    {
                                        strTaxAuthority = ITaxTD[0].Text;
                                        strMillege = ITaxTD[1].Text;
                                        strAssess = ITaxTD[2].Text;
                                        strExemption = ITaxTD[3].Text;
                                        strTaxable = ITaxTD[4].Text;
                                        strTaxAmount = ITaxTD[5].Text;
                                    }
                                    catch
                                    { }

                                    string strTaxDetails = strVTaxType + "~" + strTaxType + "~" + strTaxAuthority + "~" + strMillege + "~" + strAssess + "~" + strExemption + "~" + strTaxable + "~" + strTaxAmount;
                                    gc.insert_date(orderNumber, parcelid, 1510, strTaxDetails, 1, DateTime.Now);

                                    strTaxAuthority = ""; strMillege = ""; strAssess = ""; strExemption = ""; strTaxable = ""; strTaxAmount = "";
                                }
                            }

                            IWebElement ITaxBill = driver.FindElement(By.XPath(" //*[@id='content']/div[1]/div[8]/div/table[2]/tfoot"));
                            IList<IWebElement> ITaxBillRow = ITaxBill.FindElements(By.TagName("tr"));
                            IList<IWebElement> ITaxBillTD;
                            foreach (IWebElement bill in ITaxBillRow)
                            {
                                ITaxBillTD = bill.FindElements(By.TagName("td"));
                                if (ITaxBillTD.Count != 0 && bill.Text.Contains("Total"))
                                {
                                    try
                                    {
                                        strbillTaxAuthority = ITaxBillTD[0].Text;
                                        strbillTaxAmount = ITaxBillTD[1].Text;
                                        strbillAssess = ITaxBillTD[2].Text;
                                        strbillExemption = ITaxBillTD[3].Text;
                                        strbillTaxable = ITaxBillTD[4].Text;
                                        strbillMillege = ITaxBillTD[5].Text;
                                    }
                                    catch { }
                                    string strTaxBillDetails = strVTaxType + "~" + strTaxType + "~" + "Total" + "~" + strbillTaxAuthority + "~" + strbillMillege + "~" + strbillAssess + "~" + strbillExemption + "~" + strbillTaxAmount;
                                    gc.insert_date(orderNumber, parcelid, 1510, strTaxBillDetails, 1, DateTime.Now);
                                }
                            }

                            //*[@id="content"]/div[1]/div[8]/div/table[3]
                            try
                            {
                                strNVTaxType = driver.FindElement(By.XPath("//*[@id='content']/div[1]/div[8]/div/table[3]/caption")).Text;
                            }
                            catch { }
                            IWebElement IValoremTable = driver.FindElement(By.XPath("//*[@id='content']/div[1]/div[8]/div/table[3]/tbody"));
                            IList<IWebElement> IValoremRow = IValoremTable.FindElements(By.TagName("tr"));
                            IList<IWebElement> IValoremTD;

                            foreach (IWebElement Valorem in IValoremRow)
                            {
                                IValoremTD = Valorem.FindElements(By.TagName("td"));
                                if (IValoremTD.Count != 0 && Valorem.Text.Contains("$"))
                                {
                                    strValoremAuthority = IValoremTD[0].Text;
                                    strValoremRate = IValoremTD[1].Text;
                                    strValoremAmount = IValoremTD[2].Text;

                                    string strValoremDetails = strNVTaxType + "~" + strValoremAuthority + "~" + strValoremRate + "~" + strValoremAmount;
                                    gc.insert_date(orderNumber, parcelid, 1515, strValoremDetails, 1, DateTime.Now);
                                }
                            }

                            IWebElement IValTable = driver.FindElement(By.XPath("//*[@id='content']/div[1]/div[8]/div/table[3]/tfoot"));
                            IList<IWebElement> IvalRow = IValTable.FindElements(By.TagName("tr"));
                            IList<IWebElement> IvalTD;
                            foreach (IWebElement val in IvalRow)
                            {
                                IvalTD = val.FindElements(By.TagName("td"));
                                if (IvalTD.Count != 0 && val.Text.Contains("$"))
                                {
                                    try
                                    {
                                        strValAuthority = IvalTD[0].Text;
                                        strValRate = IvalTD[1].Text;
                                        strValAmount = IvalTD[2].Text;
                                    }
                                    catch { }

                                    string strValDetails = strNVTaxType + "~" + "Total" + "~" + strValRate + "~" + strValAuthority;
                                    gc.insert_date(orderNumber, parcelid, 1515, strValDetails, 1, DateTime.Now);
                                }

                                if (IvalTD.Count != 0 && val.Text.Contains("No non-ad valorem assessments."))
                                {
                                    strValoremAuthority = IvalTD[0].Text;

                                    string strValoremDetails = strNVTaxType + "~" + strValoremAuthority + "~" + "" + "~" + "";
                                    gc.insert_date(orderNumber, parcelid, 1515, strValoremDetails, 1, DateTime.Now);
                                }
                            }

                            //try
                            //{
                            //    //strcombine = driver.FindElement(By.XPath("//*[@id='content']/div[1]/div[8]/div/p")).Text;
                            //    //strcombine = GlobalClass.After(strcombine, "Combined taxes and assessments: ");
                            //    string strTaxDetails = strTaxType + "~" + "" + "~" + "";
                            //    gc.insert_date(orderNumber, parcelid, 1511, strTaxDetails, 1, DateTime.Now);
                            //}
                            //catch { }
                            //*[@id="content"]/div[1]/div[8]/div/table[4]/tbody
                            IWebElement ITaxCom = driver.FindElement(By.XPath("//*[@id='content']/div[1]/div[8]/div/table[4]/tbody"));
                            IList<IWebElement> ITaxComRow = ITaxCom.FindElements(By.TagName("tr"));
                            IList<IWebElement> ITaxComTD;
                            foreach (IWebElement taxrow in ITaxComRow)
                            {
                                ITaxComTD = taxrow.FindElements(By.TagName("td"));
                                if (ITaxComTD.Count != 0 && taxrow.Text.Contains(""))
                                {
                                    for (int j = 0; j < ITaxComTD.Count; j++)
                                    {
                                        strTaxCom = ITaxComTD[j].Text;
                                        string current = DateTime.Now.Year.ToString();
                                        string strCurrentYear = current.Substring(0, 2);
                                        if ((!strTaxCom.Contains("Face Amt") && !strTaxCom.Contains("Bid ") && !strTaxCom.Contains("Bidder") && !strTaxCom.Contains("Certificate")) && (strTaxCom.Contains(strCurrentYear) || (strTaxCom.Contains("$"))))
                                        {
                                            strDate = GlobalClass.Before(strTaxCom, "\r\n");
                                            strAmount = GlobalClass.After(strTaxCom, "\r\n");

                                            string strTaxDetails = strTaxType + "~" + strDate + "~" + strAmount;
                                            gc.insert_date(orderNumber, parcelid, 1511, strTaxDetails, 1, DateTime.Now);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    string certificate = "", strCertificate = "", stradvertisedNo = "", strFaceAmount = "", strIssuedDate = "", strExpirationDate = "", strBuyer = "", strInterestRate = "", IssuedYear = "", strIssuedYear = "";
                    strcombine = driver.FindElement(By.XPath("//*[@id='content']/div[1]/div[8]/div/p")).Text;
                    strcombine = GlobalClass.After(strcombine, "Combined taxes and assessments: ");
                    try
                    {
                        if (strIssue.Contains("Pay this bill:") || strTaxBill.Contains("Cannot be paid online") || strTaxBill.Contains("PAID"))
                        {
                            try
                            {
                                IWebElement ITaxSearch = driver.FindElement(By.LinkText("Parcel details"));
                                string strITaxSearch = ITaxSearch.GetAttribute("href");
                                driver.Navigate().GoToUrl(strITaxSearch);

                                strissueParcelNo = driver.FindElement(By.XPath("//*[@id='content']/div[1]/div[3]/div[2]/dl[2]/dd[1]")).Text;
                                strissueAlternetKey = driver.FindElement(By.XPath("//*[@id='content']/div[1]/div[3]/div[2]/dl[2]/dd[2]")).Text;
                                strissueMillageCode = driver.FindElement(By.XPath("//*[@id='content']/div[1]/div[3]/div[2]/dl[2]/dd[3]")).Text;
                                strissueMillageRate = driver.FindElement(By.XPath("//*[@id='content']/div[1]/div[3]/div[2]/dl[2]/dd[4]")).Text;
                                certificate = driver.FindElement(By.XPath("//*[@id='certificate']")).Text;
                                strCertificate = GlobalClass.After(certificate, "Certificate ");
                                stradvertisedNo = driver.FindElement(By.XPath("//*[@id='content']/div[1]/dl/dd[1]")).Text;
                                strFaceAmount = driver.FindElement(By.XPath("//*[@id='content']/div[1]/dl/dd[2]")).Text;
                                strIssuedDate = driver.FindElement(By.XPath("//*[@id='content']/div[1]/dl/dd[3]")).Text;
                                strExpirationDate = driver.FindElement(By.XPath("//*[@id='content']/div[1]/dl/dd[4]")).Text;
                                strBuyer = driver.FindElement(By.XPath("//*[@id='content']/div[1]/dl/dd[5]")).Text;
                                strInterestRate = driver.FindElement(By.XPath("//*[@id='content']/div[1]/dl/dd[6]")).Text;
                                IssuedYear = driver.FindElement(By.XPath("//*[@id='content']/div[1]/p")).Text;
                                strIssuedYear = GlobalClass.After(IssuedYear, "This parcel has an issued certificate for ").Replace(".", "");
                                if (IssuedYear.Contains("This parcel has an issued certificate") && !IssuedYear.Contains("This parcel has a redeemed certificate"))
                                {
                                    gc.CreatePdf(orderNumber, accno, "Tax Detailed Result Parcel Details" + strTaxType + "", driver, "FL", "Brevard");

                                    // Parcel Details 
                                    string strTaxIssue = strTaxType + "~" + strissueAlternetKey + "~" + strEscrowCode + "~" + strissueMillageCode + "~" + strissueMillageRate + "~" + strcombine + "~" + strCertificate + "~" + stradvertisedNo + "~" + strFaceAmount + "~" + strIssuedDate + "~" + strExpirationDate + "~" + strBuyer + "~" + strIssuedYear;
                                    gc.insert_date(orderNumber, strissueParcelNo, 1509, strTaxIssue, 1, DateTime.Now);
                                }
                                if (IssuedYear.Contains("This parcel has a redeemed certificate") && !IssuedYear.Contains("This parcel has an issued certificate"))
                                {
                                    gc.CreatePdf(orderNumber, accno, "Tax Detailed Result Parcel Details" + strTaxType + "", driver, "FL", "Brevard");

                                    // Parcel Details 
                                    string strTaxIssue = strTaxType + "~" + strissueAlternetKey + "~" + strEscrowCode + "~" + strissueMillageCode + "~" + strissueMillageRate + "~" + strcombine + "~" + "" + "~" + "" + "~" + "" + "~" + "" + "~" + "" + "~" + "" + "~" + "";
                                    gc.insert_date(orderNumber, strissueParcelNo, 1509, strTaxIssue, 1, DateTime.Now);
                                }
                            }
                            catch { }
                        }
                        if (certificate == "" && strIssuedYear == "")
                        {
                            gc.CreatePdf(orderNumber, accno, "Tax Detailed Result Parcel Details" + strTaxType + "", driver, "FL", "Brevard");

                            string strTaxIssue = strTaxType + "~" + strissueAlternetKey + "~" + strEscrowCode + "~" + strissueMillageCode + "~" + strissueMillageRate + "~" + strcombine + "~" + "" + "~" + "" + "~" + "" + "~" + "" + "~" + "" + "~" + "" + "~" + "";
                            gc.insert_date(orderNumber, strissueParcelNo, 1509, strTaxIssue, 1, DateTime.Now);
                        }
                    }
                    catch { }
                    try

                    {
                        IWebElement IparcelURL = driver.FindElement(By.XPath("//*[@id='content']/div[1]/div[3]/div[1]/ul/li[1]/a"));
                        string parcelhref = IparcelURL.GetAttribute("href");
                        string strParcelURL = IparcelURL.Text;
                        driver.Navigate().GoToUrl(parcelhref);
                        gc.CreatePdf(orderNumber, accno, "Tax Parcel Details" + strTaxType, driver, "FL", "Brevard");
                    }
                    catch { }
                }
                catch { }
            }
        }

    }
}