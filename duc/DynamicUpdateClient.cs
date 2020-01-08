using duc.Helpers;
using duc.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace duc
{
    public class DynamicUpdateClient
    {
        #region Variables

        private IWebDriver Driver { get; set; }
        private string Customerid { get; set; }
        private string ResellerId { get; set; }
        private string DriverPath { get; set; }
        private string DriverExecutableFileName { get; set; }
        private ArgOptions ClientOptions { get; }
        private int defaultWaitTimeout = 40;

        #endregion Variables

        #region Constructor

        public DynamicUpdateClient(ArgOptions options)
        {
            ClientOptions = options;
            if (options.Timeout > 0)
            {
                defaultWaitTimeout = options.Timeout;
            }
            SetupDriver();
        }

        //Tear Down
        ~DynamicUpdateClient()
        {
            Driver?.Quit();
        }

        #endregion Constructor

        #region Private Methods

        private void SetupDriver()
        {
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            var options = new ChromeOptions();

            if (isWindows)
            {
                //Configuration to Windows
                DriverPath = @"C:\Temp\chromedriver\";
                DriverExecutableFileName = "chromedriver.exe";
            }
            else
            {
                //Configuration to Linux - Container
                DriverPath = "/opt/selenium/";
                DriverExecutableFileName = "chromedriver";
                options.BinaryLocation = "/opt/google/chrome/chrome";
            }

            ChromeDriverService service = ChromeDriverService.CreateDefaultService(DriverPath, DriverExecutableFileName);

            if (!ClientOptions.Debug)
            {
                options.AddArgument("--log-level=3"); //Silent Mode
                service.SuppressInitialDiagnosticInformation = true;
            }

            if (ClientOptions.WindowMode)
            {
                options.AddArguments("window-size=800x600");
            }
            else
            {
                options.AddArguments("headless");
                options.AddArguments("no-sandbox");
            }

            //Bypass ssl errors
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--ignore-ssl-errors");
            options.AddArgument("--ignore-certificate-errors-spki-list");
            options.AcceptInsecureCertificates = true;

            //Generic settings
            Driver = new ChromeDriver(service, options, TimeSpan.FromSeconds(defaultWaitTimeout));
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(defaultWaitTimeout);

            if (ClientOptions.Verbose)
            {
                Console.WriteLine($"OS is: {RuntimeInformation.OSDescription}");
                Console.WriteLine($"Driver path: {DriverPath}");
                if (!isWindows)
                {
                    Console.WriteLine($"Chrome binary path: {options.BinaryLocation}");
                }
            }
        }

        private void LoginToControlPanel()
        {
            if (ClientOptions.Verbose)
            {
                Console.WriteLine($"Logging in to {ClientOptions.Url}");
            }

            this.Driver.Navigate().GoToUrl(ClientOptions.Url);

            //Wait login page to be fully loaded
            Driver.WaitForElementByXPath("//h2[contains(text(),\"Control Panel Login\")]", defaultWaitTimeout);

            if (ClientOptions.Verbose)
            {
                Console.WriteLine($"Login paged loaded: {ClientOptions.Url}");
            }

            //Get and Send username and Password
            var userNameElement = this.Driver.FindElement(By.Id("login-username"));
            var passwordElement = this.Driver.FindElement(By.Id("login-password"));
            userNameElement.SendKeys(ClientOptions.Username);
            passwordElement.SendKeys(ClientOptions.Password);

            //Get and Select Customer role
            var roleElement = this.Driver.FindElement(By.Id("login-role"));
            var selectElement = new SelectElement(roleElement);
            selectElement.SelectByValue("customer");

            //Get and Click Submit
            if (ClientOptions.Verbose)
            {
                Console.WriteLine($"Ready to submit login");
            }

            if (ClientOptions.WindowMode)
            {
                var submitButtonElement = Driver.FindElement(By.ClassName("login-submit"));
                submitButtonElement.Click();
            }
            else
            {
                var loginForm = Driver.FindElement(By.Name("loginform"));
                loginForm.Submit();
            }

            //Wait for continue to sign in page element
            Driver.WaitForElementById("control-panel-menu", defaultWaitTimeout);

            if (ClientOptions.Verbose)
            {
                Console.WriteLine("Login successful");
            }

            //Find Customer Id
            Customerid = Driver.FindElement(By.ClassName("profile-menu-user-id")).GetAttribute("innerHTML").Replace("Customer Id: ", "");
            if (ClientOptions.Verbose)
            {
                Console.WriteLine($"Customer Id found: {Customerid}");
            }
        }

        private void UpdateDnsRecords()
        {
            //Check if domainId is supplied, otherwise find it
            if (string.IsNullOrEmpty(ClientOptions.DomainId))
            {
                var ordersUrl = $"{ClientOptions.Url}/servlet/ListAllOrdersServlet?formaction=listOrders";
                this.Driver.Navigate().GoToUrl(ordersUrl);

                if (ClientOptions.Verbose)
                {
                    Console.WriteLine($"Navigated to {ordersUrl}");
                }

                //Wait for List of Orders
                Driver.WaitForElementByClassName("domain-table-body", defaultWaitTimeout);

                //Find Table of domain orders
                var domainOrdersList = Driver.FindElement(By.ClassName("table-grid"));
                var order = domainOrdersList.FindElement(By.XPath($"//a[contains(text(),\"{ClientOptions.DomainName}\")]"));
                var orderUrlPrepend = $"{ClientOptions.Url}servlet/ViewOrderServlet?orderid=";
                var orderId = order.GetAttribute("href").Replace(orderUrlPrepend, "");
                ClientOptions.DomainId = orderId;
                if (ClientOptions.Verbose)
                {
                    Console.WriteLine($"Domain {ClientOptions.DomainName} Order Id found: {orderId}");
                }
            }

            //Navigate to domain management page
            var domainManagementUrl = $"{ClientOptions.Url}/servlet/ViewOrderServlet?orderid={ClientOptions.DomainId}";
            this.Driver.Navigate().GoToUrl(domainManagementUrl);

            if (ClientOptions.Verbose)
            {
                Console.WriteLine($"Navigated to {domainManagementUrl}");
            }

            //Wait for dns management element
            Driver.WaitForElementById("dnsbox-information", defaultWaitTimeout);

            //Search for the Reseller Ids
            var scriptContent = Driver.FindElements(By.TagName("script"));
            FindResellerId(scriptContent);

            //Manage DNS
            var manageDnsServiceUrl = $"{ClientOptions.Url}/servlet/ManageFreeServiceServlet?productcategory=dnsbox&domainname={ClientOptions.DomainName}&customerid={Customerid}&resellerid={ResellerId}&orderid={ClientOptions.DomainId}";
            this.Driver.Navigate().GoToUrl(manageDnsServiceUrl);

            if (ClientOptions.Verbose)
            {
                Console.WriteLine($"Navigated to {manageDnsServiceUrl}");
            }

            //Find Table of domain aliasses
            var form = Driver.FindElement(By.Name("ListForm"));
            var links = form.FindElements(By.XPath($"//a[contains(text(),\"{ClientOptions.DomainName}\")]"));

            for (int i = 0; i < links.Count; i++)
            {
                var item = links[i];
                UpdateARecord(item);

                //Refresh the page elements as a redirect occured
                form = Driver.FindElement(By.Name("ListForm"));
                links = form.FindElements(By.XPath($"//a[contains(text(),\"{ClientOptions.DomainName}\")]"));
            }
        }

        private void FindResellerId(ReadOnlyCollection<IWebElement> scriptContent)
        {
            foreach (var item in scriptContent)
            {
                var scriptBlock = item.GetAttribute("innerHTML").Replace("\r\n", "").Replace("\t", "").Trim();

                if (!string.IsNullOrEmpty(scriptBlock) && scriptBlock.Contains("loadFreeProductSection('dnsbox'"))
                {
                    var dnsBoxItems = scriptBlock.GetBetween($"loadFreeProductSection('dnsbox', '{ClientOptions.DomainName}'", "false);").Split(',');

                    ResellerId = dnsBoxItems[1].Replace("'", "").Trim();
                    if (ClientOptions.Verbose)
                    {
                        Console.WriteLine($"Reseller Id found: {ResellerId}");
                    }

                    break;
                }
            }
        }

        private void UpdateARecord(IWebElement item)
        {
            if (item != null)
            {
                var dnsUrl = item.GetAttribute("href");
                var dnsText = item.Text;
                if (ClientOptions.Verbose)
                {
                    Console.WriteLine($"Updating Dns IP for Entry: {dnsText} to {ClientOptions.Ip}");
                }

                //update the dns A Record
                this.Driver.Navigate().GoToUrl(dnsUrl);

                //Click the modify record button btnModRecord
                var modBtn = Driver.FindElement(By.Name("btnModRecord"));
                modBtn.Click();

                //Wait for dns management element
                Driver.WaitForElementByXPath("//font [contains(text(),\"Modify A Record\")]", defaultWaitTimeout);

                //Get ipV4 text box
                var ipBox = Driver.FindElement(By.Name("IPvalue"));
                ipBox.Clear();
                ipBox.SendKeys(ClientOptions.Ip);

                //Find modify Record Button
                var modifyRecordBtn = Driver.FindElement(By.Name("submitform"));
                modifyRecordBtn.Click();
            }
        }

        #endregion Private Methods

        public void UpdateIp()
        {
            try
            {
                LoginToControlPanel();

                UpdateDnsRecords();

                if (ClientOptions.Verbose)
                {
                    Console.WriteLine($"Update domain {ClientOptions.DomainName} A Records successful.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Breaking Error: {ex.Message}");
                //throw;
            }
            finally
            {
                Driver?.Close();
            }
        }
    }
}