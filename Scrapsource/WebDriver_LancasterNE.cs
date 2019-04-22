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
    public class WebDriver_LancasterNE
    {

        IWebDriver driver;
        DBconnection db = new DBconnection();
        GlobalClass gc = new GlobalClass();

        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["MyConnectionString"].ToString());

        public string FTP_LancasterNE(string houseno, string Direction, string sname, string stype, string account, string parcelNumber, string ownername, string searchType, string orderNumber, string directParcel)
        {
            GlobalClass.global_orderNo = orderNumber;
            HttpContext.Current.Session["orderNo"] = orderNumber;
            GlobalClass.global_parcelNo = parcelNumber;

            string StartTime = "", AssessmentTime = "", TaxTime = "", CitytaxTime = "", LastEndTime = "";
            string address = "", lastName = "", firstName = "", Pinnumber = "", PropertyAdd = "", Strownername = "", Pin = "";

            var driverService = PhantomJSDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            using (driver = new PhantomJSDriver())
            {
              //  driver = new ChromeDriver();

                try
                {
                    StartTime = DateTime.Now.ToString("HH:mm:ss");


                    if (searchType == "titleflex")
                    {
                        if (Direction != "")
                        {
                            address = houseno + " " + Direction + " " + sname + " " + stype + " " + account;
                            address = address.Trim();
                        }
                        if (Direction == "")
                        {
                            address = houseno + " " + sname + " " + stype + " " + account;
                            address = address.Trim();
                        }
                        string titleaddress = address;
                        gc.TitleFlexSearch(orderNumber, "", "", titleaddress, "NE", "Lancaster");
                        if ((HttpContext.Current.Session["TitleFlex_Search"] != null && HttpContext.Current.Session["TitleFlex_Search"].ToString() == "Yes"))
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
                            driver.Navigate().GoToUrl("http://orion.lancaster.ne.gov/Appraisal/PublicAccess/PropertySearch.aspx?PropertySearchType=3&SelectedItem=9&PropertyID=&PropertyOwnerID=&NodeID=11");
                            Thread.Sleep(4000);
                        }
                        catch { }

                        driver.FindElement(By.Id("StreetNumber")).SendKeys(houseno);
                        driver.FindElement(By.Id("StreetName")).SendKeys(sname);
                       
                        gc.CreatePdf_WOP(orderNumber, "Address search", driver, "NE", "Lancaster");

                        driver.FindElement(By.Id("SearchSubmit")).SendKeys(Keys.Enter);
                        Thread.Sleep(4000);
                        try
                        {
                            int Max = 0;
                            string strowner = "", strAddress = "", strCity = "";
                            string Record = "";
                            IWebElement Irecord = driver.FindElement(By.XPath("/html/body/table/tbody/tr[2]/td[2]/table/tbody/tr[1]/td/table/tbody/tr/td[1]"));
                            Record = Irecord.Text;
                            Record = gc.Between(Record, "returned", "matches").Trim();
                            if (Record != "1")
                            {
                                gc.CreatePdf_WOP(orderNumber, "Address search Result", driver, "NE", "Lancaster");
                                IWebElement multiaddress = driver.FindElement(By.XPath("//*[@id='propertyRecordsSearchMiniSpaModule']/div/div[2]/div/div[2]/div/div/div[2]/div/div[3]/div[1]/table/tbody"));
                                IList<IWebElement> multiRow = multiaddress.FindElements(By.TagName("tr"));
                                IList<IWebElement> multiTD;
                                foreach (IWebElement multi in multiRow)
                                {
                                    multiTD = multi.FindElements(By.TagName("td"));
                                    if (multiTD.Count != 0 && multiRow.Count >= 2 && multiRow.Count <= 25 && multi.Text.Trim() != "")
                                    {
                                        Strownername = multiTD[6].Text;
                                        Pinnumber = multiTD[3].Text;
                                        parcelNumber = multiTD[4].Text;
                                        PropertyAdd = multiTD[0].Text + " " + multiTD[1].Text;

                                        string multidetails = Strownername + "~" + Pinnumber + "~" + PropertyAdd;
                                        gc.insert_date(orderNumber, parcelNumber, 1607, multidetails, 1, DateTime.Now);
                                        Max++;
                                    }

                                    if (multiTD.Count != 0 && multiRow.Count > 25)
                                    {
                                        HttpContext.Current.Session["multiparcel_Lancaster_Maximum"] = "Maximum";
                                        driver.Quit();
                                        return "Maximum";
                                    }

                                }
                                if (Max > 1 && Max < 26)
                                {
                                    HttpContext.Current.Session["multiparcel_Lancaster"] = "Yes";
                                    driver.Quit();
                                    return "MultiParcel";
                                }
                                if (Max == 0)
                                {
                                    HttpContext.Current.Session["Zero_Lancaster"] = "Zero";
                                    driver.Quit();
                                    return "No Data Found";
                                }
                            }
                        }
                        catch { }
                    }

                 
                    else if (searchType == "parcel")
                    {
                        try
                        {
                            driver.Navigate().GoToUrl("http://orion.lancaster.ne.gov/Appraisal/PublicAccess/PropertySearch.aspx?PropertySearchType=0&SelectedItem=7&PropertyID=&PropertyOwnerID=&NodeID=11");
                            Thread.Sleep(4000);
                        }
                        catch { }


                        driver.FindElement(By.Id("AccountNumber")).SendKeys(parcelNumber);
                        Thread.Sleep(3000);
                        gc.CreatePdf(orderNumber, parcelNumber, "Parcel search", driver, "NE", "Lancaster");

                        driver.FindElement(By.Id("SearchSubmit")).SendKeys(Keys.Enter);
                        Thread.Sleep(4000);
                        try
                        {
                            int Max = 0;
                            string strowner = "", strAddress = "", strCity = "";
                            string Record = "";
                            IWebElement Irecord = driver.FindElement(By.XPath("//*[@id='propertyRecordsSearchMiniSpaModule']/div/div[2]/div/div[2]/div/div/div[2]/div/div[2]/div/span"));
                            Record = Irecord.Text;
                            Record = gc.Between(Record, "to", "of").Trim();
                            if (Record != "1")
                            {
                                gc.CreatePdf(orderNumber, parcelNumber, "Parcel search Result", driver, "NE", "Lancaster");
                                IWebElement multiaddress = driver.FindElement(By.XPath("//*[@id='propertyRecordsSearchMiniSpaModule']/div/div[2]/div/div[2]/div/div/div[2]/div/div[3]/div[1]/table/tbody"));
                                IList<IWebElement> multiRow = multiaddress.FindElements(By.TagName("tr"));
                                IList<IWebElement> multiTD;
                                foreach (IWebElement multi in multiRow)
                                {
                                    multiTD = multi.FindElements(By.TagName("td"));
                                    if (multiTD.Count != 0 && multiRow.Count >= 2 && multiRow.Count <= 25 && multi.Text.Trim() != "")
                                    {
                                        Strownername = multiTD[0].Text;
                                        Pinnumber = multiTD[1].Text;
                                        parcelNumber = multiTD[2].Text;
                                        PropertyAdd = multiTD[4].Text + " " + multiTD[5].Text;

                                        string multidetails = Strownername + "~" + Pinnumber + "~" + PropertyAdd;
                                        gc.insert_date(orderNumber, parcelNumber, 1607, multidetails, 1, DateTime.Now);
                                        Max++;
                                    }
                                    if (multiTD.Count != 0 && multiRow.Count > 25)
                                    {
                                        HttpContext.Current.Session["multiparcel_Lancaster_Maximum"] = "Maximum";
                                        driver.Quit();
                                        return "Maximum";
                                    }

                                }
                                if (Max > 1 && Max < 26)
                                {
                                    HttpContext.Current.Session["multiparcel_Lancaster"] = "Yes";
                                    driver.Quit();
                                    return "MultiParcel";
                                }
                                if (Max == 0)
                                {
                                    HttpContext.Current.Session["Zero_Lancaster"] = "Zero";
                                    driver.Quit();
                                    return "No Data Found";
                                }
                            }
                        }
                        catch { }
                    }

                    if (searchType == "ownername")
                    {
                        try
                        {
                            driver.Navigate().GoToUrl("http://orion.lancaster.ne.gov/Appraisal/PublicAccess/PropertySearch.aspx?PropertySearchType=3&SelectedItem=9&PropertyID=&PropertyOwnerID=&NodeID=11");
                            Thread.Sleep(4000);
                        }
                        catch { }

                        if (ownername.Contains(" "))
                        {
                            string[] owner = ownername.Split(' ');
                            lastName = owner[0];
                            firstName = owner[1];
                        }
                        else
                        {
                            string[] owner = ownername.Split(',');
                            lastName = owner[0];
                            firstName = owner[1];
                        }
                        driver.FindElement(By.Id("lastName")).SendKeys(lastName);
                        driver.FindElement(By.Id("firstName")).SendKeys(firstName);
                        Thread.Sleep(3000);

                        gc.CreatePdf_WOP(orderNumber, "OwnerName search", driver, "NE", "Lancaster");


                        try
                        {
                            int Max = 0;
                            string strowner = "", strAddress = "", strCity = "", strbulkdata = "";
                            string Record = "";
                            IWebElement Irecord = driver.FindElement(By.XPath("//*[@id='propertyRecordsSearchMiniSpaModule']/div/div[2]/div/div[2]/div/div/div[2]/div/div[2]/div/span"));
                            Record = Irecord.Text;
                            Record = gc.Between(Record, "to", "of").Trim();
                            if (Record != "1")
                            {

                                IWebElement multiaddress = driver.FindElement(By.XPath("//*[@id='propertyRecordsSearchMiniSpaModule']/div/div[2]/div/div[2]/div/div/div[2]/div/div[3]/div[1]/table/tbody"));
                                IList<IWebElement> multiRow = multiaddress.FindElements(By.TagName("tr"));
                                IList<IWebElement> multiTD;
                                foreach (IWebElement multi in multiRow)
                                {
                                    multiTD = multi.FindElements(By.TagName("td"));
                                    if (multiTD.Count != 0 && multiRow.Count >= 2 && multiRow.Count <= 25 && multi.Text.Trim() != "")
                                    {
                                        Strownername = multiTD[0].Text;
                                        Pinnumber = multiTD[1].Text;
                                        parcelNumber = multiTD[2].Text;
                                        PropertyAdd = multiTD[4].Text + " " + multiTD[5].Text;

                                        string multidetails = Strownername + "~" + Pinnumber + "~" + PropertyAdd;
                                        gc.insert_date(orderNumber, parcelNumber, 1607, multidetails, 1, DateTime.Now);
                                        Max++;
                                    }
                                    if (multiTD.Count != 0 && multiRow.Count > 25)
                                    {
                                        HttpContext.Current.Session["multiparcel_Lancaster_Maximum"] = "Maximum";
                                        driver.Quit();
                                        return "Maximum";
                                    }

                                }
                                if (Max > 1 && Max < 26)
                                {
                                    HttpContext.Current.Session["multiparcel_Lancaster"] = "Yes";
                                    driver.Quit();
                                    return "MultiParcel";
                                }
                                if (Max == 0)
                                {
                                    HttpContext.Current.Session["Zero_Lancaster"] = "Zero";
                                    driver.Quit();
                                    return "No Data Found";
                                }
                            }
                        }
                        catch { }
                    }


                    //property details
                    string Bulkdata = "";
                    try
                    {
                        IWebElement IBulkdata = driver.FindElement(By.XPath("//*[@id='propertyRecordsSearchMiniSpaModule']/div/div[2]/div/div[2]/div/div/div[2]/label"));
                        Bulkdata = IBulkdata.Text;
                        if (Bulkdata.Contains("No records found"))
                        {
                            HttpContext.Current.Session["Zero_Lancaster"] = "Zero";
                            driver.Quit();
                            return "No Data Found";
                        }
                    }
                    catch { }
                    try
                    {
                        driver.FindElement(By.XPath("/html/body/table/tbody/tr[2]/td[2]/table/tbody/tr[3]/td/table/tbody/tr[2]/td[1]/label")).Click();
                        Thread.Sleep(5000);
                    }
                    catch { }
                    string PropertyClass = "",  MailingAddress = "", LegalDesc="", Exemption="";
                    string OwnerName = "",  PropertyAddress = "", Range = "",  YearBuilt = "";

                   

                    IWebElement parcelNo = driver.FindElement(By.XPath("/html/body/table/tbody/tr[2]/td[2]/table[1]/tbody/tr[12]/td[2]"));
                    parcelNumber = parcelNo.Text;


                    IWebElement IPropertyClass = driver.FindElement(By.XPath("/html/body/table/tbody/tr[2]/td[2]/table[1]/tbody/tr[14]/td[2]"));
                    PropertyClass = IPropertyClass.Text;
                    IWebElement StrOwnerName = driver.FindElement(By.XPath("/html/body/table/tbody/tr[2]/td[2]/table[1]/tbody/tr[4]/td[2]"));
                    OwnerName = StrOwnerName.Text;

                    IWebElement IProadd1 = driver.FindElement(By.XPath("/html/body/table/tbody/tr[2]/td[2]/table[1]/tbody/tr[6]/td[2]"));
                    PropertyAddress = IProadd1.Text.Replace("\r\n"," ");

                    IWebElement IMailAdd1 = driver.FindElement(By.XPath("/html/body/table/tbody/tr[2]/td[2]/table[1]/tbody/tr[5]/td[2]"));
                    MailingAddress = IMailAdd1.Text.Replace("\r\n"," ");

                    LegalDesc = driver.FindElement(By.XPath("/html/body/table/tbody/tr[2]/td[2]/table[1]/tbody/tr[11]/td[2]")).Text;

                    try
                    {
                        Exemption = driver.FindElement(By.XPath("/html/body/table/tbody/tr[2]/td[2]/table[1]/tbody/tr[13]/td[2]")).Text;
                    }
                    catch { }
                    try
                    {
                        YearBuilt = driver.FindElement(By.XPath("/html/body/table/tbody/tr[2]/td[2]/table[1]/tbody/tr[18]/td[2]")).Text;
                    }
                    catch { }

                    string propertydetails = OwnerName + "~" + PropertyAddress + "~" + MailingAddress + "~" + YearBuilt + "~" + LegalDesc + "~" + Exemption + "~" + PropertyClass;
                    gc.insert_date(orderNumber, parcelNumber, 1692, propertydetails, 1, DateTime.Now);

                    gc.CreatePdf(orderNumber, parcelNumber, "Property Details", driver, "NE", "Lancaster");

                    // Assessment Details

                    //try
                    //{
                    //    driver.FindElement(By.LinkText("Datasheet")).SendKeys(Keys.Enter);
                    //    Thread.Sleep(4000);
                    //}
                    //catch { }


                    var chromeOptions = new ChromeOptions();

                    var downloadDirectory = "F:\\AutoPdf\\";

                    chromeOptions.AddUserProfilePreference("download.default_directory", downloadDirectory);
                    chromeOptions.AddUserProfilePreference("download.prompt_for_download", false);
                    chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");
                    chromeOptions.AddUserProfilePreference("plugins.always_open_pdf_externally", true);
                    var driver1 = new ChromeDriver(chromeOptions);
                    driver1.Navigate().GoToUrl(driver.Url);
                    string fileName = "PropertyDataSheet.pdf";

                    IWebElement assessme = driver1.FindElement(By.LinkText("Datasheet"));
                    assessme.Click();
                    Thread.Sleep(3000);
                    //string ass = assessme.GetAttribute("href");
                    try
                    {
                       gc.AutoDownloadFile(orderNumber, parcelNumber, "Lancaster", "NE", fileName);
                    }
                    catch (Exception ex) { }
                    try
                    {

                        driver1.Quit();
                        string FilePath = gc.filePath(orderNumber, parcelNumber) + "PropertyDataSheet.pdf";
                        PdfReader reader;
                        string pdfData;
                        string pdftext = "";
                        try
                        {
                            reader = new PdfReader(FilePath);
                            String textFromPage = PdfTextExtractor.GetTextFromPage(reader, 1);
                            System.Diagnostics.Debug.WriteLine("" + textFromPage);

                            pdftext = textFromPage;
                        }
                        catch { }


                        string tableassess = gc.Between(pdftext, "Total", "NRA:").Trim();
                        string[] tableArray = tableassess.Split('\n');

                        int count1 = tableArray.Length;
                        for (int i = 0; i < count1; i++)
                        {
                            // 
                            string a1 = tableArray[i].Replace(" ", "~");
                            string[] rowarray = a1.Split('~');
                            int tdcount = rowarray.Length;
                            if (tdcount == 4)
                            {
                                gc.insert_date(orderNumber, parcelNumber, 1693, a1, 1, DateTime.Now);
                            }
                            else if (tdcount == 6)
                            {
                                int j = 0;
                                string newrow = rowarray[j + 2] + "~" + rowarray[j + 3] + "~" + rowarray[j + 4] + "~" + rowarray[j + 5];
                                gc.insert_date(orderNumber, parcelNumber, 1693, newrow, 1, DateTime.Now);
                            }

                        }

                    }
                    catch { }

                    // Tax Information Details
                    string taxAuth = "", taxauth1 = "", taxauth2 = "";

                    driver.Navigate().GoToUrl("https://www.lincoln.ne.gov/aspx/cnty/cto/default.aspx");
                    Thread.Sleep(5000);
                   

                    try
                    {
                        driver.FindElement(By.XPath("//*[@id='__tab_ctl00_ctl00_cph1_cph1_tcOptions_tpParcel']/span")).Click();
                        Thread.Sleep(4000);
                    }
                    catch { }
                    driver.FindElement(By.Id("ctl00_ctl00_cph1_cph1_tcOptions_tpParcel_txtParcel")).SendKeys(parcelNumber);
                    gc.CreatePdf(orderNumber, parcelNumber, "Tax Search", driver, "NE", "Lancaster");
                    driver.FindElement(By.Id("ctl00_ctl00_cph1_cph1_tcOptions_tpParcel_btnParcel")).Click();
                    Thread.Sleep(4000);
                    gc.CreatePdf(orderNumber, parcelNumber, "Tax Search Result", driver, "NE", "Lancaster");
                    try
                    {
                        driver.FindElement(By.Id("ctl00_ctl00_cph1_cph1_gvProperty_ctl02_btnSelect")).Click();
                        Thread.Sleep(4000);
                    }
                    catch { }

                    string Taxtype = "", OwneName = "", ProAdd = "", MailAdd="", Taxyear = "", Taxsale = "", splAssessHistory = "";
                    string TaxDistrict = "", Taxrate = "", paidAmount = "", TaxAmount = "", taxcredit = "", Ag_and_Credit = "", FinalTaxAmount = "";
                    string fees = "", penalty = "", Total_Tax_Amount = "", Interest = "";

                    gc.CreatePdf(orderNumber, parcelNumber, "Tax Information", driver, "NE", "Lancaster");

                    Taxtype = driver.FindElement(By.Id("ctl00_ctl00_cph1_cph1_fvProperty_lblRollName")).Text;
                    OwneName = driver.FindElement(By.Id("ctl00_ctl00_cph1_cph1_fvProperty_lblPowner")).Text;

                    string Taxdata = driver.FindElement(By.XPath("//*[@id='ctl00_ctl00_cph1_cph1_fvProperty']/tbody")).Text;
                    Thread.Sleep(2000);
                    MailAdd = gc.Between(Taxdata, "Owner Address", "Situs Address").Replace(":","").Replace("City", "").Trim();
                    ProAdd = gc.Between(Taxdata, "Property Class", "Legal Description").Replace(":","");

                    Taxyear = driver.FindElement(By.Id("ctl00_ctl00_cph1_cph1_fvProperty_lblTaxYear")).Text;

                    Taxsale = driver.FindElement(By.Id("ctl00_ctl00_cph1_cph1_fvProperty_lblTaxSale")).Text;
                    try
                    {
                        splAssessHistory = driver.FindElement(By.Id("ctl00_ctl00_cph1_cph1_fvProperty_rpSpecialAssessment_ctl00_lblNotFound")).Text;
                    }
                    catch { }

                    if(splAssessHistory=="")
                    {
                        try
                        {
                            splAssessHistory = driver.FindElement(By.Id("ctl00_ctl00_cph1_cph1_fvProperty_rpSpecialAssessment_ctl00_lblSpecialAssessment")).Text;
                        }
                        catch { }
                    }


                    // Tax History Details
                    try
                    {
                        IWebElement TaxHistory = driver.FindElement(By.XPath("//*[@id='ctl00_ctl00_cph1_cph1_gvHistory']/tbody"));
                        IList<IWebElement> TRTaxHistory = TaxHistory.FindElements(By.TagName("tr"));
                        IList<IWebElement> THTaxHistory = TaxHistory.FindElements(By.TagName("th"));
                        IList<IWebElement> TDTaxHistory;
                        foreach (IWebElement row in TRTaxHistory)
                        {
                            TDTaxHistory = row.FindElements(By.TagName("td"));
                            if (TDTaxHistory.Count != 0 && !row.Text.Contains("Tax Amount") && row.Text.Trim() != "")
                            {
                                string TaxDistributiondetails = TDTaxHistory[0].Text + "~" + TDTaxHistory[1].Text + "~" + TDTaxHistory[2].Text + "~" + TDTaxHistory[3].Text + "~" + TDTaxHistory[4].Text + "~" + TDTaxHistory[5].Text + "~" + TDTaxHistory[6].Text + "~" + TDTaxHistory[7].Text + "~" + TDTaxHistory[8].Text;
                                gc.insert_date(orderNumber, parcelNumber, 1701, TaxDistributiondetails, 1, DateTime.Now);
                            }
                        }
                    }
                    catch { }


                    // Current Year Tax Information 
                    string Good_through_date = "";
                  
                        try
                        {
                            //Good Through Details
                            
                            if (Taxsale == "Yes")
                            {
                               
                                IWebElement good_date = driver.FindElement(By.Id("ctl00_ctl00_cph1_cph1_txtPayDate"));
                                Good_through_date = good_date.GetAttribute("value");
                                if (Good_through_date.Contains("Select A Date"))
                                {
                                    Good_through_date = "-";
                                }
                                else
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
                                                Good_through_date = new DateTime(nxtYr, 1, DateTime.DaysInMonth(Convert.ToInt16(DateTime.Now.ToString("yyyy")), 1)).ToString("dd/MM/yyyy");

                                            }
                                            try
                                            {//*[@id="ui-datepicker-div"]/div/a[2]/span
                                                IWebElement gclick1 = driver.FindElement(By.Id("ctl00_ctl00_PrimaryPlaceHolder_ContentPlaceHolderMain_ViewBill1_AsOfDateTextBox"));
                                                gclick1.Click();
                                                IWebElement Inextmonth = driver.FindElement(By.XPath("//*[@id='ui-datepicker-div']/div/a[2]/span"));
                                                Inextmonth.Click();
                                            }
                                            catch { }

                                        }
                                        driver.FindElement(By.Id("ctl00_ctl00_cph1_cph1_txtPayDate")).Clear();

                                        string[] daysplit = Good_through_date.Split('/');
                                try
                                {
                                    driver.FindElement(By.Id("ctl00_ctl00_cph1_cph1_txtPayDate")).SendKeys(Good_through_date);
                                    driver.FindElement(By.XPath("//*[@id='ctl00_ctl00_cph1_cph1_btnCalc']")).Click();
                                    Thread.Sleep(4000);
                                    gc.CreatePdf(orderNumber, parcelNumber, "Good Through Date", driver, "NE", "Lancaster");
                                }
                                catch { }

                                //        try
                                //        {
                                //            IWebElement IValuation = driver.FindElement(By.XPath("//*[@id='ctl00_ctl00_cph1_cph1_imgPayDate']"));
                                //            IJavaScriptExecutor js5 = driver as IJavaScriptExecutor;
                                //            js5.ExecuteScript("arguments[0].click();", IValuation);
                                //            Thread.Sleep(4000);
                                //         }
                                //         catch { }


                                //IWebElement Iday = driver.FindElement(By.Id("//*[@id='ui-datepicker-div']/table/tbody"));
                                //IList<IWebElement> IdayRow = Iday.FindElements(By.TagName("a"));
                                //foreach (IWebElement day in IdayRow)
                                //{
                                //    if (day.Text != "" && day.Text == daysplit[1])
                                //    {
                                //        day.SendKeys(Keys.Enter);
                                //        Thread.Sleep(2000);
                                //        gc.CreatePdf(orderNumber, parcelNumber, "Good Through Date", driver, "MD", "Howard");

                                //    }
                                //}


                            }
                                }
                            
                        }
                        catch { }
                    

                    try
                    {
                        IWebElement CurrentTax = driver.FindElement(By.XPath("//*[@id='ctl00_ctl00_cph1_cph1_rpPaymentYears_ctl01_gvPaymentOptions']/tbody"));
                        IList<IWebElement> TRCurrentTax = CurrentTax.FindElements(By.TagName("tr"));
                        IList<IWebElement> THCurrentTax = CurrentTax.FindElements(By.TagName("th"));
                        IList<IWebElement> TDCurrentTax;
                        foreach (IWebElement row in TRCurrentTax)
                        {
                            TDCurrentTax = row.FindElements(By.TagName("td"));
                            if (TDCurrentTax.Count != 0 && !row.Text.Contains("Tax Amount") && row.Text.Trim() != "")
                            {
                                string TaxDistributiondetails = TDCurrentTax[0].Text + "~" + TDCurrentTax[1].Text + "~" + TDCurrentTax[2].Text + "~" + TDCurrentTax[3].Text + "~" + TDCurrentTax[4].Text + "~" + TDCurrentTax[5].Text + "~" + TDCurrentTax[6].Text + "~" + TDCurrentTax[7].Text + "~" + TDCurrentTax[8].Text + "~" + TDCurrentTax[9].Text + "~" + TDCurrentTax[10].Text + "~" + TDCurrentTax[11].Text + "~" + TDCurrentTax[12].Text;
                                gc.insert_date(orderNumber, parcelNumber, 1702, TaxDistributiondetails, 1, DateTime.Now);
                            }
                        }
                    }
                    catch { }


                    try
                    {
                        IWebElement TaxDue = driver.FindElement(By.XPath("//*[@id='ctl00_ctl00_cph1_cph1_gvHistory']/tbody"));
                        IList<IWebElement> TRTaxDue = TaxDue.FindElements(By.TagName("tr"));
                        IList<IWebElement> THTaxDue = TaxDue.FindElements(By.TagName("th"));
                        IList<IWebElement> TDTaxDue;
                        foreach (IWebElement row in TRTaxDue)
                        {
                            TDTaxDue = row.FindElements(By.TagName("td"));
                            if (TDTaxDue.Count != 0 && !row.Text.Contains("Tax Value"))
                            {
                                TDTaxDue[0].Click();
                                Thread.Sleep(4000);
                                break;
                            }
                        }
                    }
                    catch { }

                    string taxbulkdata = driver.FindElement(By.XPath("//*[@id='ctl00_ctl00_cph1_cph1_fvProperty']/tbody")).Text;
                    TaxDistrict = gc.Between(taxbulkdata, "Tax District", "Tax Rate").Replace(":", "").Trim();
                    Taxrate = GlobalClass.After(taxbulkdata, "Tax Rate").Replace(":","").Trim();
                    paidAmount = gc.Between(taxbulkdata, "Paid Tax", "Paid Interest").Replace(":", "").Trim();
                    TaxAmount = gc.Between(taxbulkdata, "Tax Amount", "Tax Credit").Replace(":", "").Trim();
                    taxcredit = gc.Between(taxbulkdata, "Tax Credit", "Ag Land Credit").Replace(":", "").Trim();
                    Ag_and_Credit = gc.Between(taxbulkdata, "Ag Land Credit", "Final Tax Amt").Replace(":", "").Trim();
                    FinalTaxAmount= gc.Between(taxbulkdata, "Final Tax Amt", "Fees").Replace(":", "").Trim();
                    fees = gc.Between(taxbulkdata, "Fees", "Penalty").Replace(":", "").Trim();
                    penalty = gc.Between(taxbulkdata, "Penalty", "Total Tax Amt").Replace(":", "").Trim();
                    Total_Tax_Amount = gc.Between(taxbulkdata, "Total Tax Amt", "Paid Tax").Replace(":", "").Trim();
                    Interest = gc.Between(taxbulkdata, "Paid Interest" , "Tax District").Replace(":", "").Trim();

                    
                    string Appraised = "", Assessed = "", Excemption = "", Taxable = "", Base="", CIF="";

                    Appraised = gc.Between(taxbulkdata, "Appraised", "Assessed").Replace(":","").Trim();
                    Assessed = gc.Between(taxbulkdata, "Assessed", "Exemption").Replace(":", "").Trim();
                    Excemption = gc.Between(taxbulkdata, "Exemption", "Taxable").Replace(":", "").Trim();
                    try
                    {
                        Taxable = gc.Between(taxbulkdata, "Taxable", "Tax Amount").Replace(":", "").Trim();
                    }
                    catch { }


                    string Taxassessdetails = Taxyear + "~" + Appraised + "~" + Assessed + "~" + Excemption + "~" + Taxable + "~" + Base + "~" + CIF;
                    gc.insert_date(orderNumber, parcelNumber, 1700, Taxassessdetails, 1, DateTime.Now);

                    // Tax Information Details
                    try
                    {
                        string TaxInformationdetails = Taxtype + "~" + OwneName + "~" + MailAdd + "~" + ProAdd + "~" + Taxyear + "~" + Taxsale + "~" + splAssessHistory + "~" + TaxDistrict + "~" + Taxrate + "~" + paidAmount + "~" + TaxAmount + "~" + taxcredit + "~" + Ag_and_Credit + "~" + FinalTaxAmount + "~" + fees + "~" + penalty + "~" + Total_Tax_Amount + "~" + Interest;
                        gc.insert_date(orderNumber, parcelNumber, 1699, TaxInformationdetails, 1, DateTime.Now);
                    }
                    catch { }

                    // Tax Distribution Details
                   
                    try
                    {
                        IWebElement TaxDistribution = driver.FindElement(By.XPath("//*[@id='ctl00_ctl00_cph1_cph1_gvPropertyTax']/tbody"));
                        IList<IWebElement> TRTaxDistribution = TaxDistribution.FindElements(By.TagName("tr"));
                        IList<IWebElement> THTaxDistribution = TaxDistribution.FindElements(By.TagName("th"));
                        IList<IWebElement> TDTaxDistribution;
                        foreach (IWebElement row in TRTaxDistribution)
                        {
                            TDTaxDistribution = row.FindElements(By.TagName("td"));
                            if (TDTaxDistribution.Count != 0 && !row.Text.Contains("Paid Amount") && row.Text.Trim() != "")
                            {
                                string TaxDistributiondetails = TDTaxDistribution[0].Text + "~" + TDTaxDistribution[1].Text + "~" + TDTaxDistribution[2].Text + "~" + TDTaxDistribution[3].Text + "~" + TDTaxDistribution[4].Text + "~" + TDTaxDistribution[5].Text + "~" + TDTaxDistribution[6].Text + "~" + TDTaxDistribution[7].Text;
                                gc.insert_date(orderNumber, parcelNumber, 1703, TaxDistributiondetails, 1, DateTime.Now);
                            }
                        }
                    }
                    catch { }

                    try
                    {
                        driver.FindElement(By.LinkText("Property")).SendKeys(Keys.Enter);
                        Thread.Sleep(4000);
                    }
                    catch { }

                    try
                    {
                        driver.FindElement(By.LinkText("Payment History")).Click();
                        Thread.Sleep(5000);
                        gc.CreatePdf(orderNumber, parcelNumber, "Tax Payment Details 1", driver, "NE", "Lancaster");
                    }
                    catch { }

                    try
                    {
                        driver.FindElement(By.XPath("//*[@id='ctl00_ctl00_cph1_cph1_gvReceiptMaster']/tbody/tr[22]/td/table/tbody/tr/td[2]/a")).Click();
                        Thread.Sleep(5000);
                        gc.CreatePdf(orderNumber, parcelNumber, "Tax Payment Details 2", driver, "NE", "Lancaster");
                    }
                    catch { }

                    try
                    {
                        driver.FindElement(By.LinkText("Property")).SendKeys(Keys.Enter);
                        Thread.Sleep(4000);
                    }
                    catch { }

                    try
                    {
                        driver.FindElement(By.LinkText("Special Assessment")).Click();
                        Thread.Sleep(4000);
                    }
                    catch { }

                    // Special Assessment Details
                    string splBulkdata="", spl_Ass_Type = "", District = "", status = "", LevyDate = "", DelinqDate = "", Total_principal = "", BondInterest = "", DelinqInterest = "";
                    try
                    {
                        splBulkdata = driver.FindElement(By.XPath("//*[@id='ctl00_ctl00_cph1_cph1_fvSpecialMaster1']/tbody")).Text;
                        spl_Ass_Type = gc.Between(splBulkdata, "Kind", "District").Replace(":", "").Trim();
                        District = gc.Between(splBulkdata, "District", "Status").Replace(":", "").Trim();
                        status = gc.Between(splBulkdata, "Status", "Levy Date").Replace(":", "").Trim();
                        LevyDate = gc.Between(splBulkdata, "Levy Date", "Delinquent Date").Replace(":", "").Trim();
                        DelinqDate = gc.Between(splBulkdata, "Delinquent Date", "Total Principal").Replace(":", "").Trim();
                        Total_principal = gc.Between(splBulkdata, "Total Principal", "Bond Interest").Replace(":", "").Trim();
                        BondInterest = gc.Between(splBulkdata, "Bond Interest", "Delinquent Interest").Replace(":", "").Trim();
                        DelinqInterest = GlobalClass.After(splBulkdata, "Delinquent Interest").Replace(":", "").Trim();
                    }
                    catch { }

                    try
                    {
                        IWebElement splAss = driver.FindElement(By.XPath("//*[@id='ctl00_ctl00_cph1_cph1_gvSpecialDetail']/tbody"));
                        IList<IWebElement> TRsplAss = splAss.FindElements(By.TagName("tr"));
                        IList<IWebElement> THsplAss = splAss.FindElements(By.TagName("th"));
                        IList<IWebElement> TDsplAss;
                        foreach (IWebElement row in TRsplAss)
                        {
                            TDsplAss = row.FindElements(By.TagName("td"));
                            if (TDsplAss.Count != 0 && !row.Text.Contains("Bond Interest") && row.Text.Trim() != "")
                            {
                                string splAssessmentdetails = spl_Ass_Type + "~" + District + "~" + status + "~" + LevyDate + "~" + DelinqDate + "~" + Total_principal + "~" + BondInterest + "~" + DelinqInterest + "~" + TDsplAss[0].Text + "~" + TDsplAss[1].Text + "~" + TDsplAss[2].Text + "~" + TDsplAss[3].Text + "~" + TDsplAss[4].Text + "~" + TDsplAss[5].Text + "~" + TDsplAss[6].Text + "~" + TDsplAss[7].Text + "~" + TDsplAss[8].Text;
                                gc.insert_date(orderNumber, parcelNumber, 1754, splAssessmentdetails, 1, DateTime.Now);
                            }
                        }
                    }
                    catch { }

                    // Special Assessment History Details

                    try
                    {
                        driver.FindElement(By.Id("ctl00_ctl00_cph1_cph1_btnShow")).Click();
                        Thread.Sleep(4000);
                    }
                    catch { }
                  
                    try
                    {
                        IWebElement splAssHis = driver.FindElement(By.XPath("//*[@id='ctl00_ctl00_cph1_cph1_gvSpecialDetail']/tbody"));
                        IList<IWebElement> TRsplAssHis = splAssHis.FindElements(By.TagName("tr"));
                        IList<IWebElement> THsplAssHis = splAssHis.FindElements(By.TagName("th"));
                        IList<IWebElement> TDsplAssHis;
                        foreach (IWebElement row in TRsplAssHis)
                        {
                            TDsplAssHis = row.FindElements(By.TagName("td"));
                            if (TDsplAssHis.Count != 0 && !row.Text.Contains("Bond Interest") && row.Text.Trim() != "")
                            {
                                string splAssHistory = TDsplAssHis[0].Text + "~" + TDsplAssHis[1].Text + "~" + TDsplAssHis[2].Text + "~" + TDsplAssHis[3].Text + "~" + TDsplAssHis[4].Text + "~" + TDsplAssHis[5].Text + "~" + TDsplAssHis[6].Text + "~" + TDsplAssHis[7].Text + "~" + TDsplAssHis[8].Text;
                                gc.insert_date(orderNumber, parcelNumber, 1755, splAssHistory, 1, DateTime.Now);
                            }
                        }
                    }
                    catch { }


                  
                  

                 
                  


                    TaxTime = DateTime.Now.ToString("HH:mm:ss");

                    LastEndTime = DateTime.Now.ToString("HH:mm:ss");
                    gc.insert_TakenTime(orderNumber, "NE", "Lancaster", StartTime, AssessmentTime, TaxTime, CitytaxTime, LastEndTime);

                    driver.Quit();
                    gc.mergpdf(orderNumber, "NE", "Lancaster");
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