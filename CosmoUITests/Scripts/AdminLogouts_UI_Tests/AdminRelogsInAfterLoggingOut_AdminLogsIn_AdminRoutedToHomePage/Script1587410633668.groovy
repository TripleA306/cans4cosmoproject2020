import static com.kms.katalon.core.checkpoint.CheckpointFactory.findCheckpoint
import static com.kms.katalon.core.testcase.TestCaseFactory.findTestCase
import static com.kms.katalon.core.testdata.TestDataFactory.findTestData
import static com.kms.katalon.core.testobject.ObjectRepository.findTestObject
import static com.kms.katalon.core.testobject.ObjectRepository.findWindowsObject
import com.kms.katalon.core.checkpoint.Checkpoint as Checkpoint
import com.kms.katalon.core.cucumber.keyword.CucumberBuiltinKeywords as CucumberKW
import com.kms.katalon.core.mobile.keyword.MobileBuiltInKeywords as Mobile
import com.kms.katalon.core.model.FailureHandling as FailureHandling
import com.kms.katalon.core.testcase.TestCase as TestCase
import com.kms.katalon.core.testdata.TestData as TestData
import com.kms.katalon.core.testobject.TestObject as TestObject
import com.kms.katalon.core.webservice.keyword.WSBuiltInKeywords as WS
import com.kms.katalon.core.webui.keyword.WebUiBuiltInKeywords as WebUI
import com.kms.katalon.core.windows.keyword.WindowsBuiltinKeywords as Windows
import internal.GlobalVariable as GlobalVariable

String loginURL = WebUI.getUrl()

//enter valid username
WebUI.setText(findTestObject("Admin_Login_Page_OR/AdminUserName"), "Cans4Cosmo")

//enter an valid password
WebUI.setText(findTestObject("Admin_Login_Page_OR/AdminPassword"), "Cosmo123")

//click the login button
WebUI.click(findTestObject("Admin_Login_Page_OR/AdminLoginButton"))

Thread.sleep(2000)

//Log out
WebUI.click(findTestObject('Object Repository/HomePage_OR/button_Log Out'))

//Admin relogs in
//enter valid username
WebUI.setText(findTestObject("Admin_Login_Page_OR/AdminUserName"), "Cans4Cosmo")

//enter an valid password
WebUI.setText(findTestObject("Admin_Login_Page_OR/AdminPassword"), "Cosmo123")

//click the login button
WebUI.click(findTestObject("Admin_Login_Page_OR/AdminLoginButton"))

Thread.sleep(2000)

//verify that the admin is not on the login page
WebUI.verifyNotMatch(loginURL, WebUI.getUrl(), false)

//Verify that the admin is on the home page
WebUI.verifyMatch('http://localhost:8080/#/home', WebUI.getUrl(), false)

//Log back out to reset for next test
WebUI.click(findTestObject('Object Repository/HomePage_OR/button_Log Out'))