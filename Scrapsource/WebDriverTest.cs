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
namespace ScrapMaricopa
{
    public class WebDriverTest
    {
        IWebDriver driver;
        DBconnection db = new DBconnection();
        GlobalClass gc = new GlobalClass();
        List<string> addressList = new List<string>();
        string conto_owner = "-", legal_description = "-";
        string apN = "",owner1 = "-", address1 = "-", mcr1 = "-", str1 = "-";
        string outputPath = "";
        public string assFullCashValue = "", clickTaxDue = "", delinquentAmountDue = "", paidAmtFirstHalf = "", paidAmtSecHalf = "", Tapn = "", delqFirstHalf = "", delqSecHalf = "", fstHalfTaxDue = "", fstHalfInterestDue = "", fstHalfFeesDue = "", secHalfTaxDue = "", secHalfInterestDue = "", secHalfFeesDue = "", totalTaxDue = "", totalInterestDue = "", totalFeesDue = "", totalTotDue = "";
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["MyConnectionString"].ToString());

        string globaladdress;
        string globalorderno;
        string globalsearchType;
        string subdivision1;
        string parcelAddress = "";

        List<string> treasurerAPN = new List<string>();
        List<string> APN = new List<string>();


        public void FTP(string address, string orderno, string searchType, string directParcel)
        {

            globaladdress = address;
            globalorderno = orderno;
            HttpContext.Current.Session["orderNo"] = orderno;
            globalsearchType = searchType;
            APN.Clear();
            var driverService = PhantomJSDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            driver = new PhantomJSDriver();

            try
            {

                if (HomeSearchDisplayed(driver))
                {
                    driver.FindElement(By.ClassName("homeSearchField")).SendKeys(address);
                    driver.FindElement(By.ClassName("homeSearchBtn")).SendKeys(Keys.Enter);
                }

                if (searchType != "parcel")
                {
                    //for maximum parcel no exceeds.......
                    var viewRslt = driver.FindElement(By.XPath("//*[@id='real-property-results-section']/h3/div[2]"));
                    string viewMoreResult = viewRslt.Text;
                    if (viewMoreResult.Contains("View more results"))
                    {
                        driver.Quit();
                        GlobalClass.multipleParcel = "Maximum";
                        return;
                    }


                    var waitf = new WebDriverWait(driver, TimeSpan.FromMinutes(1));
                    waitf.Until(ExpectedConditions.ElementIsVisible(By.Id("rp-table")));
                    var element = driver.FindElement(By.Id("rp-table"));

                    Actions actions = new Actions(driver);
                    actions.MoveToElement(element);
                    actions.Perform();
                    waitf.Until(ExpectedConditions.ElementIsVisible(By.Id("rp-table")));

                    IWebElement tableElement = driver.FindElement(By.Id("rp-table"));
                    IList<IWebElement> tableRow = tableElement.FindElements(By.TagName("tr"));
                    IList<IWebElement> rowTD;

                    foreach (IWebElement row in tableRow)
                    {
                        rowTD = row.FindElements(By.TagName("td"));
                        if (rowTD.Count != 0)
                        {
                            string result = string.Join("|", rowTD[0].Text.ToString(), rowTD[1].Text.ToString(), rowTD[2].Text.ToString(), rowTD[3].Text.ToString(), rowTD[4].Text.ToString(), rowTD[5].Text.ToString());
                            APN.Add(result);
                        }
                    }

                }
                else
                {
                    APN.Add(address);
                }

            }
            catch (Exception thrw)
            {
                driver.Quit();
                throw thrw;
            }
            GlobalClass.multipleParcel = "";
            if (searchType != "parcel")
            {
                if (APN.Count() > 1)
                    ForMultipleParcel(orderno);
            }
            if (APN.Count() > 1)
            {
                driver.Quit();
                return;
            }
            else
            {
                GlobalClass.multipleParcel = "Single";
            }


            for (int a = 0; a < APN.Count(); a++)
            {
                if (searchType != "parcel")
                {
                    List<string> names = APN[a].Split('|').ToList<string>();
                    string APN1 = names[0].ToString();
                    APN1 = APN1.Trim();
                    treasurerAPN.Add(APN1);
                    owner1 = names[1].ToString();
                    address1 = names[2].ToString();
                    subdivision1 = names[3].ToString();
                    mcr1 = names[4].ToString();
                    str1 = names[5].ToString();
                    //  Insert_Real_Property(APN1, owner1, address1, subdivision1, mcr1, str1, orderno);
                    apN = APN1;

                }
                else
                {                  
                    apN = address;
                    treasurerAPN.Add(address);
                }


                string url = "https://mcassessor.maricopa.gov/mcs.php?q=" + apN + "&mod=pd";
                driver.Navigate().GoToUrl(url);
                try
                {
                    string[] stringSeparators = new string[] { "\r\n" };
                    CreatePdf(orderno, apN, "Assessor");
                    if (displayValuationInfo(driver))
                    {
                        //owner name...
                        var lnk_ownerName = driver.FindElement(By.XPath("//*[@id='owner-details']/div/div/table/tbody/tr/td[1]/p/a"));
                        string ownName = lnk_ownerName.Text;

                        string conto_owner_exists = driver.FindElement(By.XPath("//*[@id='owner-details']/table")).Text;

                        if (conto_owner_exists.Contains("Conto Owner"))
                        {
                            conto_owner = driver.FindElement(By.XPath("//*[@id='owner-details']/table/tbody/tr[2]/td[2]")).Text;
                            // db.ExecuteQuery("update real_property set conto_owner='" + conto_owner + "' where apn='" + apN + "' and orderno='" + orderno + "'");
                        }


                        //leagal description.....

                        var legal_desc = driver.FindElement(By.XPath("//*[@id='property-details']/table"));
                        legal_description = legal_desc.GetAttribute("innerText");
                        legal_description = Between(legal_description, "Description:", "Lat/Long").TrimStart().TrimEnd();

                        string property = ownName + "~" + address1 + "~" + subdivision1 + "~" + mcr1 + "~" + str1 + "~" + conto_owner + "~" + legal_description;
                        gc.insert_data(orderno, DateTime.Now,apN, 41, property, 1);

                        string Taxyear, Fullcash, Ltdproperty, Legalclass, Description, Assratio, Assfcv, Asslpv, Propusecode, Pudescription, Taxareacode, Valuationsrc;
                        var elemTable1 = driver.FindElement(By.XPath(".//*[@class='ui definition selectable fluid unstackable table']"));
                        string pagesource1 = elemTable1.GetAttribute("innerText");
                        string[] strings2 = new string[] { "\r\n" };
                        string[] lines2 = pagesource1.Split(stringSeparators, StringSplitOptions.None);
                        DataTable dataTable = new DataTable();
                        dataTable.Columns.Add("Col1");
                        dataTable.Columns.Add("Col2");
                        dataTable.Columns.Add("Col3");
                        dataTable.Columns.Add("Col4");
                        dataTable.Columns.Add("Col5");
                        dataTable.Columns.Add("Col6");

                        for (int j = 0; j < lines2.Length - 1; j++)
                        {
                            string[] clumnv = lines2[j].Split('\t');
                            DataRow drow = dataTable.NewRow();

                            if (clumnv.Length == 6)
                            {
                                drow["Col1"] = clumnv[0].ToString();
                                drow["Col2"] = clumnv[1].ToString();
                                drow["Col3"] = clumnv[2].ToString();
                                drow["Col4"] = clumnv[3].ToString();
                                drow["Col5"] = clumnv[4].ToString();
                                drow["Col6"] = clumnv[5].ToString();
                                dataTable.Rows.Add(drow);
                            }
                            else if (clumnv.Length == 5)
                            {
                                drow["Col1"] = clumnv[0].ToString();
                                drow["Col2"] = clumnv[1].ToString();
                                drow["Col3"] = clumnv[2].ToString();
                                drow["Col4"] = clumnv[3].ToString();
                                drow["Col5"] = clumnv[4].ToString();

                                dataTable.Rows.Add(drow);
                            }
                            else if (clumnv.Length == 4)
                            {
                                drow["Col1"] = clumnv[0].ToString();
                                drow["Col2"] = clumnv[1].ToString();
                                drow["Col3"] = clumnv[2].ToString();
                                drow["Col4"] = clumnv[3].ToString();
                                dataTable.Rows.Add(drow);
                            }
                            else if (clumnv.Length == 3)
                            {
                                drow["Col1"] = clumnv[0].ToString();
                                drow["Col2"] = clumnv[1].ToString();
                                drow["Col3"] = clumnv[2].ToString();
                                dataTable.Rows.Add(drow);
                            }
                            else if (clumnv.Length == 2)
                            {
                                drow["Col1"] = clumnv[0].ToString();
                                drow["Col2"] = clumnv[1].ToString();
                                dataTable.Rows.Add(drow);
                            }

                        }

                        for (int j = 1; j <= dataTable.Columns.Count - 1; j++)
                        {
                            Taxyear = dataTable.Rows[0][j].ToString();
                            Fullcash = dataTable.Rows[1][j].ToString();
                            Ltdproperty = dataTable.Rows[2][j].ToString();
                            Legalclass = dataTable.Rows[3][j].ToString();
                            Description = dataTable.Rows[4][j].ToString();
                            Assratio = dataTable.Rows[5][j].ToString();
                            Assfcv = dataTable.Rows[6][j].ToString();
                            Asslpv = dataTable.Rows[7][j].ToString();
                            Propusecode = dataTable.Rows[8][j].ToString();
                            Pudescription = dataTable.Rows[9][j].ToString();
                            Taxareacode = dataTable.Rows[10][j].ToString();
                            Valuationsrc = dataTable.Rows[11][j].ToString();
                            //Taxyear~Fullcash~Ltdproperty~Legalclass~Description~Assratio~Assfcv~Asslpv~Propusecode~Pudescription~Taxareacode~Valuationsrc;
                            string ass = Taxyear + "~" + Fullcash + "~" + Ltdproperty + "~" + Legalclass + "~" + Description + "~" + Assratio + "~" + Assfcv + "~" + Asslpv + "~" + Propusecode + "~" + Pudescription + "~" + Taxareacode + "~" + Valuationsrc;
                            gc.insert_data(orderno, DateTime.Now, apN, 42, ass, 1);
                            //Insert_Valuation_Info(Taxyear, Fullcash, Ltdproperty, Legalclass, Description, Assratio, Assfcv, Asslpv, Propusecode, Pudescription, Taxareacode, Valuationsrc, apN, address, orderno);
                        }

                        IList<IWebElement> subLinks = driver.FindElements(By.TagName("a"));
                        foreach (IWebElement lnk in subLinks)
                        {

                            string attribute = lnk.GetAttribute("href");
                            if (attribute != null)
                            {

                                if (attribute.Contains("search-type=sub&mod=sm"))
                                {
                                    subdivision1 = lnk.Text;
                                    //db.ExecuteQuery("update real_property set subdivision='" + subdivision1 + "',owner='" + ownName + "',conto_owner='" + conto_owner + "' where apn='" + apN + "' and orderno='" + orderno + "'");
                                    break;
                                }
                            }
                        }

                        if (directParcel == "Yes")
                        {
                            var parAddress = driver.FindElement(By.Id("at-a-glance"));
                            parcelAddress = Between(parAddress.Text, "at ", ". and");
                            List<string> APNParcel = new List<string>();
                            driver.Navigate().GoToUrl("https://mcassessor.maricopa.gov/mcs.php?q=" + parcelAddress);

                            var waitf = new WebDriverWait(driver, TimeSpan.FromMinutes(1));
                            waitf.Until(ExpectedConditions.ElementIsVisible(By.Id("rp-table")));
                            var element = driver.FindElement(By.Id("rp-table"));

                            Actions actions = new Actions(driver);
                            actions.MoveToElement(element);
                            actions.Perform();
                            waitf.Until(ExpectedConditions.ElementIsVisible(By.Id("rp-table")));

                            IWebElement tableElement = driver.FindElement(By.Id("rp-table"));
                            IList<IWebElement> tableRow = tableElement.FindElements(By.TagName("tr"));
                            IList<IWebElement> rowTD;

                            foreach (IWebElement row in tableRow)
                            {
                                rowTD = row.FindElements(By.TagName("td"));
                                if (rowTD.Count != 0)
                                {
                                    string result = string.Join("|", rowTD[0].Text.ToString(), rowTD[1].Text.ToString(), rowTD[2].Text.ToString(), rowTD[3].Text.ToString(), rowTD[4].Text.ToString(), rowTD[5].Text.ToString());
                                    APNParcel.Add(result);
                                }
                            }

                            var newList = APNParcel.Where(x => x.Contains(apN)).ToList();
                            List<string> names = newList[0].Split('|').ToList<string>();

                            string APN1 = names[0].ToString();
                            APN1 = APN1.Trim();
                            owner1 = names[1].ToString();
                            address1 = names[2].ToString();
                            mcr1 = names[4].ToString();
                            str1 = names[5].ToString();
                            //Insert_Real_Property(APN1, owner1, address1, subdivision1, mcr1, str1, orderno);
                            //Owner1~Address1~subdivision1~mcr1~str1~conto_owner~legal_description
                            string property1 = ownName + "~" + address1 + "~" + subdivision1 + "~" + mcr1 + "~" + str1 + "~" + conto_owner + "~" + legal_description;
                            gc.insert_data(orderno, DateTime.Now, apN, 41, property1, 1);
                            // db.ExecuteQuery("update real_property set subdivision='" + subdivision1 + "',owner='" + ownName + "',conto_owner='" + conto_owner + "' where apn='" + apN + "' and orderno='" + orderno + "'");
                        }

                    }

                }
                catch (Exception ee)
                {
                    throw ee;

                }
            }

            #region treasurer Detatils

            for (int t = 0; t < treasurerAPN.Count; t++)
            {

                driver.Navigate().GoToUrl("http://treasurer.maricopa.gov/");
                Tapn = treasurerAPN[t].ToString();

                driver.FindElement(By.Id("txtParcelNumBook")).SendKeys(Tapn);
                driver.FindElement(By.Id("btnGo")).Click();
                Thread.Sleep(1000);

                string url = driver.Url.ToString();
                if (url == "https://treasurer.maricopa.gov/Parcel/Summary.aspx")
                {
                    string mailingAddress = "", Summary = "", assTax = "", taxPaid = "", totalDue = "", txYear = "";

                    List<IWebElement> txyrList = driver.FindElements(By.TagName("a")).ToList();
                    foreach (IWebElement txy in txyrList)
                    {
                        if (txy.Text.Contains("Tax Details"))
                        {
                            txYear = Between(txy.Text, "View", "Tax").Trim();
                            break;
                        }

                    }

                    List<IWebElement> iwebele = new List<IWebElement>();
                    iwebele = driver.FindElements(By.XPath(".//*[@class='panel nopad']")).ToList();
                    string ss = iwebele[1].Text;
                    string[] sss = ss.Replace("\r\n", "~").Split('~');

                    assTax = sss[1].ToString();
                    taxPaid = sss[3].ToString();
                    totalDue = sss[5].ToString();
                    mailingAddress = driver.FindElement(By.XPath(".//*[@id='cphMainContent_cphRightColumn_ParcelNASitusLegal_lblSitusAddress']/div")).GetAttribute("innerText").Trim().ToString();
                    Summary = driver.FindElement(By.XPath(".//*[@id='cphMainContent_cphRightColumn_headerSummary']/small[2]")).GetAttribute("innerText").Trim().ToString();

                    // db.ExecuteQuery("insert into tax_summary(apn,AssTax,TaxPaid,TotalDue,PropertyAddress,Summary,tax_year,orderno) values ('" + Tapn + "','" + assTax + "','" + taxPaid + "','" + totalDue + "','" + mailingAddress + "','" + Summary + "','" + txYear + "','" + orderno + "')");
                    CreatePdf(orderno, Tapn, "taxsummary");
                    delinquentAmountDue = "$" + delinquentAmountDue;

                    if (displayTaxDue(driver, totalDue, txYear))
                    {
                        //check delinquent....
                        fstHalfInterestDue = driver.FindElement(By.Id("cphMainContent_cphRightColumn_lblInterestDue1stHalf")).GetAttribute("innerText").ToString();
                        secHalfInterestDue = driver.FindElement(By.Id("cphMainContent_cphRightColumn_lblInterestDue2ndHalf")).GetAttribute("innerText").ToString();

                        if (fstHalfInterestDue != "$0.00" || secHalfInterestDue != "$0.00")
                        {
                            string currDate = DateTime.Now.ToString("MM/dd/yyyy");
                            string dateChecking = DateTime.Now.ToString("MM") + "/15/" + DateTime.Now.ToString("yyyy");

                            if (Convert.ToDateTime(currDate) > Convert.ToDateTime(dateChecking))
                            {
                                string nextEndOfMonth = "";
                                if ((Convert.ToInt16(Convert.ToInt16(DateTime.Now.ToString("MM"))) < 12))
                                {
                                    nextEndOfMonth = new DateTime(Convert.ToInt16(DateTime.Now.ToString("yyyy")), Convert.ToInt16(Convert.ToInt16(DateTime.Now.ToString("MM")) + 1), DateTime.DaysInMonth(Convert.ToInt16(DateTime.Now.ToString("yyyy")), Convert.ToInt16(DateTime.Now.ToString("MM")) + 1)).ToString("MM/dd/yyyy");
                                }
                                else
                                {
                                    int nxtYr = Convert.ToInt16(DateTime.Now.ToString("yyyy")) + 1;
                                    nextEndOfMonth = nextEndOfMonth = new DateTime(nxtYr, 1, DateTime.DaysInMonth(Convert.ToInt16(DateTime.Now.ToString("yyyy")), 1)).ToString("MM/dd/yyyy");
                                   
                                }
                                //AssTax~TaxPaid~TotalDue~PropertyAddress~Summary~tax_year~goodthroughdate

                                // db.ExecuteQuery("update tax_summary set goodthroughdate='" + nextEndOfMonth + "' where apn='" + Tapn + "' and orderno='" + orderno + "'");
                                string sum = assTax + "~" + taxPaid + "~" + totalDue + "~" + mailingAddress + "~" + Summary + "~" + txYear + "~" + nextEndOfMonth;
                                gc.insert_data(orderno, DateTime.Now, apN, 43, sum, 1);

                                if (driver.FindElement(By.Id("cphMainContent_cphRightColumn_datePicker")).Enabled == true)
                                {
                                    driver.FindElement(By.Id("cphMainContent_cphRightColumn_datePicker")).SendKeys("");
                                    driver.FindElement(By.Id("cphMainContent_cphRightColumn_datePicker")).SendKeys(nextEndOfMonth);
                                    driver.FindElement(By.Id("cphMainContent_cphRightColumn_datePicker")).Click();
                                    driver.FindElement(By.Id("cphMainContent_cphRightColumn_datePicker")).SendKeys(Keys.Down);
                                    driver.FindElement(By.Id("cphMainContent_cphRightColumn_datePicker")).SendKeys(Keys.Up);
                                    driver.FindElement(By.Id("cphMainContent_cphRightColumn_datePicker")).SendKeys(Keys.Enter);
                                }
                            }
                            else
                            {
                                string EndOfMonth = new DateTime(Convert.ToInt16(DateTime.Now.ToString("yyyy")), Convert.ToInt16(Convert.ToInt16(DateTime.Now.ToString("MM"))), DateTime.DaysInMonth(Convert.ToInt16(DateTime.Now.ToString("yyyy")), Convert.ToInt16(DateTime.Now.ToString("MM")))).ToString("MM/dd/yyyy");
                                string sum = assTax + "~" + taxPaid + "~" + totalDue + "~" + mailingAddress + "~" + Summary + "~" + txYear + "~" + EndOfMonth;
                                gc.insert_data(orderno, DateTime.Now, apN, 43, sum, 1);
                                // db.ExecuteQuery("update tax_summary set goodthroughdate='" + EndOfMonth + "' where apn='" + Tapn + "' and orderno='" + orderno + "'");
                            }
                        }


                        fstHalfTaxDue = driver.FindElement(By.Id("cphMainContent_cphRightColumn_lblTaxDue1stHalf")).GetAttribute("innerText").ToString();
                        fstHalfInterestDue = driver.FindElement(By.Id("cphMainContent_cphRightColumn_lblInterestDue1stHalf")).GetAttribute("innerText").ToString();
                        fstHalfFeesDue = driver.FindElement(By.Id("cphMainContent_cphRightColumn_lblFeesDue1stHalf")).GetAttribute("innerText").ToString();
                        delqFirstHalf = driver.FindElement(By.Id("cphMainContent_cphRightColumn_lblTotalDue1stHalf")).GetAttribute("innerText").ToString();

                        secHalfTaxDue = driver.FindElement(By.Id("cphMainContent_cphRightColumn_lblTaxDue2ndHalf")).GetAttribute("innerText").ToString();
                        secHalfInterestDue = driver.FindElement(By.Id("cphMainContent_cphRightColumn_lblInterestDue2ndHalf")).GetAttribute("innerText").ToString();
                        secHalfFeesDue = driver.FindElement(By.Id("cphMainContent_cphRightColumn_lblFeesDue2ndHalf")).GetAttribute("innerText").ToString();
                        delqSecHalf = driver.FindElement(By.Id("cphMainContent_cphRightColumn_lblTotalDue2ndHalf")).GetAttribute("innerText").ToString();

                        totalTaxDue = driver.FindElement(By.Id("cphMainContent_cphRightColumn_lblTaxDue")).GetAttribute("innerText").ToString();
                        totalInterestDue = driver.FindElement(By.Id("cphMainContent_cphRightColumn_lblInterestDue")).GetAttribute("innerText").ToString();
                        totalFeesDue = driver.FindElement(By.Id("cphMainContent_cphRightColumn_lblFeesDue")).GetAttribute("innerText").ToString();
                        totalTotDue = driver.FindElement(By.Id("cphMainContent_cphRightColumn_lblTotalDue")).GetAttribute("innerText").ToString();
                        //Tax_Breakdown~1stHalf~2ndHalf~Total
                        string break1 = "TaxDue:" + "~" + fstHalfTaxDue + "~" + secHalfTaxDue + "~" + totalTaxDue;
                        gc.insert_data(orderno, DateTime.Now, Tapn, 44, break1, 1);
                        //db.ExecuteQuery("insert into taxes_paid(orderno,parcel_no,Tax_Breakdown,1stHalf,2ndHalf,Total) values('" + orderno + "','" + Tapn + "','TaxDue:','" + fstHalfTaxDue + "','" + secHalfTaxDue + "','" + totalTaxDue + "')");
                        string break2 = "Interest Due:" + "~" + fstHalfInterestDue + "~" + secHalfInterestDue + "~" + totalInterestDue;
                        gc.insert_data(orderno, DateTime.Now,Tapn, 44, break2, 1);
                        //db.ExecuteQuery("insert into taxes_paid(orderno,parcel_no,Tax_Breakdown,1stHalf,2ndHalf,Total) values('" + orderno + "','" + Tapn + "','Interest Due:','" + fstHalfInterestDue + "','" + secHalfInterestDue + "','" + totalInterestDue + "')");
                        string break3 = "Total Due:" + "~" + fstHalfFeesDue + "~" + secHalfFeesDue + "~" + totalFeesDue;
                        gc.insert_data(orderno, DateTime.Now, Tapn, 44, break3, 1);
                        //db.ExecuteQuery("insert into taxes_paid(orderno,parcel_no,Tax_Breakdown,1stHalf,2ndHalf,Total) values('" + orderno + "','" + Tapn + "','Fees Due:','" + fstHalfFeesDue + "','" + secHalfFeesDue + "','" + totalFeesDue + "')");
                        string break4 = "Fees Due" + "~" + delqFirstHalf + "~" + delqSecHalf + "~" + totalTotDue;
                        gc.insert_data(orderno, DateTime.Now,Tapn, 44, break4, 1);
                        // db.ExecuteQuery("insert into taxes_paid(orderno,parcel_no,Tax_Breakdown,1stHalf,2ndHalf,Total) values('" + orderno + "','" + Tapn + "','Total Due:','" + delqFirstHalf + "','" + delqSecHalf + "','" + totalTotDue + "')");

                    }
                    else
                    {
                        displayTaxDue(driver, totalDue, txYear);
                    }
                    CreatePdf(orderno, Tapn, "taxDue");

                    IList<IWebElement> actLinks = driver.FindElements(By.TagName("a"));

                    foreach (IWebElement lnk in actLinks)
                    {

                        string attribute = lnk.GetAttribute("href");
                        if (lnk.Text != null)
                        {

                            if (lnk.Text.Contains("Tax Information"))
                            {
                                Actions actions = new Actions(driver);
                                actions.MoveToElement(lnk);
                                actions.Click();
                                    //lnk.SendKeys(Keys.Enter);
                                
                            }
                        }
                        if (attribute != null)
                        {
                            if (attribute.Contains("https://treasurer.maricopa.gov/Parcel/Activities.aspx"))
                            {
                                driver.Navigate().GoToUrl("https://treasurer.maricopa.gov/Parcel/Activities.aspx");
                                break;
                            }
                        }

                    }
                    string taxyear = "", description = "", amount = "", actdate = "", paydate = "", transaction = "";
                    try
                    {


                        var actv = driver.FindElement(By.XPath(".//*[@id='cphMainContent_cphRightColumn_activityList_gvActs']"));


                        List<IWebElement> actvv = driver.FindElements(By.XPath(".//*[@id='cphMainContent_cphRightColumn_activityList_gvActs']/tbody/tr")).ToList();
                        string pageacti = actv.GetAttribute("innerHTML");

                        HtmlDocument docact = new HtmlDocument();
                        docact.LoadHtml(pageacti);
                        foreach (HtmlNode tableact in docact.DocumentNode.SelectNodes("//tbody"))
                            foreach (HtmlNode rowact in tableact.SelectNodes("tr"))
                            {
                                string rtax = rowact.InnerText.Trim();

                                List ls = new List();

                                foreach (HtmlNode rowcolum in rowact.SelectNodes("td"))
                                {
                                    ls.Add(rowcolum.InnerText.ToString());

                                }

                                //  No activity records.

                                taxyear = ls.Chunks[0].ToString().Trim();
                                if (taxyear != "No activity records.")
                                {

                                    description = ls.Chunks[1].ToString().Trim();
                                    amount = ls.Chunks[2].ToString().Trim();
                                    actdate = ls.Chunks[3].ToString().Trim();
                                    paydate = ls.Chunks[4].ToString().Trim();
                                    transaction = ls.Chunks[5].ToString().Trim();

                                    if (taxyear != "&nbsp;" && description != "&nbsp;")
                                    {
                                        //TaxYear~Description~Amount~ActivityDate~PaymentDate~Transaction
                                        string activity = taxyear + "~" + description + "~" + amount + "~" + actdate + "~" + paydate + "~" + transaction;
                                        gc.insert_data(orderno, DateTime.Now, Tapn, 45, activity, 1);
                                        //db.ExecuteQuery("insert into activities(apn,TaxYear,Description,Amount,ActivityDate,PaymentDate,Transaction,status,orderno,address) values ('" + Tapn + "','" + taxyear + "','" + description + "','" + amount + "','" + actdate + "','" + paydate + "','" + transaction + "','0','" + orderno + "','" + address + "')");
                                    }
                                }
                            }
                    }
                    catch
                    {
                        driver.Navigate().GoToUrl("https://treasurer.maricopa.gov/");
                        Tapn = treasurerAPN[t].ToString();
                        driver.FindElement(By.Id("txtParcelNumBook")).SendKeys(Tapn);
                        driver.FindElement(By.Id("btnGo")).Click();
                        Thread.Sleep(1000);
                        IList<IWebElement> actLinks1 = driver.FindElements(By.TagName("a"));

                        foreach (IWebElement lnk in actLinks1)
                        {
                            string attribute = lnk.GetAttribute("href");
                            if (lnk.Text != null)
                            {
                                if (lnk.Text.Contains("Tax Information"))
                                {
                                    lnk.Click();
                                }
                            }
                            if (attribute != null)
                            {
                                if (attribute.Contains("https://treasurer.maricopa.gov/Parcel/Activities.aspx"))
                                {
                                    driver.Navigate().GoToUrl("https://treasurer.maricopa.gov/Parcel/Activities.aspx");
                                    break;
                                }
                            }

                        }
                        var actv = driver.FindElement(By.XPath(".//*[@id='cphMainContent_cphRightColumn_activityList_gvActs']"));

                        List<IWebElement> actvv = driver.FindElements(By.XPath(".//*[@id='cphMainContent_cphRightColumn_activityList_gvActs']/tbody/tr")).ToList();
                        string pageacti = actv.GetAttribute("innerHTML");

                        HtmlDocument docact = new HtmlDocument();
                        docact.LoadHtml(pageacti);
                        foreach (HtmlNode tableact in docact.DocumentNode.SelectNodes("//tbody"))
                            foreach (HtmlNode rowact in tableact.SelectNodes("tr"))
                            {
                                string rtax = rowact.InnerText.Trim();

                                List ls = new List();

                                foreach (HtmlNode rowcolum in rowact.SelectNodes("td"))
                                {
                                    ls.Add(rowcolum.InnerText.ToString());

                                }

                                taxyear = ls.Chunks[0].ToString().Trim();
                                if (taxyear != "No activity records.")
                                {
                                    description = ls.Chunks[1].ToString().Trim();
                                    amount = ls.Chunks[2].ToString().Trim();
                                    actdate = ls.Chunks[3].ToString().Trim();
                                    paydate = ls.Chunks[4].ToString().Trim();
                                    transaction = ls.Chunks[5].ToString().Trim();

                                    if (taxyear != "&nbsp;" && description != "&nbsp;")
                                    {
                                        string activity = taxyear + "~" + description + "~" + amount + "~" + actdate + "~" + paydate + "~" + transaction;
                                        gc.insert_data(orderno, DateTime.Now, Tapn, 45, activity, 1);
                                        //db.ExecuteQuery("insert into activities(apn,TaxYear,Description,Amount,ActivityDate,PaymentDate,Transaction,status,orderno,address) values ('" + Tapn + "','" + taxyear + "','" + description + "','" + amount + "','" + actdate + "','" + paydate + "','" + transaction + "','0','" + orderno + "','" + address + "')");
                                    }
                                }
                            }
                    }
                    CreatePdf(orderno, Tapn, "Activities");
                    //primary district info....
                    string rate = "", twntyFive = "", twntySix = "", change = "";
                    if (!LinkDetailed(driver))
                    {
                        DetailedStatementDisplyed(driver);
                    }

                    if (StateCreditDisplayed(driver))
                    {
                        //primary_tax....
                        string primary_tax = driver.FindElement(By.Id("cphMainContent_cphRightColumn_dtlTaxBill_lblPrimaryTotalRate")).GetAttribute("innerText").ToString();
                        //secondary_tax....
                        string secondary_tax = driver.FindElement(By.Id("cphMainContent_cphRightColumn_dtlTaxBill_lblSecondaryTotalRate")).GetAttribute("innerText").ToString();
                        //school_district....
                        string special_district = driver.FindElement(By.Id("cphMainContent_cphRightColumn_dtlTaxBill_gvSpecialDistricts")).GetAttribute("innerText").ToString();
                        special_district = Between(special_district, "TOTAL FOR SPECIAL DISTRICTS", "$").TrimStart().TrimEnd();
                        special_district = Before(special_district, "$");
                        //exemption....
                        string exemption_value = driver.FindElement(By.Id("cphMainContent_cphRightColumn_dtlTaxBill_lblSecondaryExemptionValue")).GetAttribute("innerText").ToString();

                        //db.ExecuteQuery("insert into exemption(exemption,primary_tax,secondary_tax,special_district) values ('" + exemption_value + "','" + primary_tax + "','" + secondary_tax + "','" + special_district + "')");

                        IWebElement tableElement = driver.FindElement(By.Id("cphMainContent_cphRightColumn_dtlTaxBill_gvPrimaryDistricts"));
                        IList<IWebElement> tableRow = tableElement.FindElements(By.TagName("tr"));
                        IList<IWebElement> rowTD;
                        foreach (IWebElement row in tableRow)
                        {
                            rowTD = row.FindElements(By.TagName("td"));

                            if (row.Text.Contains("Tax District"))
                            {
                                string fullYear = Between(row.Text, "100", "Change");
                                fullYear = fullYear.TrimStart().TrimEnd();
                                string[] yearList = fullYear.Split(' ');
                                GlobalClass.stateYear1 = yearList[0];
                                GlobalClass.stateYear2 = yearList[1];
                            }

                            if (rowTD.Count > 0)
                            {
                                if (rowTD[0].Text.Equals("State Aid"))
                                {
                                    rate = rowTD[1].Text;
                                    twntyFive = rowTD[2].Text;
                                    twntySix = rowTD[3].Text;
                                    change = rowTD[4].Text;
                                    break;
                                }

                            }
                        }
                        string district = "State Aid" + "~" + rate + "~" + twntyFive + "~" + twntySix + "~" + change;
                        gc.insert_data(orderno, DateTime.Now, Tapn, 46, district, 1);
                        //db.ExecuteQuery("insert into district_info(orderno,parcel_no,tax_district,rate,Five,Six,change_value) values ('" + orderno + "','" + Tapn + "','State Aid','" + rate + "','" + twntyFive + "','" + twntySix + "','" + change + "')");
                    } 
                    CreatePdf(orderno, Tapn, "Statecredit");

                }

            }

            #endregion

            driver.Quit();
            return;
        }

        public void Insert_Valuation_Info(string Taxyear, string Fullcash, string Ltdproperty, string Legalclass, string Description, string Assratio, string Assfcv, string Asslpv, string Propusecode, string Pudescription, string Taxareacode, string Valuationsrc, string apN, string address, string orderno)
        {
            MySqlCommand cmd;

            try
            {
                con.Open();
                string pp = "sp_insert_valuation_info";
                cmd = new MySqlCommand(pp, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("$TaxYear", Taxyear);
                cmd.Parameters.AddWithValue("$FullCash", Fullcash);
                cmd.Parameters.AddWithValue("$LimitedProperty", Ltdproperty);
                cmd.Parameters.AddWithValue("$LegalClass", Legalclass);
                cmd.Parameters.AddWithValue("$Description", Description);
                cmd.Parameters.AddWithValue("$AssRatio", Assratio);
                cmd.Parameters.AddWithValue("$AssFCV", Assfcv);
                cmd.Parameters.AddWithValue("$AssLPV", Asslpv);
                cmd.Parameters.AddWithValue("$PropertyUseCode", Propusecode);
                cmd.Parameters.AddWithValue("$PuDescription", Pudescription);
                cmd.Parameters.AddWithValue("$TaxAreaCode", Taxareacode);
                cmd.Parameters.AddWithValue("$ValuationSource", Valuationsrc);
                cmd.Parameters.AddWithValue("$apn", apN);
                cmd.Parameters.AddWithValue("$address", address);
                cmd.Parameters.AddWithValue("$orderno", orderno);

                int res = cmd.ExecuteNonQuery();
                con.Close();

            }
            catch
            {
                con.Close();
                con.Dispose();
            }

        }

        public void Insert_Real_Property(string APN1, string owner1, string address1, string subdivision1, string mcr1, string str1, string orderno)
        {
            MySqlCommand cmd;

            try
            {
                con.Open();
                string pp = "sp_insert_real_property";
                cmd = new MySqlCommand(pp, con);
                //Owner1~Address1~subdivision1~mcr1~str1 
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("$apn", APN1);
                cmd.Parameters.AddWithValue("$owner", owner1);
                cmd.Parameters.AddWithValue("$address", address1);
                cmd.Parameters.AddWithValue("$subdivision", subdivision1);
                cmd.Parameters.AddWithValue("$mcr", mcr1);
                cmd.Parameters.AddWithValue("$str", str1);
                cmd.Parameters.AddWithValue("$orderno", orderno);
                int res = cmd.ExecuteNonQuery();
                con.Close();

            }
            catch
            {
                con.Close();
                con.Dispose();
            }

        }

        public static void ConvertImageToPdf(string srcFilename, string dstFilename)
        {
            iTextSharp.text.Rectangle pageSize = null;

            using (var srcImage = new System.Drawing.Bitmap(srcFilename))
            {
                pageSize = new iTextSharp.text.Rectangle(0, 0, srcImage.Width, srcImage.Height);
            }
            using (var ms = new MemoryStream())
            {
                var document = new iTextSharp.text.Document(pageSize, 0, 0, 0, 0);
                iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms).SetFullCompression();
                document.Open();
                var image = iTextSharp.text.Image.GetInstance(srcFilename);
                document.Add(image);
                document.Close();

                File.WriteAllBytes(dstFilename, ms.ToArray());
            }
        }
        public bool HomeSearchDisplayed(IWebDriver Wdriver)
        {
            try
            {
                Wdriver.Navigate().GoToUrl("https://mcassessor.maricopa.gov/");
                var wait = new WebDriverWait(Wdriver, TimeSpan.FromSeconds(5));
                var myElement = wait.Until(x => x.FindElement(By.ClassName("homeSearchField")));
                return myElement.Displayed;
            }
            catch
            {
                return false;
            }
        }

        public bool ActivitiesDisplayed(IWebDriver Wdriver)
        {
            try
            {
                ((IJavaScriptExecutor)Wdriver).ExecuteScript("arguments[0].click();", Wdriver.FindElement(By.Id("linkActivities")));
                Thread.Sleep(5000);
                var wait = new WebDriverWait(Wdriver, TimeSpan.FromSeconds(5));
                var myElement = wait.Until(x => x.FindElement(By.XPath(".//*[@id='cphMainContent_cphRightColumn_activityList_gvActs']")));
                return myElement.Displayed;
            }
            catch
            {
                Wdriver.Navigate().GoToUrl("http://treasurer.maricopa.gov/");
                Wdriver.FindElement(By.Id("txtParcelNumBook")).SendKeys(Tapn);
                Wdriver.FindElement(By.Id("btnGo")).Click();
                ((IJavaScriptExecutor)Wdriver).ExecuteScript("arguments[0].click();", Wdriver.FindElement(By.Id("linkActivities")));
                return true;
            }
        }

        public bool StateCreditDisplayed(IWebDriver Wdriver)
        {
            try
            {
                Wdriver.FindElement(By.Id("linkDetailedStatement")).Click();
                var wait = new WebDriverWait(Wdriver, TimeSpan.FromSeconds(5));
                var myElement = wait.Until(x => x.FindElement(By.Id("cphMainContent_cphRightColumn_dtlTaxBill_gvPrimaryDistricts")));
                return myElement.Displayed;
            }
            catch
            {
                return false;
            }
        }

        public bool DetailedStatementDisplyed(IWebDriver Wdriver)
        {
            try
            {
                Wdriver.Navigate().GoToUrl("http://treasurer.maricopa.gov/");
                Wdriver.FindElement(By.Id("txtParcelNumBook")).SendKeys(Tapn);
                Wdriver.FindElement(By.Id("btnGo")).Click();
                IWebElement elementddd = Wdriver.FindElement(By.Id("linkDetailedStatement"));
                String js1d = "arguments[0].style.height='auto'; arguments[0].style.visibility='visible';";

                IJavaScriptExecutor executor1d = (IJavaScriptExecutor)Wdriver;
                executor1d.ExecuteScript(js1d, elementddd);

                var wait = new WebDriverWait(Wdriver, TimeSpan.FromSeconds(5));
                var myElement = wait.Until(x => x.FindElement(By.Id("linkDetailedStatement")));
                return myElement.Displayed;
            }
            catch
            {
                return false;
            }
        }

        public bool LinkDetailed(IWebDriver Wdriver)
        {
            try
            {
                var wait = new WebDriverWait(Wdriver, TimeSpan.FromSeconds(5));
                var myElement = wait.Until(x => x.FindElement(By.Id("linkDetailedStatement")));
                return myElement.Displayed;
            }
            catch
            {
                return false;
            }
        }

        public bool displayValuationInfo(IWebDriver Wdriver)
        {
            try
            {
                string url = "https://mcassessor.maricopa.gov/mcs.php?q=" + apN + "&mod=pd";
                driver.Navigate().GoToUrl(url);
                Thread.Sleep(5000);
                var wait = new WebDriverWait(Wdriver, TimeSpan.FromSeconds(5));
                var myElement = wait.Until(x => x.FindElement(By.XPath(".//*[@class='ui definition selectable fluid unstackable table']")));
                return myElement.Displayed;
            }
            catch
            {
                return false;
            }
        }

        public bool displayTaxDue(IWebDriver Wdriver, string due, string year)
        {
            try
            {
                Wdriver.FindElement(By.LinkText(due)).Click();
                var wait = new WebDriverWait(Wdriver, TimeSpan.FromSeconds(5));
                var myElement = wait.Until(x => x.FindElement(By.Id("cphMainContent_cphRightColumn_datePicker")));

                if (!myElement.Displayed)
                {
                    string url = "https://treasurer.maricopa.gov/Parcel/TaxDue.aspx?TaxYear=" + year;
                    driver.Navigate().GoToUrl(url);
                    wait = new WebDriverWait(Wdriver, TimeSpan.FromSeconds(5));
                    myElement = wait.Until(x => x.FindElement(By.Id("cphMainContent_cphRightColumn_datePicker")));
                }
                return myElement.Displayed;
            }
            catch
            {
                return false;
            }
        }

        public static string Between(string value, string a, string b)
        {
            int posA = value.IndexOf(a);
            int posB = value.LastIndexOf(b);
            if (posA == -1)
            {
                return "";
            }
            if (posB == -1)
            {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= posB)
            {
                return "";
            }
            return value.Substring(adjustedPosA, posB - adjustedPosA);
        }

        public static string After(string value, string a)
        {
            int posA = value.LastIndexOf(a);
            if (posA == -1)
            {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= value.Length)
            {
                return "";
            }
            return value.Substring(adjustedPosA);
        }

        public static string Before(string value, string a)
        {
            int posA = value.IndexOf(a);
            if (posA == -1)
            {
                return "";
            }
            return value.Substring(0, posA);
        }

        private void ForMultipleParcel(string orderno)
        {
            string owner1 = "-", address1 = "-", subdivision11 = "-", mcr1 = "-", str1 = "-";
            GlobalClass.multipleParcel = "Yes";
            for (int a = 0; a < APN.Count(); a++)
            {

                List<string> names = APN[a].Split('|').ToList<string>();
                string APN1 = names[0].ToString();
                APN1 = APN1.Trim();
                owner1 = names[1].ToString();
                address1 = names[2].ToString();
                subdivision11 = names[3].ToString();
                mcr1 = names[4].ToString();
                str1 = names[5].ToString();
                if (subdivision11.Contains("."))
                {
                    string url = "https://mcassessor.maricopa.gov/mcs.php?q=" + APN1 + "&mod=pd";
                    driver.Navigate().GoToUrl(url);

                    IList<IWebElement> subLinks = driver.FindElements(By.TagName("a"));
                    foreach (IWebElement lnk in subLinks)
                    {

                        string attribute = lnk.GetAttribute("href");
                        if (attribute != null)
                        {

                            if (attribute.Contains("search-type=sub&mod=sm"))
                            {
                                subdivision11 = lnk.Text;
                                break;
                            }
                        }
                    }
                }
                //Owner1~Address1~subdivision1~mcr1~str1~conto_owner~legal_description
                string property = owner1 + "~" + address1 + "~" + subdivision11 + "~" + mcr1 + "~" + str1 + "~" + conto_owner + "~" + legal_description;
                gc.insert_data(orderno, DateTime.Now, APN1,49, property, 1);
                //Insert_Real_Property(APN1, owner1, address1, subdivision11, mcr1, str1, orderno);
            }
        }

        public static void Hover(IWebDriver webdriver, IWebElement element)
        {
            Actions action = new Actions(webdriver);
            action.MoveToElement(element).Perform();
        }

        public static void HoverAndClick(IWebDriver driver, IWebElement elementToHover, IWebElement elementToClick)
        {
            Actions action = new Actions(driver);
            action.MoveToElement(elementToHover).Click(elementToClick).Build().Perform();
        }
        public void CreatePdf(string orderno, string parcelno, string pdfName)
        {
            outputPath = "";
            outputPath = ConfigurationManager.AppSettings["screenShotPath"];
            outputPath = outputPath + orderno + "\\" + parcelno + "\\";
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            string img = outputPath + pdfName + ".png";
            string pdf = outputPath + pdfName + ".pdf";

            driver.Manage().Window.Maximize();
            driver.TakeScreenshot().SaveAsFile(img, ScreenshotImageFormat.Png);

            WebDriverTest.ConvertImageToPdf(img, pdf);
            if (File.Exists(img))
            {
                File.Delete(img);
            }

        }

        public void CreatePdf_WOP(string orderno, string pdfName)
        {
            outputPath = "";
            outputPath = ConfigurationManager.AppSettings["screenShotPath"];
            outputPath = outputPath + orderno + "\\";
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            string img = outputPath + pdfName + ".png";
            string pdf = outputPath + pdfName + ".pdf";

            driver.Manage().Window.Maximize();
            driver.TakeScreenshot().SaveAsFile(img, ScreenshotImageFormat.Png);

            WebDriverTest.ConvertImageToPdf(img, pdf);
            if (File.Exists(img))
            {
                File.Delete(img);
            }

        }

    }
}