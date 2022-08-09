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

            // Konstruktor f�r att
            // Klicka p� acceptera kakor varje g�ng vi k�r ett test
            // B�da click nedan funkar!
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
            searchBar.SendKeys("Kronf�gel Majskyckling");
            Thread.Sleep(1500);

            searchBar.SendKeys(Keys.Enter);
            Thread.Sleep(3000);

            IWebElement addProduct = driver.FindElement(By.XPath("//a[@aria-label='Br�stfil� Av Majskyckling']"));
            addProduct.Click();
            Thread.Sleep(2500);

            IWebElement plusButton = driver.FindElement(By.XPath("//button[@aria-label='�ka antal']"));
            plusButton.Click();
            Thread.Sleep(3000);


            IWebElement shoppingCart = driver.FindElement(By.XPath("//button[@aria-label='kundvagn']"));
            shoppingCart.Click();
            Thread.Sleep(3000);

            IWebElement increaseQuantity = driver.FindElement(By.XPath("//button[@aria-label='�ka antal']"));
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
                Thread.Sleep(750);

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

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            var onlineShop = driver.FindElement(By.LinkText("Handla online"));
            onlineShop.Click();

            IList<IWebElement> miniArticles = wait.Until(driver =>
            driver.FindElements(By.ClassName("ItemTeaser-content")));

            foreach (var miniArticle in miniArticles)
            {
                Thread.Sleep(1000);
                if (miniArticle.Text.Contains("f�r"))
                {
                    try
                    {
                        miniArticle.Click();
                    }
                    catch (StaleElementReferenceException)
                    {
                        miniArticle.Click();
                    }
                    break;
                }
            }

            int quantity = 0;
            int discountPrice = 0;
            double originalPrice = 0;
            int incentive = 0;

            for (int i = 0; i < 2; i++)
            {
                try
                {
                    string quantityText = wait.Until(driver =>
                    driver.FindElement(By.ClassName("Splash-pricePre"))).Text.Substring(0, 1);
                    quantity = int.Parse(quantityText.Trim());

                    string discountPriceText = wait.Until(driver =>
                    driver.FindElement(
                        By.ClassName("Splash-priceLarge"))).Text.
                        Replace(":-", "");
                    discountPrice = int.Parse(discountPriceText.Trim());

                    string originalPriceText = wait.Until(driver =>
                    driver.FindElement(
                        By.CssSelector("span[aria-label='Pris']"))).Text.
                        Replace("/st", "").Replace(":", "");
                    originalPrice = double.Parse(originalPriceText.Trim()) / 100;

                    // L�gg till pantpris p� rabatt och originalpriset om pant finns
                    try
                    {
                        char[] recyclingInfo = driver.FindElement(
                            By.ClassName("ItemInfo-extra")).Text.ToCharArray();
                        int index = recyclingInfo.Length - 3;
                        bool parse = 
                            int.TryParse(recyclingInfo[index].ToString(), out incentive);

                        discountPrice += incentive * quantity;
                        originalPrice += incentive;
                    }
                    catch (NoSuchElementException)
                    {
                    }
                }
                catch (StaleElementReferenceException)
                {
                }
            }

            IWebElement addIcon;
            for (int i = 0; i < quantity; i++)
            {
                addIcon = wait.Until(product =>
                product.FindElement(By.CssSelector("button[aria-label='�ka antal']")));
                addIcon.Click();
            }

            //Navigera till kundvagn
            IWebElement miniCart;
            miniCart = wait.Until(driver =>
            driver.FindElement(By.CssSelector("button[aria-label='kundvagn']")));
            Thread.Sleep(1000);
            miniCart.Click();

            //H�mta avdraget pris och totalpriset
            IList<IWebElement> cartSummary;
            do
            {
                cartSummary = wait.Until(driver =>
                driver.FindElements(By.CssSelector("span[class=Cart-summaryItem]")));
            } while (cartSummary.Count == 0);
            
            string cartDiscountText = string.Join
                ("", cartSummary[1].Text.Split(':', '-')).Replace("kr", "").Trim();

            double discSubtraction = double.Parse(cartDiscountText) / 100;

            // Validera om kundvagnen visar r�tt prisavdrag
            double expected = originalPrice * 2 - discountPrice;
            double actual = discSubtraction;

            Assert.Equal(expected, actual);

            //T�m kundkorgen innan dispose
            var emptyBasketButton = wait.Until(driver =>
            driver.FindElement(By.CssSelector("button[data-test*=emptycartbutton]")));
            Thread.Sleep(1200);
            emptyBasketButton.Click();

            var confirmButton = wait.Until(driver =>
            driver.FindElement(By.XPath(
                "//div[contains(@class, 'Cart-notice')]/div/button[contains(text(), 'T�m')]")));
            Thread.Sleep(1200);
            confirmButton.Click();

            testOutput.WriteLine("Expected: " + expected + "\nActual: " + actual);
            Thread.Sleep(1200);

            driver.Quit();
            driver.Dispose();
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
            Search.SendKeys("�delost Bl� eko");
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
            testOutput.WriteLine("Added 10pcs of '�delost Bl� Eko' in the shopping cart.\n" +
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

            IWebElement enterEmail = driver.FindElement(By.XPath("//input[@id='loginEmail']"));
            enterEmail.SendKeys("testcoop123@hotmail.com");

            IWebElement enterPassword = driver.FindElement(By.XPath("//input[@id='loginPassword']"));
            enterPassword.SendKeys("Cerosifr123!");

            IWebElement login2 = driver.FindElement(By.XPath("//button[@type='submit']"));
            login2.Click();
            Thread.Sleep(3000);

            var onlineShop = driver.FindElement(By.LinkText("Recept"));
            onlineShop.Click();

            driver.Manage().Window.FullScreen();
            Thread.Sleep(3000);

            driver.Quit();
            driver.Dispose();
        }

        [Fact]
        [Trait("User story ID 8", "Search")]
        public void SearchForSpecificStoreInfo()
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

            var onlineShop = driver.FindElement(By.LinkText("Butiker & erbjudanden"));
            onlineShop.Click();

            IWebElement searchBar = driver.FindElement(By.CssSelector("input[placeholder*=postnummer]"));
            searchBar.SendKeys("Stora Coop Bor�s");

            driver.Manage().Window.FullScreen();
            Thread.Sleep(3000);

            searchBar.SendKeys(Keys.Enter);
            Thread.Sleep(3000);

            var storeInfo = driver.FindElement(By.LinkText("Erbjudanden och butiksinfo"));
            storeInfo.Click();
            Thread.Sleep(3000);

            var actual = driver.FindElement(By.CssSelector("div[class=StoreSelector--headerDesktop] span[class=Link2-text]")).Text;
            string expected = "Enedalsg. 10, BOR�S";
            Thread.Sleep(3000);

            Assert.Equal(expected, actual);

            testOutput.WriteLine("Expected result: " + expected + "\nActual result: " + actual);

            driver.Quit();
            driver.Dispose();


        }
    }
}
