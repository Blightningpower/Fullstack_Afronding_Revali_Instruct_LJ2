using NUnit.Framework;
using RevaliInstruct.Tests.UI.Pages;
using OpenQA.Selenium;

namespace RevaliInstruct.Tests.UI
{
    public class LogoutTests : BaseUITest
    {
        [Test]
        public void Test_UserCanLogoutSuccessfully()
        {
            Driver.Navigate().GoToUrl($"{BaseUrl}/login");
            new LoginPage(Driver).Login("ra_smit", "password123");

            var logoutButton = Driver.FindElement(By.XPath("//button[text()='Logout']"));
            logoutButton.Click();

            Assert.That(Driver.Url, Does.Contain("/login"), "Gebruiker is niet teruggestuurd naar login na logout.");
        }
    }
}