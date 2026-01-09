using NUnit.Framework;
using RevaliInstruct.Tests.UI.Pages;

namespace RevaliInstruct.Tests.UI
{
    [TestFixture]
    public class LoginTests : BaseUITest
    {
        [Test]
        public void Test_SuccessfulLogin()
        {
            Driver.Navigate().GoToUrl($"{BaseUrl}/login");
            var loginPage = new LoginPage(Driver);

            loginPage.Login("ra_smit", "password123");

            Wait.Until(d => d.Url.Contains("/patients"));

            var overviewPage = new PatientOverviewPage(Driver);
            Assert.That(overviewPage.IsPatientInList("Freddy Voetballer"), Is.True);
        }
    }
}