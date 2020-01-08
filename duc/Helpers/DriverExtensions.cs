using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace duc.Helpers
{
    public static class DriverExtensions
    {
        public static void WaitForElementById(this IWebDriver driver, string elementId, int secondsTimeOut = 60)
        {
            var waitForElement = new WebDriverWait(driver, TimeSpan.FromSeconds(secondsTimeOut));
            waitForElement.Until(ExpectedConditions.ElementIsVisible(By.Id(elementId)));
        }

        public static void WaitForElementByCss(this IWebDriver driver, string cssSelector, int secondsTimeOut = 60)
        {
            var waitForElement = new WebDriverWait(driver, TimeSpan.FromSeconds(secondsTimeOut));
            waitForElement.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(cssSelector)));
        }

        public static void WaitForElementByClassName(this IWebDriver driver, string className, int secondsTimeOut = 60)
        {
            var waitForElement = new WebDriverWait(driver, TimeSpan.FromSeconds(secondsTimeOut));
            waitForElement.Until(ExpectedConditions.ElementIsVisible(By.ClassName(className)));
        }

        public static void WaitForElementByXPath(this IWebDriver driver, string xpath, int secondsTimeOut = 60)
        {
            var waitForElement = new WebDriverWait(driver, TimeSpan.FromSeconds(secondsTimeOut));
            waitForElement.Until(ExpectedConditions.ElementIsVisible(By.XPath(xpath)));
        }

        public static string GetItemFromLocalStorage(this IWebDriver driver, string key)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            var value = (string)js.ExecuteScript($"return localStorage.getItem('{key}')");

            return value;
        }

        public static string GetItemFromSessionStorage(this IWebDriver driver, string key)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            var value = (string)js.ExecuteScript($"return sessionStorage.getItem('{key}')");

            return value;
        }

        public static void SetLocalStorage(this IWebDriver driver, string key, string value)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript($"window.localStorage.setItem('{key}','{value}')");
        }

        public static void SetSessionStorage(this IWebDriver driver, string key, string value)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript($"window.sessionStorage.setItem('{key}','{value}')");
        }
    }
}