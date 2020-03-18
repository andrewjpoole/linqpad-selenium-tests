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

#load "..\..\Framework\TestHelper.linq"

public class GoogleHomePage : PageBase
{
	// Portal elements
	public IWebElement SearchInput => GetElementByClass("gLFyf"); // gsfi
	public IWebElement SearchButton => GetElementByClass("gNO89b");
		
	// constructor, must pass variables on to base class.
	public GoogleHomePage(TestHelper testHelper):base(testHelper, "https://www.google.com/")
	{		
	}

	// Methods
	public void Search(string searchTerm, int durationMs = 100)
	{
		LogMethodStarting();
		
		SearchInput.SendKeys(searchTerm);
		SearchButton.Click();
		
		Task.Delay(durationMs).Wait();
		
		LogMethodEnding();
	}

	public void ClickOnLink(int resultIndex, int durationMs = 1000)
	{
		LogMethodStarting();
		
		var results = test.Driver.FindElements(By.ClassName("LC20lb")); // DKV0Md
		test.WriteLine($"returned {results.Count()} results on the first page");
		
		var selectedResult = results.Skip(resultIndex).FirstOrDefault();
		var selectedElementLink = selectedResult.FindElement(By.XPath(".."));
		
		selectedElementLink.Click();

		Task.Delay(durationMs).Wait();

		LogMethodEnding();
	}
	
	public bool HasLoaded() 
	{
		return CurrentPageTitleIsEqualTo("| ONBOARD | OVERVIEW");
	}
}