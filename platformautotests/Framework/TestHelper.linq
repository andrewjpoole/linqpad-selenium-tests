<Query Kind="Program">
  <NuGetReference>DotNetSeleniumExtras.WaitHelpers</NuGetReference>
  <NuGetReference>Humanizer</NuGetReference>
  <NuGetReference>Selenium.Support</NuGetReference>
  <NuGetReference>Selenium.WebDriver</NuGetReference>
  <Namespace>Humanizer</Namespace>
  <Namespace>OpenQA.Selenium</Namespace>
  <Namespace>OpenQA.Selenium.Chrome</Namespace>
  <Namespace>OpenQA.Selenium.Edge</Namespace>
  <Namespace>OpenQA.Selenium.Firefox</Namespace>
  <Namespace>OpenQA.Selenium.IE</Namespace>
  <Namespace>OpenQA.Selenium.Remote</Namespace>
  <Namespace>OpenQA.Selenium.Support.UI</Namespace>
  <Namespace>System.Runtime.CompilerServices</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

// Define other methods and classes here
public class TestHelper 
{	
	public User CurrentUser;
	public Environment CurrentEnvironment;
	public bool LogMethodBeginningsAndEndings = false;
	public Stopwatch _stopWatch;
	private bool _passing = true; // default to true

	public RemoteWebDriver Driver {get; set;}
	public WebDriverWait Wait {get; set;}
	public Browser CurrentBrowser = Browser.ChromeOption;
	public const string IELogPath = "iedriver.log";
	public const string ScreenshotDefaultName = "Screenshot";
	public const string ScreenshotNameTimeStampFormat = "HH-mm-ss"; //"yyyy-MM-dd-HHmmss";
	public string ArtifactStoragePath;
	public string TestName;	

	public TestHelper(Environment env, bool logMethodBeginningsAndEndings = false) 
	{
		CurrentEnvironment = env;
		LogMethodBeginningsAndEndings = logMethodBeginningsAndEndings;
		TestName = UserQuery.QueryInstance.GetTestName();
		ArtifactStoragePath = @$"C:\Temp\autotests\artifacts\{DateTime.Now:yyyy-MM-dd}\{TestName}-{DateTime.Now:HHmmss}";

		if (!Directory.Exists(ArtifactStoragePath))
			Directory.CreateDirectory(ArtifactStoragePath);
	}

	public void SetUser(User user) 
	{
		CurrentUser = user;		
	}
	
	public void WriteLine(string message, LogLevels level = LogLevels.Info, bool includeTimeAndLevel = true)
	{
		var dateAndLevel = includeTimeAndLevel ? $"{DateTime.Now:HH:mm:ss} - {level.Humanize()} - ": "";
		var formattedmessage = $"{dateAndLevel}{message}";
		
		Console.WriteLine(formattedmessage);
		
		var logPath = Path.Combine(ArtifactStoragePath, "log.txt");
		using (var log = File.AppendText(logPath))
		{
			log.WriteLine(formattedmessage);
			log.Flush();
		}
	}

	public void BeginTest(string description = "")
	{
		Driver = CreateDriver(Browser.ChromeOption);
		
		Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(500);
		
		Wait = new WebDriverWait(Driver, TimeSpan.FromMilliseconds(10000));
		
		_stopWatch = new Stopwatch();
		_stopWatch.Start();

		WriteLine($"Beginning test: {TestName} at {DateTime.Now.ToString()}", LogLevels.Info, false);
		if(!string.IsNullOrEmpty(description))
			WriteLine($"Description: {description}", LogLevels.Info, false);

		WriteLine($"Artifacts will be stored at {ArtifactStoragePath}", LogLevels.Info, false);
		WriteLine("-------------------------------------------------------------------------------------------------------------------------", LogLevels.Info, false);
	}

	public void EndTest()
	{
		_stopWatch.Stop();
		
		Driver.Dispose();
		
		WriteLine($"completed in {_stopWatch.Elapsed.Humanize()}");
		var status = _passing ? "PASSED" : "FAILED";
		WriteLine($"==Test {status}==");
	}

	public void WaitForDurationThreadSleep(int duration) 
	{
		Thread.Sleep(duration);
	}

	public void WaitForDuration(int duration)
	{
		Task.Delay(duration).Wait();
	}

	public void FailsIf(bool condition, string messageOnFail = "")
	{
		if(condition)
		{
			_passing = false;
			if (!string.IsNullOrEmpty(messageOnFail))
				WriteLine(messageOnFail, LogLevels.Exception);

			throw new ApplicationException($"Test failed {messageOnFail}");
		}
	}

	public void PassesIf(bool condition, string messageOnFail = "", string messageOnPass = "")
	{
		if (condition)
		{
			_passing = true;
			if (!string.IsNullOrEmpty(messageOnPass))
				WriteLine(messageOnPass);
		}
		else
		{
			_passing = false;
			if (!string.IsNullOrEmpty(messageOnFail))
				WriteLine(messageOnFail, LogLevels.Exception);
				
			throw new ApplicationException($"Test failed {messageOnFail}");
		}
	}	
	
	public void CaptureScreenshot(string scName = ScreenshotDefaultName, bool addTimeStampToName = true)
	{
		var screenShotableDriver = Driver as ITakesScreenshot;
		if (screenShotableDriver == null)
		{ 
			WriteLine("! Driver is not able to capture screenshots.");
			return;
		}		

		var fileNameParts = new List<string> { scName ?? ScreenshotDefaultName };
		if (addTimeStampToName)
		{
			fileNameParts.Add(DateTime.Now.ToString(ScreenshotNameTimeStampFormat));
		}
		var fileName = $"{string.Join("_", fileNameParts.ToArray())}.png";
				
		var fullFileName = Path.Combine(ArtifactStoragePath, fileName);
		
		WriteLine($"Saving screenshot {fileName}");
		screenShotableDriver.GetScreenshot().SaveAsFile(fullFileName, ScreenshotImageFormat.Png);
	}

	private static RemoteWebDriver CreateDriver(Browser browser)
	{
		var executingDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		executingDir = @"c:\chromedriver\latest";
		switch (browser)
		{
			case Browser.Chrome:
				return new ChromeDriver(executingDir);

			case Browser.ChromeOption:
				var chromeOptions = new ChromeOptions();
				chromeOptions.AddArguments("start-maximized");
				chromeOptions.AddArguments("incognito");
				return new ChromeDriver(executingDir, chromeOptions);

			case Browser.Firefox:
				return new FirefoxDriver(executingDir);

			case Browser.InternetExplorer:
				return new InternetExplorerDriver(executingDir);
			case Browser.InternetExplorerOptions:
				var ieDriverOptions = new InternetExplorerOptions
				{
					EnsureCleanSession = true,
					ForceCreateProcessApi = true,
					BrowserCommandLineArguments = "-private",
					IntroduceInstabilityByIgnoringProtectedModeSettings = true
				};
				return new InternetExplorerDriver(executingDir, ieDriverOptions);
			case Browser.InternetExplorerService:
				string log = IELogPath;
				var ieDriverService =
					InternetExplorerDriverService.CreateDefaultService(
						Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
				var ieServiceOptions = new InternetExplorerOptions();
				ieDriverService.LoggingLevel = InternetExplorerDriverLogLevel.Trace;
				ieDriverService.LogFile =
					$"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}" + log;
				ieServiceOptions.EnsureCleanSession = true;
				return new InternetExplorerDriver(ieDriverService, ieServiceOptions);
			case Browser.Edge:
				return new EdgeDriver(executingDir);
			default:
				return new ChromeDriver(executingDir);
		}
	}
}

public enum LogLevels 
{
	Exception,
	Warning,
	Info,
	Debug
}

public enum Browser
{
	Chrome,
	ChromeOption,
	Firefox,
	Edge,
	InternetExplorer,
	InternetExplorerOptions,
	InternetExplorerService
}

public class User
{
	public string Upn;
	public string Password;
}

public static class Users 
{
	public static User TestUser1 = new User { Upn="a.user@company.com", Password= "kj54hg6kj45hg6"}; // example user
}

public class Environment
{
	public string Url;
	public string Name;
}

public static class Environments
{
	public static Environment Lab1 = new Environment {Url = "https://site-under-test.com", Name = "Site Under Test"}; // example site
}

public class PageBase 
{	
	protected TestHelper test;
	protected string _url;

	public PageBase(TestHelper testHelper, string url)
	{
		test = testHelper;
		_url = url;
	}

	public void NavigateTo()
	{
		LogMethodStarting();

		test.Driver.Navigate().GoToUrl(_url);

		LogMethodEnding();
	}
	
	public void LogMethodStarting([CallerMemberName] string caller = null, string args = "")
	{
		if (test.LogMethodBeginningsAndEndings)
			Console.WriteLine($"Method starting {caller} {args} after {test._stopWatch.Elapsed.Humanize()}");
	}

	public void LogMethodEnding([CallerMemberName] string caller = null)
	{
		if (test.LogMethodBeginningsAndEndings)
			Console.WriteLine($"Method ending {caller} after {test._stopWatch.Elapsed.Humanize()}");
	}

	public void WaitForSomethingInALoop(Func<bool> thingToWaitFor, int maxNumberOfTries = 10, int waitBetweenTriesMs = 1000)
	{
		for (int i = 0; i < maxNumberOfTries; i++)
		{
			if (thingToWaitFor())
			{
				break;
			}
			else
			{
				Task.Delay(waitBetweenTriesMs).Wait();
			}
		}
	}

	public IWebElement GetElementById(string id) 
	{
		return test.Wait.Until<IWebElement>(d => d.FindElement(By.Id(id)));
	}

	public IWebElement GetElementByClass(string className)
	{
		return test.Wait.Until<IWebElement>(d => d.FindElement(By.ClassName(className)));
	}

	public bool CheckIfElementDisplayed(string id)
	{
		return test.Driver.FindElements(By.Id(id)).Count < 1;
	}

	public string GetCurrentPageTitle() 
	{
		return test.Driver.Title;
	}

	public bool CurrentPageTitleIsEqualTo(string text) 
	{
		return GetCurrentPageTitle() == text;		
	}

}

public string GetTestName()
{
	return Path.GetFileName(Util.CurrentQueryPath).Replace(".linq", "");
}