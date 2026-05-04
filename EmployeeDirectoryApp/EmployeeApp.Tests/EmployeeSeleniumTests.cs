using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;
using System;
using System.Net.Http;
using System.Threading;

namespace EmployeeApp.Tests;

public class EmployeeSeleniumTests
{
    private readonly string _baseUrl = "http://13.206.207.211:5000/";

    // ---------------- DRIVER FACTORY ----------------
    private IWebDriver CreateDriver()
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless=new");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--disable-gpu");

        return new ChromeDriver(options);
    }

    private WebDriverWait CreateWait(IWebDriver driver)
    {
        return new WebDriverWait(driver, TimeSpan.FromSeconds(30));
    }

    // ---------------- APP HEALTH CHECK ----------------
    private void WaitForApp()
    {
        using var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(5);

        for (int i = 0; i < 30; i++)
        {
            try
            {
                var res = client.GetAsync(_baseUrl).Result;
                if (res.IsSuccessStatusCode)
                    return;
            }
            catch { }

            Thread.Sleep(3000);
        }

        throw new Exception("❌ App is not reachable after waiting 90 seconds");
    }

    // ---------------- TEST 1 ----------------
    [Fact]
    public void Test_PageLoads_Successfully()
    {
        WaitForApp();

        using var driver = CreateDriver();
        var wait = CreateWait(driver);

        driver.Navigate().GoToUrl(_baseUrl);

        wait.Until(d => d.Title.Contains("Employee"));

        Assert.Contains("Employee", driver.Title);

        var addBtn = wait.Until(d => d.FindElement(By.Id("addBtn")));
        Assert.True(addBtn.Displayed);
    }

    // ---------------- TEST 2 ----------------
    [Fact]
    public void Test_AddEmployee_AppearsInList()
    {
        WaitForApp();

        using var driver = CreateDriver();
        var wait = CreateWait(driver);

        driver.Navigate().GoToUrl(_baseUrl);

        var nameInput = wait.Until(d => d.FindElement(By.Id("empName")));
        nameInput.Clear();
        nameInput.SendKeys("Jane Doe");

        var addBtn = wait.Until(d => d.FindElement(By.Id("addBtn")));
        addBtn.Click();

        wait.Until(d => d.FindElement(By.Id("empList")).Text.Contains("Jane Doe"));

        var listText = driver.FindElement(By.Id("empList")).Text;
        Assert.Contains("Jane Doe", listText);
    }

    // ---------------- TEST 3 ----------------
    [Fact]
    public void Test_AddMultipleEmployees()
    {
        WaitForApp();

        using var driver = CreateDriver();
        var wait = CreateWait(driver);

        driver.Navigate().GoToUrl(_baseUrl);

        var employees = new[] { "Tom Harris", "Sara Connor" };

        foreach (var emp in employees)
        {
            var input = wait.Until(d => d.FindElement(By.Id("empName")));
            input.Clear();
            input.SendKeys(emp);

            var addBtn = wait.Until(d => d.FindElement(By.Id("addBtn")));
            addBtn.Click();

            wait.Until(d => d.FindElement(By.Id("empList")).Text.Contains(emp));
        }

        var finalText = driver.FindElement(By.Id("empList")).Text;

        foreach (var emp in employees)
        {
            Assert.Contains(emp, finalText);
        }
    }
}
