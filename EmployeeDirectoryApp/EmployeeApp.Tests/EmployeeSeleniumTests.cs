using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace EmployeeApp.Tests;

public class EmployeeSeleniumTests : IDisposable
{
    private readonly IWebDriver _driver;
    private readonly string _baseUrl = "http://13.206.207.211:5000/"; // VM IP
    private readonly WebDriverWait _wait;

    public EmployeeSeleniumTests()
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--disable-gpu");

        _driver = new ChromeDriver(options);
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
    }

    [Fact]
    public void Test_PageLoads_Successfully()
    {
        _driver.Navigate().GoToUrl(_baseUrl);

        Assert.Contains("Employee Directory", _driver.Title);
        var addBtn = _driver.FindElement(By.Id("addBtn"));
        Assert.True(addBtn.Displayed, "Add Employee button should be visible");
    }

    [Fact]
    public void Test_AddEmployee_AppearsInList()
    {
        _driver.Navigate().GoToUrl(_baseUrl);

        // Enter employee name
        var nameInput = _wait.Until(d => d.FindElement(By.Id("empName")));
        nameInput.Clear();
        nameInput.SendKeys("Jane Doe");

        // Click Add Employee
        _driver.FindElement(By.Id("addBtn")).Click();

        // Wait and verify employee appears
        _wait.Until(d => d.FindElement(By.Id("empList")).Text.Contains("Jane Doe"));
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
            var inp = _wait.Until(d => d.FindElement(By.Id("empName")));
            inp.Clear();
            inp.SendKeys(emp);
            _driver.FindElement(By.Id("addBtn")).Click();
            Thread.Sleep(500);
        }

        var listText = _driver.FindElement(By.Id("empList")).Text;
        foreach (var emp in employees)
            Assert.Contains(emp, listText);
    }

    public void Dispose() => _driver.Quit();
}
