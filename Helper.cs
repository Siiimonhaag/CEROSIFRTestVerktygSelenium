using System;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using WebDriverManager;
using OpenQA.Selenium.Support.UI;
using Xunit;
using Xunit.Abstractions;
using System.Threading;
using System.Collections.Generic;

namespace CEROSIFRTestVerktygSelenium
{
    public class Helper
    {
        public IWebDriver driver;
        WebDriverWait wait;
        public Helper(IWebDriver _driver)
        {
            driver = _driver;
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }
        public void LogInToWebsite(string email, string password)
        {
            IWebElement logIn = driver.FindElement(By.XPath("//a[@title='Logga in / Mitt Coop']"));
            logIn.Click();
            driver.Manage().Window.FullScreen();
            Thread.Sleep(1500);

            IWebElement enterEmail = driver.FindElement(By.XPath("//input[@id='loginEmail']"));
            enterEmail.SendKeys(email);

            IWebElement enterPassword = driver.FindElement(By.XPath("//input[@id='loginPassword']"));
            enterPassword.SendKeys(password);

            IWebElement login2 = driver.FindElement(By.XPath("//button[@type='submit']"));
            login2.Click();
            Thread.Sleep(3000);
        }

        public void EmptyTheCart()
        {
            var emptyBasketButton = wait.Until(driver =>
            driver.FindElement(By.CssSelector("button[data-test*=emptycartbutton]")));
            Thread.Sleep(1200);
            emptyBasketButton.Click();

            var confirmButton = wait.Until(driver =>
            driver.FindElement(By.XPath(
                "//div[contains(@class, 'Cart-notice')]/div/button[contains(text(), 'Töm')]")));
            Thread.Sleep(1200);
            confirmButton.Click();
            Thread.Sleep(500);
        }
    }
}
