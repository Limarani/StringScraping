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
using System;
using System.Collections.Generic;
using System.Linq;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Collections.ObjectModel;

namespace ScrapMaricopa.Scrapsource
{
    public class WebDriver_AllenIN
    {
        List<string> AddressSearch = new List<string>();
        IWebDriver driver;
        DBconnection db = new DBconnection();
        GlobalClass gc = new GlobalClass();
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["MyConnectionString"].ToString());
        List<string> RevaluationNotice = new List<string>();

        public string FTP_Allen(string address, string unitno, string ownername, string parcelNumber, string searchType, string orderNumber)
        {
            GlobalClass.global_orderNo = orderNumber;
            HttpContext.Current.Session["orderNo"] = orderNumber;
            GlobalClass.global_parcelNo = parcelNumber;
            List<string> multiparcel = new List<string>();
            string StartTime = "", AssessmentTime = "", TaxTime = "", CitytaxTime = "", LastEndTime = "", TaxAuthority = "";

            var driverService = PhantomJSDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            //driver = new PhantomJSDriver();
            //driver = new ChromeDriver();
            using (driver = new PhantomJSDriver())
            {
                StartTime = DateTime.Now.ToString("HH:mm:ss");
                driver.Navigate().GoToUrl("http://lowtaxinfo.com/allencounty/#/SearchView");
                try
                {
                    //string address = streetno + " " + direction + " " + streetname + " " + streetype + " " + unitno;
                    if (searchType == "titleflex")
                    {

                        gc.TitleFlexSearch(orderNumber, "", "", address, "IN", "Allen");
                        if ((HttpContext.Current.Session["TitleFlex_Search"] != null && HttpContext.Current.Session["TitleFlex_Search"].ToString() == "Yes"))
                        {
                            driver.Quit();
                            return "MultiParcel";
                        }
                        parcelNumber = HttpContext.Current.Session["titleparcel"].ToString();
                        searchType = "parcel";
                    }
                    if (searchType == "address")
                    {
                        driver.FindElement(By.Id("address")).SendKeys(address);

                        IWebElement IAddressSearch1 = driver.FindElement(By.XPath("//*[@id='searchForm']/div[5]/button"));
                        IJavaScriptExecutor js1 = driver as IJavaScriptExecutor;
                        js1.ExecuteScript("arguments[0].click();", IAddressSearch1);
                        //Thread.Sleep(15000);
                        Thread.Sleep(2000);
                        //Multi Parcel
                        try
                        {
                            int count = 0;
                            IWebElement Multiaddresstable = driver.FindElement(By.XPath("//*[@id='gridContainer']/div/div[6]/div/div[1]/div/table/tbody"));
                            IList<IWebElement> multiaddressrow = Multiaddresstable.FindElements(By.TagName("tr"));
                            IList<IWebElement> Mutiaddressid;
                            foreach (IWebElement Multiaddress in multiaddressrow)
                            {
                                Mutiaddressid = Multiaddress.FindElements(By.TagName("td"));

                                if (Mutiaddressid.Count == 18 && Mutiaddressid[6].Text.Trim().Contains("R"))
                                {

                                    if (multiaddressrow.Count != 0 && Multiaddress.Text.Trim() != "" && Multiaddress.Text.Contains(address.Trim()))
                                    {
                                        string Parcelnumber = Mutiaddressid[2].Text;
                                        string[] multiaddressresultsplit = Mutiaddressid[3].Text.Split('\r');

                                        string Ownerparcel = multiaddressresultsplit[0].Trim();
                                        string Address = multiaddressresultsplit[2].Trim();
                                        string multiaddressresult = Ownerparcel + "~" + Address;
                                        gc.insert_date(orderNumber, Parcelnumber, 1779, multiaddressresult, 1, DateTime.Now);
                                        count++;
                                    }
                                }
                            }
                            if (count > 1)
                            {
                                HttpContext.Current.Session["multiParcel_AllenIN"] = "Yes";
                                driver.Quit();
                                return "MultiParcel";
                            }
                            if (count == 1)
                            {
                                driver.FindElement(By.XPath("//*[@id='gridContainer']/div/div[6]/div/div[1]/div/table/tbody/tr[1]/td[2]/table/tbody/tr[1]/td")).Click();
                                Thread.Sleep(2000);
                            }
                            if (multiaddressrow.Count > 91)
                            {
                                HttpContext.Current.Session["multiParcel_AllenIN_Maximum"] = "Maximum";
                                driver.Quit();
                                return "Maximum";
                            }
                        }
                        catch { }
                        try
                        {
                            //No Data Found
                            string nodata = driver.FindElement(By.Id("MainContent_lblMsg")).Text;
                            if (nodata.Contains("There were no records found."))
                            {
                                HttpContext.Current.Session["Nodata_AllenIN"] = "Yes";
                                driver.Quit();
                                return "No Data Found";
                            }
                        }
                        catch { }
                    }
                    if (searchType == "ownername")
                    {

                        driver.FindElement(By.Id("name")).SendKeys(ownername);
                        IWebElement IAddressSearch1 = driver.FindElement(By.XPath("//*[@id='searchForm']/div[5]/button"));
                        IJavaScriptExecutor js1 = driver as IJavaScriptExecutor;
                        js1.ExecuteScript("arguments[0].click();", IAddressSearch1);
                        Thread.Sleep(2000);
                        //Multi Parcel
                        try
                        {
                            int count = 0;
                            IWebElement Multiaddresstable = driver.FindElement(By.XPath("//*[@id='gridContainer']/div/div[6]/div/div[1]/div/table/tbody"));
                            IList<IWebElement> multiaddressrow = Multiaddresstable.FindElements(By.TagName("tr"));
                            IList<IWebElement> Mutiaddressid;
                            foreach (IWebElement Multiaddress in multiaddressrow)
                            {
                                Mutiaddressid = Multiaddress.FindElements(By.TagName("td"));

                                if (Mutiaddressid.Count == 18 && Mutiaddressid[6].Text.Trim().Contains("R"))
                                {

                                    if (multiaddressrow.Count != 0 && Multiaddress.Text.Trim() != "" && Multiaddress.Text.Contains(address.Trim()))
                                    {
                                        string Parcelnumber = Mutiaddressid[2].Text;
                                        string[] multiaddressresultsplit = Mutiaddressid[3].Text.Split('\r');

                                        string Ownerparcel = multiaddressresultsplit[0].Trim();
                                        string Address = multiaddressresultsplit[2].Trim();
                                        string multiaddressresult = Ownerparcel + "~" + Address;
                                        gc.insert_date(orderNumber, Parcelnumber, 1779, multiaddressresult, 1, DateTime.Now);
                                        count++;
                                    }
                                }
                            }
                            if (count > 1)
                            {
                                HttpContext.Current.Session["multiParcel_AllenIN"] = "Yes";
                                driver.Quit();
                                return "MultiParcel";
                            }
                            if (count == 1)
                            {
                                driver.FindElement(By.XPath("//*[@id='gridContainer']/div/div[6]/div/div[1]/div/table/tbody/tr[1]/td[2]/table/tbody/tr[1]/td")).Click();
                                Thread.Sleep(2000);
                            }
                            if (multiaddressrow.Count > 91)
                            {
                                HttpContext.Current.Session["multiParcel_AllenIN_Maximum"] = "Maximum";
                                driver.Quit();
                                return "Maximum";
                            }
                        }
                        catch { }
                        try
                        {
                            //No Data Found
                            string nodata = driver.FindElement(By.Id("MainContent_lblMsg")).Text;
                            if (nodata.Contains("There were no records found."))
                            {
                                HttpContext.Current.Session["Nodata_AllenIN"] = "Yes";
                                driver.Quit();
                                return "No Data Found";
                            }
                        }
                        catch { }
                    }
                    if (searchType == "parcel")
                    {
                        driver.FindElement(By.Id("txtSearch")).SendKeys(parcelNumber);
                        driver.FindElement(By.Id("MainContent_btnSearch")).SendKeys(Keys.Enter);
                        Thread.Sleep(2000);
                        gc.CreatePdf_WOP(orderNumber, "SearchAddressBefore", driver, "IN", "Allen");
                        try
                        {
                            //No Data Found
                            string nodata = driver.FindElement(By.Id("MainContent_lblMsg")).Text;
                            if (nodata.Contains("There were no records found."))
                            {
                                HttpContext.Current.Session["Nodata_AllenIN"] = "Yes";
                                driver.Quit();
                                return "No Data Found";
                            }
                        }
                        catch { }
                    }
                    //Property Details                   
                    string ParcelNumber = "", ParcelID = "", Loacalparcelid = "", RoutingNumber = "", Propertyclass = "", Propertyaddress = "", Ownername = "", Mailingaddress = "", Legaldescripton = "", County = "", Township = "", District = "", School = "", Neighborhood = "", Sectionplat = "", YearBuilt = "";
                    //Tax Information Details
                    try
                    {
                        driver.FindElement(By.XPath("//*[@id='gridContainer']/div/div[6]/div/div[1]/div/table/tbody/tr[1]/td[2]/table/tbody/tr[1]/td")).Click();
                        Thread.Sleep(2000);
                    }
                    catch (Exception ex) { }

                    string ParcelNumber1 = "", Propertyadd = "", Taxyear = "", Propertytype = "", Taxunit = "", Propertyclass1 = "", Ownername1 = "", Mailingadd = "";

                    IWebElement Taxinfo1 = driver.FindElement(By.XPath("//*[@id='PropertyInformation']/div/div[1]"));

                    Taxyear = gc.Between(Taxinfo1.Text, "Tax Year/Pay Year", "Parcel Number").Trim();
                    ParcelNumber = gc.Between(Taxinfo1.Text, "Parcel Number", "Property Type").Trim();
                    Ownername1 = gc.Between(Taxinfo1.Text, "Owner of Record", "Mailing Address").Trim();
                    Propertyadd = driver.FindElement(By.Id("PropertyAddressHeader")).Text.Trim();
                    Propertytype = gc.Between(Taxinfo1.Text, "Property Type", "Tax Unit / Description").Trim();
                    Taxunit = gc.Between(Taxinfo1.Text, "Tax Unit / Description", "Property Class").Trim();
                    Propertyclass = gc.Between(Taxinfo1.Text, "Property Class", "Owner of Record").Trim();
                    Mailingadd = GlobalClass.After(Taxinfo1.Text, "Mailing Address").Trim();

                    string Taxinformationdetail1 = Taxyear.Trim() + "~" + Ownername.Trim() + "~" + Propertyadd.Trim() + "~" + Propertytype.Trim() + "~" + Taxunit.Trim() + "~" + Propertyclass + "~" + Mailingadd;
                    gc.insert_date(orderNumber, ParcelNumber, 1775, Taxinformationdetail1, 1, DateTime.Now);

                    //Tax History Table
                    string Payyear = "", Spring = "", Fall = "", Delinquencies = "", Totaltax = "", Payments = "", Balancedue = "";

                    IWebElement History = driver.FindElement(By.XPath("//*[@id='TaxHistory']/div/div/div[6]/div/table/tbody"));
                    IList<IWebElement> TRTaxHistory = History.FindElements(By.TagName("tr"));
                    IList<IWebElement> TDTaxHistory;
                    foreach (IWebElement row1 in TRTaxHistory)
                    {
                        TDTaxHistory = row1.FindElements(By.TagName("td"));
                        if (TDTaxHistory.Count != 0 && TDTaxHistory.Count == 6)
                        {
                            Payyear = TDTaxHistory[0].Text;
                            Spring = TDTaxHistory[1].Text;
                            Fall = TDTaxHistory[2].Text;
                            Delinquencies = TDTaxHistory[3].Text;
                            Totaltax = TDTaxHistory[4].Text;
                            Payments = TDTaxHistory[5].Text;
                            try
                            {
                                Balancedue = TDTaxHistory[6].Text;
                            }
                            catch { }
                            string TaxHistory = Payyear.Trim() + "~" + Spring.Trim() + "~" + Fall.Trim() + "~" + Delinquencies.Trim() + "~" + Totaltax.Trim() + "~" + Payments.Trim() + "~" + Balancedue.Trim();
                            gc.insert_date(orderNumber, ParcelNumber, 1778, TaxHistory, 1, DateTime.Now);
                        }
                    }

                    //Tax Summary Details
                    string summaryitem = "", year1 = "", year2 = "";

                    IWebElement Summary = driver.FindElement(By.XPath("//*[@id='TaxSummary']/div/table/tbody"));
                    IList<IWebElement> TRTaxSummary = Summary.FindElements(By.TagName("tr"));
                    IList<IWebElement> TDTaxSummary;
                    foreach (IWebElement row2 in TRTaxSummary)
                    {
                        TDTaxSummary = row2.FindElements(By.TagName("td"));
                        if (TDTaxSummary.Count != 0 && TDTaxSummary.Count == 3 && !row2.Text.Contains("Tax Summary Item"))
                        {
                            summaryitem = TDTaxSummary[0].Text;
                            year1 = TDTaxSummary[1].Text;
                            year2 = TDTaxSummary[2].Text;
                            string Taxsummary = summaryitem.Trim() + "~" + year1.Trim() + "~" + year2.Trim();
                            gc.insert_date(orderNumber, ParcelNumber, 1780, Taxsummary, 1, DateTime.Now);
                        }
                    }

                    string Year = "";
                    IWebElement selecelement = driver.FindElement(By.Id("PayYears"));
                    string[] slectyear = selecelement.Text.Split('\r');

                    IWebElement tbmulti = driver.FindElement(By.Id("PayYears"));
                    IList<IWebElement> TRmulti = tbmulti.FindElements(By.TagName("option"));

                    string getsplitvalue = "";
                    IList<IWebElement> TDmulti;
                    foreach (IWebElement row in TRmulti)
                    {
                        if (!row.Text.Contains("Pay Year"))
                        {
                            getsplitvalue += row.Text + "~";
                        }
                    }
                    getsplitvalue = getsplitvalue.TrimEnd('~');
                    string[] splitgetyears = getsplitvalue.Split('~');

                    for (int i = splitgetyears.Count() - 1; i > splitgetyears.Count() - 5; i--)
                    {
                        IWebElement selecelement1 = driver.FindElement(By.Id("PayYears"));
                        SelectElement clkci = new SelectElement(selecelement1);
                        clkci.SelectByText(splitgetyears[i]);

                        Year = driver.FindElement(By.XPath("//*[@id='PropertyInformation']/div/div[1]/span[1]")).Text.Trim();
                        gc.CreatePdf(orderNumber, ParcelNumber, "Yearwise Pdf" + i, driver, "IN", "Allen");
                        //Tax Information Details2  // DB insert 1776
                        //Tax Year~Due Amount~Payable Year~Paid Date~Installment~Paid Amount~Description
                        string Payableyear = "", Entrydate = "", Payableperiod = "", Paidamount = "", Description = "";

                        IWebElement Taxdetails2 = driver.FindElement(By.XPath("//*[@id='Payments']/div/div/div[6]/div/div[1]/div/table/tbody"));
                        IList<IWebElement> TRTaxdetails2 = Taxdetails2.FindElements(By.TagName("tr"));
                        IList<IWebElement> TDTaxdetails2;
                        foreach (IWebElement row2 in TRTaxdetails2)
                        {
                            TDTaxdetails2 = row2.FindElements(By.TagName("td"));
                            if (row2.Text.Trim() != "" && TDTaxdetails2.Count != 0 && TDTaxdetails2.Count == 6 && !row2.Text.Contains("Payable Year"))
                            {
                                Payableyear = TDTaxdetails2[0].Text;
                                Entrydate = TDTaxdetails2[1].Text;
                                Payableperiod = TDTaxdetails2[2].Text;
                                Paidamount = TDTaxdetails2[3].Text;
                                Description = TDTaxdetails2[4].Text;
                                string Taxdetailstable = Year.Trim() + "~" + Payableyear.Trim() + "~" + Entrydate.Trim() + "~" + Payableperiod.Trim() + "~" + Paidamount.Trim() + "~" + Description.Trim();
                                gc.insert_date(orderNumber, ParcelNumber, 1776, Taxdetailstable, 1, DateTime.Now);
                            }
                        }

                        //Tax Details Table

                        string Field = "", Taxamount = "", Adjustments = "", Balance = "";

                        IWebElement Taxdetails1 = driver.FindElement(By.XPath("//*[@id='BillingDetail']/div/div/div[6]/div/div[1]/div/table/tbody"));
                        IList<IWebElement> TRTaxdetails1 = Taxdetails1.FindElements(By.TagName("tr"));
                        IList<IWebElement> TDTaxdetails1;
                        foreach (IWebElement row in TRTaxdetails1)
                        {
                            TDTaxdetails1 = row.FindElements(By.TagName("td"));
                            if (TDTaxdetails1.Count != 0 && TDTaxdetails1.Count == 4)
                            {
                                Field = TDTaxdetails1[0].Text;
                                Taxamount = TDTaxdetails1[1].Text;
                                Adjustments = TDTaxdetails1[2].Text;
                                Balance = TDTaxdetails1[3].Text;
                                string Taxdetailstable = Year.Trim() + "~" + Field.Trim() + "~" + Taxamount.Trim() + "~" + Adjustments.Trim() + "~" + Balance.Trim();
                                gc.insert_date(orderNumber, ParcelNumber, 1777, Taxdetailstable, 1, DateTime.Now);
                            }
                        }

                        //Assessed Value Details
                        string Landvalue = "", Improvements = "", Homesteadstand = "", Homesteadsupp = "", Mortgage = "", Count = "";
                        IWebElement Assessed = driver.FindElement(By.XPath("//*[@id='AssessmentsReport1']/div[1]/div/div[6]/div/div[1]/div/table/tbody"));
                        IList<IWebElement> TRTaxAssessed = Assessed.FindElements(By.TagName("tr"));
                        IList<IWebElement> TDTaxAssessed;
                        foreach (IWebElement row2 in TRTaxAssessed)
                        {
                            TDTaxAssessed = row2.FindElements(By.TagName("td"));
                            if (TDTaxAssessed.Count != 0 && TDTaxAssessed.Count == 2 && row2.Text.Contains("Land Value"))
                            {
                                Landvalue = TDTaxAssessed[1].Text;
                            }
                            if (TDTaxAssessed.Count != 0 && TDTaxAssessed.Count == 2 && row2.Text.Contains("Improvements"))
                            {
                                Improvements = TDTaxAssessed[1].Text;
                            }
                        }


                        IWebElement Assessed1 = driver.FindElement(By.XPath("//*[@id='ExemptionsDeductions']/div/div/div[6]/div/div[1]/div/table/tbody"));
                        IList<IWebElement> TRTaxAssessed1 = Assessed1.FindElements(By.TagName("tr"));
                        IList<IWebElement> TDTaxAssessed1;
                        foreach (IWebElement row21 in TRTaxAssessed1)
                        {
                            TDTaxAssessed1 = row21.FindElements(By.TagName("td"));

                            if (TDTaxAssessed1.Count != 0 && TDTaxAssessed1.Count == 2 && row21.Text.Contains("Stand"))
                            {
                                Homesteadstand = TDTaxAssessed1[1].Text;
                            }
                            if (TDTaxAssessed1.Count != 0 && TDTaxAssessed1.Count == 2 && row21.Text.Contains("Supp"))
                            {
                                Homesteadsupp = TDTaxAssessed1[1].Text;
                            }
                            if (TDTaxAssessed1.Count != 0 && TDTaxAssessed1.Count == 2 && row21.Text.Contains("Mortgage"))
                            {
                                Mortgage = TDTaxAssessed1[1].Text;
                            }
                        }


                        IWebElement Assessed2 = driver.FindElement(By.XPath("//*[@id='ExemptionsDeductions']/div/div/div[8]/div/table/tbody"));
                        IList<IWebElement> TRTaxAssessed2 = Assessed2.FindElements(By.TagName("tr"));
                        IList<IWebElement> TDTaxAssessed2;
                        foreach (IWebElement row31 in TRTaxAssessed2)
                        {
                            TDTaxAssessed2 = row31.FindElements(By.TagName("td"));
                            if (TDTaxAssessed2.Count != 0 && TDTaxAssessed2.Count == 2 && row31.Text.Contains("Count: 3"))
                            {
                                Count = TDTaxAssessed2[1].Text;
                            }
                        }

                        string TaxAssessed = Year + "~" + Landvalue.Trim() + "~" + Improvements.Trim() + "~" + Homesteadstand.Trim() + "~" + Homesteadsupp.Trim() + "~" + Mortgage.Trim() + "~" + Count.Trim();
                        gc.insert_date(orderNumber, ParcelNumber, 1781, TaxAssessed, 1, DateTime.Now);

                    }

                    //Property Details
                    ParcelID = driver.FindElement(By.XPath("//*[@id='PropertyInformation']/div/div[1]/span[2]")).Text.Trim().Replace(".", "").Trim().Replace("-", "").Trim();

                    driver.Navigate().GoToUrl("https://lowtaxinfo.com/AllenCounty/PRC/" + ParcelID + ".pdf");
                    Thread.Sleep(3000);
                    gc.CreatePdf(orderNumber, ParcelID, "Secured Tax Bill", driver, "IN", "Allen");
                    gc.downloadfile(driver.Url, orderNumber, ParcelID, "Dominic", "IN", "Allen");
                    string FilePath = gc.filePath(orderNumber, ParcelID) + "Dominic.pdf";
                    PdfReader reader;
                    string pdfData;
                    string pdftext = "";
                  
                    try
                    {
                        reader = new PdfReader(FilePath);
                        String textFromPage = PdfTextExtractor.GetTextFromPage(reader, 1);
                        System.Diagnostics.Debug.WriteLine("" + textFromPage);

                        pdftext = textFromPage;
                        string[] splittextt = pdftext.Split('\n');

                        Loacalparcelid = splittextt[10].Trim();
                        RoutingNumber = splittextt[19].Trim();
                        Propertyclass = splittextt[22].Trim();
                        Propertyaddress = splittextt[7].Trim();
                        Ownername = splittextt[3].Replace("Parcel Number", "").Trim();
                        Mailingaddress = splittextt[59];
                        Legaldescripton = splittextt[16];
                        string[] legalsplit = Legaldescripton.Split('\r');
                        County = splittextt[31].Trim();
                        Township = splittextt[35].Trim();
                        District = splittextt[39].Trim();
                        School = splittextt[44].Trim();
                        Neighborhood = splittextt[47].Trim();
                        Sectionplat = splittextt[52].Trim();
                        YearBuilt = "";

                        string Propertdetails = Ownername.Trim() + "~" + Loacalparcelid.Trim() + "~" + RoutingNumber.Trim() + "~" + Propertyclass.Trim() + "~" + Propertyaddress + "~" + Mailingaddress + "~" + Legaldescripton + "~" + County + "~" + Township + "~" + District + "~" + School + "~" + Neighborhood + "~" + Sectionplat + "~" + YearBuilt;
                        gc.insert_date(orderNumber, ParcelID, 1768, Propertdetails, 1, DateTime.Now);

                        //Assessment Details

                        string[] splittextt1 = pdftext.Split('\n');
                        string tableassess = gc.Between(pdftext, "Year:", "Total Non Res (3)").Trim();
                        string[] tableArray = tableassess.Split('\n');
                        string[] Assessmentsplit = tableArray[1].Split(' ');
                        string[] Reasonchange = GlobalClass.After(tableArray[3], "Reason For Change").Split(' ');
                        string[] asofdate = tableArray[5].Split(' ');
                        string[] ValuationMethod = GlobalClass.After(tableArray[8], "Valuation Method").Trim().Split(' ');

                        string Valution1 = ValuationMethod[0].Trim() + " " + ValuationMethod[1].Trim() + " " + ValuationMethod[2].Trim();
                        string Valution2 = ValuationMethod[3].Trim() + " " + ValuationMethod[4] + " " + ValuationMethod[5].Trim();
                        string Valution3 = ValuationMethod[6].Trim() + " " + ValuationMethod[7] + " " + ValuationMethod[8].Trim();
                        string Valution4 = ValuationMethod[9].Trim() + " " + ValuationMethod[10] + " " + ValuationMethod[11].Trim();
                        string Valution5 = ValuationMethod[12].Trim() + " " + ValuationMethod[13] + " " + ValuationMethod[14].Trim();

                        string[] Equalizationfactor = tableArray[9].Trim().Split(' ');
                        string Noticerequired = "YES";
                        string[] Land = tableArray[14].Trim().Split('$');
                        string[] Landres1 = tableArray[16].Trim().Split('$');
                        string[] Landnonres2 = tableArray[17].Trim().Split('$');
                        string[] Landnonres3 = tableArray[19].Trim().Split('$');
                        string[] improvement = tableArray[21].Trim().Split('$');
                        string[] impres1 = tableArray[22].Trim().Split('$');
                        string[] impnonres2 = tableArray[24].Trim().Split('$');
                        string[] impnonres3 = tableArray[25].Trim().Split('$');
                        string[] total = tableArray[27].Trim().Split('$');
                        string[] total1 = tableArray[30].Trim().Split('$');
                        string[] totalres2 = tableArray[31].Trim().Split('$');
                        string[] totalres3 = tableArray[34].Trim().Split('$');

                        string Type1 = Assessmentsplit[0].Trim() + "~" + Reasonchange[1].Trim() + "~" + asofdate[0] + "~" + Valution1 + "~" + Equalizationfactor[0] + "~" + Noticerequired + "~" + Land[2] + "~" + Landres1[2] + "~" + Landnonres2[2] + "~" + Landnonres3[2] + "~" + improvement[2] + "~" + impres1[2] + "~" + impnonres2[2] + "~" + impnonres3[2] + "~" + total[2] + "~" + total1[2] + "~" + totalres2[2] + "~" + "";
                        string Type2 = Assessmentsplit[1].Trim() + "~" + Reasonchange[2].Trim() + "~" + asofdate[1] + "~" + Valution2 + "~" + Equalizationfactor[1] + "~" + Noticerequired + "~" + Land[3] + "~" + Landres1[3] + "~" + Landnonres2[3] + "~" + Landnonres3[3] + "~" + improvement[3] + "~" + impres1[3] + "~" + impnonres2[3] + "~" + impnonres3[3] + "~" + total[3] + "~" + total1[3] + "~" + totalres2[3] + "~" + "";
                        string Type3 = Assessmentsplit[2].Trim() + "~" + Reasonchange[3].Trim() + "~" + asofdate[2] + "~" + Valution3 + "~" + Equalizationfactor[2] + "~" + Noticerequired + "~" + Land[4] + "~" + Landres1[4] + "~" + Landnonres2[4] + "~" + Landnonres3[4] + "~" + improvement[4] + "~" + impres1[4] + "~" + impnonres2[4] + "~" + impnonres3[4] + "~" + total[4] + "~" + total1[4] + "~" + totalres2[4] + "~" + "";
                        string Type4 = Assessmentsplit[3].Trim() + "~" + Reasonchange[4].Trim() + "~" + asofdate[3] + "~" + Valution4 + "~" + Equalizationfactor[3] + "~" + Noticerequired + "~" + Land[5] + "~" + Landres1[5] + "~" + Landnonres2[5] + "~" + Landnonres3[5] + "~" + improvement[5] + "~" + impres1[5] + "~" + impnonres2[5] + "~" + impnonres3[5] + "~" + total[5] + "~" + total1[5] + "~" + totalres2[5] + "~" + "";
                        string Type5 = Assessmentsplit[4].Trim() + "~" + Reasonchange[5].Trim() + "~" + asofdate[4] + "~" + Valution5 + "~" + Equalizationfactor[4] + "~" + Noticerequired + "~" + Land[6] + "~" + Landres1[6] + "~" + Landnonres2[6] + "~" + Landnonres3[6] + "~" + improvement[6] + "~" + impres1[6] + "~" + impnonres2[6] + "~" + impnonres3[6] + "~" + total[6] + "~" + total1[6] + "~" + totalres2[6] + "~" + "";
                        gc.insert_date(orderNumber, ParcelNumber, 1769, Type1, 1, DateTime.Now);
                        gc.insert_date(orderNumber, ParcelNumber, 1769, Type2, 1, DateTime.Now);
                        gc.insert_date(orderNumber, ParcelNumber, 1769, Type3, 1, DateTime.Now);
                        gc.insert_date(orderNumber, ParcelNumber, 1769, Type4, 1, DateTime.Now);
                        gc.insert_date(orderNumber, ParcelNumber, 1769, Type5, 1, DateTime.Now);
                    }
                    catch (Exception ex)
                    { }

                    TaxTime = DateTime.Now.ToString("HH:mm:ss");
                    driver.Quit();
                    gc.mergpdf(orderNumber, "IN", "Allen");
                    LastEndTime = DateTime.Now.ToString("HH:mm:ss");
                    gc.insert_TakenTime(orderNumber, "IN", "Allen", StartTime, AssessmentTime, TaxTime, CitytaxTime, LastEndTime);
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
        public string latestfilename()
        {
             var downloadDirectory1 = ConfigurationManager.AppSettings["AutoPdf"];
            var files = new DirectoryInfo(downloadDirectory1).GetFiles("*.*");
            string latestfile = "";
            DateTime lastupdated = DateTime.MinValue;
            foreach (FileInfo file in files)
            {
                if (file.LastWriteTime > lastupdated)
                {
                    lastupdated = file.LastWriteTime;
                    latestfile = file.Name;
                }
            }
            return latestfile;
        }
    }
}