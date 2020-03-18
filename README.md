# linqpad-selenium-tests

## Overview

* Everything is written in C#
* TestHelper is likely to be maintained 80% by devs, main object is to abstract complexity awaiy to make writting the tests as simple and intuitive as possible.
* Page Object Models contain page specific logic and are likely to be a collaborative effort 50-50% split between devs and testers.
* Tests should be hopefully maintained mostly by testers.
* The framework is powered by Selenium Web Driver, which is used to interogate and manipulate web pages.
* Uses Linqpad as an IDE because it is simpler and cheaper than visual studio.
* An NunitTestCaseSource is provided to allow these tests to be run as part of a DevOps build.

## Environment Setup

1. Pull down source code from DevOps repo
2. Install Linqpad 6 from [here](https://www.linqpad.net/LINQPad6.aspx)
3. Ideally enter an activation code to access premium features such as intellisense and debugging. The build agent wont need an activation code.
4. In Linqpad, set the MyQueries location to the git repo.
5. Run the GetCorrectChromeDriverVersion.ps1 script to obtain the latest Chrome driver compatible with your version of Chrome.
6. Open one of the tests in Linqpad and press f5 or click the play button.

## Tests

Tests are linqpad scripts located in the linqpad-selenium-tests\linqpad-selenium-tests\Tests directory. The scripts need to be set to 'C# Program' with a Main() method

The top is the place to reference any scripts:
```
#load "..\..\Framework\TestHelper.linq"
#load "..\..\Framework\Pages\GoogleHome.linq"
#load "..\SharedSteps.linq"
```
The top one is the testHelper script, which contains the whole framework.
The bottom one contains shared steps.
The references in between are the Page Object Models needed by the test.

Inside the main method:
```csharp
// instantiate a new testHelper
var test = new TestHelper(Environments.Lab1);

// select a user object if needed i.e. for authenication
test.SetUser(Users.TestUser1);

// instantiate any pages that the test will need to use
var googleHome = new GoogleHomePage(test);

test.BeginTest(); // this starts the test, creates and configures the web driver and starts a stopwatch

googleHome.NavigateTo();	
googleHome.Search("frogs");
test.CaptureScreenshot("01-GoogleHome-Frogs"); 

googleHome.ClickOnLink(1); // call method on Page Object Model
test.CaptureScreenshot("02-FirstLink"); // Capture a screenshot

var pageTitle = googleHome.GetCurrentPageTitle();
test.WriteLine($"Title of clicked link is {pageTitle}");
test.PassesIf(pageTitle != ""); // make an assertion

test.EndTest(); // this ends the test, disposing the webdriver and stopping the stopwatch and writing out the result
```

## Page Object Model

These live in the linqpad-selenium-tests\linqpad-selenium-tests\Framework\Pages directory
They should be fairly self explanatory, the main point is to place any ids, CSS selectors or page specific logic in one place, so that if a page changes, those things only need to be changed in one place, rather than a large number of broken tests.

```csharp
#load "..\..\Framework\TestHelper.linq" // reference any other scripts here

public class GoogleHomePage : PageBase // derrive from PageBase
{
	// Portal elements
	public IWebElement SearchInput => GetElementByClass("gLFyf");
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
		
		var results = test.Driver.FindElements(By.ClassName("LC20lb")); // find these ids or classnames using f12 dev tools
		test.WriteLine($"returned {results.Count()} results on the first page");
		
		var selectedResult = results.Skip(resultIndex).FirstOrDefault();
		var selectedElementLink = selectedResult.FindElement(By.XPath(".."));
		
		selectedElementLink.Click();

		Task.Delay(durationMs).Wait();

		LogMethodEnding();
	}
	
	public bool HasLoaded() 
	{
		return CurrentPageTitleIsEqualTo("Google");
	}
}
```

## Running the tests on a build with LinqpadAutoTestCaseSource and LinqpadAutoTestRunner

This small nunit test project will look for linqpad scipts in the tests folder and run them as nunit tests using the linqpad commandline runner. 

Any tests with `//[NUNIT IGNORE]` at the top will be ignored.

This is intended to be used in a dotnet test step of a DevOps build, although there is only one unit test, each discovered linqpad script will be reported as a a test, grouped by directory name, in the build test results.

In future the test outputs could/should be stored in the build artifacts.
