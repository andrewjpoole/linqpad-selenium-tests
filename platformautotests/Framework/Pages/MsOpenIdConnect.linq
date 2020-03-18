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

public class Portal : PageBase
{
	// Portal elements
	public IWebElement SignInBtn => GetElementById("SignIn");
	public IWebElement SignOutBtn => GetElementById("SignOut");
	public IWebElement SubPlanBtn => GetElementById("SubPlan");
	public IWebElement DashboardMenu => GetElementById("DashboardLink");
	public IWebElement ProfileMenu => GetElementById("MyProfileLink");
	public IWebElement NewsletterCheckbox => GetElementById("inlineCheckbox1");
	public IWebElement UpdatesCheckbox => GetElementById("inlineCheckbox2");
	public IWebElement TechnicalCheckbox => GetElementById("l-ac-op-role-01");
	public IWebElement DecisionMakerCheckbox => GetElementById("l-ac-op-role-02");
	public IWebElement RecommenderCheckbox => GetElementById("l-ac-op-role-03");
	
	// Micosoft login elements
	public IWebElement MsLoginNextButton => GetElementById("idSIButton9");
	public IWebElement MsLoginUserInput => GetElementById("i0116");
	public IWebElement MsLoginPasswordInput => GetElementById("i0118");
	public IWebElement MsLoginReduceInput => GetElementById("KmsiCheckboxField");
	public IWebElement PasswordErrorNotice => GetElementById("passwordError");
		
	// constructor, must pass variables on to base class.
	public Portal(TestHelper testHelper):base(testHelper, testHelper.CurrentEnvironment.PortalUrl)
	{		
	}

	// Methods
	public void ClickNextAndWait(int durationMs = 100)
	{
		LogMethodStarting();
				
		MsLoginNextButton.Click();
		Task.Delay(durationMs).Wait();

		LogMethodEnding();
	}

	public void ClickSignIn()
	{
		LogMethodStarting();
		
		SignInBtn.Click();
		
		LogMethodEnding();
	}
	
	public void ClickEnterLab()
	{
		LogMethodStarting();
		
		SubPlanBtn.Click();
		
		LogMethodEnding();
	}

	public void EnterPassword()
	{
		LogMethodStarting(args:test.CurrentUser.Upn);
		
		MsLoginPasswordInput.SendKeys(test.CurrentUser.Password);
		
		LogMethodEnding();
	}
	
	public bool PasswordErrorNoticeIsNotDisplaid()
	{
		return CheckIfElementDisplayed("passwordError");
	}
	
	public void EnterUpn()
	{
		LogMethodStarting();
		
		MsLoginUserInput.SendKeys(test.CurrentUser.Upn);
		
		LogMethodEnding();
	}

	public bool IsLoaded() 
	{
		return CurrentPageTitleIsEqualTo("InFlight | home");
	}	

	public void WaitForMsLoginNextButton()
	{
		LogMethodStarting();
		
		WaitForSomethingInALoop (() => MsLoginNextButton.Displayed, 10, 1000);
		
		LogMethodEnding();
	}
}