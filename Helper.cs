using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace CEROSIFRTestVerktygSelenium
{
    public class Helper
    {
        public static void StaleClick(IWebElement element)
        {
            int attempts = 0;
            do
            {
                try
                {
                    element.Click();
                }
                catch (StaleElementReferenceException e)
                {
                }
                attempts++;
            } while (attempts != 2);
        }
    }
}
