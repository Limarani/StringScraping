﻿using System;
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
using OpenQA.Selenium.Edge;

namespace ScrapMaricopa.Scrapsource
{
    public class Webdriver_CASantamenta
    {
        
        string outputPath = "";
        IWebDriver driver;
        DBconnection db = new DBconnection();

        GlobalClass gc = new GlobalClass();
        MySqlParameter[] mParam;
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["MyConnectionString"].ToString());
        public string FTP_Santamenta(string Address, string account, string parcelNumber, string ownername, string searchType, string orderno, string directParcel)
        {
          
            //parcelNumber=GlobalClass.global_parcelNo;
            var driverService = PhantomJSDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            using (driver = new PhantomJSDriver())
            {
                driver.Manage().Window.Size = new Size(1920, 1080);

                if (ownername.Trim() != "" || Address.Trim() != "")
                {
                    searchType = "titleflex";

                }
                string[] stringSeparators1 = new string[] { "\r\n" };
                List<string> listurl = new List<string>();
                List<string> Columnurl = new List<string>();
                string Date = "";
                Date = DateTime.Now.ToString("M/d/yyyy");
                try
                {
                    if (searchType == "titleflex")
                    {
                        gc.TitleFlexSearch(orderno, parcelNumber, ownername, Address, "CA", "Sacramento");

                        parcelNumber = GlobalClass.global_parcelNo;
                        if (GlobalClass.TitleFlex_Search == "Yes")
                        {
                            driver.Quit();
                            return "MultiParcel";
                        }
                        searchType = "parcel";
                    }
                    //if (searchType == "address")
                    //{
                    //    driver.Navigate().GoToUrl("http://assessorparcelviewer.saccounty.net/jsviewer/assessor.html?apn={0}");
                    //    Thread.Sleep(2000);
                    //    driver.FindElement(By.XPath("//*[@id='omniInput']")).SendKeys(Address);

                    //    Thread.Sleep(2000);
                    //    //gc.CreatePdf_WOP(orderNumber, "Address search", driver, "FL", "Miami Dade");

                    //    driver.FindElement(By.XPath("//*[@id='goOmniSearch']")).SendKeys(Keys.Enter);
                    //    Thread.Sleep(3000);
                    //   // gc.CreatePdf_WOP(orderNumber, "Address search Result", driver, "FL", "Miami Dade");
                    //    Thread.Sleep(1000);

                    //    try
                    //    {
                    //        string multi = driver.FindElement(By.XPath("//*[@id='SearchDescription']/span")).Text;
                    //        if (multi.Trim()!="")
                    //        {
                    //            IWebElement MuliparcTB = driver.FindElement(By.XPath("//*[@id='datatable']"));
                    //            IList<IWebElement> MuliparcTR = MuliparcTB.FindElements(By.TagName("tr"));


                    //        }
                    //    }
                    //    catch { }


                    //}
                    if (searchType == "parcel")
                    {

                        driver.Navigate().GoToUrl("http://assessorparcelviewer.saccounty.net/jsviewer/assessor.html?apn={0}");
                        Thread.Sleep(2000);
                        driver.FindElement(By.XPath("//*[@id='omniInput']")).SendKeys(parcelNumber);
                        gc.CreatePdf_WOP(orderno, "Address search", driver, "CA", "Sacramento");
                        Thread.Sleep(2000);



                        //IWebElement webElement = driver.FindElement(By.XPath("//*[@id='goOmniSearch']/i"));
                        //Actions builder = new Actions(driver);
                        //builder.MoveToElement(webElement).Click(webElement);
                        //builder.Perform();
                        driver.FindElement(By.XPath("//*[@id='ui-id-1']")).Click();
                        Thread.Sleep(3000);

                        //IWebElement AddreTB = driver.FindElement(By.XPath("//*[@id='SearchInputPanel']/div/div/span"));
                        //IList<IWebElement> AddreTR = AddreTB.FindElements(By.TagName("button"));
                        //foreach (IWebElement row1 in AddreTR)
                        //{

                        //        row1.SendKeys(Keys.Enter);
                        //    Thread.Sleep(10000);

                        //}


                        gc.CreatePdf_WOP(orderno, "Address search res", driver, "CA", "Sacramento");


                    }



                    // assessment details
                    string PropertyAddress = "", City_Zip = "", Jurisdiction = "", CountySupervisorDistrict = "", TaxRateAreaCode = "", ApproxParcelArea = "";


                    //  gc.CreatePdf(orderno, parcelNumber, "Assessment Details", driver, "CA", "El Dorado");
                    parcelNumber = driver.FindElement(By.XPath("//*[@id='ParcelAPNNoDashes']")).Text;
                    PropertyAddress = driver.FindElement(By.XPath("//*[@id='Address']")).Text;
                    City_Zip = driver.FindElement(By.XPath("//*[@id='PropertyInformationTable']/tbody/tr[3]/td[2]")).Text;
                    Jurisdiction = driver.FindElement(By.XPath("//*[@id='Jurisdiction']")).Text;
                    TaxRateAreaCode = driver.FindElement(By.XPath("//*[@id='TaxRateAreaCodeLink']")).Text;
                    CountySupervisorDistrict = driver.FindElement(By.XPath("//*[@id='SupervisorDistrictLink']")).Text;

                    driver.FindElement(By.XPath("//*[@id='AssessorPortlet']/h3")).Click();
                    //gc.CreatePdf(orderno, parcelNumber, "Input Passed Tax Search", driver, "CA", "El Dorado");
                    Thread.Sleep(1000);

                    ApproxParcelArea = driver.FindElement(By.XPath("//*[@id='LotSize']")).Text;

                    string ProperTyDetail = PropertyAddress + "~" + City_Zip + "~" + Jurisdiction + "~" + TaxRateAreaCode + "~" + CountySupervisorDistrict + "~" + ApproxParcelArea;

                    gc.insert_date(orderno, parcelNumber, 368, ProperTyDetail, 1, DateTime.Now);

                    string TaxRollYear = "", LandValue = "", ImprovementValue = "", PersonalPropertyValue = "", Fixtures = "", HomeownerExemption = "", OtherExemption = "", NetAssessedValue = "";


                    TaxRollYear = driver.FindElement(By.XPath("//*[@id='RollYear']")).Text;
                    LandValue = driver.FindElement(By.XPath("//*[@id='ValueLand']")).Text;
                    ImprovementValue = driver.FindElement(By.XPath("//*[@id='ValueStructure']")).Text;
                    PersonalPropertyValue = driver.FindElement(By.XPath("//*[@id='ValuePersonalProperty']")).Text;
                    Fixtures = driver.FindElement(By.XPath("//*[@id='ValueFixtures']")).Text;
                    HomeownerExemption = driver.FindElement(By.XPath("//*[@id='ValueHomeOwnersException']")).Text;
                    OtherExemption = driver.FindElement(By.XPath("//*[@id='ValueOtherExemption']")).Text;
                    NetAssessedValue = driver.FindElement(By.XPath("//*[@id='ValueTotal']")).Text;


                    string Assessment = TaxRollYear + "~" + LandValue + "~" + ImprovementValue + "~" + PersonalPropertyValue + "~" + Fixtures + "~" + HomeownerExemption + "~" + OtherExemption + "~" + NetAssessedValue;
                    gc.insert_date(orderno, parcelNumber, 369, Assessment, 1, DateTime.Now);

                    driver.Navigate().GoToUrl("https://eproptax.saccounty.net/");
                    Thread.Sleep(2000);

                    string Pa1 = "", Pa2 = "", Pa3 = "", Pa4 = "";

                    Pa1 = parcelNumber.Substring(0, 3);
                    Pa2 = parcelNumber.Substring(3, 4);
                    Pa3 = parcelNumber.Substring(7, 3);
                    Pa4 = parcelNumber.Substring(10, 4);


                    driver.FindElement(By.XPath("//*[@id='parcel1']")).SendKeys(Pa1);
                    driver.FindElement(By.XPath("//*[@id='parcel2']")).SendKeys(Pa2);
                    driver.FindElement(By.XPath("//*[@id='parcel3']")).SendKeys(Pa3);
                    driver.FindElement(By.XPath("//*[@id='parcel4']")).SendKeys(Pa4);
                    Thread.Sleep(1000);
                    driver.FindElement(By.XPath("//*[@id='btnSubmit']")).Click();
                    //gc.CreatePdf(orderno, parcelNumber, "Input Passed Tax Search", driver, "CA", "El Dorado");
                    Thread.Sleep(3000);
                    int I = 0;
                    IWebElement CurrentTaxHistoryTB1 = driver.FindElement(By.XPath("//*[@id='billDetailGrid']/table/tbody[1]"));
                    IList<IWebElement> CurrentTaxHistoryTR1 = CurrentTaxHistoryTB1.FindElements(By.TagName("tr"));
                    IList<IWebElement> CurrentTaxHistoryTD1;
                    string BillNumber = "", Billtype = "", DirectLevyPortion = "", OriginalbillAmount = "";
                    try
                    {
                        CurrentTaxHistoryTB1 = driver.FindElement(By.XPath("//*[@id='billDetailGrid']/table/tbody[1]"));
                        CurrentTaxHistoryTR1 = CurrentTaxHistoryTB1.FindElements(By.TagName("tr"));



                        foreach (IWebElement row1 in CurrentTaxHistoryTR1)
                        {

                            CurrentTaxHistoryTD1 = row1.FindElements(By.TagName("td"));
                            if (CurrentTaxHistoryTD1.Count != 0)
                            {
                                BillNumber = CurrentTaxHistoryTD1[0].Text;
                                Billtype = CurrentTaxHistoryTD1[1].Text;
                                DirectLevyPortion = CurrentTaxHistoryTD1[2].Text;
                                OriginalbillAmount = CurrentTaxHistoryTD1[3].Text;
                                if (DirectLevyPortion.Trim() != "")
                                {
                                    CurrentTaxHistoryTD1[I].Click();
                                    Thread.Sleep(2000);
                                    string SecuredAnnual = "", FirstInstallment = "", SecondInstallment = "", TaxRate = "";


                                    IWebElement CurrentTaxHistoryTB = driver.FindElement(By.XPath("//*[@id='billDetailGrid']/table/tbody"));
                                    IList<IWebElement> CurrentTaxHistoryTR = CurrentTaxHistoryTB.FindElements(By.TagName("tr"));
                                    IList<IWebElement> CurrentTaxHistoryTD;

                                    foreach (IWebElement row in CurrentTaxHistoryTR)
                                    {
                                        CurrentTaxHistoryTD = row.FindElements(By.TagName("td"));
                                        if (CurrentTaxHistoryTD.Count != 0)
                                        {
                                            TaxRate = driver.FindElement(By.XPath("//*[@id='taxRateGlobal']/a/span")).Text;
                                            SecuredAnnual = CurrentTaxHistoryTD[0].Text;
                                            FirstInstallment = CurrentTaxHistoryTD[1].Text;
                                            SecondInstallment = CurrentTaxHistoryTD[2].Text;

                                            string Deliquent = Billtype + "~" + TaxRate + "~" + SecuredAnnual + "~" + FirstInstallment + "~" + SecondInstallment;

                                            gc.insert_date(orderno, parcelNumber, 371, Deliquent, 1, DateTime.Now);

                                        }
                                    }
                                }
                                // CurrentTaxHistoryTR1.Clear();
                                // I++;
                                driver.FindElement(By.XPath("//*[@id='navIcons']/li[3]/a")).Click();
                                Thread.Sleep(2000);

                            }

                        }
                    }
                    catch { }
                    driver.FindElement(By.XPath("//*[@id='navIcons']/li[3]/a")).Click();
                    Thread.Sleep(2000);
                    try
                    {
                        CurrentTaxHistoryTB1 = driver.FindElement(By.XPath("//*[@id='billDetailGrid']/table/tbody[1]"));
                        CurrentTaxHistoryTR1 = CurrentTaxHistoryTB1.FindElements(By.TagName("tr"));



                        foreach (IWebElement row1 in CurrentTaxHistoryTR1)
                        {

                            CurrentTaxHistoryTD1 = row1.FindElements(By.TagName("td"));
                            if (CurrentTaxHistoryTD1.Count != 0)
                            {
                                BillNumber = CurrentTaxHistoryTD1[0].Text;
                                Billtype = CurrentTaxHistoryTD1[1].Text;
                                DirectLevyPortion = CurrentTaxHistoryTD1[2].Text;
                                OriginalbillAmount = CurrentTaxHistoryTD1[3].Text;
                                if (DirectLevyPortion.Trim() != "")
                                {
                                    IWebElement a = CurrentTaxHistoryTD1[2].FindElement(By.TagName("a"));
                                    a.Click();

                                    Thread.Sleep(2000);
                                    IWebElement CurrentPayHistoryTB = driver.FindElement(By.XPath("//*[@id='billDetailGrid']/table/tbody"));
                                    //*[@id="billDetailGrid"]/table/tbody
                                    IList<IWebElement> CurrentPayHistoryTR = CurrentPayHistoryTB.FindElements(By.TagName("tr"));
                                    IList<IWebElement> CurrentPayHistoryTD;
                                    string DirectLevyNumber = "", LevyName = "", LevyAmount = "", Bond = "";
                                    foreach (IWebElement row2 in CurrentPayHistoryTR)
                                    {
                                        CurrentPayHistoryTD = row2.FindElements(By.TagName("td"));
                                        if (CurrentPayHistoryTD.Count != 0)
                                        {

                                            // string TaxRate1 = driver.FindElement(By.XPath("//*[@id='taxRateGlobal']/a/span")).Text;
                                            DirectLevyNumber = CurrentPayHistoryTD[0].Text;
                                            Bond = CurrentPayHistoryTD[1].Text;
                                            LevyName = CurrentPayHistoryTD[2].Text;
                                            LevyAmount = CurrentPayHistoryTD[3].Text;


                                            string taxlevy = DirectLevyNumber + "~" + Bond + "~" + LevyName + "~" + LevyAmount;

                                            gc.insert_date(orderno, parcelNumber, 372, taxlevy, 1, DateTime.Now);

                                        }
                                    }
                                }



                            }

                        }
                    }
                    catch { }

                    driver.FindElement(By.XPath("//*[@id='navIcons']/li[3]/a")).Click();
                    Thread.Sleep(2000);
                    CurrentTaxHistoryTB1 = driver.FindElement(By.XPath("//*[@id='billDetailGrid']/table/tbody[1]"));
                    CurrentTaxHistoryTR1 = CurrentTaxHistoryTB1.FindElements(By.TagName("tr"));

                    foreach (IWebElement row1 in CurrentTaxHistoryTR1)
                    {

                        CurrentTaxHistoryTD1 = row1.FindElements(By.TagName("td"));
                        if (CurrentTaxHistoryTD1.Count != 0)
                        {
                            BillNumber = CurrentTaxHistoryTD1[0].Text;
                            Billtype = CurrentTaxHistoryTD1[1].Text;
                            DirectLevyPortion = CurrentTaxHistoryTD1[2].Text;
                            OriginalbillAmount = CurrentTaxHistoryTD1[3].Text;


                            string Tax = BillNumber + "~" + Billtype + "~" + DirectLevyPortion + "~" + OriginalbillAmount;
                            gc.insert_date(orderno, parcelNumber, 370, Tax, 1, DateTime.Now);



                        }

                    }
                    driver.FindElement(By.XPath("//*[@id='navIcons']/li[4]/a")).Click();
                    Thread.Sleep(3000);

                    string BillYear = "", TaxBillNumber = "", BillType = "", PayByDate = "", Amount = "", PaidDate = "";


                    IWebElement PayHistoryTB = driver.FindElement(By.XPath("//*[@id='billDetailGrid']/table/tbody"));
                    IList<IWebElement> PayHistoryTR = PayHistoryTB.FindElements(By.TagName("tr"));
                    IList<IWebElement> PayHistoryTD;
                    foreach (IWebElement row2 in PayHistoryTR)
                    {
                        PayHistoryTD = row2.FindElements(By.TagName("td"));
                        if (PayHistoryTD.Count != 0)
                        {
                            BillYear = PayHistoryTD[0].Text;
                            TaxBillNumber = PayHistoryTD[1].Text;
                            BillType = PayHistoryTD[2].Text;
                            PayByDate = PayHistoryTD[3].Text;
                            Amount = PayHistoryTD[4].Text;
                            PaidDate = PayHistoryTD[5].Text;



                            string TaxHistory = BillYear + "~" + TaxBillNumber + "~" + BillType + "~" + PayByDate + "~" + Amount + "~" + PaidDate;

                            gc.insert_date(orderno, parcelNumber, 373, TaxHistory, 1, DateTime.Now);

                        }
                    }

                    // gc.CreatePdf(orderno, parcelNumber, "Tax History Detail", driver, "CA", "El Dorado");


                    driver.Quit();
                    // gc.mergpdf(orderno, "CA", "El Dorado");
                    return "Data Inserted Successfully";
                }

                catch (Exception ex)
                {
                    driver.Quit();
                    GlobalClass.LogError(ex, orderno);
                    throw ex;
                }
            }
        }
    }
}