using NUnit.Framework;
using RevaliInstruct.Tests.UI.Pages; // Voor LoginPage
using OpenQA.Selenium;

namespace RevaliInstruct.Tests.UI
{
    public class PatientOverviewTests : BaseUITest 
    {
        [Test]
        public void Test_FreddyVoetballer_DisplaysCorrectData()
        {
            Driver.Navigate().GoToUrl($"{BaseUrl}/login");
            var loginPage = new LoginPage(Driver);
            loginPage.Login("ra_smit", "password123");

            Wait.Until(d => d.Url.Contains("/patients"));

            var name = Wait.Until(d => d.FindElement(By.XPath("//td[contains(., 'Freddy Voetballer')]")));
            
            var status = Driver.FindElement(By.XPath("//td[contains(., 'Freddy Voetballer')]/following-sibling::td//span"));

            Assert.Multiple(() =>
            {
                Assert.That(name.Displayed, Is.True);
                Assert.That(status.Text, Is.EqualTo("Actief"));
            });
        }
    }
}