using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Tests;

public class WebsiteTests : IDisposable
{
    private readonly IWebDriver _driver;
    private readonly IConfiguration _config;
    private readonly WebDriverWait _wait;

    public WebsiteTests()
    {
        _driver = new ChromeDriver();
        _config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("testsettings.json")
            .AddUserSecrets<WebsiteTests>()
            .AddEnvironmentVariables()
            .Build();
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void TestDeveloperAuthorizationTrue()
    {
        DeveloperTrueAuthorization();
        Assert.EndsWith("courses",_driver.Url);
    }

    [Fact]
    public void TestDeveloperAuthorizationFalse()
    {
        _driver.Navigate().GoToUrl(_config["BaseUrl"]);
        var emailInput = _driver.FindElement(By.Id("email"));
        var passwordInput = _driver.FindElement(By.Id("password"));
        var button = _driver.FindElement(By.XPath("/html/body/div[1]/main/div[2]/form/button"));
        emailInput.SendKeys("123");
        passwordInput.SendKeys("1");
        button.Click();
        _wait.Until(x => x.SwitchTo().Alert());
        var alert = _driver.SwitchTo().Alert();
        Assert.Contains("Неправильный пароль или почта", alert.Text);
    }

    private void DeveloperTrueAuthorization()
    {
        _driver.Navigate().GoToUrl(_config["BaseUrl"] + "auth");
        var emailInput = _driver.FindElement(By.Id("email"));
        var passwordInput = _driver.FindElement(By.Id("password"));
        var button = _driver.FindElement(By.XPath("/html/body/div[1]/main/div[2]/form/button"));
        emailInput.SendKeys(_config["DeveloperEmail"]);
        passwordInput.SendKeys(_config["DeveloperPassword"]);
        button.Click();
        _wait.Until(x => !x.Url.EndsWith("auth"));
    }

    private IWebElement TryFindElement(By by)
    {
        _wait.Until(x => _driver.FindElement(by));
        return _driver.FindElement(by);
    }
    private IAlert TryGetAlert()
    {
        _wait.Until(x => x.SwitchTo().Alert());
        return _driver.SwitchTo().Alert();
    }

    [Fact]
    public void TestAddCourseTrue()
    {
        DeveloperTrueAuthorization();
        _driver.Navigate().GoToUrl(_config["BaseUrl"] + "my-courses");
        var addButton = TryFindElement(By.XPath("//button[text()='Добавить новый курс']"));
        addButton.Click();
        var nameInput = TryFindElement(By.Id("course-name"));
        var descInput = _driver.FindElement(By.Id("course-desc"));
        var priceInput = _driver.FindElement(By.Id("course-price"));
        nameInput.SendKeys("TestName");
        descInput.SendKeys("TestDescription");
        priceInput.SendKeys("125");
        var saveButton = _driver.FindElement(By.XPath("//button[text()='Сохранить']"));
        saveButton.Click();
        var alert = TryGetAlert();
        Assert.Contains("Курс успешно создан", alert.Text);
    }
    [Fact]
    public void TestModulesImportTrue()
    {
        DeveloperTrueAuthorization();
        _driver.Navigate().GoToUrl(_config["BaseUrl"] + "my-courses");

        var courseLink = TryFindElement(By.CssSelector("div[class*='card']"));
        courseLink.Click();

        var showImportButton = TryFindElement(By.XPath("/html/body/div[1]/main/div[2]/h4/button[2]"));
        showImportButton.Click();

        var fileInput = TryFindElement(By.CssSelector("input[type='file']"));

        fileInput.SendKeys(Path.GetFullPath("../../../Files/modulesValidImport.xlsx"));
        
        var sendFileButton = TryFindElement(By.XPath("/html/body/div[1]/main/div[2]/h4/button[3]"));
        sendFileButton.Click();

        var alert = TryGetAlert();
        Assert.Contains("Модули успешно импортированы", alert.Text);
    }
    [Fact]
    public void TestModulesImportFalse()
    {
        DeveloperTrueAuthorization();
        _driver.Navigate().GoToUrl(_config["BaseUrl"] + "my-courses");
        var courseLink = TryFindElement(By.CssSelector("div[class*='card']"));
        courseLink.Click();

        var showImportButton = TryFindElement(By.XPath("/html/body/div[1]/main/div[2]/h4/button[2]"));
        showImportButton.Click();

        var fileInput = TryFindElement(By.CssSelector("input[type='file']"));

        fileInput.SendKeys(Path.GetFullPath("../../../Files/modulesInvalidImport.xlsx"));
        
        var sendFileButton = TryFindElement(By.XPath("/html/body/div[1]/main/div[2]/h4/button[3]"));
        sendFileButton.Click();

        var alert = TryGetAlert();
        Assert.Contains("Ошибка", alert.Text, StringComparison.OrdinalIgnoreCase);
    }

    public void Dispose()
    {
        _driver.Dispose();
    }
}