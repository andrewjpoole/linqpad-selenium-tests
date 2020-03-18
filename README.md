# linqpad-selenium-tests

## Overview

* Everything is written in C#
* TestHelper is likely to be maintained 80% by devs, main object is to make writting the tests as simple and intuitive as possible.
* Page Object Models are likely to be a collaborative effort 50-50% split between devs and testers.
* Tests are maintained 80% by testers.
* Powered by Selenium Web Driver, which is used to interogate and manipulate web pages.
* Uses Linqpad as an IDE because it is simpler and cheaper than visual studio.
* An NunitTestCaseSource is provided to allow these tests to be run as part of a DevOps build.

## Environment Setup

1. Pull down source code from DevOps repo
2. Install LInqpad 6 from [here](https://www.linqpad.net/LINQPad6.aspx)
3. Ideally enter an activation code to access premium features such as intellisense and debugging. The build agent wont need an activation code.
4. In Linqpad, set the MyQueries location to the git repo.
5. Run the GetCorrectChromeDriverVersion.ps1 script to obtain the latest Chrome driver compatible with your version of Chrome.
6. Open one of the tests in Linqpad and press f5 or click the play button.

## Tests

Tests are linqpad scripts, set to 'C# Program' with a Main() method

At the top, any referenced scipts are referenced:
```
#load "..\..\Framework\TestHelper.linq"
#load "..\..\Framework\Pages\GoogleHome.linq"
#load "..\SharedSteps.linq"
```
The top is the testHelper script, which contains the whole framework.
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

## NunitTestCaseSource

This small nunit test project will look for linqpad scipts in the tests folder and run them as nunit tests using the linqpad commandline runner. Any tests with `//[NUNIT IGNORE]` at the top will be ignored.

This is intended to be used in a dotnet test step of a DevOps build.