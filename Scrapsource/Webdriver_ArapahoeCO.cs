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

    public class Webdriver_ArapahoeCO
    {
        IWebDriver driver;
        DBconnection db = new DBconnection();
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["MyConnectionString"].ToString());
        GlobalClass gc = new GlobalClass();
        public string FTP_ArapahoeCO(string streetno, string direction, string streetname, string city, string streettype, string unitnumber, string ownernm, string parcelNumber, string searchType, string orderNumber, string directParcel)
        {
            GlobalClass.global_orderNo = orderNumber;
            HttpContext.Current.Session["orderNo"] = orderNumber;
            GlobalClass.global_parcelNo = parcelNumber;
            string StartTime = "", AssessmentTime = "", TaxTime = "", CitytaxTime = "", LastEndTime = "";
            string Parcel_number = "", Tax_Authority = "", Year = "", Taxresult1 = "", taxresult2 = "", taxresult3 = "", Firsthalf = "", Secondhalf = "", fullhalf = "", prepayresult1 = "", prepayresult2 = "", prepayresult3 = "", paiedamt = "", prepayment = "";
            var driverService = PhantomJSDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;

            using (driver = new PhantomJSDriver())
            {
                //rdp
                try
                {
                    StartTime = DateTime.Now.ToString("HH:mm:ss");
                    driver.Navigate().GoToUrl("https://parcelsearch.arapahoegov.com");
                    if (searchType == "titleflex")
                    {
                        string address = streetno + " " + direction + " " + streetname + " " + streettype + " " + unitnumber;
                        gc.TitleFlexSearch(orderNumber, "", ownernm, address.Trim(), "CO", "Arapahoe");
                        if (HttpContext.Current.Session["TitleFlex_Search"] != null && HttpContext.Current.Session["TitleFlex_Search"].ToString() == "Yes")
                        {
                            driver.Quit();
                            return "MultiParcel";
                           
                        }
                        parcelNumber = HttpContext.Current.Session["titleparcel"].ToString();
                        searchType = "parcel";
                    }
                    if (searchType == "address")
                    {
                        driver.FindElement(By.Id("txtAddressNumFrom")).SendKeys(streetno);
                        driver.FindElement(By.Id("txtAddressStreet")).SendKeys(streetname);
                        try
                        {
                            driver.FindElement(By.Id("txtAddressUnit")).SendKeys(unitnumber);
                        }
                        catch { }
                        driver.FindElement(By.Id("btnAddressSearch")).SendKeys(Keys.Enter);
                        Thread.Sleep(2000);
                        try
                        {
                            IWebElement addressmultitable = driver.FindElement(By.XPath("//*[@id='dataGrdList']/tbody"));
                            IList<IWebElement> addressmultirow = addressmultitable.FindElements(By.TagName("tr"));
                            IList<IWebElement> Addressid;
                            foreach (IWebElement Addressmulti in addressmultirow)
                            {
                                Addressid = Addressmulti.FindElements(By.TagName("td"));
                                if (Addressid.Count != 0 && !Addressmulti.Text.Contains("Type"))
                                {
                                    string AddressmultiresultAie = Addressid[1].Text;
                                    string Addressmultiresult = Addressid[2].Text + "~" + Addressid[3].Text;
                                    gc.insert_date(orderNumber, AddressmultiresultAie, 883, Addressmultiresult, 1, DateTime.Now);
                                }

                            }
                            if (addressmultirow.Count != 0 && addressmultirow.Count < 26)
                            {
                                HttpContext.Current.Session["multiParcel_Arapahoe"] = "Maximum";
                                gc.CreatePdf_WOP(orderNumber, "Multiple Parcel", driver, "CO", "Arapahoe");
                                driver.Quit();
                                return "MultiParcel";
                            }
                            if (addressmultirow.Count != 0 && addressmultirow.Count > 25)
                            {
                                HttpContext.Current.Session["multiParcel_Arapahoe_Multicount"] = "Maximum";
                                gc.CreatePdf_WOP(orderNumber, "MultyAddressSearch", driver, "CO", "Arapahoe");
                                driver.Quit();
                                return "Maximum";
                            }

                        }
                        catch { }
                    }
                    if (searchType == "parcel")
                    {
                        driver.FindElement(By.Id("txtPIN")).SendKeys(parcelNumber);
                        driver.FindElement(By.Id("btnParcelSearchPIN")).Click();
                        Thread.Sleep(2000);
                    }
                    if (searchType == "unitnumber")
                    {
                        driver.FindElement(By.Id("txtAIN")).SendKeys(unitnumber);
                        driver.FindElement(By.Id("btnParcelSearchAIN")).Click();
                        Thread.Sleep(2000);
                    }
                    IWebElement propertydetailtable = driver.FindElement(By.XPath("//*[@id='shadedTable']/tbody/tr/td/table/tbody/tr/td/table/tbody/tr/td/table[1]/tbody/tr[3]/td/table/tbody"));
                    Parcel_number = driver.FindElement(By.Id("ucParcelHeader_lblPinTxt")).Text;
                    string AIN = driver.FindElement(By.Id("ucParcelHeader_lblAinTxt")).Text;
                    string OwnerName = driver.FindElement(By.Id("ucParcelHeader_lblFullOwnerListTxt")).Text;
                    string Property_Address1 = driver.FindElement(By.Id("ucParcelHeader_lblSitusAddressTxt")).Text;
                    string Property_Address2 = driver.FindElement(By.Id("ucParcelHeader_lblSitusCityTxt")).Text;
                    string Property_Address = Property_Address1 + Property_Address2;
                    string MailingAddress1 = driver.FindElement(By.Id("ucParcelHeader_lblOwnerAddressTxt")).Text;
                    string mailingaddress2 = driver.FindElement(By.Id("ucParcelHeader_lblOwnerCSZTxt")).Text;
                    string mailingaddress = MailingAddress1 + mailingaddress2;
                    string legalDescription = driver.FindElement(By.Id("ucParcelHeader_lblLegalDescTxt")).Text;
                    try
                    {
                        IWebElement yearbuildtable = driver.FindElement(By.XPath("//*[@id='shadedTable']/tbody/tr/td/table/tbody/tr/td/table/tbody/tr/td/table[4]/tbody/tr/td/table/tbody"));
                        Year = gc.Between(yearbuildtable.Text, "Year Built", "Roof");
                    }
                    catch { }
                    string Landuse = driver.FindElement(By.Id("ucParcelHeader_lblLandUseTxt")).Text;
                    string MillLevy = driver.FindElement(By.XPath("//*[@id='ucParcelValue_lnkLevy']")).Text;
                    string propertyresult = AIN + "~" + OwnerName + "~" + Property_Address + "~" + mailingaddress + "~" + legalDescription + "~" + Year + "~" + Landuse + "~" + MillLevy;
                    gc.insert_date(orderNumber, Parcel_number, 852, propertyresult, 1, DateTime.Now);
                    gc.CreatePdf(orderNumber, Parcel_number, "Property Search Result", driver, "CO", "Arapahoe");
                    AssessmentTime = DateTime.Now.ToString("HH:mm:ss");
                    IWebElement Assessmenttable = driver.FindElement(By.XPath("//*[@id='shadedTable']/tbody/tr/td/table/tbody/tr/td/table/tbody/tr/td/table[2]/tbody"));
                    IList<IWebElement> assessmentrow = Assessmenttable.FindElements(By.TagName("tr"));
                    IList<IWebElement> Assessmentid;
                    foreach (IWebElement assessment in assessmentrow)
                    {
                        Assessmentid = assessment.FindElements(By.TagName("td"));
                        if (Assessmentid.Count != 0 && !assessment.Text.Contains("Total") && !assessment.Text.Contains("Mill Levy") && Assessmentid.Count != 1)
                        {
                            string assessmentresult = Assessmentid[0].Text + "~" + Assessmentid[1].Text + "~" + Assessmentid[2].Text + "~" + Assessmentid[3].Text;
                            gc.insert_date(orderNumber, Parcel_number, 854, assessmentresult, 1, DateTime.Now);
                        }
                        if (assessment.Text.Contains("Mill Levy"))
                        {
                            break;
                        }
                    }
                    driver.Navigate().GoToUrl("https://taxsearch.arapahoegov.com/");
                    driver.FindElement(By.Id("ContentPlaceHolder1_txtPIN")).SendKeys(Parcel_number);
                    gc.CreatePdf(orderNumber, Parcel_number, "Tax search before", driver, "CO", "Arapahoe");
                    driver.FindElement(By.Id("ContentPlaceHolder1_btnByPIN")).Click();
                    Thread.Sleep(2000);
                    TaxTime = DateTime.Now.ToString("HH:mm:ss");
                    string tax_owner = driver.FindElement(By.Id("ContentPlaceHolder1_lblOwner")).Text;
                    string tax_city = driver.FindElement(By.Id("ContentPlaceHolder1_lblCity")).Text;
                    string tax_aie = driver.FindElement(By.Id("ContentPlaceHolder1_lblAIN")).Text;
                    IWebElement payyear1 = driver.FindElement(By.Id("ContentPlaceHolder1_lblPayable"));
                    string payyear = gc.Between(payyear1.Text, "for", "Payable").Trim();
                    string fullpaymentdue = driver.FindElement(By.Id("ContentPlaceHolder1_lblDueFull")).Text;
                    string firsthalfpayment = driver.FindElement(By.Id("ContentPlaceHolder1_lblDue1st")).Text;
                    string sendhalfpayment = driver.FindElement(By.Id("ContentPlaceHolder1_lblDue2nd")).Text;
                    IWebElement taxauthorityrow = driver.FindElement(By.XPath("//*[@id='ContentPlaceHolder1_Table4']/tbody/tr[13]/td/p[3]"));
                    Tax_Authority = gc.Between(taxauthorityrow.Text, "Payments can be mailed to:", "If using").Trim();
                    IWebElement Tax_instaltable = driver.FindElement(By.XPath("//*[@id='ContentPlaceHolder1_Table4']/tbody"));
                    IList<IWebElement> Tax_instalrow = Tax_instaltable.FindElements(By.TagName("tr"));
                    IList<IWebElement> tax_instalid;
                    foreach (IWebElement tax_instal in Tax_instalrow)
                    {
                        tax_instalid = tax_instal.FindElements(By.TagName("td"));
                        if (tax_instalid.Count != 0 && tax_instal.Text.Contains("1st Half Amounts"))
                        {
                            Firsthalf = tax_instalid[1].Text;
                            Secondhalf = tax_instalid[2].Text;
                            fullhalf = tax_instalid[3].Text.Replace("/", " ");
                        }
                        if (tax_instalid.Count != 0 && tax_instal.Text.Contains("Assessed Tax:") || tax_instal.Text.Contains("Special Assessment:") || tax_instal.Text.Contains("Fees:") || tax_instal.Text.Contains("Interest:") || tax_instal.Text.Contains("Payments:") || tax_instal.Text.Contains("Total Due:"))
                        {
                            Taxresult1 += tax_instalid[1].Text.Trim() + "~";
                            taxresult2 += tax_instalid[2].Text.Trim() + "~";
                            taxresult3 += tax_instalid[3].Text.Trim() + "~";
                        }

                    }
                    string taxresult1 = payyear + "~" + tax_owner + "~" + tax_city + "~" + tax_aie + "~" + Firsthalf + "~" + Taxresult1.Remove(Taxresult1.Length - 1) + "~" + firsthalfpayment + "~" + Tax_Authority;
                    string Taxresult2 = payyear + "~" + tax_owner + "~" + tax_city + "~" + tax_aie + "~" + Secondhalf + "~" + taxresult2.Remove(taxresult2.Length - 1) + "~" + sendhalfpayment + "~" + Tax_Authority;
                    string Taxresult3 = payyear + "~" + tax_owner + "~" + tax_city + "~" + tax_aie + "~" + fullhalf + "~" + taxresult3.Remove(taxresult3.Length - 1) + "~" + fullpaymentdue + "~" + Tax_Authority;
                    gc.insert_date(orderNumber, Parcel_number, 856, taxresult1, 1, DateTime.Now);
                    gc.insert_date(orderNumber, Parcel_number, 856, Taxresult2, 1, DateTime.Now);
                    gc.insert_date(orderNumber, Parcel_number, 856, Taxresult3, 1, DateTime.Now);
                    gc.CreatePdf(orderNumber, Parcel_number, "Tax search After", driver, "CO", "Arapahoe");
                    //amg amount
                    string PriorYeardue = driver.FindElement(By.Id("ContentPlaceHolder1_lblPriorYear")).Text;
                    string Bankruptcy = driver.FindElement(By.Id("ContentPlaceHolder1_lblBankruptcy")).Text;
                    string Treasurer_assessment = driver.FindElement(By.Id("ContentPlaceHolder1_lblTreasAssess")).Text;
                    string TaxLiens = driver.FindElement(By.Id("ContentPlaceHolder1_lblTaxLien")).Text;
                    string Treasurer_deed = driver.FindElement(By.Id("ContentPlaceHolder1_lblTreasDeed")).Text;
                    string amgresult = PriorYeardue + "~" + Bankruptcy + "~" + Treasurer_assessment + "~" + TaxLiens + "~" + Treasurer_deed;
                    gc.insert_date(orderNumber, Parcel_number, 859, amgresult, 1, DateTime.Now);
                    //previous year
                    string current = driver.CurrentWindowHandle;
                    driver.FindElement(By.Id("ContentPlaceHolder1_aPrevYear")).Click();
                    Thread.Sleep(2000);
                    driver.SwitchTo().Window(driver.WindowHandles.Last());
                    gc.CreatePdf(orderNumber, Parcel_number, "previous year", driver, "CO", "Arapahoe");
                    try
                    {
                        IWebElement paymentyearweb = driver.FindElement(By.Id("ContentPlaceHolder1_lblPayable"));
                        prepayment = gc.Between(paymentyearweb.Text, "for", "Payable").Trim();
                    }
                    catch { }
                    IWebElement prepaytable = driver.FindElement(By.XPath("//*[@id='ContentPlaceHolder1_Table2']/tbody"));
                    IList<IWebElement> prepayrow = prepaytable.FindElements(By.TagName("tr"));
                    IList<IWebElement> prepayid;
                    foreach (IWebElement prepay in prepayrow)
                    {
                        prepayid = prepay.FindElements(By.TagName("td"));
                        if (prepayid.Count != 0 && prepay.Text.Contains("Original Amount"))
                        {
                            prepayresult1 = prepayid[1].Text;
                            paiedamt = prepayid[2].Text;
                        }
                        if (prepayid.Count != 0 && !prepay.Text.Contains("Original Amount") && !prepayid[1].Text.Contains(" "))
                        {
                            prepayresult2 += prepayid[1].Text + "~";
                            prepayresult3 += prepayid[2].Text + "~";
                        }

                    }
                    string preresultorg = prepayment + "~" + prepayresult1 + "~" + prepayresult2.Remove(prepayresult2.Length - 1);
                    string preresultpaied = prepayment + "~" + paiedamt + "~" + prepayresult3.Remove(prepayresult3.Length - 1);
                    gc.insert_date(orderNumber, Parcel_number, 869, preresultorg, 1, DateTime.Now);
                    gc.insert_date(orderNumber, Parcel_number, 869, preresultpaied, 1, DateTime.Now);

                    //driver.Quit();

                    driver.SwitchTo().Window(current);
                    try
                    {
                        var chromeOptions = new ChromeOptions();
                        var downloadDirectory = "F:\\AutoPdf\\";
                        chromeOptions.AddUserProfilePreference("download.default_directory", downloadDirectory);
                        chromeOptions.AddUserProfilePreference("download.prompt_for_download", false);
                        chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");
                        chromeOptions.AddUserProfilePreference("plugins.always_open_pdf_externally", true);
                        var driver1 = new ChromeDriver(chromeOptions);
                        string fileName = "";
                        driver1.Navigate().GoToUrl(driver.Url);
                        IWebElement printable = driver1.FindElement(By.Id("ContentPlaceHolder1_aTaxNotice"));
                        fileName = AIN + ".pdf";
                        //  fileName = "abc";
                        IJavaScriptExecutor js1 = driver1 as IJavaScriptExecutor;
                        js1.ExecuteScript("arguments[0].click();", printable);
                        Thread.Sleep(4000);
                        driver1.SwitchTo().Window(driver1.WindowHandles.Last());
                        fileName = AIN + ".pdf";
                        Thread.Sleep(2000);
                        //2073-10-2-05-012
                        gc.AutoDownloadFile(orderNumber, Parcel_number, "Arapahoe", "CO", fileName);
                        driver1.Quit();
                    }
                    catch (Exception ex)
                    {

                    }

                    TaxTime = DateTime.Now.ToString("HH:mm:ss");
                    LastEndTime = DateTime.Now.ToString("HH:mm:ss");
                    gc.insert_TakenTime(orderNumber, "CO", "Arapahoe", StartTime, AssessmentTime, TaxTime, CitytaxTime, LastEndTime);
                    driver.Quit();
                    gc.mergpdf(orderNumber, "CO", "Arapahoe");
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