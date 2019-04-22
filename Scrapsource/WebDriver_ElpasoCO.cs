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
    public class WebDriver_ElpasoCO
    {

        IWebDriver driver;
        DBconnection db = new DBconnection();
        GlobalClass gc = new GlobalClass();

        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["MyConnectionString"].ToString());

        public string FTP_ElpasoCO(string address, string parcelNumber, string searchType, string ownername, string orderNumber, string directParcel)
        {
            GlobalClass.global_orderNo = orderNumber;
            HttpContext.Current.Session["orderNo"] = orderNumber;
            GlobalClass.global_parcelNo = parcelNumber;

            string StartTime = "", AssessmentTime = "", TaxTime = "", CitytaxTime = "", LastEndTime = "";

            var driverService = PhantomJSDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            using (driver = new PhantomJSDriver())
            {
                //  driver = new ChromeDriver();
                // driver = new PhantomJSDriver();
                try
                {
                    StartTime = DateTime.Now.ToString("HH:mm:ss");
                    driver.Navigate().GoToUrl("https://property.spatialest.com/co/elpaso/");
                    Thread.Sleep(4000);

                    if (searchType == "titleflex")
                    {
                        string titleaddress = address;
                        gc.TitleFlexSearch(orderNumber, "", address, "", "CO", "El Paso");

                        if (HttpContext.Current.Session["TitleFlex_Search"] != null && HttpContext.Current.Session["TitleFlex_Search"].ToString() == "Yes")
                        {
                            return "MultiParcel";
                        }
                        parcelNumber = HttpContext.Current.Session["titleparcel"].ToString();
                        searchType = "parcel";
                    }

                    if (searchType == "address")
                    {
                        try
                        {
                            driver.FindElement(By.Id("primary_search")).SendKeys(address);
                            gc.CreatePdf_WOP(orderNumber, "Address search", driver, "CO", "El Paso");
                            driver.FindElement(By.Id("primary_search")).Click();
                            driver.FindElement(By.Id("primary_search")).SendKeys(Keys.Enter);
                            Thread.Sleep(15000);
                            // driver.FindElement(By.XPath("//*[@id='main-app']/div/div[1]/div[1]/div/div/div/div/div/div[1]/div/button/span[1]")).SendKeys(Keys.Enter);
                            // Thread.Sleep(4000);
                            gc.CreatePdf_WOP(orderNumber, "Address Search result", driver, "CO", "El Paso");
                        }
                        catch { }
                        try
                        {
                            int Max = 0;
                            string strowner = "", strAddress = "", strCity = "";
                            string returnStatement = "", Record = "", MulAddress = "", Parcelnum = "", strOwner = "", strAdd = "";
                            //IWebElement Irecord = driver.FindElement(By.XPath("//*[@id='main-app']/div/div[2]/div/div[1]/div/div[1]/div/div[2]/span"));
                            //Record = Irecord.Text;
                            IWebElement multiaddress = driver.FindElement(By.XPath("//*[@id='main-app']/div/div[1]/div[2]/div/div/div/div[2]/div"));
                            IList<IWebElement> multiRow = multiaddress.FindElements(By.TagName("a"));
                            IList<IWebElement> multiTD = multiaddress.FindElements(By.TagName("div"));
                            foreach (IWebElement multi in multiRow)
                            {
                                multiTD = multi.FindElements(By.TagName("div"));
                                if (multiTD.Count != 0 && multiTD.Count >= 3)
                                {
                                    MulAddress = multiTD[0].Text.Replace("Market Value", "").Replace("\r\n", "~").Trim();
                                    string[] split = MulAddress.Split('~');
                                    string[] parcel = multiTD[6].Text.Replace("\r\n", "~").Trim().Split('~');
                                    Parcelnum = parcel[0];
                                    //strOwner = split[1];
                                    try
                                    {
                                        strAdd = split[0];
                                    }
                                    catch { }
                                    string multidetails =strAdd;
                                    gc.insert_date(orderNumber, Parcelnum, 1597, multidetails, 1, DateTime.Now);
                                    Max++;

                                }
                                if (multiTD.Count != 0 && multiRow.Count > 25)
                                {
                                    HttpContext.Current.Session["multiparcel_Elpaso_Maximum"] = "Maximum";
                                    driver.Quit();
                                    return "Maximum";
                                }

                            }
                            if (Max > 1 && Max < 26)
                            {
                                HttpContext.Current.Session["multiparcel_Elpaso"] = "Yes";
                                driver.Quit();
                                return "MultiParcel";
                            }
                            if (Max == 0)
                            {
                                HttpContext.Current.Session["Zero_Elpaso"] = "Zero";
                                driver.Quit();
                                return "No Data Found";
                            }
                        }
                        catch { }
                    }


                    else if (searchType == "ownername")
                    {
                        try
                        {
                            driver.FindElement(By.Id("primary_search")).SendKeys(ownername);
                            gc.CreatePdf_WOP(orderNumber, "OwnerName search", driver, "CO", "El Paso");
                            driver.FindElement(By.Id("primary_search")).Click();
                            driver.FindElement(By.Id("primary_search")).SendKeys(Keys.Enter);
                            Thread.Sleep(7000);
                            gc.CreatePdf_WOP(orderNumber, "OwnerName Search result", driver, "CO", "El Paso");
                        }
                        catch { }
                        try
                        {
                            int Max = 0;
                            string strowner = "", strAddress = "", strCity = "";
                            string returnStatement = "", Record = "", MulAddress = "", Parcelnum = "", strOwner = "", strAdd = "";
                            IWebElement Irecord = driver.FindElement(By.XPath("//*[@id='main-app']/div/div[1]/div[2]/div/div/div/div[1]/div/div[2]/span"));
                            Record = Irecord.Text;
                            IWebElement multiaddress = driver.FindElement(By.XPath("//*[@id='main-app']/div/div[2]/div/div[1]/div/div[2]"));
                            IList<IWebElement> multiRow = multiaddress.FindElements(By.TagName("a"));
                            IList<IWebElement> multiTD = multiaddress.FindElements(By.TagName("div"));
                            foreach (IWebElement multi in multiRow)
                            {
                                multiTD = multi.FindElements(By.TagName("div"));
                                if (multiTD.Count != 0 && multiTD.Count >= 3)
                                {
                                    MulAddress = multiTD[0].Text.Replace("Market Value", "").Replace("\r\n", "~").Trim();
                                    string[] split = MulAddress.Split('~');
                                    Parcelnum = split[0];
                                    strOwner = split[1];
                                    try
                                    {
                                        strAdd = split[2];
                                    }
                                    catch { }
                                    string multidetails = strOwner + " " + strAdd;
                                    gc.insert_date(orderNumber, Parcelnum, 1597, multidetails, 1, DateTime.Now);
                                    Max++;

                                }
                                if (multiTD.Count != 0 && multiRow.Count > 25)
                                {
                                    HttpContext.Current.Session["multiparcel_Elpaso_Maximum"] = "Maximum";
                                    driver.Quit();
                                    return "Maximum";
                                }

                            }
                            if (Max > 1 && Max < 26)
                            {
                                HttpContext.Current.Session["multiparcel_Elpaso"] = "Yes";
                                driver.Quit();
                                return "MultiParcel";
                            }
                            if (Max == 0)
                            {
                                HttpContext.Current.Session["Zero_Elpaso"] = "Zero";
                                driver.Quit();
                                return "No Data Found";
                            }
                        }
                        catch { }
                    }

                    else if (searchType == "parcel")
                    {
                        try
                        {
                            driver.FindElement(By.Id("primary_search")).SendKeys(parcelNumber);
                            gc.CreatePdf(orderNumber, parcelNumber, "parcel search", driver, "CO", "El Paso");
                            driver.FindElement(By.Id("primary_search")).Click();
                            driver.FindElement(By.Id("primary_search")).SendKeys(Keys.Enter);
                            Thread.Sleep(10000);
                            gc.CreatePdf(orderNumber, parcelNumber, "parcel search Result", driver, "CO", "El Paso");
                        }
                        catch { }


                        try
                        {
                            int Max = 0;
                            string strowner = "", strAddress = "", strCity = "";
                            string returnStatement = "", Record = "", MulAddress = "", Parcelnum = "", strOwner = "", strAdd = "";
                            IWebElement Irecord = driver.FindElement(By.XPath("//*[@id='main-app']/div/div[2]/div/div[1]/div/div[1]/div/div[2]/span"));
                            Record = Irecord.Text;
                            IWebElement multiaddress = driver.FindElement(By.XPath("//*[@id='main-app']/div/div[2]/div/div[1]/div/div[2]"));
                            IList<IWebElement> multiRow = multiaddress.FindElements(By.TagName("a"));
                            IList<IWebElement> multiTD = multiaddress.FindElements(By.TagName("div"));
                            foreach (IWebElement multi in multiRow)
                            {
                                multiTD = multi.FindElements(By.TagName("div"));
                                if (multiTD.Count != 0 && multiTD.Count >= 3)
                                {
                                    MulAddress = multiTD[0].Text.Replace("Market Value", "").Replace("\r\n", "~").Trim();
                                    string[] split = MulAddress.Split('~');
                                    Parcelnum = split[0];
                                    strOwner = split[1];
                                    try
                                    {
                                        strAdd = split[2];
                                    }
                                    catch { }
                                    string multidetails = strOwner + " " + strAdd;
                                    gc.insert_date(orderNumber, Parcelnum, 1597, multidetails, 1, DateTime.Now);
                                    Max++;

                                }
                                if (multiTD.Count != 0 && multiRow.Count > 25)
                                {
                                    HttpContext.Current.Session["multiparcel_Elpaso_Maximum"] = "Maximum";
                                    driver.Quit();
                                    return "Maximum";
                                }

                            }
                            if (Max > 1 && Max < 26)
                            {
                                HttpContext.Current.Session["multiparcel_Elpaso"] = "Yes";
                                driver.Quit();
                                return "MultiParcel";
                            }
                            if (Max == 0)
                            {
                                HttpContext.Current.Session["Zero_Elpaso"] = "Zero";
                                driver.Quit();
                                return "No Data Found";
                            }
                        }
                        catch { }
                    }

                    //property details

                    // gc.CreatePdf(orderNumber, parcelNumber, "Property Details1", driver, "CO", "El Paso");
                    string OwnerName = "", Propertylocation = "", legaldesc = "", landuse = "";
                    string MailingAddress = "", TaxStatus = "", Zoning = "", PlatNo = "";
                    string PropertyAddress = "", YearBuilt = "", OwnerName1 = "", OwnerName2 = "";
                    try
                    {
                                                                             //*[@id="prccontent"]/div/section/div/div[1]/div[2]/header/div/div/div[1]/div[1]/span
                        IWebElement parcelNo = driver.FindElement(By.XPath("//*[@id='prccontent']/div/section/div/div[1]/div[2]/header/div/div/div[1]/div[1]/span"));
                        parcelNumber = parcelNo.Text.Trim();
                    }
                    catch { }
                    string bulkdata1 = driver.FindElement(By.XPath("//*[@id='keyinformation']/div[1]/div[2]/div")).Text;
                    try
                    {
                        OwnerName1 = driver.FindElement(By.XPath("//*[@id='keyinformation']/div[1]/div/div/ul/li[1]/p/select/option[1]")).Text;
                        OwnerName2 = driver.FindElement(By.XPath("//*[@id='keyinformation']/div[1]/div/div/ul/li[1]/p/select/option[2]")).Text;
                        OwnerName = OwnerName1 + " " + OwnerName2;
                    }
                    catch { }
                    try
                    {
                        OwnerName = driver.FindElement(By.XPath("//*[@id='keyinformation']/div[1]/div/div/ul/li[1]/p/span[2]")).Text;
                    }
                    catch { }
                    MailingAddress = gc.Between(bulkdata1, "Mailing Address", "Location").Replace(":", "").Trim();
                    Propertylocation = gc.Between(bulkdata1, "Location", "Tax Status").Replace(":", "").Trim();
                    TaxStatus = gc.Between(bulkdata1, "Tax Status", "Zoning").Replace(":", "").Trim();
                    Zoning = gc.Between(bulkdata1, "Zoning", "Plat No").Replace(":", "").Trim();
                    PlatNo = gc.Between(bulkdata1, "Plat No", "Legal Description").Replace(":", "").Trim();
                    try
                    {
                        legaldesc = GlobalClass.After(bulkdata1, "Legal Description:").Replace("\r\n", "").Trim();
                    }
                    catch { }
                    try
                    {
                        landuse = driver.FindElement(By.XPath("//*[@id='land']/div/div/div/table/tbody/tr/td[2]")).Text;
                    }
                    catch { }
                    try
                    {
                        YearBuilt = driver.FindElement(By.XPath("//*[@id='BuildingSection_residential_0']/div/div/div/div/ul/li[5]/p[1]/span[2]")).Text;
                    }
                    catch { }





                    string propertydetails = OwnerName + "~" + MailingAddress + "~" + Propertylocation + "~" + TaxStatus + "~" + Zoning + "~" + PlatNo + "~" + legaldesc + "~" + landuse + "~" + YearBuilt;
                    gc.insert_date(orderNumber, parcelNumber, 1593, propertydetails, 1, DateTime.Now);


                    // Assessment Details
                    try
                    {
                        IWebElement Assessment = driver.FindElement(By.XPath("//*[@id='keyinformation']/div[2]/div/div"));
                        IList<IWebElement> TRAssessment = Assessment.FindElements(By.TagName("li"));
                        IList<IWebElement> THAssessment = Assessment.FindElements(By.TagName("p"));
                        IList<IWebElement> TDAssessment;
                        foreach (IWebElement row1 in TRAssessment)
                        {
                            TDAssessment = row1.FindElements(By.TagName("p"));
                             if (!row1.Text.Contains("Market Value") && !row1.Text.Contains("Assessed Value") && TDAssessment.Count != 0 && row1.Text.Trim() != "" && TDAssessment.Count >= 3)
                            {

                                string AssessDetails = TDAssessment[0].Text + "~" + TDAssessment[1].Text + "~" + TDAssessment[2].Text;
                                gc.insert_date(orderNumber, parcelNumber, 1594, AssessDetails, 1, DateTime.Now);
                            }

                        }
                    }
                    catch { }

                    // Taxing Authority

                    driver.Navigate().GoToUrl("https://treasurer.elpasoco.com/");
                    Thread.Sleep(5000);
                    string taxAuth = "", ta1 = "", ta2 = "", ta3 = "";
                    try
                    {
                        ta1 = driver.FindElement(By.XPath("//*[@id='content']/div[4]/div[3]/div/div/div/div/p[2]/span[2]")).Text.Trim();
                        ta2 = driver.FindElement(By.XPath("//*[@id='content']/div[4]/div[3]/div/div/div/div/p[2]/span[3]")).Text;
                        ta3 = driver.FindElement(By.XPath("//*[@id='content']/div[4]/div[3]/div/div/div/div/p[2]/span[4]")).Text;
                        taxAuth = ta1 + " " + ta2 + " " + ta3;
                    }
                    catch { }
                    // Current Tax Details


                    string Tax_Year = "", ParcelNo = "", Alert1 = "", Alert2 = "";
                    int Syear = DateTime.Now.Year;
                    int Smonth = DateTime.Now.Month;
                    int iyear = 0;
                    int i = 0, j = 0, k = 0;
                    if (Smonth >= 9)
                    {
                        iyear = Syear;
                    }
                    else
                    {
                        iyear = Syear - 1;
                    }

                    driver.Navigate().GoToUrl("http://epmt.trs.elpasoco.com/epui/Search.aspx");
                    Thread.Sleep(5000);

                    driver.FindElement(By.Id("ContentPlaceHolder1_txtParcel")).SendKeys(parcelNumber);
                    gc.CreatePdf(orderNumber, parcelNumber, "Tax Search", driver, "CO", "El Paso");
                    driver.FindElement(By.Id("ContentPlaceHolder1_btnSubmit")).Click();
                    Thread.Sleep(5000);
                    gc.CreatePdf(orderNumber, parcelNumber, "Tax Search Result", driver, "CO", "El Paso");

                    string Owner_Name = "", Pro_Address = "", Pro_Type = "", Legal_Desc = "", Alerts = "";
                    Owner_Name = driver.FindElement(By.Id("ContentPlaceHolder1_txtName")).Text.Replace("\r\n", " ");
                    Pro_Address = driver.FindElement(By.Id("ContentPlaceHolder1_txtPropertyAddress")).Text.Trim();
                    Pro_Type = driver.FindElement(By.Id("ContentPlaceHolder1_lblPropertyType")).Text;
                    Legal_Desc = driver.FindElement(By.Id("ContentPlaceHolder1_lblLegalDescription")).Text;
                    try
                    {
                        Alerts = driver.FindElement(By.Id("ContentPlaceHolder1_txtNA")).Text;
                    }
                    catch { }


                    try
                    {
                        Alerts = driver.FindElement(By.Id("ContentPlaceHolder1_txtContact")).Text;
                    }
                    catch { }



                    string Taxdetails = Owner_Name + "~" + Pro_Address + "~" + Pro_Type + "~" + Legal_Desc + "~" + Alerts + "~" + taxAuth;
                    gc.insert_date(orderNumber, parcelNumber, 1595, Taxdetails, 1, DateTime.Now);

                    // current Property Valuation Table

                    string Total_Assessed_Land = "", Total_Assessed_Improvements = "", Total_Assessed = "", Total_Market_Value = "", Base_Tax_Amount = "";
                    string Special_Assessment_Amount = "", Improvement_District_Amount = "", Total_Current_Year_Taxes = "";

                    Total_Assessed_Land = driver.FindElement(By.Id("ContentPlaceHolder1_lblTotalAssessedLand")).Text;
                    Total_Assessed_Improvements = driver.FindElement(By.Id("ContentPlaceHolder1_lblTotalAssessedImprovements")).Text;
                    Total_Assessed = driver.FindElement(By.Id("ContentPlaceHolder1_lblTotalAssessed")).Text;
                    Total_Market_Value = driver.FindElement(By.Id("ContentPlaceHolder1_lblTotalMarketValue")).Text;
                    Base_Tax_Amount = driver.FindElement(By.Id("ContentPlaceHolder1_lblBaseTaxAmount")).Text;
                    Special_Assessment_Amount = driver.FindElement(By.Id("ContentPlaceHolder1_lblSpecialAssessmentAmount")).Text;
                    Improvement_District_Amount = driver.FindElement(By.Id("ContentPlaceHolder1_lblImprovementDistrictAmount")).Text;
                    Total_Current_Year_Taxes = driver.FindElement(By.Id("ContentPlaceHolder1_lblTotalAmount")).Text;

                    string PropertyValuation = Total_Assessed_Land + "~" + Total_Assessed_Improvements + "~" + Total_Assessed + "~" + Total_Market_Value + "~" + Base_Tax_Amount + "~" + Special_Assessment_Amount + "~" + Improvement_District_Amount + "~" + Total_Current_Year_Taxes;
                    gc.insert_date(orderNumber, parcelNumber, 1596, PropertyValuation, 1, DateTime.Now);

                    // Current Year Payment Due Details Table Option 1:
                    string currenttaxliability1 = "";
                    currenttaxliability1 = driver.FindElement(By.Id("ContentPlaceHolder1_lblTotalAmountDueOption1")).Text.Replace("Current Tax Liability:", "").Trim();
                    try
                    {
                        IWebElement PayDue = driver.FindElement(By.XPath("//*[@id='ContentPlaceHolder1_gvCurrentYearPaymentsDueOption1']/tbody"));
                        IList<IWebElement> TRPayDue = PayDue.FindElements(By.TagName("tr"));
                        IList<IWebElement> THPayDue = PayDue.FindElements(By.TagName("th"));
                        IList<IWebElement> TDPayDue;
                        foreach (IWebElement row1 in TRPayDue)
                        {
                            TDPayDue = row1.FindElements(By.TagName("td"));
                            if (!row1.Text.Contains("Payment Type") && row1.Text.Contains("First Half") && TDPayDue.Count != 0 && row1.Text.Trim() != "" && TDPayDue.Count >= 5)
                            {

                                string PayDueDetails = TDPayDue[0].Text + "~" + TDPayDue[1].Text + "~" + TDPayDue[2].Text + "~" + TDPayDue[3].Text + "~" + TDPayDue[4].Text + "~" + "";
                                gc.insert_date(orderNumber, parcelNumber, 1598, PayDueDetails, 1, DateTime.Now);
                            }
                            if (!row1.Text.Contains("Payment Type") && row1.Text.Contains("Second Half") && TDPayDue.Count != 0 && row1.Text.Trim() != "" && TDPayDue.Count >= 5)
                            {

                                string PayDueDetails = TDPayDue[0].Text + "~" + TDPayDue[1].Text + "~" + TDPayDue[2].Text + "~" + TDPayDue[3].Text + "~" + TDPayDue[4].Text + "~" + currenttaxliability1;
                                gc.insert_date(orderNumber, parcelNumber, 1598, PayDueDetails, 1, DateTime.Now);
                            }
                        }
                    }
                    catch { }

                    // Current Year Payment Due Details Table Option 2:

                    string currenttaxliability2 = "";
                    currenttaxliability2 = driver.FindElement(By.Id("ContentPlaceHolder1_lblTotalAmountDueOption2")).Text.Replace("Current Tax Liability:", "").Trim();
                    try
                    {
                        IWebElement PayDue1 = driver.FindElement(By.XPath("//*[@id='ContentPlaceHolder1_gvCurrentYearPaymentsDueOption2']/tbody"));
                        IList<IWebElement> TRPayDue1 = PayDue1.FindElements(By.TagName("tr"));
                        IList<IWebElement> THPayDue1 = PayDue1.FindElements(By.TagName("th"));
                        IList<IWebElement> TDPayDue1;
                        foreach (IWebElement row2 in TRPayDue1)
                        {
                            TDPayDue1 = row2.FindElements(By.TagName("td"));
                            if (!row2.Text.Contains("Payment Type") && TDPayDue1.Count != 0 && row2.Text.Trim() != "" && TDPayDue1.Count >= 5)
                            {

                                string PayDueDetails1 = TDPayDue1[0].Text + "~" + TDPayDue1[1].Text + "~" + TDPayDue1[2].Text + "~" + TDPayDue1[3].Text + "~" + TDPayDue1[4].Text + "~" + currenttaxliability2;
                                gc.insert_date(orderNumber, parcelNumber, 1598, PayDueDetails1, 1, DateTime.Now);
                            }

                        }
                    }
                    catch { }

                    // Current Year Payments Received Details Table:
                    try
                    {
                        IWebElement PayReceived = driver.FindElement(By.XPath("//*[@id='ContentPlaceHolder1_gvCurrentYearPaymentsReceived']/tbody"));
                        IList<IWebElement> TRPayReceived = PayReceived.FindElements(By.TagName("tr"));
                        IList<IWebElement> THPayReceived = PayReceived.FindElements(By.TagName("th"));
                        IList<IWebElement> TDPayReceived;
                        foreach (IWebElement row2 in TRPayReceived)
                        {
                            TRPayReceived = row2.FindElements(By.TagName("td"));
                            if (!row2.Text.Contains("Amount") && TRPayReceived.Count != 0 && row2.Text.Trim() != "")
                            {

                                string PayReceivedDetails = TRPayReceived[0].Text + "~" + TRPayReceived[1].Text;
                                gc.insert_date(orderNumber, parcelNumber, 1599, PayReceivedDetails, 1, DateTime.Now);
                            }

                        }
                    }
                    catch { }

                    //Prior Year(s) Transaction History
                    try
                    {
                        IWebElement TransactionHistory = driver.FindElement(By.XPath("//*[@id='ContentPlaceHolder1_gvHistoryPaymentsReceived']/tbody"));
                        IList<IWebElement> TRTransactionHistory = TransactionHistory.FindElements(By.TagName("tr"));
                        IList<IWebElement> THTransactionHistory = TransactionHistory.FindElements(By.TagName("th"));
                        IList<IWebElement> TDTransactionHistory;
                        foreach (IWebElement row2 in TRTransactionHistory)
                        {
                            TDTransactionHistory = row2.FindElements(By.TagName("td"));
                            if (!row2.Text.Contains("Amount") && TDTransactionHistory.Count != 0 && row2.Text.Trim() != "")
                            {

                                string TransactionHistoryDetails = TDTransactionHistory[0].Text + "~" + TDTransactionHistory[1].Text;
                                gc.insert_date(orderNumber, parcelNumber, 1600, TransactionHistoryDetails, 1, DateTime.Now);
                            }

                        }
                    }
                    catch { }

                    try
                    {

                        var chromeOptions = new ChromeOptions();
                        var downloadDirectory = "F:\\AutoPdf\\";
                        chromeOptions.AddUserProfilePreference("download.default_directory", downloadDirectory);
                        chromeOptions.AddUserProfilePreference("download.prompt_for_download", false);
                        chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");
                        chromeOptions.AddUserProfilePreference("plugins.always_open_pdf_externally", true);
                        var driver1 = new ChromeDriver(chromeOptions);
                        driver1.Navigate().GoToUrl("http://epmt.trs.elpasoco.com/epui/Search.aspx");
                        Thread.Sleep(5000);

                        driver1.FindElement(By.Id("ContentPlaceHolder1_txtParcel")).SendKeys(parcelNumber);

                        driver1.FindElement(By.Id("ContentPlaceHolder1_btnSubmit")).Click();
                        Thread.Sleep(3000);

                        string fileName = "";
                        driver1.FindElement(By.Id("ContentPlaceHolder1_btnGetTaxStatementDoc")).Click();
                        IWebElement Receipttable = driver1.FindElement(By.Id("ContentPlaceHolder1_btnGetTaxStatementDoc"));

                        Receipttable.Click();
                        Thread.Sleep(3000);
                        fileName = "TaxStatement_" + parcelNumber + ".pdf";
                        gc.AutoDownloadFile(orderNumber, parcelNumber, "El Paso", "CO", fileName);
                    }
                    catch { }

                    TaxTime = DateTime.Now.ToString("HH:mm:ss");

                    LastEndTime = DateTime.Now.ToString("HH:mm:ss");
                    gc.insert_TakenTime(orderNumber, "CO", "El Paso", StartTime, AssessmentTime, TaxTime, CitytaxTime, LastEndTime);

                    driver.Quit();
                    gc.mergpdf(orderNumber, "CO", "El Paso");
                    return "Data Inserted Successfully";
                }

                catch (Exception ex)
                {
                    driver.Quit();
                    throw ex;
                }
            }
        }
    }
}