using OpenQA.Selenium;

namespace RevaliInstruct.Tests.UI.Pages
{
    public class LoginPage
    {
        private readonly IWebDriver _driver;
        public LoginPage(IWebDriver driver) => _driver = driver;

        private IWebElement UsernameField => _driver.FindElement(By.CssSelector("input[type='text']"));
        private IWebElement PasswordField => _driver.FindElement(By.CssSelector("input[type='password']"));
        private IWebElement LoginButton => _driver.FindElement(By.CssSelector("button.btn-primary"));

        public void Login(string user, string pass)
        {
            UsernameField.Clear();
            UsernameField.SendKeys(user);
            PasswordField.Clear();
            PasswordField.SendKeys(pass);
            LoginButton.Click();
        }
    }
}