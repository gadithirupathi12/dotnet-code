using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace EmployeeApp.Tests;

public class EmployeeSeleniumTests : IDisposable
{
    private readonly IWebDriver _driver;
    private readonly string _baseUrl = "http://13.206.207.211:5000/";
    private readonly WebDriverWait _wait;

    public EmployeeSeleniumTests()
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--disable-gpu");

        _driver = new ChromeDriver(options);
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
    }

    [Fact]
    public void Test_PageLoads_Successfully()
    {
        _driver.Navigate().GoToUrl(_baseUrl);

        _wait.Until(d => d.Title.Contains("Employee"));

        Assert.Contains("Employee", _driver.Title);

        var addBtn = _wait.Until(d => d.FindElement(By.Id("addBtn")));
        Assert.True(addBtn.Displayed);
    }

    [Fact]
    public void Test_AddEmployee_AppearsInList()
    {
        _driver.Navigate().GoToUrl(_baseUrl);

        var nameInput = _wait.Until(d => d.FindElement(By.Id("empName")));
        nameInput.Clear();
        nameInput.SendKeys("Jane Doe");

        _driver.FindElement(By.Id("addBtn")).Click();

        // Wait until list contains new employee
        _wait.Until(d =>
        {
            var text = d.FindElement(By.Id("empList")).Text;
            return text.Contains("Jane Doe");
        });

        var listText = _driver.FindElement(By.Id("empList")).Text;
        Assert.Contains("Jane Doe", listText);
    }

    [Fact]
    public void Test_AddMultipleEmployees()
    {
        _driver.Navigate().GoToUrl(_baseUrl);

        var employees = new[] { "Tom Harris", "Sara Connor" };

        foreach (var emp in employees)
        {
            var input = _wait.Until(d => d.FindElement(By.Id("empName")));
            input.Clear();
            input.SendKeys(emp);

            _driver.FindElement(By.Id("addBtn")).Click();

            // Wait until each employee appears before moving on
            _wait.Until(d =>
            {
                var text = d.FindElement(By.Id("empList")).Text;
                return text.Contains(emp);
            });
        }

        var finalText = _driver.FindElement(By.Id("empList")).Text;

        foreach (var emp in employees)
        {
            Assert.Contains(emp, finalText);
        }
    }

    public void Dispose()
    {
        _driver.Quit();
    }
}
