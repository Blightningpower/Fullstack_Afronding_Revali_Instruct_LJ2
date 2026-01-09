using OpenQA.Selenium;

namespace RevaliInstruct.Tests.UI.Pages
{
    public class PatientOverviewPage
    {
        private readonly IWebDriver _driver;
        public PatientOverviewPage(IWebDriver driver) => _driver = driver;

        private IWebElement SearchInput => _driver.FindElement(By.Id("q"));
        private IWebElement StatusFilter => _driver.FindElement(By.Id("status"));
        private IWebElement SearchButton => _driver.FindElement(By.CssSelector("button[type='submit']"));

        public void SearchPatient(string name)
        {
            SearchInput.Clear();
            SearchInput.SendKeys(name);
            SearchButton.Click();
        }

        public bool IsPatientInList(string fullName)
        {
            try {
                return _driver.FindElement(By.XPath($"//td/strong[text()='{fullName}']")).Displayed;
            } catch (NoSuchElementException) {
                return false;
            }
        }
    }
}