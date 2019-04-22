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
    public class WebDriver_LakeIL
    {

        IWebDriver driver;
        DBconnection db = new DBconnection();
        GlobalClass gc = new GlobalClass();

        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["MyConnectionString"].ToString());

        public string FTP_LakeIL(string houseno, string Direction, string sname, string stype, string unitNumber,string zipcode ,string parcelNumber, string ownername, string searchType, string orderNumber, string directParcel)
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
                // using (driver = new ChromeDriver())

                    try
                {
                    StartTime = DateTime.Now.ToString("HH:mm:ss");

                   
                        if (searchType == "titleflex")
                       {
                            if (Direction != "")
                            {
                                address = houseno + " " + Direction + " " + sname + " " + stype + " " + unitNumber;
                                address = address.Trim();
                            }
                            if (Direction == "")
                            {
                                address = houseno + " " + sname + " " + stype + " " + unitNumber;
                                address = address.Trim();
                            }
                            string titleaddress = address;
                            gc.TitleFlexSearch(orderNumber, "", "", titleaddress, "IL", "Lake");
                        if ((HttpContext.Current.Session["TitleFlex_Search"] != null && HttpContext.Current.Session["TitleFlex_Search"].ToString() == "Yes"))
                        {
                            return "MultiParcel";
                        }
                        parcelNumber = HttpContext.Current.Session["titleparcel"].ToString();
                        searchType = "parcel";
                    }

                    if (searchType == "address")
                        {
                            driver.Navigate().GoToUrl("https://apps03.lakecountyil.gov/comparables/PTAIAddress.aspx");
                            Thread.Sleep(4000);

                            driver.FindElement(By.XPath("//*[@id='tbStreetNum']")).SendKeys(houseno);
                            driver.FindElement(By.Id("tbStreetName")).SendKeys(sname);
                            driver.FindElement(By.Id("tbZip")).SendKeys(zipcode);
                            gc.CreatePdf_WOP(orderNumber, "Address search", driver, "IL", "Lake");
                        driver.FindElement(By.Id("cmdSubmit")).SendKeys(Keys.Enter);
                        Thread.Sleep(3000);
                        gc.CreatePdf_WOP(orderNumber, "Address search Result", driver, "IL", "Lake");

                        try
                        {
                            int Max = 0;
                            string strowner = "", strAddress = "", strCity = "";
                            string Record = "";
                            IWebElement multiaddress;

                            gc.CreatePdf(orderNumber, parcelNumber, "Parcel search Result", driver, "IL", "Lake");

                            multiaddress = driver.FindElement(By.XPath("//*[@id='ext-gen683']"));

                            IList<IWebElement> multiRow = multiaddress.FindElements(By.TagName("tr"));
                            IList<IWebElement> multiTD;
                            foreach (IWebElement multi in multiRow)
                            {
                                multiTD = multi.FindElements(By.TagName("td"));
                                if (multiTD.Count != 0 && multiRow.Count >= 1 && multiRow.Count <= 25 && multi.Text.Trim() != "")
                                {
                                    Strownername = multiTD[1].Text;

                                    parcelNumber = multiTD[0].Text.Trim();
                                    PropertyAdd = multiTD[2].Text + " " + multiTD[3].Text;

                                    string multidetails = Strownername + "~" + PropertyAdd;
                                    gc.insert_date(orderNumber, parcelNumber, 1715, multidetails, 1, DateTime.Now);
                                    Max++;
                                }
                                if (multiTD.Count != 0 && multiRow.Count > 25)
                                {
                                    HttpContext.Current.Session["multiparcel_LakeIL_Maximum"] = "Maximum";
                                    driver.Quit();
                                    return "Maximum";
                                }

                            }
                            if (Max > 1 && Max < 26)
                            {
                                HttpContext.Current.Session["multiparcel_LakeIL"] = "Yes";
                                driver.Quit();
                                return "MultiParcel";
                            }
                            if (Max == 1)
                            {
                                string nparcel = "R" + parcelNumber;
                                string url = "http://douglascone.wgxtreme.com/java/wgx_douglasne/static/accountinfo.jsp?accountno=" + nparcel;
                                driver.Navigate().GoToUrl(url);
                                Thread.Sleep(4000);
                            }
                            if (Max == 0)
                            {
                                HttpContext.Current.Session["Zero_LakeIL"] = "Zero";
                                driver.Quit();
                                return "No Data Found";
                            }

                        }
                        catch { }
                    }

                   

                    else if (searchType == "parcel")
                    {
                        
                            driver.Navigate().GoToUrl("https://apps03.lakecountyil.gov/comparables/PTAIPIN.aspx");
                            Thread.Sleep(4000);
                          
                            driver.FindElement(By.Id("txtPIN")).SendKeys(parcelNumber);
                        Thread.Sleep(3000);
                        gc.CreatePdf(orderNumber, parcelNumber, "Parcel search", driver, "IL", "Lake");
                        try
                        {
                            driver.FindElement(By.XPath("//*[@id='Button2']")).SendKeys(Keys.Enter);
                            Thread.Sleep(3000);
                        }
                        catch { }
                     
                        try
                        {
                            int Max = 0;
                            string strowner = "", strAddress = "", strCity = "";
                            string Record = "";
                            IWebElement multiaddress;

                            gc.CreatePdf(orderNumber, parcelNumber, "Parcel search Result", driver, "IL", "Lake");

                            multiaddress = driver.FindElement(By.XPath("//*[@id='ext-gen683']/div/table/tbody"));

                            IList<IWebElement> multiRow = multiaddress.FindElements(By.TagName("tr"));
                            IList<IWebElement> multiTD;
                            foreach (IWebElement multi in multiRow)
                            {
                                multiTD = multi.FindElements(By.TagName("td"));
                                if (multiTD.Count != 0 && multiRow.Count >= 1 && multiRow.Count <= 25 && multi.Text.Trim() != "")
                                {
                                    Strownername = multiTD[1].Text;

                                    parcelNumber = multiTD[0].Text;
                                    PropertyAdd = multiTD[2].Text + " " + multiTD[3].Text;

                                    string multidetails = Strownername + "~" + PropertyAdd;
                                    gc.insert_date(orderNumber, parcelNumber, 1715, multidetails, 1, DateTime.Now);
                                    Max++;
                                }
                                if (multiTD.Count != 0 && multiRow.Count > 25)
                                {
                                    HttpContext.Current.Session["multiparcel_DouglasNE_Maximum"] = "Maximum";
                                    driver.Quit();
                                    return "Maximum";
                                }

                            }
                            if (Max > 1 && Max < 26)
                            {
                                HttpContext.Current.Session["multiparcel_LakeIL"] = "Yes";
                                driver.Quit();
                                return "MultiParcel";
                            }
                            if (Max == 1)
                            {
                                driver.Navigate().GoToUrl("http://douglascone.wgxtreme.com/java/wgx_douglasne/static/accountinfo.jsp?ext=false&accountno=R2321820562");
                                Thread.Sleep(4000);
                            }
                            if (Max == 0)
                            {
                                HttpContext.Current.Session["Zero_LakeIL"] = "Zero";
                                driver.Quit();
                                return "No Data Found";
                            }
                            // }
                        }
                        catch { }
                    }

                   

                    //property details

                    string streetAddress = "", City = "", Zipcode = "", LandAmount = "";
                    string BuildingAmount = "", TotalAmount = "", Township = "", AssessDate="",ClassDesc="", YearBuilt = "";

                    string bulkdata = driver.FindElement(By.XPath("//*[@id='PropertyCharacteristics1_tblPropertyAddress']/tbody")).Text;
                        parcelNumber = gc.Between(bulkdata, "Pin", "Street Address").Replace(":", "").Replace("-", "").Trim();

                        streetAddress = gc.Between(bulkdata, "Street Address", "City").Replace(":","").Trim();
                        City = gc.Between(bulkdata, "City", "Zip Code").Replace(":", "").Trim();
                        Zipcode = gc.Between(bulkdata, "Zip Code", "Land Amount").Replace(":", "").Trim();
                        LandAmount = gc.Between(bulkdata, "Land Amount", "Building Amount").Replace(":", "").Trim();
                        BuildingAmount = gc.Between(bulkdata, "Building Amount", "Total Amount").Replace(":", "").Trim();
                        TotalAmount = gc.Between(bulkdata, "Total Amount", "Township").Replace(":", "").Trim();
                        Township = gc.Between(bulkdata, "Township", "Assessment Date").Replace(":", "").Trim();
                        AssessDate = GlobalClass.After(bulkdata, "Assessment Date").Replace(":", "").Trim();
                        ClassDesc = driver.FindElement(By.XPath("//*[@id='PropertyCharacteristics1_lblClassDescription']")).Text;
                        try
                        {
                          
                        YearBuilt = driver.FindElement(By.Id("PropertyCharacteristics1_lblEffectiveAge")).Text;
                        }
                        catch { }
                   
                    string propertydetails = streetAddress + "~" + City + "~" + Zipcode + "~" + LandAmount + "~" + BuildingAmount + "~" + TotalAmount + "~" + Township + "~" + AssessDate + "~" + ClassDesc + "~" + YearBuilt;
                    gc.insert_date(orderNumber, parcelNumber, 1793, propertydetails, 1, DateTime.Now);

                    gc.CreatePdf(orderNumber, parcelNumber, "Property Details", driver, "IL", "Lake");

                  

                    // Tax Information Details
                    string taxAuth = "", taxauth1 = "", taxauth2 = "";
                    try
                    {
                            driver.Navigate().GoToUrl("http://www.lakecountyil.gov/508/Current-Payment-Status");
                            taxAuth = driver.FindElement(By.XPath("//*[@id='divInfoAdvae35de24-85e5-460b-b9ce-18eda922e6cb']/div[1]/div/div/ol/li/span")).Text.Replace("Contact Us", "").Replace("Lake County, IL", "").Replace("Parking and Directions", "").Trim();
                            gc.CreatePdf(orderNumber, parcelNumber, "Tax Authority", driver, "IL", "Lake");
                        }
                    catch { }
                    driver.Navigate().GoToUrl("https://apps03.lakecountyil.gov/treasurer/collbook/collbook2.asp");
                    Thread.Sleep(5000);

                    driver.FindElement(By.XPath("//*[@id='table53']/tbody/tr[3]/td/input")).SendKeys(parcelNumber);
                    gc.CreatePdf(orderNumber, parcelNumber, "Tax Search", driver, "IL", "Lake");
                    driver.FindElement(By.XPath("//*[@id='table53']/tbody/tr[5]/td/input")).Click();
                    Thread.Sleep(4000);
                    gc.CreatePdf(orderNumber, parcelNumber, "Tax Search Result", driver, "IL", "Lake");
                        int taxyear =0;
                        string tax_year = "";
                        tax_year = driver.FindElement(By.XPath("//*[@id='table56']/tbody/tr[4]/td[1]/a")).Text;
                        taxyear = Convert.ToInt32(tax_year);

                        for (int i = 0; i < 3; i++)
                        {
                            string current = driver.CurrentWindowHandle;
                            try
                            {
                                IWebElement TaxBill = driver.FindElement(By.XPath("//*[@id='table56']/tbody"));
                                IList<IWebElement> TRTaxBill = TaxBill.FindElements(By.TagName("tr"));
                                IList<IWebElement> THTaxBill = TaxBill.FindElements(By.TagName("th"));
                                IList<IWebElement> TDTaxBill;
                                foreach (IWebElement row in TRTaxBill)
                                {
                                    TDTaxBill = row.FindElements(By.TagName("td"));
                                    if (TDTaxBill.Count != 0 && !row.Text.Contains("Amount Billed") && !row.Text.Contains("Billing History") && !row.Text.Contains("Original") && row.Text.Trim() != "" && TDTaxBill.Count == 5 && TDTaxBill[0].Text==taxyear.ToString())
                                    {
                                        IWebElement ITAXBill = TDTaxBill[0].FindElement(By.TagName("a"));
                                        ITAXBill.Click();
                                        Thread.Sleep(2000);
                                        break;
                                    }
                                }
                            }
                            catch { }

                            driver.SwitchTo().Window(driver.WindowHandles.Last());
                            Thread.Sleep(2000);

                         
                            string PropertyLocation = "", PropertyLocation2 = "", TaxYear = "", TaxCode = "", Acres = "", LegalDesc = "";

                            PropertyLocation = driver.FindElement(By.XPath("//*[@id='lblSitusLine1']")).Text;
                            PropertyLocation2 = driver.FindElement(By.XPath("//*[@id='lblSitusCity']")).Text;
                            PropertyLocation = PropertyLocation + " " + PropertyLocation2;
                            TaxYear = driver.FindElement(By.XPath("//*[@id='lblTaxYear2']")).Text;
                            TaxCode = driver.FindElement(By.Id("lblTcaNumber")).Text;
                            Acres = driver.FindElement(By.Id("lblAcres")).Text;
                            LegalDesc = driver.FindElement(By.Id("lblLegalDescription")).Text;

                            gc.CreatePdf(orderNumber, parcelNumber, "Tax Bill" + TaxYear, driver, "IL", "Lake");

                            try
                            {
                                string TaxInfodetails = PropertyLocation + "~" + TaxYear + "~" + TaxCode + "~" + Acres + "~" + LegalDesc + "~" + taxAuth;
                                gc.insert_date(orderNumber, parcelNumber, 1800, TaxInfodetails, 1, DateTime.Now);

                            }
                            catch { }


                            // Taxing Body Details
                            try
                            {
                                IWebElement TaxBody = driver.FindElement(By.XPath("//*[@id='tblPriorYearsBillingDistrict']/tbody"));
                                IList<IWebElement> TRTaxBody = TaxBody.FindElements(By.TagName("tr"));
                                IList<IWebElement> THTaxBody = TaxBody.FindElements(By.TagName("th"));
                                IList<IWebElement> TDTaxBody;
                                foreach (IWebElement row in TRTaxBody)
                                {
                                    TDTaxBody = row.FindElements(By.TagName("td"));
                                    if (TDTaxBody.Count != 0 && !row.Text.Contains("Taxing Body") && row.Text.Trim() != "")
                                    {
                                        string TaxBodydetails = TDTaxBody[0].Text + "~" + TDTaxBody[1].Text + "~" + TDTaxBody[2].Text + "~" + TDTaxBody[3].Text;
                                        gc.insert_date(orderNumber, parcelNumber, 1801, TaxBodydetails, 1, DateTime.Now);
                                    }
                                }
                            }
                            catch { }

                            // Tax Assessment Details

                            try
                            {
                                IWebElement TaxAssessment = driver.FindElement(By.XPath("//*[@id='tblPropertyCharacteristics']/tbody"));
                                IList<IWebElement> TRTaxAssessment = TaxAssessment.FindElements(By.TagName("tr"));
                                IList<IWebElement> THTaxAssessment = TaxAssessment.FindElements(By.TagName("th"));
                                IList<IWebElement> TDTaxAssessment;
                                foreach (IWebElement row in TRTaxAssessment)
                                {
                                    TDTaxAssessment = row.FindElements(By.TagName("td"));
                                    if (TDTaxAssessment.Count != 0 && row.Text.Trim() != "")
                                    {
                                        string TaxAssessmentdetails = TDTaxAssessment[0].Text + "~" + TDTaxAssessment[1].Text + "~" + TDTaxAssessment[2].Text;
                                        gc.insert_date(orderNumber, parcelNumber, 1802, TaxAssessmentdetails, 1, DateTime.Now);
                                    }
                                }
                            }
                            catch { }
                            try
                            {
                                driver.SwitchTo().Window(current);
                                Thread.Sleep(2000);
                            }
                            catch { }

                            string current1 = driver.CurrentWindowHandle;
                            try
                            {
                                IWebElement TaxBill = driver.FindElement(By.XPath("//*[@id='table56']/tbody"));
                                IList<IWebElement> TRTaxBill = TaxBill.FindElements(By.TagName("tr"));
                                IList<IWebElement> THTaxBill = TaxBill.FindElements(By.TagName("th"));
                                IList<IWebElement> TDTaxBill;
                                foreach (IWebElement row in TRTaxBill)
                                {
                                    TDTaxBill = row.FindElements(By.TagName("td"));
                                    if (TDTaxBill.Count != 0 && !row.Text.Contains("Amount Billed") && !row.Text.Contains("Billing History") && !row.Text.Contains("Original") && row.Text.Trim() != "" && TDTaxBill.Count == 5 && TDTaxBill[4].Text.Trim()==taxyear.ToString())
                                    {
                                        IWebElement IBill = TDTaxBill[4].FindElement(By.TagName("a"));
                                        IBill.Click();
                                        Thread.Sleep(2000);
                                        break;
                                    }
                                }
                            }
                            catch { }

                            driver.SwitchTo().Window(driver.WindowHandles.Last());
                            Thread.Sleep(2000);

                            string PropertyIndexNo = "", ProAddress = "", StrTaxYear = "";
                            PropertyIndexNo = driver.FindElement(By.Id("lblPin")).Text;
                            ProAddress = driver.FindElement(By.Id("lblPropertySitusAddress")).Text.Replace("\r\n", " ");
                            StrTaxYear = driver.FindElement(By.Id("lblTaxYear")).Text.Replace("Tax Year", "").Trim();
                            StrTaxYear = GlobalClass.After(StrTaxYear, "tax year").Replace(".","").Trim();
                            gc.CreatePdf(orderNumber, parcelNumber, "Treasurers Receipt" + TaxYear, driver, "IL", "Lake");

                            // Tax Payment Details
                            try
                            {
                                IWebElement Taxpayment = driver.FindElement(By.XPath("//*[@id='tblOnlineReceipt']/tbody"));
                                IList<IWebElement> TRTaxpayment = Taxpayment.FindElements(By.TagName("tr"));
                                IList<IWebElement> THTaxpayment = Taxpayment.FindElements(By.TagName("th"));
                                IList<IWebElement> TDTaxpayment;
                                foreach (IWebElement row in TRTaxpayment)
                                {
                                    TDTaxpayment = row.FindElements(By.TagName("td"));
                                    if (TDTaxpayment.Count != 0 && !row.Text.Contains("Advertising") && row.Text.Trim() != "")
                                    {
                                        string TaxLevydetails = ProAddress + "~" + StrTaxYear + "~" + TDTaxpayment[0].Text + "~" + TDTaxpayment[1].Text + "~" + TDTaxpayment[2].Text + "~" + TDTaxpayment[3].Text + "~" + TDTaxpayment[4].Text + "~" + TDTaxpayment[5].Text + "~" + TDTaxpayment[6].Text + "~" + TDTaxpayment[7].Text;
                                        gc.insert_date(orderNumber, parcelNumber, 1814, TaxLevydetails, 1, DateTime.Now);
                                    }
                                }
                            }
                            catch { }

                            driver.SwitchTo().Window(current1);
                            Thread.Sleep(2000);

                            taxyear--;

                        }

                        try
                        {
                            driver.FindElement(By.XPath("//*[@id='table56']/tbody/tr[18]/td/b/a")).Click();
                            Thread.Sleep(4000);
                            driver.SwitchTo().Window(driver.WindowHandles.Last());
                            Thread.Sleep(2000);
                            gc.CreatePdf(orderNumber, parcelNumber, "Tax sale or Redemption", driver, "IL", "Lake");

                        }
                        catch { }
                        // Tax Sale/Redemption Receipts
                        try
                        {
                            IWebElement Taxsale = driver.FindElement(By.Id("tblOnlineReceipt"));
                            IList<IWebElement> TRTaxsale = Taxsale.FindElements(By.TagName("tr"));
                            IList<IWebElement> THTaxsale = Taxsale.FindElements(By.TagName("th"));
                            IList<IWebElement> TDTaxsale;
                            foreach (IWebElement row in TRTaxsale)
                            {
                                TDTaxsale = row.FindElements(By.TagName("td"));
                                if (TDTaxsale.Count != 0 && !row.Text.Contains("Sale or Redemption") && row.Text.Trim() != "")
                                {
                                    string TaxSaledetails = TDTaxsale[0].Text + "~" + TDTaxsale[1].Text + "~" + TDTaxsale[2].Text + "~" + TDTaxsale[3].Text + "~" + TDTaxsale[4].Text + "~" + TDTaxsale[5].Text;
                                    gc.insert_date(orderNumber, parcelNumber, 1848, TaxSaledetails, 1, DateTime.Now);
                                }
                            }
                        }
                        catch { }



                        TaxTime = DateTime.Now.ToString("HH:mm:ss");

                    LastEndTime = DateTime.Now.ToString("HH:mm:ss");
                    gc.insert_TakenTime(orderNumber, "IL", "Lake", StartTime, AssessmentTime, TaxTime, CitytaxTime, LastEndTime);

                    driver.Quit();
                    gc.mergpdf(orderNumber, "IL", "Lake");
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