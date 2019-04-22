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
    public class Webdriver_WeberUT
    {
        IWebDriver driver;
        DBconnection db = new DBconnection();
        GlobalClass gc = new GlobalClass();
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["MyConnectionString"].ToString());
        public string FTP_WeberUT(string address, string parcelNumber, string searchType, string ownername, string orderNumber, string directParcel)
        {
            GlobalClass.global_orderNo = orderNumber;
            HttpContext.Current.Session["orderNo"] = orderNumber;
            GlobalClass.global_parcelNo = parcelNumber;
            string Parcel_number = "", Tax_Authority = "", yearbuild = "", Nodeliquent = "", Addresshrf = "", TaxesDue = "", Multiaddressadd = "", MailingAddress = "";
            string StartTime = "", AssessmentTime = "", TaxTime = "", CitytaxTime = "", LastEndTime = "";
            var driverService = PhantomJSDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            using (driver = new PhantomJSDriver())
            {
                try
                {
                    driver.Navigate().GoToUrl("http://www.webercountyutah.gov/Treasurer/contact.php");
                    string taxauthority = driver.FindElement(By.XPath("//*[@id='main']/div/div[2]/a[2]")).Text;
                    string office = driver.FindElement(By.XPath("//*[@id='main']/div/div[2]/text()")).Text;
                    Tax_Authority = taxauthority + " " + office;
                }
                catch { }
                try
                {
                    driver.Navigate().GoToUrl("http://www3.co.weber.ut.us/psearch/index.php");

                    if (searchType == "address")
                    {
                        driver.FindElement(By.Id("address")).SendKeys(address);
                        driver.FindElement(By.XPath("//*[@id='form3']/p/input[3]")).Click();
                        Thread.Sleep(2000);

                        int Max = 0;
                        gc.CreatePdf_WOP(orderNumber, "Address After", driver, "UT", "Weber");
                        IWebElement Addresstable = driver.FindElement(By.XPath("/html/body/table/tbody/tr[6]/td/table/tbody"));
                        IList<IWebElement> Addresrow = Addresstable.FindElements(By.TagName("tr"));
                        IList<IWebElement> AddressTD;
                        foreach (IWebElement AddressT in Addresrow)
                        {
                            AddressTD = AddressT.FindElements(By.TagName("td"));
                            if (AddressTD.Count > 1 && AddressTD[2].Text.Contains(address.ToUpper()))
                            {
                                IWebElement Parcellink = AddressTD[0].FindElement(By.TagName("a"));
                                Addresshrf = Parcellink.GetAttribute("href");
                                string parcelno = AddressTD[0].Text;
                                string OwnerName = AddressTD[1].Text;
                                string Address = AddressTD[2].Text;
                                Max++;
                            }
                        }
                        if (Max == 1)
                        {
                            driver.Navigate().GoToUrl(Addresshrf);
                            Thread.Sleep(2000);
                        }
                    }

                    Parcel_number = driver.FindElement(By.XPath("/html/body/table/tbody/tr[5]/td/table/tbody/tr[3]/td/table/tbody/tr[4]/td[1]")).Text;
                    string market = driver.FindElement(By.XPath("/html/body/table/tbody/tr[5]/td/table/tbody/tr[5]/td/table/tbody/tr/td[2]/table/tbody/tr/td/table/tbody/tr[2]/td[1]")).Text;
                    string tax = driver.FindElement(By.XPath("/html/body/table/tbody/tr[5]/td/table/tbody/tr[5]/td/table/tbody/tr/td[2]/table/tbody/tr/td/table/tbody/tr[2]/td[2]")).Text;
                    string rate = driver.FindElement(By.XPath("/html/body/table/tbody/tr[5]/td/table/tbody/tr[5]/td/table/tbody/tr/td[2]/table/tbody/tr/td/table/tbody/tr[2]/td[4]")).Text;
                    string TaxArea1 = driver.FindElement(By.XPath("/html/body/table/tbody/tr[5]/td/table/tbody/tr[3]/td/table/tbody/tr[4]/td[2]")).Text;
                    string taxarea = GlobalClass.After(TaxArea1, "Tax Area:").Trim();
                    string Netassessment = driver.FindElement(By.XPath("/html/body/table/tbody/tr[5]/td/table/tbody/tr[5]/td/table/tbody/tr/td[2]/table/tbody/tr/td/table/tbody/tr[4]/td[3]")).Text;
                    string totaldirect = driver.FindElement(By.XPath("/html/body/table/tbody/tr[5]/td/table/tbody/tr[5]/td/table/tbody/tr/td[2]/table/tbody/tr/td/table/tbody/tr[5]/td[3]")).Text;
                    string PenaltyCharge = driver.FindElement(By.XPath("/html/body/table/tbody/tr[5]/td/table/tbody/tr[5]/td/table/tbody/tr/td[2]/table/tbody/tr/td/table/tbody/tr[6]/td[3]")).Text;
                    string subtotal = driver.FindElement(By.XPath("/html/body/table/tbody/tr[5]/td/table/tbody/tr[5]/td/table/tbody/tr/td[2]/table/tbody/tr/td/table/tbody/tr[7]/td[3]")).Text;
                    string Totalpayment = driver.FindElement(By.XPath("/html/body/table/tbody/tr[5]/td/table/tbody/tr[5]/td/table/tbody/tr/td[2]/table/tbody/tr/td/table/tbody/tr[9]/td[3]")).Text;
                    string Balance = driver.FindElement(By.XPath("/html/body/table/tbody/tr[5]/td/table/tbody/tr[5]/td/table/tbody/tr/td[2]/table/tbody/tr/td/table/tbody/tr[10]/td[3]")).Text;
                    string type = driver.FindElement(By.XPath("/html/body/table/tbody/tr[5]/td/table/tbody/tr[10]/td")).Text;
                    string Discription = driver.FindElement(By.XPath("/html/body/table/tbody/tr[5]/td/table/tbody/tr[10]/td")).Text;
                    string amount = driver.FindElement(By.XPath("/html/body/table/tbody/tr[5]/td/table/tbody/tr[10]/td")).Text;
                    string Status = driver.FindElement(By.XPath("/html/body/table/tbody/tr[5]/td/table/tbody/tr[10]/td")).Text;
                    string loancompany1 = driver.FindElement(By.XPath("/html/body/table/tbody/tr[5]/td/table/tbody/tr[5]/td/table/tbody/tr/td[1]/p")).Text;
                    string Loancompany = GlobalClass.After(loancompany1, "Loan Company:");
                    gc.CreatePdf(orderNumber, Parcel_number, "Property" , driver, "UT", "Weber");
                    driver.FindElement(By.LinkText("Ownership Info")).Click();
                    Thread.Sleep(2000);
                    gc.CreatePdf(orderNumber, Parcel_number, "owner Name", driver, "UT", "Weber");
                    string owner = driver.FindElement(By.XPath("/html/body/table/tbody/tr[5]/td/table/tbody/tr[6]/td[1]/table/tbody/tr[1]/td[2]")).Text;
                    string proaddress = driver.FindElement(By.XPath("/html/body/table/tbody/tr[5]/td/table/tbody/tr[6]/td[1]/table/tbody/tr[3]/td[2]")).Text;
                    string Mailingaddress = driver.FindElement(By.XPath("/html/body/table/tbody/tr[5]/td/table/tbody/tr[6]/td[1]/table/tbody/tr[5]/td[2]")).Text;
                    string Taxunit = driver.FindElement(By.XPath("/html/body/table/tbody/tr[5]/td/table/tbody/tr[6]/td[1]/table/tbody/tr[7]/td[2]")).Text;
                    string priorParcel = driver.FindElement(By.XPath("/html/body/table/tbody/tr[5]/td/table/tbody/tr[9]/td/table/tbody/tr[7]/td")).Text;
                    string Legal = driver.FindElement(By.XPath("/html/body/table/tbody/tr[5]/td/table/tbody/tr[9]/td/table/tbody/tr[9]/td/table/tbody/tr")).Text;
                    driver.FindElement(By.XPath("/html/body/table/tbody/tr[4]/td/div/table/tbody/tr/td[4]/div/a")).Click();
                    Thread.Sleep(2000);
                    gc.CreatePdf(orderNumber, Parcel_number, "Property Characteristics", driver, "UT", "Weber");
                    string Propertytype = driver.FindElement(By.XPath("/html/body/table/tbody/tr[6]/td/table/tbody/tr[1]/td[2]")).Text;
                    string yearbuilt = driver.FindElement(By.XPath("/html/body/table/tbody/tr[6]/td/table/tbody/tr[12]/td[2]")).Text;
                    string lotsize = driver.FindElement(By.XPath("/html/body/table/tbody/tr[6]/td/table/tbody/tr[13]/td[2]")).Text;
                    string Propertydetail = owner + "~" + proaddress + "~" + Mailingaddress + "~" + Taxunit + "~" + priorParcel + "~" + Legal + "~" + Propertytype + "~" + yearbuilt + "~" + lotsize;
                    gc.insert_date(orderNumber, Parcel_number, 1841, Propertydetail, 1, DateTime.Now);
                    string Assessment = taxarea + "~" + Loancompany + "~" + market + "~" + tax + "~" + rate + "~" + Netassessment + "~" + totaldirect + "~" + PenaltyCharge + "~" + subtotal + "~" + Totalpayment + "~" + Balance + "~" + type + "~" + Discription + "~" + amount + "~" + Status + "~" + Tax_Authority;
                    gc.insert_date(orderNumber, Parcel_number, 1842, Assessment, 1, DateTime.Now);
                    //Tax History
                    driver.FindElement(By.LinkText("Tax History")).Click();
                    Thread.Sleep(2000);
                    driver.FindElement(By.XPath("/html/body/table/tbody/tr[7]/td/table/tbody/tr[2]/td[1]/table/tbody/tr[2]/td[1]/a")).Click();
                    Thread.Sleep(2000);
                   
                    IWebElement Propertytable = driver.FindElement(By.XPath("//*[@id='charge_2019']/table/tbody"));
                    IList<IWebElement> Propertyrow = Propertytable.FindElements(By.TagName("tr"));
                    IList<IWebElement> Propertyid;
                    foreach (IWebElement Property in Propertyrow)
                    {
                        Propertyid = Property.FindElements(By.TagName("td"));
                        if (Propertyid.Count > 1 && Property.Text.Trim() != "")
                        {
                            string Propertyresult = Propertyid[1].Text + "~" + Propertyid[2].Text;
                            gc.insert_date(orderNumber, Parcel_number, 1843, Propertyresult, 1, DateTime.Now);
                        }
                    }
                    driver.FindElement(By.XPath("/html/body/table/tbody/tr[9]/td/table[1]/tbody/tr/td/div/a[2]")).Click();
                    Thread.Sleep(2000);
                    IWebElement Propertyvaluestable = driver.FindElement(By.XPath("/html/body/table/tbody/tr[9]/td/table[2]/tbody"));
                    IList<IWebElement> Propertyvaluesrow = Propertyvaluestable.FindElements(By.TagName("tr"));
                    IList<IWebElement> Propertyvaluesid;
                    foreach (IWebElement Propertyvalues in Propertyvaluesrow)
                    {
                        Propertyvaluesid = Propertyvalues.FindElements(By.TagName("td"));
                        if (Propertyvaluesid.Count > 1 && !Propertyvalues.Text.Contains("Year") && Propertyvaluesid[0].Text.Trim() != "")
                        {
                            string Propertyvaluesresult = Propertyvaluesid[0].Text + "~" + Propertyvaluesid[1].Text + "~" + Propertyvaluesid[2].Text + "~" + Propertyvaluesid[3].Text;
                            gc.insert_date(orderNumber, Parcel_number, 1844, Propertyvaluesresult, 1, DateTime.Now);
                        }
                    }
                    // Taxing Unit Areas
                    driver.FindElement(By.XPath("/html/body/table/tbody/tr[10]/td/table/tbody/tr[1]/td[1]/div/a[1]")).Click();
                    Thread.Sleep(2000);
                    IWebElement TaxingUnittable = driver.FindElement(By.XPath("/html/body/table/tbody/tr[10]/td/table/tbody/tr[2]/td[1]/table/tbody"));
                    IList<IWebElement> TaxingUnitrow = TaxingUnittable.FindElements(By.TagName("tr"));
                    IList<IWebElement> TaxingUnitid;
                    foreach (IWebElement TaxingUnit in TaxingUnitrow)
                    {
                        TaxingUnitid = TaxingUnit.FindElements(By.TagName("td"));
                        if (TaxingUnitid.Count > 1 && !TaxingUnit.Text.Contains("Year") && TaxingUnitid[0].Text.Trim() != "")
                        {
                            string TaxingUnitresult = TaxingUnitid[0].Text + "~" + TaxingUnitid[1].Text + "~" + TaxingUnitid[2].Text;
                            gc.insert_date(orderNumber, Parcel_number, 1845, TaxingUnitresult, 1, DateTime.Now);
                        }
                    }
                    //Payment
                    driver.FindElement(By.XPath("/html/body/table/tbody/tr[10]/td/table/tbody/tr[4]/td[1]/a[1]")).Click();
                    Thread.Sleep(2000);
                    IWebElement Paymenttable = driver.FindElement(By.XPath("/html/body/table/tbody/tr[10]/td/table/tbody/tr[4]/td[1]/table/tbody"));
                    IList<IWebElement> Paymentrow = Paymenttable.FindElements(By.TagName("tr"));
                    IList<IWebElement> Paymentid;
                    foreach (IWebElement Payment in Paymentrow)
                    {
                        Paymentid = Payment.FindElements(By.TagName("td"));
                        if (Paymentid.Count == 3 && !Payment.Text.Contains("Pay Date"))
                        {
                            string Paymentresult = Paymentid[0].Text + "~" + "" + "~" + Paymentid[1].Text + "~" + Paymentid[2].Text;
                            gc.insert_date(orderNumber, Parcel_number, 1846, Paymentresult, 1, DateTime.Now);
                        }
                        if (Paymentid.Count == 4 && !Payment.Text.Contains("Pay Date"))
                        {
                            string Paymentresult = Paymentid[0].Text + "~" + Paymentid[1].Text + "~" + Paymentid[2].Text + "~" + Paymentid[3].Text;
                            gc.insert_date(orderNumber, Parcel_number, 1846, Paymentresult, 1, DateTime.Now);
                        }
                    }
                    gc.CreatePdf(orderNumber, Parcel_number, "Tax History", driver, "UT", "Weber");
                    //Delinquent Tax
                    int D = 0;
                    driver.FindElement(By.XPath("/html/body/table/tbody/tr[4]/td/div/table/tbody/tr/td[5]/div/a")).Click();
                    Thread.Sleep(2000);
                    gc.CreatePdf(orderNumber, Parcel_number, "Delinquent Tax", driver, "UT", "Weber");
                    string currentwindow = driver.CurrentWindowHandle;
                    try
                    {
                        Nodeliquent = driver.FindElement(By.XPath("/html/body/table/tbody/tr[6]/td/center")).Text;
                    }
                    catch { }
                    if (Nodeliquent.Contains("No Record of Past"))
                    {
                        string Delinquentresult = "" + "~" + "" + "~" + Nodeliquent + "~" + "";
                    }
                    else
                    {
                        IWebElement Delinquenttable = driver.FindElement(By.XPath("/html/body/table[3]/tbody/tr/td[3]/table/tbody"));
                        IList<IWebElement> Delinquentrow = Delinquenttable.FindElements(By.TagName("tr"));
                        IList<IWebElement> Delinquentid;
                        foreach (IWebElement Delinquent in Delinquentrow)
                        {
                            Delinquentid = Delinquent.FindElements(By.TagName("td"));
                            if (Delinquentid.Count == 3 && Delinquent.Text.Contains("Pay Date"))
                            {
                                string Delinquentresult = Delinquentid[0].Text + "~" + "" + "~" + Delinquentid[2].Text + "~" + Delinquentid[3].Text;
                                gc.insert_date(orderNumber, Parcel_number, 1847, Delinquentresult, 1, DateTime.Now);
                            }
                            if (D < 3 && Delinquent.Text.Contains("Pay Date"))
                            {
                                Delinquentid[0].Click();
                                Thread.Sleep(2000);
                                driver.SwitchTo().Window(driver.WindowHandles.Last());
                                gc.CreatePdf(orderNumber, Parcel_number, "Delinquent" + D, driver, "UT", "Weber");
                                driver.SwitchTo().Window(currentwindow);
                                D++;
                            }
                        }
                    }
                    driver.Quit();

                    gc.mergpdf(orderNumber, "UT", "Weber");
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