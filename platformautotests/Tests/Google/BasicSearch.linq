<Query Kind="Program">
  <Namespace>OpenQA.Selenium</Namespace>
</Query>

#load "..\..\Framework\TestHelper.linq"
#load "..\..\Framework\Pages\GoogleHome.linq"
#load "..\SharedSteps.linq"
//[NUNIT IGNORE]
void Main()
{
	var test = new TestHelper(Environments.Lab1);
	test.SetUser(Users.TestUser1);
	
	var googleHome = new GoogleHomePage(test);

	test.BeginTest();
	
	googleHome.NavigateTo();	
	googleHome.Search("frogs");
	test.CaptureScreenshot("01-GoogleHome-Frogs");
	
	googleHome.ClickOnLink(1);
	test.CaptureScreenshot("02-FirstLink");

	var pageTitle = googleHome.GetCurrentPageTitle();
	test.WriteLine($"Title of clicked link is {pageTitle}");
	test.PassesIf(pageTitle != "");
		
	test.EndTest();
	
}