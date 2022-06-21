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

    public class SeleniumTests
    {
        ITestOutputHelper testOutput;
        ChromeDriver driver = new ChromeDriver();
        DriverManager driverManager = new DriverManager();
        string url = "https://www.coop.se/";

        public SeleniumTests(ITestOutputHelper _testOutput)
        {

            // Konstruktor för att
            // Klicka på acceptera kakor varje gång vi kör ett test
            // Båda click nedan funkar!
            testOutput = _testOutput;
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
        [Trait("User story ID 1","Input, Button, Anchor")]
        public void ChangeQuantityInTheShoppingCart()
        {

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            IWebElement logIn = driver.FindElement(By.XPath("//a[@title='Logga in / Mitt Coop']"));
            logIn.Click();
            driver.Manage().Window.FullScreen();
            Thread.Sleep(1500);

            IWebElement enterEmail = driver.FindElement(By.XPath("//input[@id='loginEmail']"));
            enterEmail.SendKeys("testcoop123@hotmail.com");

            IWebElement enterPassword = driver.FindElement(By.XPath("//input[@id='loginPassword']"));
            enterPassword.SendKeys("Cerosifr123!");

            IWebElement login2 = driver.FindElement(By.XPath("//button[@type='submit']"));
            login2.Click();
            Thread.Sleep(3000);

            IWebElement handlaOnline = driver.FindElement(By.LinkText("Handla online"));
            handlaOnline.Click();

            driver.Manage().Window.FullScreen();
            Thread.Sleep(1000);

            IWebElement searchBar = driver.FindElement(By.ClassName("Search-input"));
            searchBar.SendKeys("Kronfågel Majskyckling");
            Thread.Sleep(1500);

            searchBar.SendKeys(Keys.Enter);
            Thread.Sleep(2000);

            IWebElement addProduct = driver.FindElement(By.XPath("//a[@aria-label='Bröstfilé Av Majskyckling']"));
            addProduct.Click();
            Thread.Sleep(2500);

            IWebElement plusButton = driver.FindElement(By.XPath("//button[@aria-label='Öka antal']"));
            plusButton.Click();
            Thread.Sleep(2000);


            IWebElement shoppingCart = driver.FindElement(By.XPath("//button[@aria-label='kundvagn']"));
            shoppingCart.Click();
            Thread.Sleep(2000);

            IWebElement increaseQuantity = driver.FindElement(By.XPath("//button[@aria-label='Öka antal']"));
            increaseQuantity.Click();
            Thread.Sleep(2000);
            string actual = driver.FindElement(By.XPath("//input[@type='numeric']")).GetAttribute("value").ToString();
            string expected = "2";
            Assert.Equal(expected,actual);
            testOutput.WriteLine("Added an extra 'Majskyckling' in the shopping cart.\n" +
                "Expected result: 2\n" +
                "Actual result: " + actual);

            for (int i = 0; i < 2; i++)
            {
                IWebElement decreaseQuantity = driver.FindElement(By.XPath("//button[@aria-label='Minska antal']"));
                decreaseQuantity.Click();
                Thread.Sleep(500);

            }
            Thread.Sleep(2000);

            driver.Quit();
            driver.Dispose();
        }
        
        [Fact]
        [Trait("User Story ID 11", "Div")]
        public void ShoppingCartShowCorrectPriceDiscount()
        {
            IWebElement logIn = driver.FindElement(By.XPath("//a[@title='Logga in / Mitt Coop']"));
            logIn.Click();
            driver.Manage().Window.FullScreen();
            Thread.Sleep(1500);

            IWebElement enterEmail = driver.FindElement(By.XPath("//input[@id='loginEmail']"));
            enterEmail.SendKeys("testcoop123@hotmail.com");

            IWebElement enterPassword = driver.FindElement(By.XPath("//input[@id='loginPassword']"));
            enterPassword.SendKeys("Cerosifr123!");

            IWebElement login2 = driver.FindElement(By.XPath("//button[@type='submit']"));
            login2.Click();
            Thread.Sleep(3000);

            int quantity = 0;
            double price = 0;
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            
            IWebElement addIcon;
            IWebElement product = null;
            IWebElement miniCart;

            //double subtractedPrice;
            //double totalPrice;

            var onlineShop = driver.FindElement(By.LinkText("Handla online"));
            onlineShop.Click();

            IList<IWebElement> miniArticles = wait.Until(driver => 
            driver.FindElements(By.ClassName("ItemTeaser-container")));

            foreach (var miniArticle in miniArticles)
            {
                if(miniArticle.Text.Contains("för"))
                {
                    product = miniArticle;
                    string cleanText = miniArticle.Text.
                        Remove(miniArticle.Text.IndexOf("r") + 1, 2).
                        Replace("för", "").
                        Replace(":-", "").
                        Trim();
                        
                    int space = cleanText.IndexOf(" ");

                    quantity = int.Parse(cleanText.Substring(0, space));
                    price = double.Parse(cleanText.Substring(space + 1));

                    testOutput.WriteLine("Kvantitet: " + quantity);
                    testOutput.WriteLine("Pris: " + price);
                    break;
                }
            }
            
            for (int i = 0; i < quantity; i++)
            {
                addIcon = wait.Until(product =>
                product.FindElement(By.CssSelector("button[aria-label='Öka antal']")));
                addIcon.Click();
            }

            //Navigera till kundvagn
            miniCart = wait.Until(driver => 
            driver.FindElement(By.CssSelector("button[aria-label='kundvagn']")));
            miniCart.Click();

            //Hämta totalpriset och rabatterat pris

        }

        [Fact]
        [Trait("User story ID 3","Input")]
        public void Add10pcsDirectlyOnInputOfTheProduct()
        {

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            var HandlaOnline = driver.FindElement(By.LinkText("Handla online"));
            HandlaOnline.Click();

            driver.Manage().Window.FullScreen();
            Thread.Sleep(1000);

            IWebElement Search = driver.FindElement(By.ClassName("Search-input"));
            Search.SendKeys("Ädelost Blå eko");
            Thread.Sleep(1500);

            Search.SendKeys(Keys.Enter);
            Thread.Sleep(2000);

            IWebElement AddAmount = driver.FindElement(By.ClassName("AddToCart-input"));
            AddAmount.SendKeys("10");
            Thread.Sleep(2000);

            IWebElement ClickOk = driver.FindElement(By.ClassName("AddToCart-input"));
            ClickOk.SendKeys(Keys.Enter);
            
            Thread.Sleep(3000);
           
            IList<IWebElement> AddZipCode = driver.FindElements(By.CssSelector("input"));
            int stop = 0;
                       
            foreach (var box in AddZipCode)
                {
                    if (stop == 5)
                        {
                            break;
                        }
                    box.SendKeys("4");
                    Thread.Sleep(500);
                    stop++;
                }
            
            Thread.Sleep(2000);

            IWebElement ClickTime = driver.FindElement(By.ClassName("TimeslotCell-content"));
            ClickTime.SendKeys(Keys.Enter);
            
            Thread.Sleep(2000);

            string actual = driver.FindElement(By.XPath("//input[@type='numeric']")).GetAttribute("value").ToString();
            string expected = "10";
            Assert.Equal(expected, actual);
            testOutput.WriteLine("Added 10pcs of 'Ädelost Blå Eko' in the shopping cart.\n" +
                "Expected result: 10\n" +
                "Actual result: " + actual);

            Thread.Sleep(1000);

            driver.Quit();
            driver.Dispose();
        }

        [Fact]
        [Trait("User story ID 5", "Search")]
        public void NavigateWithSearchForRecipes()
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            IWebElement logIn = driver.FindElement(By.XPath("//a[@title='Logga in / Mitt Coop']"));
            logIn.Click();
            driver.Manage().Window.FullScreen();
            Thread.Sleep(1500);
        }

        [Fact]
        [Trait("User story ID 8", "Search")]
        public void SearchForStoreThatsClose()
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            IWebElement logIn = driver.FindElement(By.XPath("//a[@title='Logga in / Mitt Coop']"));
            logIn.Click();
            driver.Manage().Window.FullScreen();
            Thread.Sleep(1500);
        }
    }
}
