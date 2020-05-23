import static com.kms.katalon.core.checkpoint.CheckpointFactory.findCheckpoint
import static com.kms.katalon.core.testcase.TestCaseFactory.findTestCase
import static com.kms.katalon.core.testdata.TestDataFactory.findTestData
import static com.kms.katalon.core.testobject.ObjectRepository.findTestObject
import com.kms.katalon.core.checkpoint.Checkpoint as Checkpoint
import com.kms.katalon.core.cucumber.keyword.CucumberBuiltinKeywords as CucumberKW
import com.kms.katalon.core.mobile.keyword.MobileBuiltInKeywords as Mobile
import com.kms.katalon.core.model.FailureHandling as FailureHandling
import com.kms.katalon.core.testcase.TestCase as TestCase
import com.kms.katalon.core.testdata.TestData as TestData
import com.kms.katalon.core.testobject.TestObject as TestObject
import com.kms.katalon.core.webservice.keyword.WSBuiltInKeywords as WS
import com.kms.katalon.core.webui.keyword.WebUiBuiltInKeywords as WebUI
import internal.GlobalVariable as GlobalVariable

//This test will ensure that an adminstrator cannot access the admin home page by entering invalid credentials (blank username and blank password)

//get the current url of the login page
String currentURL = WebUI.getUrl()

//click the login button
WebUI.click(findTestObject("Admin_Login_Page_OR/AdminLoginButton"))

Thread.sleep(2000)

WebUI.verifyTextPresent("Username or password invalid", false)

//verify that the url has not changed. this means that the admin is still on the login page
WebUI.verifyMatch(currentURL, WebUI.getUrl(), false)