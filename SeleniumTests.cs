using System;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using WebDriverManager;
using OpenQA.Selenium.Support;
using Xunit;
using System.Threading;

namespace CEROSIFRTestVerktygSelenium
{

    public class SeleniumTests
    {

        //ChromeDriver driver = new ChromeDriver();
        ChromeDriver driver = new ChromeDriver(@"C:\Users\simon\Downloads");
        DriverManager driverManager = new DriverManager();
        string url = "https://www.coop.se/";

        public SeleniumTests()
        {

            // Konstruktor för att
            // Klicka på acceptera kakor varje gång vi kör ett test
            // Båda click nedan funkar!
            driver.Navigate().GoToUrl(url);
            driver.Manage().Window.FullScreen();
            Thread.Sleep(2000);
            driver.FindElement(By.XPath("//*[@id='cmpbntyestxt']")).Click();

        }

        [Fact]
        public void FullscreenAndSearchForKetchup()
        {
            
            IWebElement handlaOnline = driver.FindElement(By.LinkText("Handla online"));
            handlaOnline.Click();
            driver.Manage().Window.FullScreen();
            Thread.Sleep(1000);
            IWebElement searchBar = driver.FindElement(By.ClassName("Search-input"));
            searchBar.SendKeys("Ketchup");
            Thread.Sleep(2000);
            searchBar.SendKeys(Keys.Enter);
            Thread.Sleep(1500);
            driver.Quit();
            driver.Dispose();

        }

        [Fact]
        public void ThisIsTheFirstTestAgainJustForShowcase()
        {
            IWebElement handlaOnline = driver.FindElement(By.LinkText("Handla online"));
            handlaOnline.Click();
            driver.Manage().Window.FullScreen();
            Thread.Sleep(1000);
            IWebElement searchBar = driver.FindElement(By.ClassName("Search-input"));
            searchBar.SendKeys("Ketchup");
            Thread.Sleep(2000);
            searchBar.SendKeys(Keys.Enter);
            Thread.Sleep(1500);
            driver.Quit();
            driver.Dispose();
        }
        /*
        [Fact]
        public void Test3()
        {

        }

        [Fact]
        public void Test4()
        {

        }*/
    }
}
