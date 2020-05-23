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

/**
 * This Test will verify that the "opt out of pickup" button colour changes after being clicked for the first time
 */

//Admin Page

//Open the browser
WebUI.openBrowser('http://localhost:8081')

//verify subscriber option on the nav bar exists
WebUI.verifyElementPresent(findTestObject('Page_Admin - Subscribers/Navigation_Bar/button_Subscriber_Nav'), 5)

//click the subscriber option on the nav bar
WebUI.click(findTestObject('Page_Admin - Subscribers/Navigation_Bar/button_Subscriber_Nav'))

//Verify that the subscriber table is present
WebUI.verifyElementPresent(findTestObject('Page_Admin - Subscribers/Subscriber_Table/tbl_Subscriber_Table'), 5)

//Verify that the subscriber Header is present
WebUI.verifyElementPresent(findTestObject('Page_Admin - Subscribers/Subscriber_Table/h1_Subcriber_Header'), 5)

//click the button which will force an opt out of a subscriber's upcoming pickup
WebUI.click(findTestObject('Page_Admin - Subscribers/Subscriber_Table/btn_OptOut_Sub1')) // ****************** Test Object not created yet

//Verify colour has changed on the opt out button
WebUI.verifyElementAttributeValue(findTestObject('Page_Admin - Subscribers/Subscriber_Table/btn_OptOut_Sub1'), 'color', 'red', 5)

//close the browser
WebUI.closeBrowser();