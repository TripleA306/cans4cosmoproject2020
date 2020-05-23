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

//This test will ensure that an adminstrator can access the admin home page by entering valid credentials (username and password)

//get the current url of the login page
String currentURL = WebUI.getUrl()

//enter valid username
WebUI.setText(findTestObject("Admin_Login_Page_OR/AdminUserName"), "Cans4Cosmo")

//enter an valid password
WebUI.setText(findTestObject("Admin_Login_Page_OR/AdminPassword"), "Cosmo123")

//click the login button
WebUI.click(findTestObject("Admin_Login_Page_OR/AdminLoginButton"))

Thread.sleep(2000)

//verify that the url has not changed. this means that the admin is still on the login page
WebUI.verifyNotMatch(currentURL, WebUI.getUrl(), false)

WebUI.verifyMatch('http://localhost:8080/#/home', WebUI.getUrl(), false)