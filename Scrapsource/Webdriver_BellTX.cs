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
using System.Web;


namespace ScrapMaricopa.Scrapsource
{
    public class Webdriver_BellTX
    {

        IWebElement PaidCount;
        IWebDriver driver;
        DBconnection db = new DBconnection();
        List<string> link = new List<string>();
        GlobalClass gc = new GlobalClass();
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["MyConnectionString"].ToString());


        public string FTP_Bell(string houseno, string sname, string stype, string unitno, string direction, string ownername, string parcelNumber, string searchType, string orderNumber, string directParcel)
        {
            GlobalClass.global_orderNo = orderNumber;
            HttpContext.Current.Session["orderNo"] = orderNumber;
            GlobalClass.global_parcelNo = parcelNumber;
            List<string> taxinformation = new List<string>();
            string StartTime = "", AssessmentTime = "", TaxTime = "", CitytaxTime = "", LastEndTime = "";
            var driverService = PhantomJSDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            using (driver = new PhantomJSDriver())
            {
                try
                {
                    StartTime = DateTime.Now.ToString("HH:mm:ss");

                    driver.Navigate().GoToUrl("https://propaccess.trueautomation.com/clientdb/?cid=66");
                    var SelectY = driver.FindElement(By.Id("propertySearchOptions_taxyear"));
                    var selectElementY = new SelectElement(SelectY);
                    selectElementY.SelectByIndex(1);
                    driver.FindElement(By.Id("propertySearchOptions_advanced")).Click();
                    if (searchType == "titleflex")
                    {
                        string titleaddress = houseno + " " + sname + " " + stype + " " + unitno;
                        gc.TitleFlexSearch(orderNumber, "", "", titleaddress, "TX", "Bell");
                        if (HttpContext.Current.Session["TitleFlex_Search"].ToString() == "Yes")
                        {
                            driver.Quit();
                            return "MultiParcel";
                        }
                        parcelNumber = HttpContext.Current.Session["titleparcel"].ToString();
                        searchType = "parcel";
                    }

                    if (searchType == "address")
                    {
                        //var Select1 = driver.FindElement(By.Id("propertySearchOptions_searchType"));
                        //var selectElement11 = new SelectElement(Select1);
                        //selectElement11.SelectByText("Property Address");
                        driver.FindElement(By.Id("propertySearchOptions_streetNumber")).SendKeys(houseno);
                        driver.FindElement(By.Id("propertySearchOptions_streetName")).SendKeys(sname);
                        gc.CreatePdf_WOP(orderNumber, "address search", driver, "TX", "Bell");
                        driver.FindElement(By.Id("propertySearchOptions_searchAdv")).Click();
                        Thread.Sleep(2000);
                        gc.CreatePdf_WOP(orderNumber, "address search result", driver, "TX", "Bell");
                    }
                    if (searchType == "parcel")
                    {
                        //var Select1 = driver.FindElement(By.Id("propertySearchOptions_searchType"));
                        //var selectElement11 = new SelectElement(Select1);
                        //selectElement11.SelectByText("Account Number");
                        driver.FindElement(By.Id("propertySearchOptions_propertyid")).SendKeys(parcelNumber);
                        gc.CreatePdf(orderNumber, parcelNumber, "Parcel search", driver, "TX", "Bell");
                        driver.FindElement(By.Id("propertySearchOptions_searchAdv")).Click();
                        Thread.Sleep(2000);
                        gc.CreatePdf(orderNumber, parcelNumber, "Parcel search result", driver, "TX", "Bell");
                    }

                    if (searchType == "ownername")
                    {
                        driver.FindElement(By.Id("propertySearchOptions_ownerName")).SendKeys(ownername);
                        gc.CreatePdf_WOP(orderNumber, "owner search", driver, "TX", "Bell");
                        driver.FindElement(By.Id("propertySearchOptions_searchAdv")).Click();
                        Thread.Sleep(2000);
                        gc.CreatePdf_WOP(orderNumber, "owner search result", driver, "TX", "Bell");
                    }
                    if (searchType == "block")
                    {
                        //var Select1 = driver.FindElement(By.Id("propertySearchOptions_searchType"));
                        //var selectElement11 = new SelectElement(Select1);
                        //selectElement11.SelectByText("Account Number");
                        driver.FindElement(By.Id("propertySearchOptions_geoid")).SendKeys(unitno);
                        gc.CreatePdf_WOP(orderNumber, "Account Number Search ", driver, "TX", "Bell");
                        driver.FindElement(By.Id("propertySearchOptions_searchAdv")).Click();
                        Thread.Sleep(2000);
                        gc.CreatePdf_WOP(orderNumber, "Account Number Search  result", driver, "TX", "Bell");

                        //       gc.CreatePdf_WOP(orderNumber, "owner search result", driver, "TX", "Williamson");
                    }
                    //Geographic ID~Type~Property Address~Owner Name
                    //*[@id="propertySearchResults_resultsTable"]/tbody
                    int trCount = driver.FindElements(By.XPath("//*[@id='propertySearchResults_resultsTable']/tbody/tr")).Count;
                    if (trCount > 3)
                    {
                        int maxCheck = 0;
                        IWebElement tbmulti = driver.FindElement(By.XPath("//*[@id='propertySearchResults_resultsTable']/tbody"));
                        IList<IWebElement> TRmulti5 = tbmulti.FindElements(By.TagName("tr"));
                        IList<IWebElement> TDmulti5;
                        foreach (IWebElement row in TRmulti5)
                        {
                            if (maxCheck <= 25)
                            {
                                TDmulti5 = row.FindElements(By.TagName("td"));
                                if (TDmulti5.Count == 10 && TDmulti5[3].Text.Contains("Real"))
                                {
                                    string multi1 = TDmulti5[2].Text + "~" + TDmulti5[3].Text + "~" + TDmulti5[4].Text + "~" + TDmulti5[6].Text;
                                    gc.insert_date(orderNumber, TDmulti5[1].Text, 846, multi1, 1, DateTime.Now);
                                }
                                maxCheck++;
                            }
                        }
                        if (TRmulti5.Count > 25)
                        {
                            HttpContext.Current.Session["multiParcel_Bell_Multicount"] = "Maximum";
                        }
                        else
                        {
                            HttpContext.Current.Session["multiparcel_Bell"] = "Yes";
                        }

                        driver.Quit();
                        return "MultiParcel";
                    }
                    else
                    {
                        string type = driver.FindElement(By.XPath("//*[@id='propertySearchResults_resultsTable']/tbody/tr[2]/td[4]")).Text.Replace("\r\n", " ");
                        if (type == "Real")
                        {
                            //*[@id="propertySearchResults_resultsTable"]/tbody/tr[2]/td[10]
                            driver.FindElement(By.XPath("//*[@id='propertySearchResults_resultsTable']/tbody/tr[2]/td[10]/a")).Click();
                            Thread.Sleep(2000);
                        }
                        else
                        {
                            HttpContext.Current.Session["alert_msg"] = "Yes";
                            driver.Quit();
                            return "MultiParcel";
                        }

                    }

                    //property details
                    string PropertyID = "", GeographicID = "", Name = "", MailingAddress = "", OwnerID = "", LegalDescription = "", Neighborhood = "", Type = "", Address = "", NeighborhoodCD = "", Exemptions = "", MapID = "", YearBuilt = "", Acres = "";
                    string ownerdetails = "";
                    string asd = "";
                    //Property ID~Geographic ID~Name~Mailing Address~Owner ID~Mailing Address~Owner ID~Type~Legal Description~Address~Neighborhood~Neighborhood CD~Map ID~Exemptions~Year Built~Acres~Tax Authority                    


                    string fulltext = driver.FindElement(By.XPath("//*[@id='propertyDetails']/table/tbody")).Text.Replace("\r\n", " ");
                    PropertyID = gc.Between(fulltext, "Account Property ID:", " Legal Description: ");
                    //   gc.CreatePdf(orderNumber, parcelNumber, "property details", driver, "TX", "Bell");
                    GeographicID = gc.Between(fulltext, "Geographic ID: ", "Agent Code: ");
                    Name = gc.Between(fulltext, "Owner Name:", "Owner ID:");
                    MailingAddress = gc.Between(fulltext, "Mailing Address:", " % Ownership:");
                    OwnerID = gc.Between(fulltext, "Owner ID:", "Mailing Address:");
                    LegalDescription = gc.Between(fulltext, "Legal Description:", "Geographic ID:");
                    Neighborhood = gc.Between(fulltext, "Neighborhood:", "Map ID:");
                    Type = gc.Between(fulltext, "Type:", "Property Use Code:");
                    Address = gc.Between(fulltext, "Location Address:", "Mapsco:");
                    NeighborhoodCD = gc.Between(fulltext, "Neighborhood CD:", "Owner");
                    Exemptions = GlobalClass.After(fulltext, "Exemptions:");
                    MapID = gc.Between(fulltext, "Map ID:", "Neighborhood CD:");
                    //*[@id="improvementBuilding"]
                    driver.FindElement(By.XPath("//*[@id='improvementBuilding']")).Click();
                    Thread.Sleep(2000);
                    try
                    {
                        YearBuilt = driver.FindElement(By.XPath("//*[@id='improvementBuildingDetails']/table[2]/tbody/tr[2]/td[6]")).Text;
                    }
                    catch { }
                    driver.FindElement(By.Id("land")).Click();
                    Thread.Sleep(2000);
                    try
                    {
                        Acres = driver.FindElement(By.XPath("//*[@id='landDetails']/table/tbody/tr[2]/td[4]")).Text;
                    }
                    catch { }
                    string property_details = PropertyID + "~" + GeographicID + "~" + Name + "~" + MailingAddress + "~" + OwnerID + "~" + Type + "~" + LegalDescription + "~" + Address + "~" + Neighborhood + "~" + NeighborhoodCD + "~" + MapID + "~" + Exemptions + "~" + YearBuilt + "~" + Acres + "~" + "P.O.Box 390 Belton, TX 76513 Phone Number:  254 - 939 - 5841";
                    gc.insert_date(orderNumber, PropertyID, 841, property_details, 1, DateTime.Now);

                    //    Assessment Details Table:

                    //  Values Details Table:
                    //Description~Sign~Value1~Value2
                    driver.FindElement(By.Id("values")).Click();
                    Thread.Sleep(2000);


                    IWebElement multitableElement1 = driver.FindElement(By.XPath("//*[@id='valuesDetails']/table/tbody"));
                    IList<IWebElement> multitableRow1 = multitableElement1.FindElements(By.TagName("tr"));
                    IList<IWebElement> multirowTD1;
                    foreach (IWebElement row in multitableRow1)
                    {
                        multirowTD1 = row.FindElements(By.TagName("td"));
                        if (multirowTD1.Count != 0 && multirowTD1[0].Text != " ")
                        {
                            string ValueDetails = multirowTD1[0].Text.Trim() + "~" + multirowTD1[1].Text.Trim() + "~" + multirowTD1[2].Text.Trim() + "~" + multirowTD1[3].Text.Trim();
                            gc.insert_date(orderNumber, PropertyID, 844, ValueDetails, 1, DateTime.Now);

                        }
                    }

                    //Taxing Jurisdiction Details Table:
                    //Owner~% Ownership~Total Value~Entity~Description~Tax Rate~Appraised Value~Taxable Value~Estimated Tax
                    driver.FindElement(By.Id("taxingJurisdiction")).Click();
                    Thread.Sleep(2000);
                    string owner = "", Ownership = "", TotalValue = "";
                    string ValueDetails1 = "", msg = "";
                    string fulltext1 = driver.FindElement(By.XPath("//*[@id='taxingJurisdictionDetails']/table[1]/tbody")).Text.Replace("\r\n", " ");
                    List<string> myList = new List<string>();
                    if (myList.Any(str => str.Contains("Mdd LH")))
                    { }
                    List<string> strSecured = new List<string>();
                    owner = gc.Between(fulltext1, "Owner:", "% Ownership:");
                    Ownership = gc.Between(fulltext1, "% Ownership:", "Total Value:");
                    TotalValue = GlobalClass.After(fulltext1, "Total Value:");
                    ValueDetails1 = owner + "~" + Ownership + "~" + TotalValue + "~" + "" + "~" + "" + "~" + "" + "~" + "" + "~" + "" + "~" + "";
                    gc.insert_date(orderNumber, PropertyID, 843, ValueDetails1, 1, DateTime.Now);
                    string msg1 = "You must call the specific Collector's Office";
                    string city = "";
                    IWebElement multitableElement11 = driver.FindElement(By.XPath("//*[@id='taxingJurisdictionDetails']/table[2]/tbody"));
                    IList<IWebElement> multitableRow11 = multitableElement11.FindElements(By.TagName("tr"));
                    IList<IWebElement> multirowTD11;
                    foreach (IWebElement row in multitableRow11)
                    {
                        multirowTD11 = row.FindElements(By.TagName("td"));
                        if (multirowTD11.Count != 0)
                        {
                            if (multirowTD11[2].Text.Trim() != "0.000000" && multirowTD11[0].Text.Trim() != "")
                            {
                                myList.Add(multirowTD11[1].Text);
                            }
                            ValueDetails1 = " " + "~" + " " + "~" + " " + "~" + multirowTD11[0].Text.Trim() + "~" + multirowTD11[1].Text.Trim() + "~" + multirowTD11[2].Text.Trim() + "~" + multirowTD11[3].Text.Trim() + "~" + multirowTD11[4].Text.Trim() + "~" + multirowTD11[5].Text.Trim();
                            gc.insert_date(orderNumber, PropertyID, 843, ValueDetails1, 1, DateTime.Now);

                        }
                    }

                    // Roll Value History Details Table:
                    //Year~Improvements~Land Market~Ag Valuation~Appraised~HS Cap~Assessed
                    driver.FindElement(By.Id("rollHistory")).Click();
                    Thread.Sleep(2000);

                    IWebElement multitableElement2 = driver.FindElement(By.XPath("//*[@id='rollHistoryDetails']/table/tbody"));
                    IList<IWebElement> multitableRow2 = multitableElement2.FindElements(By.TagName("tr"));
                    IList<IWebElement> multirowTD2;
                    foreach (IWebElement row in multitableRow2)
                    {
                        multirowTD2 = row.FindElements(By.TagName("td"));
                        if (multirowTD2.Count != 0)
                        {
                            string rollDetails = multirowTD2[0].Text.Trim() + "~" + multirowTD2[1].Text.Trim() + "~" + multirowTD2[2].Text.Trim() + "~" + multirowTD2[3].Text.Trim() + "~" + multirowTD2[4].Text.Trim() + "~" + multirowTD2[5].Text.Trim() + "~" + multirowTD2[6].Text.Trim();
                            gc.insert_date(orderNumber, PropertyID, 842, rollDetails, 1, DateTime.Now);
                        }
                    }


                    //Tax Due (Payoff Calculation) Details Table:


                    driver.FindElement(By.Id("taxDue")).Click();
                    Thread.Sleep(2000);
                    string msgdeli = "", taxdue = "";
                    int count = driver.FindElements(By.XPath("//*[@id='taxDueDetails_dataSection']/table/tbody/tr")).Count();
                    taxdue = driver.FindElement(By.XPath("//*[@id='taxDueDetails_dataSection']/table/tbody/tr[" + count + "]/td[7]")).Text;

                    if (!taxdue.Contains("$0.00"))
                    {
                        IWebElement dt = driver.FindElement(By.Id("taxDueDetails_recalculateDate"));
                        string date = dt.Text;

                        DateTime G_Date = Convert.ToDateTime(date);
                        string dateChecking = DateTime.Now.ToString("MM") + "/15/" + DateTime.Now.ToString("yyyy");

                        if (G_Date < Convert.ToDateTime(dateChecking))
                        {
                            //end of the month
                            date = new DateTime(Convert.ToInt16(DateTime.Now.ToString("yyyy")), Convert.ToInt16(Convert.ToInt16(DateTime.Now.ToString("MM"))), DateTime.DaysInMonth(Convert.ToInt16(DateTime.Now.ToString("yyyy")), Convert.ToInt16(DateTime.Now.ToString("MM")))).ToString("MM/dd/yyyy");

                        }

                        else if (G_Date > Convert.ToDateTime(dateChecking))
                        {
                            // nextEndOfMonth 
                            if ((Convert.ToInt16(Convert.ToInt16(DateTime.Now.ToString("MM"))) < 12))
                            {
                                date = new DateTime(Convert.ToInt16(DateTime.Now.ToString("yyyy")), Convert.ToInt16(Convert.ToInt16(DateTime.Now.ToString("MM")) + 1), DateTime.DaysInMonth(Convert.ToInt16(DateTime.Now.ToString("yyyy")), Convert.ToInt16(DateTime.Now.ToString("MM")) + 1)).ToString("MM/dd/yyyy");
                            }
                            else
                            {
                                int nxtYr = Convert.ToInt16(DateTime.Now.ToString("yyyy")) + 1;
                                date = new DateTime(nxtYr, 1, DateTime.DaysInMonth(Convert.ToInt16(DateTime.Now.ToString("yyyy")), 1)).ToString("MM/dd/yyyy");

                            }
                        }

                        Thread.Sleep(2000);
                        // dt.Clear();

                        driver.FindElement(By.Id("taxDueDetails_date")).SendKeys(date);

                        driver.FindElement(By.Id("taxDueDetails_recalculate")).Click();
                        Thread.Sleep(4000);
                    }
                    string goodthroughdate = driver.FindElement(By.Id("taxDueDetails_recalculateDate")).Text;
                    string payoff1 = goodthroughdate + "~" + "" + "~" + "" + "~" + "" + "~" + "" + "~" + "" + "~" + "" + "~" + "" + "~" + "" + "~" + "";
                    gc.insert_date(orderNumber, PropertyID, 845, payoff1, 1, DateTime.Now);

                    string yearnow = driver.FindElement(By.XPath("//*[@id='taxDueDetails_dataSection']/table/tbody/tr[2]/td[1]")).Text;
                    //Amount Due if Paid on~Year~Taxing Jurisdiction~Taxable Value~Base Tax~Base Taxes Paid~Base Tax Due~Discount / Penalty & Interest~Attorney Fees~Amount Due
                    IWebElement multitableElement5 = driver.FindElement(By.XPath("//*[@id='taxDueDetails_dataSection']/table/tbody"));
                    IList<IWebElement> multitableRow5 = multitableElement5.FindElements(By.TagName("tr"));
                    IList<IWebElement> multirowTD5;
                    foreach (IWebElement row in multitableRow5)
                    {
                        multirowTD5 = row.FindElements(By.TagName("td"));
                        if (multirowTD5.Count != 0)
                        {
                            if (multirowTD5[0].Text == yearnow)
                            {
                                if (myList.Any(str => str.Contains(multirowTD5[1].Text)))
                                {
                                    myList.RemoveAt(myList.IndexOf(multirowTD5[1].Text));

                                }
                            }
                            int countlist = myList.Count();
                            string payoff11 = "" + "~" + multirowTD5[0].Text.Trim() + "~" + multirowTD5[1].Text.Trim() + "~" + multirowTD5[2].Text.Trim() + "~" + multirowTD5[3].Text.Trim() + "~" + multirowTD5[4].Text.Trim() + "~" + multirowTD5[5].Text.Trim() + "~" + multirowTD5[6].Text.Trim() + "~" + multirowTD5[7].Text.Trim() + "~" + multirowTD5[8].Text.Trim();
                            gc.insert_date(orderNumber, PropertyID, 845, payoff11, 1, DateTime.Now);
                        }
                    }
                    string entity = "";
                    if (myList.Count() > 0)
                    {
                        for (int i = 0; i < myList.Count(); i++)
                        {
                            if (i == 0)
                            {
                                entity = myList[i];
                            }
                            else
                            {
                                entity = entity + "&" + myList[i];
                            }
                        }
                        string msged = entity + "~" + "Need to call Tax Office";
                        gc.insert_date(orderNumber, PropertyID, 893, msged, 1, DateTime.Now);
                    }
                    gc.CreatePdf(orderNumber, PropertyID, "property details", driver, "TX", "Bell");

                    AssessmentTime = DateTime.Now.ToString("HH:mm:ss");

                    LastEndTime = DateTime.Now.ToString("HH:mm:ss");
                    gc.insert_TakenTime(orderNumber, "TX", "Bell", StartTime, AssessmentTime, TaxTime, CitytaxTime, LastEndTime);
                    driver.Quit();
                    GlobalClass.titleparcel = "";
                    gc.mergpdf(orderNumber, "TX", "Bell");
                    return "Data Inserted Successfully";
                }
                catch (Exception ex)
                {
                    driver.Quit();
                    driver.Dispose();
                    throw ex;
                }
            }
        }



    }
}